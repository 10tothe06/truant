using Unity.VisualScripting.FullSerializer;
using UnityEngine;

// just know that i don't like this script name
// i picked it because i didnt want to think of a name


public class DebugManager : MonoBehaviour
{
    private static DebugManager _instance;

    public static DebugManager Instance {
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

    void Awake()
    {
        Instance = this;
    }

    public GameObject p_line;
    public GameObject p_sphere;


    // the syntax for these two is a little bit similar to "Debug.DrawLine" and "Debug.DrawSphere" (built-in unity functions)
    // i accept this and will do nothing about it

    public static void DrawLine(Vector3 start, Vector3 end, float width = 0.2f)
    {
        GameObject g_newLine = Instantiate(Instance.p_line, Instance.transform);

        // todo: maybe have a component on the object itself that handles this part?
        LineRenderer comp = g_newLine.GetComponent<LineRenderer>();
        
        comp.positionCount = 2;
        comp.SetPositions(new Vector3[] {start,end});

        comp.startWidth = width;
        comp.endWidth = width;
    }

    public static void DrawSphere()
    {
        
    }
}
