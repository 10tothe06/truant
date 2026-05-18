using UnityEngine;

// i really don't want to bloat the WorldManager.cs script cuz i'll be doing heavy lifting later,
// so this is the script that will contain resource information and such

public class WorldData : MonoBehaviour
{

    private static WorldData _instance;

    public static WorldData Instance
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
    }

    public Item[] items;

    
    // returns the index of the newly created gas so it can be easily referenced
    public int CreateExoticGas()
    {
        return 0;
    }
}