using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class new_drone : UdonSharpBehaviour
{   // Third Person Perspective Drone with Stabilizer
    public Rigidbody rb;
    [SerializeField] public float rollSpeed;
    [SerializeField] public float thrustSpeed;
    [SerializeField] public float yawSpeed, stabilizationForce = 10f;
    public bool seated=false, stabilize;
    public VRCObjectSync obj;
    public float maxAngularVel = 2f;
    private float mass;
    private float drag;
    public float my_upward_drag;
    private bool grounded = true;
    Quaternion rotation;
    Vector3 position;
    public Slider m_slide;
    // public Slider d_slide;
    public Slider ad_slide;
    public Slider t_slide;
    public Slider y_slide;
    public Slider r_slide;
    public Slider p_slide;
    public Slider n_slide;
    // public Slider g_slide;
    public VRC.SDK3.Components.VRCStation seat;
    public Vector3 gScale = new Vector3(0,-9.81f,0);
    public Vector3 directionVector;
    private Vector3 directionVectorFwd;
    private Vector3 directionVectorBwd;
    private Vector3 directionVectorRight;
    private Vector3 directionVectorLeft;
    [SerializeField]private float time;

    //Input cache
    private float desktopPitch, desktopRoll, desktopYaw, desktopThrottle;
    private float vrPitch;
    private float vrThrottle, inputDeadzone = 0.1f;
    private float vrYaw;
    private float vrRoll;
    private bool resetInput, isPiloting;
    
    public void Start()
    {
        rb.useGravity = false;
        rb.drag = 0;

        seat.disableStationExit = true;
        mass = rb.mass;
        drag = rb.drag;
        rotation = transform.rotation;
        position = transform.position;
        directionVectorFwd = transform.up + transform.forward;
        directionVectorBwd = transform.up - transform.forward;
        directionVectorRight = transform.up + transform.right;
        directionVectorLeft = transform.up - transform.right;
    }
    void ResetPosition(){
        rb.MovePosition(position);
        rb.MoveRotation(rotation);
    }

    void DesktopControls()
    {
        if (desktopYaw != 0)
        {
            rb.AddRelativeTorque(desktopYaw * Vector3.up * (yawSpeed / 2), ForceMode.Force);
        }

        if (desktopThrottle != 0)
        {
            rb.AddRelativeForce(desktopThrottle * Vector3.up * thrustSpeed, ForceMode.Force);
        }

        if (desktopPitch != 0)
        {
            rb.AddRelativeTorque(Vector3.right * (rollSpeed / 2) * desktopPitch, ForceMode.Force);
        }

        if (desktopRoll != 0)
        {
            rb.AddRelativeTorque(Vector3.forward * (rollSpeed / 2) * desktopRoll, ForceMode.Force);
        }

    }
    void VRControls()
    {
        if (vrYaw != 0)
        {
            rb.AddRelativeTorque(Vector3.up * (yawSpeed / 2) * vrYaw, ForceMode.Force);
        }

        if (vrThrottle >= 0)
        {
            rb.AddRelativeForce(Vector3.up * vrThrottle * thrustSpeed, ForceMode.Force);
        }
        if (vrRoll != 0)
        {
            rb.AddRelativeTorque(-Vector3.forward * (rollSpeed / 2) * vrRoll, ForceMode.Force);
        }

        if (vrPitch != 0)
        {
            rb.AddRelativeTorque(Vector3.right * (rollSpeed / 2) * vrPitch, ForceMode.Force);
        }
    }

    void Stabilizer()
    {
        Vector3 worldUp = Vector3.up;
        Vector3 droneUp = transform.up;

        float alignment = Vector3.Dot(worldUp, droneUp); // 1 if both vectors are in same direction, 0 if both are perpendicular. 
        // Our stabilization force needs to be 0 if alignment is 1. 
        // So we use 'error' to scale the stabilization force.
        float error = Mathf.Clamp01(1.0f - alignment);

        Vector3 stabilizationAxis = Vector3.Cross(droneUp, worldUp);

        Vector3 alignForce = error * stabilizationAxis * stabilizationForce;

        rb.AddTorque(alignForce, ForceMode.Acceleration);

    }
    private void Update()
    {
        // Read all input in Update

        desktopYaw = Input.GetAxis("Horizontal");
        desktopThrottle = Input.GetAxis("Vertical");

        desktopPitch = 0.0f;
        if(Input.GetKey(KeyCode.UpArrow)) desktopPitch = 1.0f;
        if(Input.GetKey(KeyCode.DownArrow)) desktopPitch = -1.0f;
        desktopRoll = 0.0f;
        if(Input.GetKey(KeyCode.LeftArrow)) desktopRoll = 1.0f;
        if(Input.GetKey(KeyCode.RightArrow)) desktopRoll = -1.0f;
        
        vrPitch = Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical") * p_slide.value;
        vrThrottle = Input.GetAxis("Oculus_CrossPlatform_PrimaryThumbstickVertical") * t_slide.value;
        vrYaw = Input.GetAxis("Oculus_CrossPlatform_PrimaryThumbstickHorizontal") * y_slide.value;
        vrRoll = Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickHorizontal") * r_slide.value;
        resetInput = Input.GetButtonDown("Oculus_CrossPlatform_PrimaryThumbstick") || Input.GetKeyDown(KeyCode.R);

        isPiloting =
            Mathf.Abs(vrPitch) > inputDeadzone ||
            Mathf.Abs(vrRoll) > inputDeadzone ||
            Mathf.Abs(vrYaw) > inputDeadzone ||
            Mathf.Abs(desktopPitch) > inputDeadzone ||
            Mathf.Abs(desktopRoll) > inputDeadzone ||
            Mathf.Abs(desktopYaw) > inputDeadzone;
        
        //Handle Reset
        if (resetInput)
        {
            ResetPosition();
        }

        // rb.drag = d_slide.value;
        rb.angularDrag = ad_slide.value;
        rb.mass = m_slide.value;
    }

    private void FixedUpdate()
    {
        if(seated){
            rb.AddForce(Vector3.down * gScale.magnitude, ForceMode.Acceleration);

            bool isThrusting = (vrThrottle > 0) || (desktopThrottle != 0);

            if (!isThrusting && rb.velocity.y > 0)
            {
                UnityEngine.Debug.Log("Upward BRAKE // DRAG exerted");
                // This force ONLY kills upward velocity. It never fights gravity. 
                // Decoupling gravity & drag to only keep upward drag. Gravity is separately exerted.
                Vector3 upwardBrakeForce = Vector3.down * rb.velocity.y * my_upward_drag;
                rb.AddForce(upwardBrakeForce, ForceMode.Acceleration);
            }

            if(!isPiloting && stabilize)
            {
                //invoke coroutine for stabilizer invoking after few seconds.
                Stabilizer();
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
