using UnityEngine;

// an item that, when holding,
// you can throw

// you throw just by clicking, there is no holding process
// (at least not yet)

public class int_throwable : MonoBehaviour
{
    public float throw_force = 10f; 

    void Awake()
    {
        // checking IF the item is being held by the player,
        // and IF SO, pass the note information to the note HUD
        Player.item_holder.onUpdateHeldObject.AddListener(OnItemHeld);
    }

    public void Throw()
    {
        ui_playerhud.ClearItemPrompt();

        // calling this function will result in the destruction of this object,
        // so we really can't run any logic after it
        Player.ThrowSelectedItem(CameraController.t_cam.forward * throw_force + Player.rb.linearVelocity, 

        // throwing in this case also gives a random rotation,
        // to make it look more like you threw it
        new Vector3(Random.Range(-5f,5f),
        Random.Range(-5f,5f),
        Random.Range(-5f,5f)));
    }

    void Update()
    {
        if (Player.GetHeldObject() == gameObject)
        {
            if (Input.mouseButtonDownLeft)
            {
                Throw();
            }
        }
    }

    void OnItemHeld()
    {
        if (Player.GetHeldObject() == gameObject)
        {
            // tell the player HUD to show the player the "press E to read" prompt
            ui_playerhud.DrawItemPrompts(gameObject, new string[] {"press LMB to throw"});
        } else
        {
            if (UIManager.Instance.player_hud.g_promptObject == gameObject)
            {
                ui_playerhud.ClearItemPrompt();
            }
        }
    }
}
