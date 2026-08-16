using UnityEngine;

// teleports the player to a spawn,
// upon the poi being generated

public class poi_playerspawn : MonoBehaviour
{
    public Transform spawn_position;

    void Awake()
    {
        // so that it gets called after all the positioning stuff is sorted out
        GetComponent<poi_generic>().onInitialize.AddListener(TeleportPlayer);
    }

    public void TeleportPlayer()
    {
        Player.TeleportTo(spawn_position.position);
    }
}
