using UnityEngine;
using UnityEngine.InputSystem;

// note:
// swivels on the transform.RIGHT axis of the t_pivot

public class int_physicslever : MonoBehaviour
{
    private Rigidbody rb;
    private InteractableObject3D interactComponent;

    public bool isHeld {get; private set;}

    public float swing_force;


    [Header("REFERENCES")]
    public Transform t_handle;
    public Transform t_tip;

    public Transform t_pivot;
    private float lever_length;
    private float grabDistance;

    void Awake()
    {
        lever_length = Vector3.Distance(t_pivot.position, t_tip.position);

        rb = GetComponent<Rigidbody>();

        interactComponent = GetComponent<InteractableObject3D>();
        if (interactComponent != null)
        {
            interactComponent.onInteract.AddListener(Interact);
        }
    }

    public void Interact()
    {
        if (!isHeld)
        {
            Grab();
        } else {Release();}
    }

    // called when the player grabs the lever
    public void Grab() {
        isHeld = true;

        grabDistance = Vector3.Distance(CameraController.t_cam.position, t_tip.position);
    }

    public void Release()
    {
        isHeld = false;
    }


    void Update()
    {
        if (isHeld)
        {
            HandlePosition();

            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                Release();
            }
        }
    }

    public void HandlePosition()
    {
        if (isHeld)
        {
            Vector3 tipPosition = CameraController.t_cam.position + CameraController.t_cam.forward * grabDistance;
            Vector3 diff = tipPosition - t_tip.position;
            
            
            Vector3 axis = -Vector3.Cross(diff, t_handle.up);

            rb.angularVelocity = axis * swing_force * Time.deltaTime;
        }
    }
}
