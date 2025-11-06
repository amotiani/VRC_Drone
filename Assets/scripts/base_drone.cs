using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Components;
using VRC.SDKBase;

public class base_drone : UdonSharpBehaviour
{
    [Header("Core Components")]
    public Rigidbody rb;
    public VRC.SDK3.Components.VRCStation seat;
    public bool seated = false;
    
    [Header("Physics Settings")]
    [SerializeField] public float rollSpeed;
    [SerializeField] public float thrustSpeed;
    [SerializeField] public float yawSpeed;
    public Vector3 gScale = new Vector3(0, -9.81f, 0);

    [Header("UI Sliders")]
    public Slider m_slide;
    public Slider ad_slide;
    public Slider t_slide;
    public Slider y_slide;
    public Slider r_slide;
    public Slider p_slide;
    public Slider n_slide;

    // Reset
    protected Quaternion rotation;
    protected Vector3 position;

    // Input Cache (protected so child classes can read them)
    protected float desktopPitch, desktopRoll, desktopYaw, desktopThrottle;
    protected float vrPitch, vrThrottle, vrYaw, vrRoll;
    protected bool resetInput;

    // 'virtual' allows child classes to add to this method
    public virtual void Start()
    {
        seat.disableStationExit = true;
        rotation = transform.rotation;
        position = transform.position;
    }

    public void ResetPosition()
    {
        rb.MovePosition(position);
        rb.MoveRotation(rotation);
    }

    // 'virtual' allows child classes to override this method
    public virtual void Update()
    {
        // --- Read All Inputs ---
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

        if (resetInput)
        {
            ResetPosition();
        }

        rb.angularDrag = ad_slide.value;
        rb.mass = m_slide.value;
    }

    // (protected so they can only be called by this class or a child class)
    
    protected void DesktopControls()
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

    protected void VRControls()
    {
        if (vrYaw != 0)
        {
            rb.AddRelativeTorque(Vector3.up * (yawSpeed / 2) * vrYaw, ForceMode.Force);
        }

        if (vrThrottle > 0)
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
}