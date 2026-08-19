using UnityEngine;

public class int_gun : MonoBehaviour
{
    private int_item item_comp;
    private Vector3 targetPosition;




    [Header("CONFIG")]
    public string bullet_name;
    public float time_between_shots = 0.8f;

    // meters per second
    public float muzzle_velocity = 50f;

    private float last_shot_time;
    public int ammo_count;


    [Header("SOUNDS")]
    public string shoot_sound;
    public string no_ammo_sound;

    

    void Awake()
    {
        item_comp = GetComponent<int_item>();

        if (item_comp != null)
        {
            item_comp.onDataUpdate.AddListener(UpdateFromItemData);
            item_comp.onInitialize.AddListener(UpdateItemData);
        }

        // checking IF the item is being held by the player,
        // and IF SO, pass the note information to the note HUD
        Player.item_holder.onUpdateHeldObject.AddListener(OnItemHeld);
    }

    private void Shoot()
    {  
        // can't shoot if we're on cooldown
        if (last_shot_time != 0)
        {
            if (Time.time > last_shot_time + time_between_shots)
            {
                last_shot_time = 0;
            } else
            {
                return;
            }
        }

        if (ammo_count <= 0)
        {
            return;
        }

        ui_debugmessager.PostMessage("bang");

        last_shot_time = Time.time;
        ammo_count--;

        GameObject g_newBullet = ObjectManager.SpawnObject(bullet_name, transform.position);
        g_newBullet.transform.forward = transform.forward;

        // the bullet script will automatically set the velocity and everything,
        // provided we tell it how fast to go:
        g_newBullet.GetComponent<int_bullet>().OnShoot(muzzle_velocity);
    }

    void UpdateFromItemData()
    {
        if (!item_comp.item_data.HasEntryAt("ammo_count"))
        {
            return; // we want to make sure we don't treat a non-existent entry as a reading of "no battery"
        }

        ammo_count = item_comp.item_data.GetInt("ammo_count");
    }

    void UpdateItemData()
    {
        if (item_comp == null) {return;}
        item_comp.item_data.SetData("ammo_count", ammo_count);
    }

    void Update()
    {
        if (!Player.item_holder.is_inspecting_item)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, 0.7f);
        }

        // handling where the flashlight is facing
        RaycastHit hit;

        if (util_physics.LookRaycast(out hit, 20f, LayerMask.GetMask(new string[] {"IsWalkable"})))
        {
            transform.forward = hit.point - transform.position;
        } else
        {
            transform.forward = CameraController.t_cam.forward;
        }


        // the aiming logic
        if (Input.mouseButtonRight)
        {
           CameraController.SetCameraFov(CameraController.default_fov/2f); 

           targetPosition = CameraController.t_cam.position - CameraController.t_cam.up * 0.1f + CameraController.t_cam.forward * 0.3f;
        } else
        {
            CameraController.SetCameraFov(CameraController.default_fov);

            targetPosition = Player.item_holder.t_heldItemContainer.position;
        }



        // the shooting logic
        if (Input.mouseButtonDownLeft)
        {
            Shoot();
        }
    }

    void OnItemHeld()
    {
        if (Player.GetHeldObject() == gameObject)
        {
            // tell the player HUD to show the player the "press E to read" prompt
            ui_playerhud.DrawItemPrompts(gameObject, new string[] {"press RMB to aim", "press LMB to shoot"});
        } else
        {
            if (UIManager.Instance.player_hud.g_promptObject == gameObject)
            {
                ui_playerhud.ClearItemPrompt();
            }
        }
    }
}
