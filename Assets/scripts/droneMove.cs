using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class droneMove : UdonSharpBehaviour
{
    public Rigidbody rigid;     //Rigidbody of the drone (assigned in inspector)
    private BoxCollider seatCollider;   //Seat collider of the drone seat

    //vrRoll speed used for both vrPitch AND vrRoll, thrustSpeed used for thrust, vrYawSpeed used for vrYaw
    [SerializeField] public float rollSpeed, thrustSpeed, vrYawSpeed, dragValue;
    public GameObject _seat;    //Drone Seat gameobject
    public bool seated = false;      //Boolean check if player is seated
    public UnityEngine.Vector3 gScale = new Vector3(0,-9.81f,0);
    private UnityEngine.Quaternion rotation;    //rotation variable is of a Quaternion type which stores rotations in Unity. A quaternion is a four-tuple of real numbers {x,y,z,w}
    private UnityEngine.Vector3 position;
    public Slider m_slide, ad_slide, t_slide, y_slide, r_slide, p_slide, n_slide;
    public VRC.SDK3.Components.VRCStation seat;
    private UnityEngine.Vector3 directionVectorFwd, directionVectorBwd, directionVectorRight, directionVectorLeft;
    public UnityEngine.Vector3 directionVector;
    public bool resultantDirection = true;
    private float vrPitch, vrThrottle, vrYaw, vrRoll, input_horizontal_axis, input_vertical_axis, desktopPitch, desktopRoll;
    private bool resetPosition = false;

    public void Start()
    {
        seat.disableStationExit = true;
        rotation = transform.rotation;
        position = transform.position;
        seatCollider = _seat.GetComponent<BoxCollider>();
    }

    void ResetPosition(){
        rigid.MovePosition(position);
        rigid.MoveRotation(rotation);
    }

    void DesktopControls(){

            if (input_horizontal_axis != 0)
            {
                rigid.AddRelativeTorque(input_horizontal_axis * Vector3.up * (vrYawSpeed / 2), ForceMode.Force);
            }
            if (input_vertical_axis != 0)
            {
                rigid.AddRelativeForce(Vector3.up * thrustSpeed * input_vertical_axis, ForceMode.Force);
            }
            if (desktopPitch != 0)
            {
                rigid.AddRelativeTorque(desktopPitch * Vector3.right * (rollSpeed / 2), ForceMode.Force);
            }
            if (desktopRoll != 0)
            {
                rigid.AddRelativeTorque(desktopRoll * Vector3.forward * (rollSpeed / 2), ForceMode.Force);
            }
    }

    void VRControls(){

            if (vrYaw != 0)
            {
                rigid.AddRelativeTorque(Vector3.up * (vrYawSpeed / 2) * vrYaw, ForceMode.Force);
            }

            if (vrThrottle >= 0)
            {
                rigid.AddRelativeForce(Vector3.up * vrThrottle * thrustSpeed, ForceMode.Force);
            }

            if (vrRoll != 0)
            {
                rigid.AddRelativeTorque(-Vector3.forward * (rollSpeed / 2) * vrRoll, ForceMode.Force);
            }

            if (vrPitch != 0)
            {
                rigid.AddRelativeTorque(Vector3.right * (rollSpeed / 2) * vrPitch, ForceMode.Force);
            }
    }

    private void Update()
    {
        vrPitch = Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical") * p_slide.value;
        vrThrottle = Input.GetAxis("Oculus_CrossPlatform_PrimaryThumbstickVertical") * t_slide.value;
        vrYaw = Input.GetAxis("Oculus_CrossPlatform_PrimaryThumbstickHorizontal") * y_slide.value;
        vrRoll = Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickHorizontal") * r_slide.value;

        input_horizontal_axis = Input.GetAxis("Horizontal");
        input_vertical_axis = Input.GetAxis("Vertical");
        desktopPitch = 0.0f;
        if(Input.GetKey(KeyCode.UpArrow)) desktopPitch = 1.0f;
        if(Input.GetKey(KeyCode.DownArrow)) desktopPitch = -1.0f;
        desktopRoll = 0.0f;
        if(Input.GetKey(KeyCode.LeftArrow)) desktopRoll = 1.0f;
        if(Input.GetKey(KeyCode.RightArrow)) desktopRoll = -1.0f;

        rigid.angularDrag = ad_slide.value;
        rigid.mass = m_slide.value;

        if(seated){
            seatCollider.enabled=false;
            // HandleRotations();
        }
        else{
            seatCollider.enabled=true;
        }

        //Reset position of drone
        if (Input.GetButtonDown("Oculus_CrossPlatform_PrimaryThumbstick") || Input.GetKeyDown(KeyCode.R))
        {
            ResetPosition();
        }
    }
    private void FixedUpdate()
    {
        

        if(seated){
            rigid.AddForce(Vector3.down * gScale.magnitude, ForceMode.Acceleration);

            bool isThrusting = (vrThrottle > 0) || (input_vertical_axis != 0);

            if(!isThrusting && rigid.velocity.y > 0)    //no input & still moving up, apply drag
            {
                UnityEngine.Debug.Log("Apply drag");
                Vector3 downwardDrag = Vector3.down * dragValue * rigid.velocity.y;
                rigid.AddForce(downwardDrag, ForceMode.Acceleration);
            }

            if (vrPitch != 0 || vrThrottle != 0 || vrYaw != 0 || vrRoll != 0)
            {
                VRControls();
            }
            else
            {
                DesktopControls();
            }
        }
    }

}
