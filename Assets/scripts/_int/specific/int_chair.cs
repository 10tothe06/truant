using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


// this class is used for every sit-able spot in the game, including for vehicles

public class int_chair : MonoBehaviour
{
    private InteractableObject3D interactComponent;

    [Header("CONFIG")]
    // if this is enabled, instead of teleporting the player to a sitting position,
    // they will sort-of glide into one
    public bool animate_inout;
    private bool is_animating;
    private float animation_time; // [0..1]
    [SerializeField]
    private float animation_speed;
    [SerializeField]
    private float animation_curve_factor;


    [Space(12)]


    public Transform t_sitPoint;

    public bool isSitting;
    private bool satThisFrame;
    private Vector3 originalOffset;

    public UnityEvent onSit;
    public UnityEvent onStand;

    void Awake()
    {
        interactComponent = GetComponent<InteractableObject3D>();
        if (interactComponent != null)
        {
            interactComponent.onInteract.AddListener(Interact);
        }
    }

    public void Interact()
    {
        if (!isSitting)
        {
            Sit();
        } else {Stand();}
    }

    public void Sit()
    {
        satThisFrame = true;
        onSit.Invoke();
        Player.Lock();


        // still want the player to be able to look around
        Cursor.lockState = CursorLockMode.Locked;
        Player.controller.lockCameraHorizontal = false;
        Player.controller.lockCameraVertical = false;
        // *** 

        originalOffset = t_sitPoint.position - Player.t.position;
        Player.t.rotation = t_sitPoint.rotation;
        Player.controller.DisableCollider();
        isSitting = true;


        is_animating = true;
        animation_time = 0;
    }
    public void Stand()
    {
        onStand.Invoke();
        Player.Unlock();
        Player.controller.EnableCollider();
        
        Player.t.position = t_sitPoint.position + -originalOffset;
        // no change to rotation

        isSitting = false;
    }

    void Update()
    {
        if (isSitting)
        {
            HandleParenting();

            if (!satThisFrame && Keyboard.current.eKey.wasPressedThisFrame)
            {
                Stand();
            }
        }

        satThisFrame = false;
    }

    void HandleParenting()
    {
        if (animate_inout)
        {
            if (is_animating)
            {
                animation_time += animation_speed * Time.deltaTime;

                if (animation_time >= 1)
                {
                    is_animating = false;
                    animation_time = 1;
                }
            }

            Player.t.position = Vector3.Lerp(Player.t.position, t_sitPoint.position, animation_time) + Vector3.up * animation_curve_factor * Mathf.Sin(animation_time * Mathf.PI);
        } else
        {
            Player.t.position = t_sitPoint.position;
        }
    }
}
