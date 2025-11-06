using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;

public class new_drone : base_drone
{   
    [Header("New Drone Specific")]
    [SerializeField] public float stabilizationForce = 10f;
    [SerializeField] public float my_upward_drag;
    public bool stabilize;

    private float inputDeadzone = 0.1f;
    private bool isPiloting;
    
    public override void Start()
    {
        base.Start(); 
        rb.useGravity = false; 
        rb.drag = 0;
    }

    // 'override' adds to the parent's Update() method
    public override void Update()
    {
        base.Update(); 
        
        isPiloting =
            Mathf.Abs(vrPitch) > inputDeadzone ||
            Mathf.Abs(vrRoll) > inputDeadzone ||
            Mathf.Abs(vrYaw) > inputDeadzone ||
            Mathf.Abs(desktopPitch) > inputDeadzone ||
            Mathf.Abs(desktopRoll) > inputDeadzone ||
            Mathf.Abs(desktopYaw) > inputDeadzone;
    }

    void Stabilizer()
    {
        Vector3 worldUp = Vector3.up;
        Vector3 droneUp = transform.up;
        float alignment = Vector3.Dot(worldUp, droneUp);
        float error = Mathf.Clamp01(1.0f - alignment);
        Vector3 stabilizationAxis = Vector3.Cross(droneUp, worldUp);
        Vector3 alignForce = error * stabilizationAxis * stabilizationForce;
        rb.AddTorque(alignForce, ForceMode.Acceleration);
    }

    private void FixedUpdate()
    {
        if(seated){
            rb.AddForce(Vector3.down * gScale.magnitude, ForceMode.Acceleration);

            bool isThrusting = (vrThrottle > 0) || (desktopThrottle != 0);

            if (!isThrusting && rb.velocity.y > 0)
            {
                Vector3 upwardBrakeForce = Vector3.down * rb.velocity.y * my_upward_drag;
                rb.AddForce(upwardBrakeForce, ForceMode.Acceleration);
            }

            if(!isPiloting && stabilize)
            {
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
