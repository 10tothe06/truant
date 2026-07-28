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
        Player.TeleportTo(player_spawn_position.position);
    }
}
