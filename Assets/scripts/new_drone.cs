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
    [SerializeField] private GameObject _seat;
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
    public bool resultantDirection = true;
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
    private void Update()
    {
        rigid.drag = d_slide.value;
        rigid.angularDrag = ad_slide.value;
        rigid.mass = m_slide.value;
        //Reset position of drone.
        if (Input.GetButtonDown("Oculus_CrossPlatform_PrimaryThumbstick") || Input.GetKeyDown(KeyCode.R))
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
            transform.position = position;
            transform.rotation = rotation;
        }

        //Handle Rotations
        if(seated){
            _seat.SetActive(false);
            HandleRotations();
        }
        else{
            _seat.SetActive(true);
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
        //DESKTOP CONTROLS
        DesktopControls();
        }
    }

    //Function to convert euler angles from 0,360 scale to -180,180 scale. This allows for checking -ve degree rotations.
    private float NormalizeAngle(float angle){
        if(angle>180f){
            angle-=360f;
        }
        return angle;
    }
    void HandleRotations(){

        float angleX = NormalizeAngle(transform.rotation.eulerAngles.x);
        float angleY = NormalizeAngle(transform.rotation.eulerAngles.y);
        float angleZ = NormalizeAngle(transform.rotation.eulerAngles.z);

            if (angleZ< 7f && angleZ > -7f && angleX < 7f && angleX > -7f)
            {
                Debug.Log("up");
                rigid.AddRelativeForce(Vector3.up * droneIdleSpeed);
                resultantDirection=false;
            }
            else if (angleX > 7f && angleX < 88f)
            {
                //Debug.Log("fwd");
                directionVector = directionVectorFwd;
                resultantDirection = true;
                rigid.AddRelativeForce(Vector3.forward * droneIdleSpeed);
            }
            else if (angleX < 7f && angleX > 88f)
            {
                //Debug.Log("bwd");
                directionVector = directionVectorBwd;
                resultantDirection = true;
                rigid.AddRelativeForce(-Vector3.forward * droneIdleSpeed);
            }
            else if (angleZ < 7f && angleZ > 88f)
            {
                //Debug.Log("right");
                directionVector = directionVectorRight;
                resultantDirection=false;
                rigid.AddRelativeForce(Vector3.right * droneIdleSpeed);
            }
            else if (angleZ > 7f && angleZ < 88f)
            {
                //Debug.Log("left");
                directionVector = directionVectorLeft;
                resultantDirection=false;
                rigid.AddRelativeForce(-Vector3.right * droneIdleSpeed);
            }
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
                if (!resultantDirection)
                {
                    rigid.AddRelativeForce(Vector3.up * moveSpeed, ForceMode.Impulse);
                }
                else
                {
                    rigid.AddRelativeForce(directionVector * (moveSpeed / 2), ForceMode.Impulse);
                }
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
                rigid.AddRelativeTorque(Vector3.up * (yawSpeed / 2) * yaw * Time.deltaTime, ForceMode.Impulse);
            }

            if (throttle >= 0)
            {
                if (!resultantDirection)
                {
                    rigid.AddRelativeForce(Vector3.up * throttle * moveSpeed, ForceMode.Impulse);
                }
                else
                {
                    rigid.AddRelativeForce(directionVector * throttle * (moveSpeed / 2), ForceMode.Impulse);
                }
            }

            if (roll != 0)
            {
                rigid.AddRelativeTorque(-Vector3.forward * (rotateSpeed / 2) * roll * Time.deltaTime, ForceMode.Impulse);
            }

            if (pitch != 0)
            {
                rigid.AddRelativeTorque(Vector3.right * (rotateSpeed / 2) * pitch * Time.deltaTime, ForceMode.Impulse);
            }
    }

}