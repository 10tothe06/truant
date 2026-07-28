using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// 2nd in command, basically, after Program.cs

// the UIManager script is probably the only thing that's stayed consistent in my projects
public class UIManager : MonoBehaviour
{
    private static UIManager _instance;

    public static UIManager Instance
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
        
        LoadMenuObjects();
    }

    void Start()
    {
        //inventory.Initialize();
    }

    public static bool isTyping;

    public void StartTyping()
    {
        isTyping = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void StopTyping()
    {
        isTyping = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public Transform t_canvas;

    public GameObject g_console;

    public List<string> menuNames;
    public List<int> menuSiblingIndices;

    // for convinience
    public bool isInMapView;

    [Space(30)]
    [Header("Component References")]
    public ui_settingsmenu settingsMenu;

    public GameObject g_bugReportWidget;
    public ui_advancementwidget advancementsWidget;

    public GameObject g_pauseMenu;


    public GameObject g_creditsMenu;

    #region OPEN/CLOSE

    public static void OpenCreditsMenu()
    {
        Instance.g_creditsMenu.SetActive(true);
    }

    public static void CloseCreditsMenu()
    {
        Instance.g_creditsMenu.SetActive(false);
    }


    // these are all static functions
    // (new rule to avoid verbosity)
    // TODO: do the same shit for all the open/close functions in Launch Sequence

    public static void OpenBugReportWidget()
    {
        Instance.g_bugReportWidget.SetActive(true);
    }

    public static void OpenSettingsMenu()
    {
        SwitchMenu("settings menu");
        Instance.settingsMenu.EnterMenu();
    }

    public static void OpenAdvancementsWidget()
    {
        Instance.advancementsWidget.gameObject.SetActive(true);
        Instance.advancementsWidget.RenderAchievements();
    }
    public static void CloseAdvancementsWidget()
    {
        Instance.advancementsWidget.gameObject.SetActive(false);
    }


    // BOTH OF THESE ARE CALLED THROUGH THE GAME MANAGER

    
    // DON'T FUCKING CALL THEM FROM HERE
    // (you dimwit)
    public static void OpenPauseMenu()
    {
        Instance.g_pauseMenu.SetActive(true);
    }
    public static void ClosePauseMenu()
    {
        Instance.g_pauseMenu.SetActive(false);
    }

    #endregion


    #region ADVANCEMENTS

    public IEnumerator ShowAdvancementPopup(adv_advancementdata data)
    {
        // TODO: this

        yield return new WaitForSeconds(1f);
    }

    #endregion


    public IEnumerator RunGameIntro()
    {
        yield return new WaitForSeconds(0f);

        EnterMainMenu();
    }


    public void EnterMainMenu()
    {
        SwitchMenu("main menu");
        g_console.SetActive(false);
    }


    public void InMenuUpdate()
    {
        CameraController.Instance.UpdateCamera();
    }


    // not just 'update', because i only want to run this sometimes
    public void InGameUpdate()
    {
        CameraController.Instance.UpdateCamera();

        if (Keyboard.current.backquoteKey.wasPressedThisFrame)
        {
            ToggleConsole();
        }

        if (!isTyping)
        {

            // keypress checks
        }
    }

    public void ShowConsole()
    {
        g_console.SetActive(true);
    }

    public void HideConsole()
    {
        g_console.SetActive(false);
    }

    public void ToggleConsole()
    {
        g_console.SetActive(!g_console.activeSelf);
        if (g_console.activeSelf)
        {
            StartTyping();
        } else
        {
            StopTyping();
        }
    }

    public void LoadMenuObjects()
    {
        for (int i = 0; i < t_canvas.childCount; i++)
        {
            if (t_canvas.GetChild(i).name[0] != '[') {continue;}

            char tag = t_canvas.GetChild(i).name[1];
            if (tag == 'm')
            {
                menuSiblingIndices.Add(i);
                menuNames.Add(t_canvas.GetChild(i).name.Substring(4));
            }
        }
    }

    public static void SwitchMenu(string name)
    {
        int index = -1;
        for (int i = 0; i < Instance.menuNames.Count; i++)
        {
            Instance.t_canvas.GetChild(Instance.menuSiblingIndices[i]).gameObject.SetActive(false);
            if (Instance.menuNames[i] == name)
            {
                index = Instance.menuSiblingIndices[i];
            }
        }

        if (index == -1) {Debug.Log("Menu name not found!"); return;}

        Instance.t_canvas.GetChild(index).gameObject.SetActive(true);
    }
}
