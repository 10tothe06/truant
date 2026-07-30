using UnityEngine;

// unlike what the name may suggest,
// this script is used for BASICALLY EVERY HINGED OBJECT IN THE GAME

// this means levers, yes,
// but also car doors,
// regular doors,
// gates,
// and so on

public class int_lever : MonoBehaviour
{
    private Transform t_cam;
    private InteractableObject3D interactComponent;
    public bool isHeld;
    private float grabDistance;

    [Header("CONFIG")]
    private float leverLength;
    public Transform t_maxAngle;
    public Transform t_minAngle;
    public Transform t_handle;
    public Transform t_base;
    public Transform t_tip;
    public Transform t_pivot;
    public bool useMaxMin;
    public float maxTurnAngle;


    [Header("SOUND EFFECTS")]
    public bool use_sound_effects = false;
    public int minimum_hit_sound_index; // hits the minimum limit
    public int maximum_hit_sound_index; // hits the maximum limit

    // to avoid playing sounds multiple times
    private bool has_hit_limit;



    void Awake()
    {
        if (useMaxMin)
        {
            maxTurnAngle = Vector3.Angle(t_maxAngle.position-t_pivot.position,t_minAngle.position-t_pivot.position);
            t_pivot.transform.forward = (t_maxAngle.position-t_pivot.position + t_minAngle.position-t_pivot.position)/2f;
        }

        interactComponent = GetComponent<InteractableObject3D>();
        if (interactComponent != null)
        {
            interactComponent.onInteract.AddListener(Interact);
        }

        leverLength = Vector3.Distance(t_tip.position, t_pivot.position);
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

        t_cam = CameraController.t_cam;
        grabDistance = Vector3.Distance(t_cam.position, t_tip.position);
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
        }
    }

    public void HandlePosition()
    {
        if (isHeld)
        {
            Vector3 tipPosition = t_cam.position + t_cam.forward * grabDistance;
            Vector3 rawForwardVector = tipPosition - t_pivot.position;
            rawForwardVector = rawForwardVector - Vector3.Project(rawForwardVector, t_base.right);
            t_tip.position = t_pivot.position + rawForwardVector.normalized * leverLength;

            if (Vector3.Angle(rawForwardVector, t_pivot.forward) < maxTurnAngle/2f)
            {
                t_handle.position = (t_pivot.position + t_tip.position) / 2f;
                t_handle.forward = rawForwardVector;

                has_hit_limit = false;
            } else if (!has_hit_limit)
            {
                float angleToMin = Vector3.Angle(rawForwardVector, t_minAngle.position - t_base.position);
                float angleToMax = Vector3.Angle(rawForwardVector, t_maxAngle.position - t_base.position);

                if (angleToMin < angleToMax)
                {
                    //Debug.Log("min");
                    
                    // TODO: play sound
                    has_hit_limit = true;
                    
                } else
                {
                    //Debug.Log("max");

                    // TODO: play sound

                    has_hit_limit = true;
                }
            }
        }
    }
}
