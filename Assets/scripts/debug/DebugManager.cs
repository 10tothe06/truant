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
}
