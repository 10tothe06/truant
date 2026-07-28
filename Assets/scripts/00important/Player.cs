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

        // assinging components that can be accessed by other scripts
        rb = GetComponent<Rigidbody>();
        controller= GetComponent<PlayerController>();
        generic_controller = GetComponent<player_genericcontroller>();
        t = transform;
    }

    void Start()
    {
        player_inventory = new inv_inventorydata(8, 1);
    }

    public static Transform t;
    public static PlayerController controller;
    public static player_genericcontroller generic_controller;
    public static Rigidbody rb;


    public static inv_inventorydata player_inventory;




    public static void TeleportTo(Vector3 position)
    {
        Instance.transform.position = position;
    }
}
