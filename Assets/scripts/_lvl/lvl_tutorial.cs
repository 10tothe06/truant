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

    // the player spawns sitting at this chair, at a table
    public Transform t_playerSpawn;
    public int_chair player_spawn_chair;
    public CarController player_car;

    void Awake()
    {
        g =GetComponent<lvl_generic>();

        g.onLevelEnter.AddListener(OnLevelEnter);
    }

    void Update()
    {
        // once the player has picked up the note, we can let them go
        if (Player.GetSelectedItemData()!= null)
        {
            if (Player.GetSelectedItemData().item_name == "note")
            {
                player_spawn_chair.Unlock();
            }
            
        }
    }

    // called by the generic component
    public void OnLevelEnter()
    {
        player_car = ObjectManager.SpawnObject("car_1", Vector3.forward * 5).GetComponent<CarController>();

        // making it so that starting the car finishes the level
        player_car.onEngineStart.AddListener(() => g.ExitLevel(true));
        
        UIManager.SwitchMenu("");

        // move the player to the starting point of the level
        Player.TeleportTo(t_playerSpawn.position);
        player_spawn_chair.Sit();
        player_spawn_chair.Lock();


        // set the camera mode
        CameraController.SetControlMode(CameraControlMode.PlayerFirstPerson);

        UIManager.ShowPlayerHUD();

        
        UIManager.PlayLevelIntro(level_intro_data);
    }
}
