using UdonSharp;
using UnityEngine;

public class droneMove : base_drone
{
    [Header("DroneV1 Specific")]
    private BoxCollider seatCollider;
    [SerializeField] public float dragValue;
    public GameObject _seat;

    
    public override void Start()
    {
        base.Start();
        
        seatCollider = _seat.GetComponent<BoxCollider>();
    }

    public override void Update()
    {
        base.Update();
        
        if(seated){
            seatCollider.enabled=false;
        }
        else{
            seatCollider.enabled=true;
        }
    }

    private void FixedUpdate()
    {
        if(seated){
            rb.AddForce(Vector3.down * gScale.magnitude, ForceMode.Acceleration);

            bool isThrusting = (vrThrottle > 0) || (desktopThrottle != 0);

            if(!isThrusting && rb.velocity.y > 0)
            {
                Vector3 downwardDrag = Vector3.down * dragValue * rb.velocity.y;
                rb.AddForce(downwardDrag, ForceMode.Acceleration);
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
