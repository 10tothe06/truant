using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    private static InteractionManager _instance;

    public static InteractionManager Instance {
        get => _instance;
        private set {
            if (_instance == null) {
                _instance = value;
            }
            else if (_instance != value) {
                Debug.Log("You messed up buddy.");
                Destroy(value);
            }
        }
    }

    void Awake()
    {
        Instance = this;
    }

    public ui_prompt interactionPrompt;
    public ui_prompt itemNamePrompt;
    public int_crosshair crosshair;


    [Header("INFO")]
    // stopping multiple interaction events from happening at once
    public static bool cooldown {get; private set;}
    private float cooldown_time;
    private float cooldown_interval = 0.1f;


    public static void Cooldown()
    {
        cooldown = true;
        Instance.cooldown_time = Time.time;
    }


    void Update()
    {
        if (cooldown)
        {
            if (Time.time > cooldown_time + cooldown_interval)
            {
                cooldown = false;
            }
        }



        // showing the prompt on the LOCAL device
        // ***
        InteractableObject3D interactingWith = CheckLocalPlayerForInteractableObject();

        bool show_prompt = false;
        if (interactingWith != null)
        {
            if (interactingWith.can_be_interacted_with)
            {
                // show the prompt
                interactionPrompt.DisplayPrompt(interactingWith.hoverPrompt);

                int_item item_comp = interactingWith.GetComponent<int_item>();
                if (item_comp != null)
                {
                    itemNamePrompt.DisplayPrompt(item_comp.item_type);
                } else
                {
                    itemNamePrompt.DisplayPrompt("");
                }

                if (crosshair != null) {crosshair.SetInteractable();}

                show_prompt = true;
            }
        }
        
        if (!show_prompt)
        {
            interactionPrompt.DisplayPrompt("");
            itemNamePrompt.DisplayPrompt("");

            if (crosshair != null) {crosshair.SetDefault();}
        }
        // ***

        // then we check if they have a player_genericcontroller on them
        player_genericcontroller comp = Player.generic_controller;
        if (comp != null)
        {
            // and we check if they're pressing the interaction button
            if (comp.mostRecentPacket != null)
            {
                if (!comp.mostRecentPacket.mouseLeft) {comp.GetComponent<int_interactionsource>().StopDraggingObject();}

                if (comp.mostRecentPacket.up || comp.mostRecentPacket.mouseLeft) // the name for the 'e' key
                {
                    // so they're attempting to interact, now we do the raycast check
                    RaycastHit hit; 

                    Vector3 pos = comp.GetComponent<int_interactionsource>().src.position;
                    Vector3 dir = comp.GetComponent<int_interactionsource>().src.forward;

                    InteractableObject3D ioComp = null;

                    if (Physics.Raycast(pos, dir, out hit))
                    {
                        if (hit.collider.gameObject.GetComponent<InteractableObject3D>() != null)
                        {
                            ioComp = hit.collider.gameObject.GetComponent<InteractableObject3D>();
                        } else if (hit.collider.gameObject.GetComponent<InteractCollider>() != null)
                        {
                            ioComp = hit.collider.gameObject.GetComponent<InteractCollider>().parentObject;
                        }
                    }

                    if (ioComp != null)
                    {
                        if (ioComp.can_be_interacted_with)
                        {
                            if (comp.mostRecentPacket.up && !comp.oldPacket.up)
                            {
                                ioComp.HandleInteractByObject(comp.gameObject);
                                Cooldown();
                            } else if (comp.mostRecentPacket.mouseLeft && !comp.mostRecentPacket.isTyping)
                            {
                                // dragging is implemented separately from the rest of the interaction system,
                                // because basically every interactable object can be dragged

                                if (ioComp.has_physics)
                                {
                                    comp.GetComponent<int_interactionsource>().StartDraggingObject(ioComp.gameObject);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public InteractableObject3D CheckLocalPlayerForInteractableObject()
    {
        RaycastHit hit;

        Vector3 pos = CameraController.t_cam.position;
        Vector3 dir = CameraController.t_cam.forward;

        if (Physics.Raycast(pos, dir, out hit))
        {
            if (hit.collider.gameObject.GetComponent<InteractableObject3D>() != null)
            {
                return hit.collider.gameObject.GetComponent<InteractableObject3D>();
            } else if (hit.collider.gameObject.GetComponent<InteractCollider>() != null)
            {
                return hit.collider.gameObject.GetComponent<InteractCollider>().parentObject;
            }
        }

        return null;
    }
}
