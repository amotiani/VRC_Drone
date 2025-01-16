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
    public droneMove scriptDroneMove;

    private void Start() {
        scriptDroneMove=bike.GetComponent<droneMove>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.B) && scriptDroneMove.seated == true)        //B on quest
        {
            c1.enabled=!c1.enabled;
            c2.enabled= !c2.enabled;
        }
        if(scriptDroneMove.seated==false){
            c1.enabled=true;
            c2.enabled=false;
        }
    }
    public override void OnStationEntered(VRCPlayerApi player)
    {
        Networking.SetOwner(player, bike);
        scriptDroneMove.seated = true;
        Debug.Log("seated true");
        player.SetJumpImpulse(0);
        player.SetRunSpeed(0);
        player.SetStrafeSpeed(0);
        player.SetWalkSpeed(0);
    }
    public override void OnStationExited(VRCPlayerApi player)
    {
        scriptDroneMove.seated = false;
        Debug.Log("seated false");
        player.SetGravityStrength(1);
        player.SetJumpImpulse(5);
        player.SetRunSpeed(8);
        player.SetStrafeSpeed(9);
        player.SetWalkSpeed(4);
    }
}
