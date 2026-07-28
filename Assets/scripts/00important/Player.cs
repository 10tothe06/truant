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
        t = transform;
    }

    public static Transform t;
    public static Rigidbody rb;







    public static void TeleportTo(Vector3 position)
    {
        Instance.transform.position = position;
    }
}
