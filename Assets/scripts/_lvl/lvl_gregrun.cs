using UnityEngine;

// the level in which you run from greg

public class lvl_gregrun : MonoBehaviour
{
    private lvl_generic g;

    [Header("INTRO")]
    public TextSequence level_intro_data;

    void Awake()
    {
        g =GetComponent<lvl_generic>();

        g.onLevelEnter.AddListener(OnLevelEnter);
    }

    public void OnLevelEnter()
    {
        Player.TeleportTo(new Vector3(0,3,0));
        WorldManager.InitializeChunkGeneration(new NoiseProfile());

        UIManager.SwitchMenu("");

        // set the camera mode
        CameraController.SetControlMode(CameraControlMode.PlayerFirstPerson);

        UIManager.ShowPlayerHUD();

        
        UIManager.PlayLevelIntro(level_intro_data);
    }
}
