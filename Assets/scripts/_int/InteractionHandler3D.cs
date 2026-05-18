using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
// reworked 12/13/2025

// main class for general handling of the interaction system

// it does all of the raycasting and calling of interaction events,
// plus the actual 'carrying' mechanic since its much simple to do it centrally




/*
INFO:

this class should probably go in the 'sys' object, but it does need a few references

*/

public class InteractionHandler3D : MonoBehaviour
{
    private static InteractionHandler3D _instance;

    public static InteractionHandler3D Instance {
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
    
    // important references, set/used at runtime
    [Header("REFERENCES")]
    public Transform t_heldObject;
    //public FirstPersonController3D player;
    public Transform t_camera;
    
    

    [Header("CONFIG")]
    public audio_soundmaterial[] impactSounds;
    public audio_soundset defaultImpactSound;
    public float defaultHoldDistance;
    private float currentHoldDistance;
    public float interactDistance; // how far the player can "reach" to interact with an object
    public float pullForce; // the strength of the force pulling an object to where the player is looking (drag force, basically)

    public TextMeshProUGUI promptDisplay; // the text that prompts the player to interact
    public string whatIsInteractable; // saves raycast performance

    [Header("Crosshair")]
    public bool crosshairChange;
    public Image i_crosshair;
    public Sprite crosshair_normal;
    public Sprite crosshair_interact;

    void Awake()
    {
        currentHoldDistance = defaultHoldDistance;
        Instance = this;
    }

    public GameObject GetHoveringObject()
    {
        RaycastHit hit;
        if (Physics.Raycast(t_camera.position, t_camera.forward, out hit, interactDistance))
        {
            if (hit.collider.gameObject.tag == whatIsInteractable)
            {
                return hit.collider.gameObject;
            } else {return null;}
        }
        else
        {
            return null;
        }
    }

    void Update()
    {
        HandleInteractionRaycast();
    }

    void HandleInteractionRaycast()
    {
        if (t_heldObject != null) // player is holding SOMETHING
        {
            t_heldObject.GetComponent<Rigidbody>().angularVelocity = Vector3.Lerp(t_heldObject.GetComponent<Rigidbody>().angularVelocity, Vector3.zero, 0.05f);

            Vector3 targetDir = pullForce * (t_camera.position + t_camera.forward * currentHoldDistance - t_heldObject.position);

            //t_heldObject.GetComponent<Rigidbody>().linearVelocity = targetDir + Player.controller.GetComponent<Rigidbody>().linearVelocity;
            t_heldObject.GetComponent<Rigidbody>().useGravity = true;

            if (Mouse.current.rightButton.isPressed && t_heldObject != null)
            {
                t_heldObject.Rotate(-1 * transform.up * Input.mouseMovement.x, Space.World);
                t_heldObject.Rotate(1 * transform.right * Input.mouseMovement.y, Space.World);
            }

            currentHoldDistance += Input.scrollWheelAxis;

            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                t_heldObject.GetComponent<InteractableObject3D>().HandleInteract();
            }
        }
        else // not already holding an object
        {
            bool objectFound = false;
            RaycastHit hit;
            // not sure if this is the right place for the canInteract boolean check?
            if (Physics.Raycast(t_camera.position, t_camera.forward, out hit, interactDistance))
            {
                currentHoldDistance = hit.distance;
                // there is NO LAYERMASK CHECK above, bc if there was you could interact through walls (non-layer geometry is ignored, remember)
                // so instead of a layermask check we have to do the layer check later
                if (hit.collider.gameObject.tag == whatIsInteractable)
                {
                    // a previous iteration of this script used WAYY to many different GetComponent<>() calls, 
                    // because there were so many classes that could be attached to interactable objects

                    // what I've done is do something more like what InteractableObject3D was SUPPOSED to be
                    // basically, there's one class (InteractableObject3D) that can pass interaction data back and forth,
                    // and all other classes essentially "subscribe" to it via a unity action

                    // so yes, InteractableObject3D isn't for JUST PHYISCS OBJECTS anymore

                    InteractableObject3D comp = hit.collider.gameObject.GetComponent<InteractableObject3D>();

                    if (comp != null)
                    {
                        DisplayPrompt(comp.hoverPrompt);
                        objectFound = true;

                        if (Keyboard.current.eKey.wasPressedThisFrame)
                        {
                            comp.HandleInteract();
                        }
                    } else
                    {
                        InteractCollider col = hit.collider.gameObject.GetComponent<InteractCollider>();
                        if (col != null) comp = col.parentObject;

                        if (comp != null)
                        {
                            DisplayPrompt(comp.hoverPrompt);
                            objectFound = true;

                            if (Keyboard.current.eKey.wasPressedThisFrame)
                            {
                                comp.HandleInteract();
                            }
                        }
                    }
                }
            }
            
            if (!objectFound)
            {
                DisplayPrompt("");
            }
        }
    }

    public void DisplayPrompt(string prompt) {
        if (crosshairChange)
        {
            if (prompt.Length > 0)
            {
                i_crosshair.sprite = crosshair_interact;
            } else
            {
                i_crosshair.sprite = crosshair_normal;
            }
        }
        if (promptDisplay != null) promptDisplay.text = prompt;
    }
}
