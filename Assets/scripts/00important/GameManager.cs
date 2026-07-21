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

    [SerializeField]
    private float ins_game_update_speed; // just for show
    private static float game_update_speed;


    [SerializeField]
    private float ins_last_game_update_speed; // just for show
    private static float last_game_update_speed; // stored to allow reverting


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

    #region GAME UPDATING

    // this is the function that is accessed by all scripts,
    // not a variable in case we want more logic

    // 0 means the game is paused
    // 0.5 means half speed
    // 1 means full speed
    public static float GetUpdateSpeed()
    {
        return game_update_speed;
    }

    public static void SetUpdateSpeed(float new_speed)
    {
        // storing the old speed
        last_game_update_speed = game_update_speed;
        GameManager.Instance.ins_last_game_update_speed = game_update_speed;

        // setting the new speed
        game_update_speed = new_speed;
        GameManager.Instance.ins_game_update_speed = new_speed;
    }

    #endregion

    #region PAUSING

    // one-stop-shop:
    // * opens the pause menu
    // * stops physics
    public static void PauseGame()
    {
        SetUpdateSpeed(0);
    }

    public static void ResumeGame()
    {
        // return the update speed to whatever we had it at before
       SetUpdateSpeed(last_game_update_speed);
    }


    #endregion
}
