
using UdonSharp;
using UnityEngine;
using VRC.SDK3.ClientSim;
using VRC.SDKBase;
using VRC.Udon;

public class playerBehaviourOnLand : UdonSharpBehaviour
{
    public GameObject bike;
    public Camera c1;
    public Camera c2;

    private void Update()
    {
        if((Input.GetButtonDown("Fire2") || Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.B)) && bike.GetComponent<droneMove>().seated == true)        //B on quest
        {
            c1.enabled=!c1.enabled;
            c2.enabled= !c2.enabled;
        }
    }
    public override void OnStationEntered(VRCPlayerApi player)
    {
        Networking.SetOwner(player, bike);
        droneMove droneScript = bike.GetComponent<droneMove>();
        droneScript.seated = true;
        Debug.Log("seated true");
        player.SetJumpImpulse(0);
        player.SetRunSpeed(0);
        player.SetStrafeSpeed(0);
        player.SetWalkSpeed(0);
    }
    public override void OnStationExited(VRCPlayerApi player)
    {
        droneMove droneScript = bike.GetComponent<droneMove>();
        droneScript.seated = false;
        Debug.Log("seated false");
        player.SetGravityStrength(1);
        player.SetJumpImpulse(5);
        player.SetRunSpeed(8);
        player.SetStrafeSpeed(9);
        player.SetWalkSpeed(4);
    }
}
