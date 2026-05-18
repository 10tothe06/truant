using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
// reworked 12/13/2025

// new version of this class!!! (as of oct. 15 2025)

/*

//DIRECTIONS FOR SETUP

//CHILDREN:
//the camera MUST be the first child of the character
//set ALL the position/rotation data to zero, and change the local y position to whatever you want

//RIGIDBODY:
//set the rigidbody mass to whatever you want, leave useGravity on, and set ALL the rotation constraints to ON
//OPTIONAL: set collision detection to continuous instead of discrete

// COLLIDER:
// use a capsule collider! make sure it's the right height, and set the defaultHeight variable to that
// THE OBJECT WITH THE COLLIDER HAS TO BE THE SECOND CHILD

//this controller uses WASD for movement, SHIFT for sprinting, and SPACE for jumping
//it also does not allow you to move the camera while holding right click, in order for it to work with the interaction system

// there is a crouch system too! it just shrinks/grows the player's collider

*/

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("General Config")]
    public FootstepController footstepController;
    public float crouchPercentHeight;
    public CapsuleCollider col;
    private float colDefaultHeight;
    public float cameraBounceAmplitude;
    public float cameraBounceFrequency;
    private float defaultCameraHeight;
    private float currentCameraHeight;
    private float walkingTime;
    private float lastWalkingTime;
    public Transform t_camera;
    public Transform t_foot;
    [HideInInspector]
    public float raycastDistanceFromFoot;


    public bool allowJump = true;
    public bool allowSprint = true;
    public bool allowFlight = false;
    public bool allowCrouch = false;

    [Header("Speed/Strength Values")]
    public float jumpStrength = 3f;
    public float cameraTurnSpeed = 3f;
    public float movementSpeed = 0.5f;
    public float sprintSpeed = 1.75f;
    public float flySpeed = 5f;

    [Header("Locks")]
    public bool lockCameraHorizontal = false;
    public bool lockCameraVertical = false;
    public bool lockMovement = false;
    public bool lockCursor = true;

    [Header("Sprinting")]
    private bool isSprinting;
    public float maxSprint = 200; // maximum player sprint
    [HideInInspector]
    public float sprintValue; // the amount of sprint that the player has left

    [Header("Misc.")]
    public float standAfterFall;
    public LayerMask whatIsGround;
    public LayerMask whatIsMoving;

    /* tracking variables */
    private bool isFlying;
    private bool isCrouching;

    private bool activeJump;
    private float sprintTimer;
    /**/

    /* references */
    private Rigidbody rb; // easier than GetComponent<>()
    public GameObject controllingObject;
    private Transform controllingTransform;
    public bool isRemote;
    /**/ 

    private RaycastHit hit;

    /* Initialization */
    void Awake()
    {
        colDefaultHeight = col.height;

        defaultCameraHeight = t_camera.parent.localPosition.y;
        currentCameraHeight = defaultCameraHeight;

        hit = new RaycastHit();
        raycastDistanceFromFoot = 0.1f;
        // its annoying to have to wait for the sprint to replenish when playing
        sprintValue = maxSprint;

        rb = GetComponent<Rigidbody>();

        lockCursor = true;

        if (t_camera == null)
        {
            t_camera = transform.GetChild(0);
        }

        controllingTransform = transform;
    }

    public void ToggleFlight()
    {
        isFlying = !isFlying;
    }

    public void GiveControlToObject(GameObject obj)
    {
        isRemote = true;
        controllingObject = obj;
        controllingTransform = obj.transform;
    }
    public void TakeControl()
    {
        controllingObject = null;
        isRemote = false;
        controllingTransform = transform;
    }

    public void DisableCollider()
    {
        col.enabled = false;
        rb.useGravity = false;
    }

    public void EnableCollider()
    {
        col.enabled = true;
        rb.useGravity = true;
    }

    /* Messing With Constraints */
    // I have elected to make all of the functions like Freeze() and SetColliderActive() separate instead of combining them
    // seems like it'll give me more flexibility later, even if it takes up a bunch of room
    public void Freeze()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }
    public void Unfreeze()
    {
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }
    /**/

    // this function doesn't make sure that flight SHOULD be enabled,
    // it just does it

    // the enableDeveloperFeatures check happens in the cheat menu
    public void ToggleFlyMode()
    {
        isFlying = !isFlying;
    }

    void Update()
    {
        // the lockCursor variable is used when doing interactions with UI
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }

        // if flying, no gravity
        // if NOT flying, gravity
        if (col.enabled) rb.useGravity = !isFlying;

        float mult = 1;
        bool ableToRotate = true;
        
        if (Mouse.current.rightButton.isPressed)
        {
            mult = 0.2f;
            
            if (InteractionHandler3D.Instance.t_heldObject != null)
            {
                ableToRotate = false;
            } else
            {
                Player.Instance.fovMultiplier = 0.75f;
            }
        } else
        {
            Player.Instance.fovMultiplier = 1f;
        }

        if (ableToRotate)
        {
            // mouse x leads to a rotation AROUND the players's up vector
            // (not the camera's)
            if (!lockCameraHorizontal)
            {
                //transform.rotation *= Quaternion.Euler(new Vector3(0, 1, 0) * Input.GetAxis("Mouse X") * cameraTurnSpeed);
                controllingTransform.Rotate(Vector3.up * Input.mouseMovement.x * cameraTurnSpeed * mult, Space.World);
            }

            // mouse y leads to a rotation around the CAMERA's right vector
            // it obeys limits to avoid rotational glitches when looking straight up
            if (!lockCameraVertical)
            {
                float maxAngle = 0.3f;
                if (Input.mouseMovement.y < 0)
                {
                    // looking further down
                    if (t_camera.forward.y > -maxAngle)
                    {
                        t_camera.rotation *= Quaternion.Euler(new Vector3(-1, 0, 0) * Input.mouseMovement.y * cameraTurnSpeed * mult);
                    }
                    else if (Vector3.Dot(Vector3.up, util_math.RotateVector(t_camera.up, new Vector3(-1, 0, 0), Input.mouseMovement.y * cameraTurnSpeed * mult * Mathf.PI / 180)) > maxAngle)
                    {
                        t_camera.rotation *= Quaternion.Euler(new Vector3(-1, 0, 0) * Input.mouseMovement.y * cameraTurnSpeed * mult);  
                    }
                }
                else
                {
                    // looking further down
                    if (t_camera.forward.y < maxAngle)
                    {
                        t_camera.rotation *= Quaternion.Euler(new Vector3(-1, 0, 0) * Input.mouseMovement.y * cameraTurnSpeed * mult);
                    }
                    else if (Vector3.Dot(Vector3.up, util_math.RotateVector(t_camera.up, new Vector3(-1, 0, 0), Input.mouseMovement.y * cameraTurnSpeed * mult * Mathf.PI / 180)) > maxAngle)
                    {
                        t_camera.rotation *= Quaternion.Euler(new Vector3(-1, 0, 0) * Input.mouseMovement.y * cameraTurnSpeed * mult);
                    }
                }
            }
        }

        /* jumping */
        if (Keyboard.current.spaceKey.isPressed && allowJump && !activeJump && !isFlying)
        {
            rb.linearVelocity += new Vector3(0, jumpStrength, 0);
            activeJump = true;

            // TODO: investigate a bug with fall damage
            //GetComponent<GenericCreature>().PrimeFallDamage();
        }
        /**/
    }

    void FixedUpdate()
    {
        if (t_camera.parent != null) t_camera.parent.localPosition = new Vector3(0, currentCameraHeight, 0);
        float cameraTiltTarget = 0;

        if (col.enabled)
        {
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.layer == 9)
                {
                    transform.parent = hit.collider.transform;
                }
                else
                {
                    transform.parent = null;
                }
            }
        }

        if (isCrouching)
        {
            col.height = colDefaultHeight * crouchPercentHeight;
            t_foot.transform.localPosition = new Vector3(0,-col.height/2f*col.transform.localScale.y, 0);
        } else {col.height = colDefaultHeight;t_foot.transform.localPosition = new Vector3(0,-col.height/2f*col.transform.localScale.y, 0);}

        if (!lockMovement && ImprovedRaycast() && !isFlying)
        {
            if (Keyboard.current.wKey.isPressed)
            {
                if (isSprinting)
                {
                    walkingTime += Time.deltaTime * sprintSpeed;
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
                if (Keyboard.current.shiftKey.isPressed && sprintValue > 0 && allowSprint)
                {
                    isSprinting = true;
                    controllingTransform.GetComponent<Rigidbody>().linearVelocity += controllingTransform.forward * movementSpeed * sprintSpeed;

                    if (Time.time > sprintTimer + 0.05f)
                    {
                        sprintValue--;
                        sprintTimer = Time.time;
                    }
                }
                else
                {
                    isSprinting = false;
                    controllingTransform.GetComponent<Rigidbody>().linearVelocity += controllingTransform.forward * movementSpeed;
                }
            }
            else {
                isSprinting = false;
            }

            if (Keyboard.current.sKey.isPressed)
            {
                controllingTransform.GetComponent<Rigidbody>().linearVelocity -= controllingTransform.forward * movementSpeed;
            }
            if (Keyboard.current.dKey.isPressed)
            {
                controllingTransform.GetComponent<Rigidbody>().linearVelocity += controllingTransform.right * movementSpeed;
                cameraTiltTarget = -1;

            }

            if (Keyboard.current.aKey.isPressed)
            {
                controllingTransform.GetComponent<Rigidbody>().linearVelocity -= controllingTransform.right * movementSpeed;
                cameraTiltTarget = 1;
            }

            if (!Keyboard.current.dKey.isPressed && !Keyboard.current.aKey.isPressed)
            {
                cameraTiltTarget = 0;
            }
        }
        else if (isFlying) {
            transform.position += (transform.right * Input.inputAxisHorizontal + transform.forward * Input.inputAxisForward + transform.up * Input.inputAxisVertical) * (Keyboard.current.shiftKey.isPressed ? 2.5f : 1) * flySpeed;
            rb.linearVelocity = Vector3.zero;
        } else {
            cameraTiltTarget = 0;
        }

        if (t_camera.parent != null) t_camera.localRotation = Quaternion.Lerp(Quaternion.Euler(t_camera.localEulerAngles), Quaternion.Euler(new Vector3(t_camera.localEulerAngles.x, t_camera.localEulerAngles.y, cameraTiltTarget)), 0.4f);

        if (!isFlying) {
            if (ImprovedRaycast())
            {
                if (activeJump && rb.linearVelocity.y <= 0)
                {
                    // originally I had the Vector3.up here as hit.normal, 
                    // but that seemed to cause really weird drifting bugs when walking/jumping on angled terrain

                    // so, we doin' Vector3.up now

                    Vector3 lateralVelocity = rb.linearVelocity - Vector3.Project(rb.linearVelocity, Vector3.up);
                    rb.linearVelocity -= lateralVelocity;

                    activeJump = false;
                    // if (timeJumpStarted < Time.time - 0.75f) { readyForImpactSound = true; }
                    // if (readyForImpactSound) { GetComponent<FootstepController>().Step(true); readyForImpactSound = false; }
                    // GetComponent<GenericCreature>().ApplyFallDamage();
                }

                // friction
                rb.linearVelocity -= new Vector3(rb.linearVelocity.x * 0.1f, 0, rb.linearVelocity.z * 0.1f);
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

        if (!Keyboard.current.shiftKey.isPressed && sprintValue < maxSprint)
        {
            sprintValue += 0.5f;
        }
        isCrouching = Keyboard.current.leftCtrlKey.isPressed;

        lastWalkingTime = walkingTime;
    }

    // kept getting stuck on everything because the raycast was missing (like standing on a ledge)
    // so we're shooting more rays now
    bool ImprovedRaycast()
    {
        if (Physics.Raycast(t_foot.position + Vector3.up * 0.05f, -controllingTransform.up, out hit, raycastDistanceFromFoot + 0.001f, whatIsGround))
        {
            return true;
        } else if (Physics.Raycast(t_foot.position + Vector3.up * 0.05f + controllingTransform.right * 0.15f, -controllingTransform.up, out hit, raycastDistanceFromFoot + 0.001f, whatIsGround))
        {
            return true;
        } else if (Physics.Raycast(t_foot.position + Vector3.up * 0.05f + controllingTransform.right * -0.15f, -controllingTransform.up, out hit, raycastDistanceFromFoot + 0.001f, whatIsGround))
        {
            return true;
        } else if (Physics.Raycast(t_foot.position + Vector3.up * 0.05f + controllingTransform.forward * 0.15f, -controllingTransform.up, out hit, raycastDistanceFromFoot + 0.001f, whatIsGround))
        {
            return true;
        } else if (Physics.Raycast(t_foot.position + Vector3.up * 0.05f + controllingTransform.forward * -0.15f, -controllingTransform.up, out hit, raycastDistanceFromFoot + 0.001f, whatIsGround))
        {
            return true;
        }
        
        return false;
    }

    // don't do anything with the cursor, just the camera
    public void LockCamera()
    {
        lockCameraHorizontal = true;
        lockCameraVertical = true;
    }
    
    public void UnlockCamera()
    {
        lockCameraHorizontal = false;
        lockCameraVertical = false;
    }

    public void LockCharacter()
    {
        lockCursor = false;
        lockCameraHorizontal = true;
        lockCameraVertical = true;
        allowJump = false;

        lockMovement = true;
    }

    public void UnlockCharacter() {
        lockCursor = true;
        lockCameraHorizontal = false;
        lockCameraVertical = false;
        allowJump = true;

        lockMovement = false;
    }
}
