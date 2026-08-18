using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// used so that very high-level scripts like the WorldManagr can only run certain logic when in-game
// essentially the updated version of the inGame variable all the way back from Tempest
public enum GameState
{
    InMenu,
    InGame,
}


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

    public static bool is_game_paused {get; private set;}

    // the static one is the one that scripts look for
    // the ins_ variable is just so that I can see
    [Header("vv READ ONLY vv")]
    public GameState ins_gameState;
    public static GameState gameState;



    [Header("UNITY EVENTS")]
    public UnityEvent onGamePaused;
    public UnityEvent onGameResume;

    


    void Awake()
    {
        Instance = this;
    }

    // called upon the player clicking the "start" button
    
    // decides what to do based on progression
    public static void StartGame()
    {
        if (GameFlags.flags.has_completed_tutorial)
        {
            // tutorial done, load into lobby
            LevelManager.LoadLevel("lobby");
        } else
        {
            // gotta do the tutorial
            LevelManager.LoadLevel("tutorial");
        }
    }

    // quits to desktop, basically
    public static void QuitGame()
    {
        
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


    public void UpdateGame()
    {
        // ====================================
        // updating UI (no distinction made here between sandbox and game)
        // ====================================

        // we want to try and have an update function in as few scripts as possible
        if (gameState == GameState.InGame)
        {
            UIManager.Instance.InGameUpdate();
        } else if (gameState == GameState.InMenu)
        {
            UIManager.Instance.InMenuUpdate();
        }
    }

    #endregion

    #region PAUSING





    // only the updates, not opening the menu
    public static void ToggleGameUpdates()
    {
        if (is_game_paused)
        {
            ResumeGameUpdates();
        } else
        {
            PauseGameUpdates();
        }
    }

    public static void PauseGameUpdates()
    {
        ui_debugmessager.PostMessage("updates paused");
        is_game_paused = true;
        Instance.onGamePaused.Invoke();
        SetUpdateSpeed(0f);
    }
    public static void ResumeGameUpdates()
    {
        ui_debugmessager.PostMessage("updates resumed");
        is_game_paused = false;
        Instance.onGameResume.Invoke();
        SetUpdateSpeed(last_game_update_speed);
    }




    // includes the menu
    public static void TogglePause()
    {
        if (is_game_paused)
        {
            ResumeGame();
        } else
        {
            PauseGame();
        }
    }

    // one-stop-shop:
    // * opens the pause menu
    // * stops physics
    public static void PauseGame()
    {
        PauseGameUpdates();

        UIManager.OpenPauseMenu();
    }

    public static void ResumeGame()
    {
        // return the update speed to whatever we had it at before
       ResumeGameUpdates();

       UIManager.ClosePauseMenu();
    }


    #endregion
}
