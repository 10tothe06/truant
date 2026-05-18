using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance {
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

    void Start()
    {
        // try loading the game flags from disk
        GameFlags loadedFlags = rw_utils.LoadFlags();

        if (loadedFlags != null)
        {
            Debug.Log("Using loaded game flags...");
            GameFlags.Apply(loadedFlags);
        } else
        {
            Debug.Log("Using factory default game flags...");
            GameFlags.ApplyFactoryDefaults();
        }
    }

    // endings marked as 'true' are possible with that character, 'false' aren't
    // because some endings just can't happen with certain characters
    public bool[] endingsPossibleWithHiker;
    public bool[] endingsPossibleWithRanger;
    public bool[] endingsPossibleWithCamper;

    public int characterIndex;
    public Item[] items;

    public Transform[] t_playerSpawns;

    // may have already coded this function,
    // but can't be bothered to look for it
    public Item FindItem(string name)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].name == name)
            {
                return items[i];
            }
        }

        return null;
    }
}
