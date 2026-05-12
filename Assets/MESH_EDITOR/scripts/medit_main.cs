using UnityEngine;

// **
// the structure for the mesh editor is as follows:

// - main editor object

// - mesh objects, each with their own console

// - vertices, edges, etc.

// everything is going to be controlled using widgets

// **

// one-stop-shop for making mesh objects

// NOT USING BUTTONS YET, BOOLEANS INSTEAD
[ExecuteAlways]
public class medit_main : MonoBehaviour
{
    private static medit_main _instance;

    // this is used for most things, static functions can also be used when verbosity is a concern
    public static medit_main Instance
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

    public Texture2D[] ins_widgetIcons;
    public static Texture2D[] widgetIcons;
    [Header("CONSOLE")]
    public bool spawnNewObject;

    public GameObject p_object;
    public Transform t_objectContainer;

    // for whatever reason OnGUI() doesn't work here??? it should
    void Update()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        
        widgetIcons = ins_widgetIcons;
        
        if (spawnNewObject)
        {
            spawnNewObject = false;

            SpawnNewObject();
        }
    }


    // objects don't have a type, they're just objects
    // so no need for a dropdown selection menu or anything of the sort
    public void SpawnNewObject()
    {
        Transform t_newObject = Instantiate(p_object, t_objectContainer).transform;
        t_newObject.localPosition = Vector3.zero; // pretty much js working in local space bc I want to be able to move the mesh editor around
    }
}
