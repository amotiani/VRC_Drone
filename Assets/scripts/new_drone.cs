using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class new_drone : UdonSharpBehaviour
{
    public Rigidbody rigid;
    [SerializeField] public float rotateSpeed;
    [SerializeField] public float moveSpeed;
    [SerializeField] public float droneIdleSpeed;
    [SerializeField] public float yawSpeed;
    [SerializeField] public float fwdSpeed;
    public float ogRotateSpeed;
    public float ogMoveSpeed;
    public float ogYawSpeed;
    public float ogFwdSpeed;
    public bool seated=false;
    public VRCObjectSync obj;
    public float maxAngularVel = 2f;
    private float mass;
    private float drag;
    private bool grounded = true;
    Quaternion rotation;
    Vector3 position;
    public Slider m_slide;
    public Slider d_slide;
    public Slider ad_slide;
    public Slider t_slide;
    public Slider y_slide;
    public Slider r_slide;
    public Slider p_slide;
    public Slider n_slide;
    public Slider g_slide;
    public VRC.SDK3.Components.VRCStation seat;
    private Vector3 gScale;
    public Vector3 directionVector;
    private Vector3 directionVectorFwd;
    private Vector3 directionVectorBwd;
    private Vector3 directionVectorRight;
    private Vector3 directionVectorLeft;
    [SerializeField]private float time;

    //Input cache
    private float input_horizontal_axis;
    private float input_vertical_axis;
    private float vrPitch;
    private float vrThrottle;
    private float vrYaw;
    private float vrRoll;
    private bool resetInput;
    
    public void Start()
    {
        seat.disableStationExit = true;
        mass = rigid.mass;
        drag = rigid.drag;
        rotation = transform.rotation;
        position = transform.position;
        directionVectorFwd = transform.up + transform.forward;
        directionVectorBwd = transform.up - transform.forward;
        directionVectorRight = transform.up + transform.right;
        directionVectorLeft = transform.up - transform.right;
    }
    void ResetPosition(){
        rigid.MovePosition(position);
        rigid.MoveRotation(rotation);
    }

    void DesktopControls()
    {
        if (input_horizontal_axis != 0)
        {
            rigid.AddRelativeTorque(input_horizontal_axis * Vector3.up * (yawSpeed / 2), ForceMode.Force);
        }

        if (input_vertical_axis != 0)
        {
            rigid.AddRelativeForce(input_vertical_axis * Vector3.up * moveSpeed, ForceMode.Force);
        }
    }
    void VRControls(){
            if (vrYaw != 0)
            {
                rigid.AddRelativeTorque(Vector3.up * (yawSpeed / 2) * vrYaw, ForceMode.Force);
            }

            if (vrThrottle >= 0)
            {
                rigid.AddRelativeForce(Vector3.up * vrThrottle * moveSpeed, ForceMode.Force);
            }
            if (vrRoll != 0)
            {
                rigid.AddRelativeTorque(-Vector3.forward * (rotateSpeed / 2) * vrRoll, ForceMode.Force);
            }

            if (vrPitch != 0)
            {
                rigid.AddRelativeTorque(Vector3.right * (rotateSpeed / 2) * vrPitch, ForceMode.Force);
            }
    }

    private void Update()
    {
        // Read all input in Update
        input_horizontal_axis = Input.GetAxis("Horizontal");
        input_vertical_axis = Input.GetAxis("Vertical");
        
        vrPitch = Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical") * p_slide.value;
        vrThrottle = Input.GetAxis("Oculus_CrossPlatform_PrimaryThumbstickVertical") * t_slide.value;
        vrYaw = Input.GetAxis("Oculus_CrossPlatform_PrimaryThumbstickHorizontal") * y_slide.value;
        vrRoll = Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickHorizontal") * r_slide.value;
        resetInput = Input.GetButtonDown("Oculus_CrossPlatform_PrimaryThumbstick") || Input.GetKeyDown(KeyCode.R);
        
        //Handle Reset
        if (resetInput)
        {
            ResetPosition();
        }

        Physics.gravity = new Vector3(0, -g_slide.value, 0);
        rigid.drag = d_slide.value;
        rigid.angularDrag = ad_slide.value;
        rigid.mass = m_slide.value;
    }

    private void FixedUpdate()
    {
        if(seated){
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
