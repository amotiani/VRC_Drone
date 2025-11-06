using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class new_drone : UdonSharpBehaviour
{
    VRCPlayerApi playerApi;
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
    float pitch;
    float throttle;
    float yaw;
    float roll;
    float nitro;
    
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

    void DesktopControls(){
            if (Input.GetKey(KeyCode.A))
            {
                rigid.AddRelativeTorque(-Vector3.up * (yawSpeed / 2) * Time.fixedDeltaTime, ForceMode.Impulse);
            }
            if (Input.GetKey(KeyCode.D))
            {
                rigid.AddRelativeTorque(Vector3.up * (yawSpeed / 2) * Time.fixedDeltaTime, ForceMode.Impulse);
            }
            if (Input.GetKey(KeyCode.W))
            {
                rigid.AddRelativeForce(Vector3.up * moveSpeed, ForceMode.Impulse);

            }
            if (Input.GetKey(KeyCode.S))
            {
                rigid.AddRelativeForce(-Vector3.up * moveSpeed, ForceMode.Impulse);
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                rigid.AddRelativeTorque(Vector3.right * (rotateSpeed / 2) * Time.fixedDeltaTime, ForceMode.Impulse);
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                rigid.AddRelativeTorque(-Vector3.right * (rotateSpeed / 2) * Time.fixedDeltaTime, ForceMode.Impulse);
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                rigid.AddRelativeTorque(Vector3.forward * (rotateSpeed / 2) * Time.fixedDeltaTime, ForceMode.Impulse);
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                rigid.AddRelativeTorque(-Vector3.forward * (rotateSpeed / 2) * Time.fixedDeltaTime, ForceMode.Impulse);
            }
    }
    void VRControls(){
        if (Input.GetButton("Oculus_CrossPlatform_SecondaryThumbstick") || Input.GetKeyDown(KeyCode.T))
            {
                rigid.AddRelativeForce(Vector3.forward * moveSpeed * 5f, ForceMode.Impulse);
            }

            if (yaw != 0)
            {
                rigid.AddRelativeTorque(Vector3.up * (yawSpeed / 2) * yaw * Time.fixedDeltaTime, ForceMode.Impulse);
            }

            if (throttle >= 0)
            {
                    rigid.AddRelativeForce(Vector3.up * throttle * moveSpeed, ForceMode.Impulse);
            }
            if (roll != 0)
            {
                rigid.AddRelativeTorque(-Vector3.forward * (rotateSpeed / 2) * roll * Time.fixedDeltaTime, ForceMode.Impulse);
            }

            if (pitch != 0)
            {
                rigid.AddRelativeTorque(Vector3.right * (rotateSpeed / 2) * pitch * Time.fixedDeltaTime, ForceMode.Impulse);
            }
    }

    private void Update()
    {
        Physics.gravity = new Vector3(0, -g_slide.value, 0);
        rigid.drag = d_slide.value;
        rigid.angularDrag = ad_slide.value;
        rigid.mass = m_slide.value;
        //Handle Reset
        if (Input.GetButtonDown("Oculus_CrossPlatform_PrimaryThumbstick") || Input.GetKeyDown(KeyCode.R))
        {
            ResetPosition();
        }
    }

    private void FixedUpdate()
    {
        pitch = Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical") * p_slide.value;
        throttle = Input.GetAxis("Oculus_CrossPlatform_PrimaryThumbstickVertical") * t_slide.value;
        yaw = Input.GetAxis("Oculus_CrossPlatform_PrimaryThumbstickHorizontal") * y_slide.value;
        roll = Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickHorizontal") * r_slide.value;
        nitro = fwdSpeed * n_slide.value;

        if(seated){
            //VR Controls
            VRControls();
            /* DESKTOP CONTROLS */
            DesktopControls();
        }
    }
}
