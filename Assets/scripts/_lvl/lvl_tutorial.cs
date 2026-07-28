using System.Diagnostics;
using UnityEngine;

// players load in to this level when they first boot up the game,
// and ofc they can select it from the lobby too

public class lvl_tutorial : MonoBehaviour
{
    public Transform player_spawn_position;

    void Awake()
    {
        GetComponent<lvl_generic>().onLevelEnter.AddListener(OnLevelEnter);
    }

    // called by the generic component
    public void OnLevelEnter()
    {
        UIManager.SwitchMenu("");

        // move the player to the starting point of the level
        Player.TeleportTo(player_spawn_position.position);


        // set the camera mode
        CameraController.SetControlMode(CameraControlMode.PlayerFirstPerson);
    }
}
