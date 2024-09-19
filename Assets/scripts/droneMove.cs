﻿using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class droneMove : UdonSharpBehaviour
{
    public Rigidbody rigid;
    [SerializeField] public float rotateSpeed, moveSpeed, yawSpeed, fwdSpeed;
    public float ogRotateSpeed, ogMoveSpeed, ogYawSpeed, ogFwdSpeed, maxAngularVel = 2f;
    public GameObject sliderr;
    public bool seated=false;
    public VRCObjectSync obj;
    private bool grounded = true;
    Quaternion rotation;
    Vector3 position;
    public Slider m_slide, d_slide, ad_slide, t_slide, y_slide, r_slide, p_slide, n_slide, g_slide;
    public VRC.SDK3.Components.VRCStation seat;
    private Vector3 gScale, directionVectorFwd, directionVectorBwd, directionVectorRight, directionVectorLeft;
    public Vector3 directionVector;
    public bool resultantDirection = true;
    private float pitch, throttle, yaw, roll, nitro, mass, drag;

    public void Start()
    {
        seat.disableStationExit = true;
        mass = rigid.mass;
        drag = rigid.drag;
        rotation = transform.rotation;
        position = transform.position;
        directionVectorFwd = (transform.up + transform.forward).normalized;
        directionVectorBwd = (transform.up - transform.forward).normalized;
        directionVectorRight = (transform.up + transform.right).normalized;
        directionVectorLeft = (transform.up - transform.right).normalized;
    }

    void ResetPosition(){
        transform.position = position;
        transform.rotation = rotation;
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
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

        Physics.gravity = new Vector3(0,-g_slide.value,0);

        if (angleZ< 7f && angleZ > -7f && angleX < 7f && angleX > -7f)
        {
            //Debug.Log("up");
            resultantDirection=false;
        }
        else if (angleX > 7f && angleX < 88f)
        {
            //Debug.Log("fwd");
            directionVector = directionVectorFwd;
            resultantDirection = true;
        }
        else if (angleX < 7f && angleX > 88f)
        {
            //Debug.Log("bwd");
            directionVector = directionVectorBwd;
            resultantDirection = true;
        }
        else if (angleZ < 7f && angleZ > 88f)
        {
            //Debug.Log("right");
            directionVector = directionVectorRight;
            resultantDirection=false;
        }
        else if (angleZ > 7f && angleZ < 88f)
        {
            //Debug.Log("left");
            directionVector = directionVectorLeft;
            resultantDirection=false;
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
                    rigid.AddRelativeForce(directionVector * throttle * moveSpeed, ForceMode.Impulse);
                }
            }

            if (roll != 0)
            {
                rigid.AddRelativeForce(-Vector3.forward * roll * Time.deltaTime * 10f, ForceMode.Impulse);
                rigid.AddRelativeTorque(-Vector3.forward * (rotateSpeed / 2) * roll * Time.deltaTime, ForceMode.Impulse);
            }

            if (pitch != 0)
            {
                rigid.AddRelativeForce(Vector3.right * pitch * Time.deltaTime * 10f, ForceMode.Impulse);
                rigid.AddRelativeTorque(Vector3.right * (rotateSpeed / 2) * pitch * Time.deltaTime, ForceMode.Impulse);
            }
    }

    private void Update()
    {
        rigid.drag = d_slide.value;
        rigid.angularDrag = ad_slide.value;
        rigid.mass = m_slide.value;
        if(seated){
            HandleRotations();
        }

        //Reset position of drone
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