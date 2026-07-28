using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private static Player _instance;

    // this is used for most things, static functions can also be used when verbosity is a concern
    public static Player Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
            {
                _instance = value;
            }
            else if (_instance != value)
            {
                Debug.Log("You messed up buddy.");
                Destroy(value);
            }
        }
    }

    void Awake()
    {
        Instance = this;

        controller = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();

        inventory = ins_inventory;
        //itemDisplay = ins_itemDisplay;

        controller.t_camera.GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;

        defaultFOV = controller.t_camera.GetComponent<Camera>().fieldOfView;
    }

    public static PlayerController controller;
    public Inventory ins_inventory;
    public static Inventory inventory;
    // public ItemDisplay ins_itemDisplay;
    // public static ItemDisplay itemDisplay;
    public static Transform t;
    public static Rigidbody rb;

    // PLAYER STATS
    // * health is handled by the GenericCreature class, so no need to worry about that
    // * we can't really have a tiredness mechanic because there's no way to sleep (and sleeping doesn't fit the game),
    // so you'd have to just chug energy drinks and I don't think I want that
    // * alcohol level is really the only stat atp
    [Header("EFFECTS")]
    public float drunkLevel;
    public float energyLevel; // no special effect, just changes the camera's FOV
    public ImageEffect alcoholEffect;
    public float alcoholEffectSpeed;

    public float alcoholFadeRate;
    public float energyFadeRate;

    private float defaultFOV;
    public float fovMultiplier;
    private float defaultMoveSpeed;


    public static void TeleportTo(Vector3 position)
    {
        
    }




















    void Start()
    {
        t = transform;
        fovMultiplier = 1;

        defaultMoveSpeed = controller.movementSpeed;
    }

    public static void ModifyDrunkLevel(float amt)
    {
        Instance.drunkLevel += amt;
    }
    public static void SetDrunkLevel(float amt)
    {
        Instance.drunkLevel = amt;
    }

    public static void ModifyEnergyLevel(float amt)
    {
        Instance.energyLevel += amt;
    }
    public static void SetEnergyLevel(float amt)
    {
        Instance.energyLevel = amt;
    }

    void Update()
    {
        drunkLevel -= alcoholFadeRate * Time.deltaTime;
        drunkLevel = Mathf.Max(drunkLevel, 0);
        energyLevel -= energyFadeRate * Time.deltaTime;
        energyLevel = Mathf.Max(energyLevel, 0);
        
        controller.t_camera.GetComponent<Camera>().fieldOfView = (defaultFOV + energyLevel) * fovMultiplier;
        controller.t_camera.GetChild(0).GetComponent<Camera>().fieldOfView = (defaultFOV + energyLevel) * fovMultiplier;

        controller.movementSpeed = defaultMoveSpeed + energyLevel/10f;

        // I don't like having too many things in perodic functions, 
        // but I'm doing it with this because I want a smoother transition
        Instance.alcoholEffect.strength = Mathf.Lerp(Instance.alcoholEffect.strength, Instance.drunkLevel, alcoholEffectSpeed);
    }
}
