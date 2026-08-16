using UnityEngine;

// the level in which you run from greg

public class lvl_gregrun : MonoBehaviour
{
    private lvl_generic g;
    

    // the way the poi system works is that most pois are spawned in by default,
    // but some have to be requested
    // the requested ones vary by level,
    // and so are included here
    public string[] special_pois;

    [Header("INTRO")]
    public TextSequence level_intro_data;

    void Awake()
    {
        g =GetComponent<lvl_generic>();

        g.onLevelEnter.AddListener(OnLevelEnter);
    }

    public void OnLevelEnter()
    {
        // first we teleport the player to the origin and freeze them there for a bit
        Player.TeleportTo(new Vector3(0,3,0));
        // then we generate the level (chunks, lake, POIs, etc.)
        WorldManager.InitializeLevelEnvironment(new NoiseProfile(), special_pois);
        
        // once the 'lookout' POI has spawned in (that's the spawn point for this level),
        // the player will be automatically teleported to it
        // and hopefully will nolonger be hovering over the lake
        

        // set the camera mode
        CameraController.SetControlMode(CameraControlMode.PlayerFirstPerson);

        // making sure inventory, etc. is visible
        UIManager.SwitchMenu("");
        UIManager.ShowPlayerHUD();

        // provided i have loading screens turned on,
        // this will draw all of the intro text
        UIManager.PlayLevelIntro(level_intro_data);
    }
}
