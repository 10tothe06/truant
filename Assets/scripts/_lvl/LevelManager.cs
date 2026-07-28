using UnityEngine;

// script to handle SPECIFICALLY LEVEL-RELATED things,
// because I forsee the GameManager script getting too full of shit

// and I like manager scripts

public class LevelManager : MonoBehaviour
{
    private static LevelManager _instance;

    public static LevelManager Instance
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

    void Start()
    {
        LoadLevelList();
    }

    // very similar to the UIManager's menuarray system
    // could POSSIBLY make a modular script but ehh

    public Transform t_levelContianer;


    // no need for a custom data struct here
    public string[] level_names;
    public lvl_generic[] level_list;

    private lvl_generic current_level;


    private void LoadLevelList()
    {
        // grab every object with a level component, and add it to the list
        level_list = t_levelContianer.GetComponentsInChildren<lvl_generic>(true);

        level_names = new string[level_list.Length];

        for (int i = 0; i < level_names.Length; i++)
        {
            // this means the gameobject names have to be EXACTLY the same as their backend level names
            level_names[i] = level_list[i].gameObject.name;

            // make sure all levels start out as disabled
            level_list[i].gameObject.SetActive(false);
        }


    }

    #region SWITCHING LEVELS
    
    private static void ExitCurrentLevel()
    {
        if (Instance.current_level == null) {return;}
    }

    public static void LoadLevel(string level_name)
    {
        ExitCurrentLevel();

        for (int i = 0; i < Instance.level_names.Length; i++)
        {
            if (Instance.level_names[i] == level_name)
            {
                // we really only need to call one function on the generic class
                Instance.level_list[i].EnterLevel();
            }
        }
    }

    #endregion
}
