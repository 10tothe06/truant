using Unity.VisualScripting;
using UnityEngine;


// the board-remover

public class int_hammer : MonoBehaviour
{
    void Awake()
    {
        if (Player.item_holder != null)
        {
            // checking IF the item is being held by the player,
            // and IF SO, pass the note information to the note HUD
            Player.item_holder.onUpdateHeldObject.AddListener(OnItemHeld);
        }
    }

    void Update()
    {
        if (Player.GetHeldObject() == gameObject)
        {
            if (Input.mouseButtonDownRight)
            {
                // first, check if we are looking at a board
                int_board lookingAt = null;

                RaycastHit hit;

                if (util_physics.LookRaycast(out hit, 10f, ~0))
                {

                    // holy fucking GetComponent<>() calls
                    // this shit is ass

                    if (hit.collider.gameObject.GetComponent<int_board>() != null)
                    {
                        lookingAt = hit.collider.gameObject.GetComponent<int_board>();
                    } else if (hit.collider.gameObject.GetComponent<InteractCollider>())
                    {
                        if (hit.collider.gameObject.GetComponent<InteractCollider>().parentObject.GetComponent<int_board>() != null)
                        {
                            lookingAt = hit.collider.gameObject.GetComponent<InteractCollider>().parentObject.GetComponent<int_board>();
                        }
                    }



                }

                if (lookingAt != null)
                {
                    lookingAt.PopOff();
                }
            }
        }
    }

    void OnItemHeld()
    {
        if (Player.GetHeldObject() == gameObject)
        {
            // tell the player HUD to show the player the "press E to read" prompt
            ui_playerhud.DrawItemPrompts(gameObject, new string[] {"press RMB to remove boards"});
        } else
        {
            if (UIManager.Instance.player_hud.g_promptObject == gameObject)
            {
                ui_playerhud.ClearItemPrompt();
            }
        }
    }
}
