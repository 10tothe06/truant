using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum player_movementmode
{
    Walking,
    Crouching,
    Flying,
    NoClip,
}

// one of the first projects in a long while not to use my classic FirstPersonController3D.cs class



// this project is complicated enough that I'm writing another from scratch,
// with the idea in mind that gravity can be any direction

// also:
// because of character-switching, this controler does NOT just read keypresses
// it's given keypresses through a public function, 
// then uses those

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("VERSION 0.1s")] // s is for singleplayer
    [Space(30)]


    

    public bool isActive; // very, very important

    // despite this project not being multiplayer, I do want to run it through the keypresspacket system
    // so that controllers can be supported
    private player_genericcontroller gComp;


    public Vector3 gravityDirection; // the most important addition to this controller
    public float gravitationalAcceleration;

    #region references
    private Rigidbody rb;
    private RaycastHit hit;
    public CapsuleCollider col;
    private Transform t_camera;
    // keep in mind that eventually I will have to create ghost copies of player entities (for moving vehicle physics)
    // this means I'll need a controlling object of sorts, or similar system
    #endregion

    // since this is a multiplayer game, the player controller takes in this data class 
    // instead of reading keypresses directly
    public player_keypresspacket lastPacket;

    # region flags
    [Header("Flags")]
    public bool allowJump = true;
    public bool allowSprint = true;
    public bool allowCrouch = false;
    # endregion

    #region locks
    [Header("Locks")]
    [Header("Locks")]
    public bool lockCameraHorizontal = false;
    public bool lockCameraVertical = false;
    public bool lockMovement = false;
    #endregion

    #region parameters
    // used for raycast checks with the ground
    public Transform t_foot;
    public float raycastDistanceFromFoot;
    public FootstepController footstepController;


    // for now this applies to both the visual model and the collider, 
    // but will eventually just be collider
    public float crouchPercentHeight; 
    private float colDefaultHeight;

    public float moveSpeed; // moving forwards/backwards
    public float sprintBoost;
    public float strafeSpeed; // moving sideways
    public float turnSpeed; // looking around
    public float flySpeed;

    public float jumpStrength;

    // the player's camera oscillates when walking, which for now is just done by changing the camera offset
    // I likely won't change this, even after adding the player model - 
    // though I may just parent the camera to a bone and do that instead
    public float cameraBounceAmplitude;
    public float cameraBounceFrequency;
    private float defaultCameraHeight;
    private float currentCameraHeight;



    private bool isSprinting;
    public float maxSprint; // this may need to change variably
    [HideInInspector]
    public float sprintValue; // the amount of sprint that the player has left
    public LayerMask whatIsGround;
    #endregion
    
    #region tracking variables
    private bool isCrouching;


    /* tracking variables */
    public player_movementmode mode;

    public bool activeJump;
    private float sprintTimer;


    private float lastWalkingTime;
    private float walkingTime;

    public bool isFlying; // either flying OR noclip
    /**/

    
    #endregion

    void Awake()
    {
        SetupReferences();

        colDefaultHeight = col.height;
        sprintValue = maxSprint;

        defaultCameraHeight = t_camera.localPosition.y;

        currentCameraHeight = defaultCameraHeight + Mathf.Sin(0 * cameraBounceFrequency) * cameraBounceAmplitude;
    }

    void SetupReferences()
    {
        // setting references
        gComp = GetComponent<player_genericcontroller>();
        rb = GetComponent<Rigidbody>();
        hit = new RaycastHit();
        t_camera = transform.GetChild(0);
    }

    public void UpdatePlayer()
    {
        Vector3 test = t_camera.forward - Vector3.Project(t_camera.forward, transform.up);
        if (Vector3.Angle(test, transform.forward) > 1f)
        {
            t_camera.rotation = transform.rotation;
        }

        float cameraTiltTarget = 0;
        lastPacket = gComp.mostRecentPacket;
        if (lastPacket == null) {return;}

        gravityDirection = -Vector3.up;
        if (Vector3.Angle(transform.up, -gravityDirection) > 5)
        {
            transform.up = -gravityDirection;
        }

        // updating the entity position from the rigidbody is done by e_physicsbased

        if (isCrouching)
        {
            col.height = colDefaultHeight * crouchPercentHeight;
            t_foot.transform.localPosition = new Vector3(0,-col.height/2f*col.transform.localScale.y, 0);
        } else {col.height = colDefaultHeight;t_foot.transform.localPosition = new Vector3(0,-col.height/2f*col.transform.localScale.y, 0);}

        if (!lockMovement && ImprovedRaycast() && !isFlying)
        {
            
            if (lastPacket.forward)
            {
                if (isSprinting)
                {
                    walkingTime += Time.deltaTime * sprintBoost;
                } else
                {
                    walkingTime += Time.deltaTime;
                }
                if (walkingTime * cameraBounceFrequency > Mathf.PI * 3f/2f && lastWalkingTime * cameraBounceFrequency < Mathf.PI * 3f/2f)
                {
                    footstepController.Step();
                }
                if (walkingTime * cameraBounceFrequency > Mathf.PI*2f)
                {
                    walkingTime -= Mathf.PI*2f/cameraBounceFrequency;
                }
                currentCameraHeight = defaultCameraHeight + Mathf.Sin(walkingTime * cameraBounceFrequency) * cameraBounceAmplitude;
                if (lastPacket.sprint && sprintValue > 0 && allowSprint)
                {
                    isSprinting = true;
                    
                    rb.linearVelocity += transform.forward * moveSpeed * sprintBoost * Time.deltaTime;

                    if (Time.time > sprintTimer + 0.05f)
                    {
                        sprintValue--;
                        sprintTimer = Time.time;
                    }
                }
                else
                {
                    isSprinting = false;
                    
                    rb.linearVelocity += transform.forward * moveSpeed * Time.deltaTime;
                }
            }
            else {
                isSprinting = false;
            }

            if (lastPacket.back)
            {
                rb.linearVelocity -= transform.forward * moveSpeed * Time.deltaTime;
            }
            if (lastPacket.right)
            {
                rb.linearVelocity += transform.right * moveSpeed * Time.deltaTime;
                cameraTiltTarget = -1;

            }

            if (lastPacket.left)
            {
                rb.linearVelocity -= transform.right * moveSpeed * Time.deltaTime;
                cameraTiltTarget = 1;
            }

            if (!lastPacket.right && !lastPacket.left)
            {
                cameraTiltTarget = 0;
            }
        }
        else if (isFlying) {
            transform.position += (transform.right * Input.inputAxisHorizontal + transform.forward * Input.inputAxisForward + transform.up * Input.inputAxisVertical) * (lastPacket.sprint ? 2.5f : 1) * flySpeed;
            rb.linearVelocity = Vector3.zero;
        } else {
            cameraTiltTarget = 0;
        }

        if (t_camera.parent != null) t_camera.localRotation = Quaternion.Lerp(Quaternion.Euler(t_camera.localEulerAngles), Quaternion.Euler(new Vector3(t_camera.localEulerAngles.x, t_camera.localEulerAngles.y, cameraTiltTarget)), 0.4f);

        if (!isFlying) {
            if (ImprovedRaycast())
            {
                if (activeJump && util_math.ProjectedMagnitude(rb.linearVelocity, gravityDirection) >= 0)
                {
                    Vector3 lateralVelocity = rb.linearVelocity - Vector3.Project(rb.linearVelocity, gravityDirection);
                    rb.linearVelocity -= lateralVelocity;

                    activeJump = false;
                    // if (timeJumpStarted < Time.time - 0.75f) { readyForImpactSound = true; }
                    // if (readyForImpactSound) { GetComponent<FootstepController>().Step(true); readyForImpactSound = false; }
                    // GetComponent<GenericCreature>().ApplyFallDamage();
                }

                // friction
                Vector3 fric = rb.linearVelocity * 0.1f;
                rb.linearVelocity -= (fric - Vector3.Project(fric, gravityDirection));
            }
            else
            {
                // drag
                //rb.linearVelocity -= new Vector3(rb.linearVelocity.x * 0.0001f, 0, rb.linearVelocity.z * 0.0001f);
                if (!activeJump)
                {
                    activeJump = true;
                }
            }
        }

        if (isActive)
        {
            if (t_camera != null) t_camera.localPosition = new Vector3(0, currentCameraHeight, 0);

            // mouse x leads to a rotation AROUND the players's up vector
            // (not the camera's)
            if (!lockCameraHorizontal)
            {
                //transform.rotation *= Quaternion.Euler(new Vector3(0, 1, 0) * Input.GetAxis("Mouse X") * cameraTurnSpeed);
                transform.Rotate(-gravityDirection * lastPacket.horizontalMouse * turnSpeed * Time.deltaTime, Space.World);
            }

            // mouse y leads to a rotation around the CAMERA's right vector
            // it obeys limits to avoid rotational glitches when looking straight up
            if (!lockCameraVertical)
            {
                float maxAngle = 0.8f;

                if (lastPacket.verticalMouse < 0)
                {
                    // looking further down
                    if (util_math.ProjectedMagnitude(t_camera.forward, gravityDirection) < maxAngle)
                    {
                        t_camera.Rotate(new Vector3(-1, 0, 0) * lastPacket.verticalMouse * turnSpeed * Time.deltaTime, Space.Self);
                    }
                }
                else
                {
                    // looking further down
                    if (util_math.ProjectedMagnitude(t_camera.forward, -gravityDirection) < maxAngle)
                    {
                        t_camera.Rotate(new Vector3(-1, 0, 0) * lastPacket.verticalMouse * turnSpeed * Time.deltaTime, Space.Self);
                    }
                }
            }
            
        }

        if (!lastPacket.sprint && sprintValue < maxSprint)
        {
            sprintValue += 0.5f;
        }
        isCrouching = lastPacket.crouch;

        lastWalkingTime = walkingTime;

        /* jumping */
        if (lastPacket.jump && allowJump && !activeJump)
        {
            rb.linearVelocity += -gravityDirection.normalized * jumpStrength;
            activeJump = true;
        }
        /**/



        // GRAVITY
        if (!isFlying)
        {
            rb.linearVelocity += gravityDirection * gravitationalAcceleration * Time.deltaTime;
        }
    }

    // kept getting stuck on everything because the raycast was missing (like standing on a ledge)
    // so we're shooting more rays now
    bool ImprovedRaycast()
    {
        if (Physics.Raycast(t_foot.position + -gravityDirection * 0.05f, gravityDirection, out hit, raycastDistanceFromFoot + 0.001f, whatIsGround))
        {
            return true;
        } else if (Physics.Raycast(t_foot.position + -gravityDirection * 0.05f + transform.right * 0.15f, gravityDirection, out hit, raycastDistanceFromFoot + 0.001f, whatIsGround))
        {
            return true;
        } else if (Physics.Raycast(t_foot.position + -gravityDirection * 0.05f + transform.right * -0.15f, gravityDirection, out hit, raycastDistanceFromFoot + 0.001f, whatIsGround))
        {
            return true;
        } else if (Physics.Raycast(t_foot.position + -gravityDirection * 0.05f + transform.forward * 0.15f, gravityDirection, out hit, raycastDistanceFromFoot + 0.001f, whatIsGround))
        {
            return true;
        } else if (Physics.Raycast(t_foot.position + -gravityDirection * 0.05f + transform.forward * -0.15f, gravityDirection, out hit, raycastDistanceFromFoot + 0.001f, whatIsGround))
        {
            return true;
        }
        
        return false;
    }

    #region toggling

    public void DisableCollider()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
        col.enabled = false;
        rb.useGravity = false;
    }

    public void EnableCollider()
    {
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        col.enabled = true;
        rb.useGravity = true;
    }

    # endregion
}
