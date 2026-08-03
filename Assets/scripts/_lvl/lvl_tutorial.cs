using System.Diagnostics;
using UnityEngine;

// players load in to this level when they first boot up the game,
// and ofc they can select it from the lobby too

public class lvl_tutorial : MonoBehaviour
{
    private lvl_generic g;

    [Header("INTRO")]
    public TextSequence level_intro_data;


    [Space(20)]
    [Header("CONFIG")]
    public Transform player_spawn_position;
    public CarController player_car;

    void Awake()
    {
        g =GetComponent<lvl_generic>();

        g.onLevelEnter.AddListener(OnLevelEnter);

        // making it so that starting the car finishes the level
        player_car.onEngineStart.AddListener(() => g.ExitLevel(true));
    }

    // called by the generic component
    public void OnLevelEnter()
    {
        UIManager.SwitchMenu("");

        // move the player to the starting point of the level
        Player.TeleportTo(player_spawn_position.position);


        // set the camera mode
        CameraController.SetControlMode(CameraControlMode.PlayerFirstPerson);

        UIManager.ShowPlayerHUD();

        
        UIManager.PlayLevelIntro(level_intro_data);


        /*
        TEMP vvv
        */

        Player.player_inventory.AddItem(new inv_itemstack(0, 1, 0));
        Player.player_inventory.AddItem(new inv_itemstack(2, 1, 1));
    }
}
