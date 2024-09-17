using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class playerBehavior_flyDrone : UdonSharpBehaviour
{
    public GameObject bike;
    public override void OnStationEntered(VRCPlayerApi player)
    {
        Networking.SetOwner(player, bike);
        new_drone droneScript = bike.GetComponent<new_drone>();
        droneScript.seated = true;
        player.SetJumpImpulse(0);
        player.SetRunSpeed(0);
        player.SetStrafeSpeed(0);
        player.SetWalkSpeed(0);
    }
    public override void OnStationExited(VRCPlayerApi player)
    {
        new_drone droneScript = bike.GetComponent<new_drone>();
        droneScript.seated = false;
        player.SetGravityStrength(1);
        player.SetJumpImpulse(5);
        player.SetRunSpeed(8);
        player.SetStrafeSpeed(9);
        player.SetWalkSpeed(4);
    }
}