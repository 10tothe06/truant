using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class int_flashlight : MonoBehaviour
{
    private int_item item_comp;

    // this is just a number, like batteries from Launch Sequence
    public float battery_amount = 100f;
    public float max_battery = 100f;


    public float battery_loss_per_second = 0.1f;

    public bool is_on;


    public int_light light_component;

    void Awake()
    {
        item_comp = GetComponent<int_item>();

        if (item_comp != null)
        {
            item_comp.onDataUpdate.AddListener(UpdateFromItemData);
            item_comp.onInitialize.AddListener(UpdateItemData);
        }

        if (Player.item_holder != null)
        {
            // checking IF the item is being held by the player,
            // and IF SO, pass the note information to the note HUD
            Player.item_holder.onUpdateHeldObject.AddListener(OnItemHeld);
        }

        UpdateLight();
    }

    void UpdateFromItemData()
    {
        if (!item_comp.item_data.HasEntryAt("battery_amount"))
        {
            return; // we want to make sure we don't treat a non-existent entry as a reading of "no battery"
        }

        battery_amount = item_comp.item_data.GetFloat("battery_amount");
    }

    void UpdateItemData()
    {
        if (item_comp == null) {return;}
        item_comp.item_data.SetData("battery_amount", battery_amount);
    }

    private void SwitchOff()
    {
        is_on = false;
        AudioManager.PlaySound("flashlight_off");
    }
    private void SwitchOn()
    {
        is_on = true;
        AudioManager.PlaySound("flashlight_on");
    }

    private void UpdateLight()
    {
        if (is_on && !light_component.is_on)
        {
            light_component.SwitchOn();
        } else if (!is_on && light_component.is_on)
        {
            light_component.SwitchOff();
        }
    }

    void Update()
    {
        if (Player.GetHeldObject() == gameObject)
        {
            // handling where the flashlight is facing
            RaycastHit hit;

            if (util_physics.LookRaycast(out hit, 20f, LayerMask.GetMask(new string[] {"IsWalkable"})))
            {
                transform.up = hit.point - transform.position;
            } else
            {
                transform.up = CameraController.t_cam.forward;
            }

            // the actual flashlight turning on/off logic
            if (Input.mouseButtonDownLeft)
            {
                if (is_on)
                {
                    SwitchOff();
                } else
                {
                    SwitchOn();
                }
            }


            if (battery_amount > 0)
            {
                UpdateLight();

                battery_amount -= Time.deltaTime * battery_loss_per_second;
            } else
            {
                // no battery, no light
                if (light_component.is_on) // boolean check is here so we don't just keep calling this function over and over
                {
                    light_component.SwitchOff();
                }
            }
        }
    }



    void OnItemHeld()
    {
        if (Player.GetHeldObject() == gameObject)
        {
            // tell the player HUD to show the player the "press E to read" prompt
            ui_playerhud.DrawItemPrompts(gameObject, new string[] {"press LMB to toggle on/off"});
        } else
        {
            if (UIManager.Instance.player_hud.g_promptObject == gameObject)
            {
                ui_playerhud.ClearItemPrompt();
            }
        }
    }
}
