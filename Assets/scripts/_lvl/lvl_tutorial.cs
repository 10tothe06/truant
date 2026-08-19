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
    public int_chair player_spawn_chair;
    public CarController player_car;

    void Awake()
    {
        g =GetComponent<lvl_generic>();

        g.onLevelEnter.AddListener(OnLevelEnter);
    }

    // called by the generic component
    public void OnLevelEnter()
    {
        player_car = ObjectManager.SpawnObject("car_1", Vector3.forward * 5).GetComponent<CarController>();

        // making it so that starting the car finishes the level
        player_car.onEngineStart.AddListener(() => g.ExitLevel(true));
        
        UIManager.SwitchMenu("");

        // move the player to the starting point of the level
        player_spawn_chair.Sit();


        // set the camera mode
        CameraController.SetControlMode(CameraControlMode.PlayerFirstPerson);

        UIManager.ShowPlayerHUD();

        
        UIManager.PlayLevelIntro(level_intro_data);
    }
}
