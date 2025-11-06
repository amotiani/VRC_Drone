using UdonSharp;
using UnityEngine;

// Notice it inherits from our new base class
public class droneMove : base_drone
{
    [Header("DroneV1 Specific")]
    private BoxCollider seatCollider;
    [SerializeField] public float dragValue;
    public GameObject _seat;

    // 'override' adds to the parent's Start() method
    public override void Start()
    {
        base.Start(); // This runs the Start() logic from BaseDroneController
        
        // This logic is unique to droneMove
        seatCollider = _seat.GetComponent<BoxCollider>();
    }

    // 'override' adds to the parent's Update() method
    public override void Update()
    {
        base.Update(); // This runs all the input, slider, and reset logic
        
        // This logic is unique to droneMove
        if(seated){
            seatCollider.enabled=false;
        }
        else{
            seatCollider.enabled=true;
        }
    }

    // This FixedUpdate is unique to droneMove
    private void FixedUpdate()
    {
        if(seated){
            rb.AddForce(Vector3.down * gScale.magnitude, ForceMode.Acceleration);

            // Use the input variables from the base class (desktopThrottle)
            bool isThrusting = (vrThrottle > 0) || (desktopThrottle != 0);

            if(!isThrusting && rb.velocity.y > 0)
            {
                Vector3 downwardDrag = Vector3.down * dragValue * rb.velocity.y;
                rb.AddForce(downwardDrag, ForceMode.Acceleration);
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