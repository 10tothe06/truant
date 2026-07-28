using UnityEngine;

// this is a very, very forward-thinking feature for when I decide to add dedicated servers
// changing this will affect how the build works, avoiding all client code if set to 'ServerBuild'
public enum ProgramBuildMode
{
    SingleplayerBuild,
}

// how the game should boot
// saves me a lot of time that would have been wasted hanging around the main menu
public enum ProgramStartMode
{
    SceneOnly, // don't run any game logic at all
    FullGame,
    ImmediateLevel,
}

public class Program : MonoBehaviour
{
    private static Program _instance;

    public static Program Instance
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

        buildMode = ins_buildMode;
        startMode = ins_startMode;
    }

    public bool resetAdvancements;

    public string version;

    public ProgramBuildMode ins_buildMode;
    public static ProgramBuildMode buildMode;
    public ProgramStartMode ins_startMode;
    public static ProgramStartMode startMode;
    public string level_to_load;

    // should almost be the ONLY use of the start function
    void Start()
    {
        // attempts to grab game progression data from disk
        GameFlags.TryLoadFlags();

        Boot();
    }

    void OnApplicationQuit()
    {
        Settings.Instance.WriteToSettingsFile();
        Settings.Instance.SaveTrackedAdvancements();

        GameFlags.SaveFlagsToDisk();
    }

    public void Boot()
    {
        if (startMode == ProgramStartMode.FullGame)
        {
            StartCoroutine(UIManager.Instance.RunGameIntro());
        } else if (startMode == ProgramStartMode.ImmediateLevel)
        {
            LevelManager.LoadLevel(level_to_load);
        } else if (startMode == ProgramStartMode.SceneOnly)
        {
            // do nothing
        }
    }

    void Update()
    {
        // the game manager handles the specifics
        GameManager.Instance.UpdateGame();
    }

    // forget exiting to main, just quit the damn program
    public void HardQuit()
    {
        Application.Quit(); // NO OTHER SCRIPT IS ALLOWED TO CALL THIS
    }

    public string GetPreviousVersion()
    {
        // TODO: this function
        return version;
    }
}
