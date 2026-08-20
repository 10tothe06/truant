using System.Collections;
using UnityEngine;

public class int_board : MonoBehaviour
{   
    public bool start_as_placed;

    private int_item item_comp;

    public float max_length = 1.5f;

    private Vector3 point_a;
    private Vector3 point_b;
    private bool is_placing;


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
    }

    void Start()
    {
        if (start_as_placed)
        {
            GetComponent<InteractableObject3D>().DisablePhysics();
            
            is_placing = true;
            UpdateItemData();
            UpdateFromItemData();
        }
    }

    void UpdateFromItemData()
    {
        if (!item_comp.item_data.HasEntryAt("pos_x"))
        {
            return; // we want to make sure we don't treat a non-existent entry as a reading of "no battery"
        }

        //GetComponent<InteractableObject3D>().DisableAllColliders();
        GetComponent<InteractableObject3D>().DisablePhysics();

        float pos_x = item_comp.item_data.GetFloat("pos_x");
        float pos_y = item_comp.item_data.GetFloat("pos_y");
        float pos_z = item_comp.item_data.GetFloat("pos_z");

        float rot_x = item_comp.item_data.GetFloat("rot_x");
        float rot_y = item_comp.item_data.GetFloat("rot_y");
        float rot_z = item_comp.item_data.GetFloat("rot_z");

        float scl_x = item_comp.item_data.GetFloat("scl_x");
        float scl_y = item_comp.item_data.GetFloat("scl_y");
        float scl_z = item_comp.item_data.GetFloat("scl_z");

        transform.position = new Vector3(pos_x, pos_y, pos_z);
        transform.eulerAngles = new Vector3(rot_x, rot_y, rot_z);
        transform.GetChild(0).localScale = new Vector3(scl_x, scl_y, scl_z);
    }

    void UpdateItemData()
    {
        if (item_comp == null) {return;}
        if (!is_placing) {return;}
        
        item_comp.item_data.SetData("pos_x", transform.position.x);
        item_comp.item_data.SetData("pos_y", transform.position.y);
        item_comp.item_data.SetData("pos_z", transform.position.z);


        item_comp.item_data.SetData("rot_x", transform.eulerAngles.x);
        item_comp.item_data.SetData("rot_y", transform.eulerAngles.y);
        item_comp.item_data.SetData("rot_z", transform.eulerAngles.z);

        item_comp.item_data.SetData("scl_x", transform.GetChild(0).localScale.x);
        item_comp.item_data.SetData("scl_y", transform.GetChild(0).localScale.y);
        item_comp.item_data.SetData("scl_z", transform.GetChild(0).localScale.z);
    }

    void Update()
    {
        if (Player.GetHeldObject() == gameObject)
        {
            if (Input.mouseButtonDownLeft && is_placing)
            {
                Player.DropSelectedItem();
            }
            
            // when the player clicks, we start board placement
            if (Input.mouseButtonDownLeft && !is_placing)
            {
                RaycastHit hit;

                if (util_physics.LookRaycast(out hit, 10f, LayerMask.GetMask(new string[]{"IsWalkable"})))
                {
                    point_a = hit.point;
                    is_placing = true;
                }
            }

            if (is_placing)
            {
                RaycastHit hit;

                if (util_physics.LookRaycast(out hit, 10f, LayerMask.GetMask(new string[]{"IsWalkable"})))
                {
                    point_b = hit.point;
                    point_b = point_a + (point_b-point_a).normalized * Mathf.Min(max_length, Vector3.Distance(point_b, point_a));
                    UpdateTransform();
                }
            }
        }
    }

    private void UpdateTransform()
    {
        transform.position = (point_a + point_b)/2f;

        transform.forward = point_a-point_b;

        transform.GetChild(0).localScale = new Vector3(0.2f, 0.2f, Vector3.Distance(point_a, point_b));

        UpdateItemData();
    }


    // removing a placed board
    public void PopOff()
    {
        StartCoroutine(Pop());
    }


    public IEnumerator Pop()
    {
        AudioManager.PlaySound("board_break");

        transform.position += (Player.t.position - transform.position).normalized * 0.1f;
        transform.Rotate(new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)));

        yield return new WaitForSeconds(0.4f);

        GetComponent<InteractableObject3D>().EnablePhysics();

        GetComponent<Rigidbody>().linearVelocity += (Player.t.position - transform.position).normalized * 3f;
    }


    void OnItemHeld()
    {
        if (Player.GetHeldObject() == gameObject)
        {
            // tell the player HUD to show the player the "press E to read" prompt
            ui_playerhud.DrawItemPrompts(gameObject, new string[] {"press LMB to place"});
        } else
        {
            if (UIManager.Instance.player_hud.g_promptObject == gameObject)
            {
                ui_playerhud.ClearItemPrompt();
            }
        }
    }
}
