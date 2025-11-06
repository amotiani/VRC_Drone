using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;

public class new_drone : base_drone
{   
    [Header("New Drone Specific")]
    [SerializeField] public float stabilizationForce = 10f;
    [SerializeField] public float my_upward_drag;
    public bool stabilize;

    // Input state
    private float inputDeadzone = 0.1f;
    private bool isPiloting;
    
    // 'override' adds to the parent's Start() method
    public override void Start()
    {
        base.Start(); // This runs the Start() logic from BaseDroneController
        rb.useGravity = false; // This is unique to new_drone
        rb.drag = 0;
        // The rest of the shared Start() logic is in the base class
    }

    // 'override' adds to the parent's Update() method
    public override void Update()
    {
        base.Update(); // This runs all the input and slider logic from the base class

        // To check if input is given
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

            // Use the input variables from the base class
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

            // Call the control methods from the base class
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