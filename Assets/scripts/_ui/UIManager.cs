using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
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


        // making sure all of these objects start in their proper states
        if (inventory != null) {inventory.gameObject.SetActive(true);}
        if (player_hud != null) {player_hud.gameObject.SetActive(false);}

        // needs to be true for initialization to work
        if (level_intro != null) {level_intro.gameObject.SetActive(true);}

        if (g_console != null) {HideConsole();}
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

    public List<string> menuNames;
    public List<int> menuSiblingIndices;

    // for convinience
    public bool isInMapView;

    [Space(30)]
    [Header("Component References")]
    public GameObject g_console;

    public ui_tabs consoleTabs;
    public ui_console consoleOutput;
    public ui_cheatmenu cheatMenu;


    public ui_settingsmenu settingsMenu;

    public GameObject g_bugReportWidget;
    public ui_advancementwidget advancementsWidget;

    public GameObject g_pauseMenu;


    public GameObject g_creditsMenu;

    public ui_inventories inventory;
    private bool is_inventory_open;


    public ui_playerhud player_hud;



    public ui_levelintro level_intro;

    public GameObject g_debugMenu;

    public Image i_blackScreen;
    private float fade_percent;
    public float fade_speed;


    

    public static void PlayLevelIntro(TextSequence data)
    {
        if (Program.skip_loading_screens) {return;}

        Debug.Log("📽 Playing level intro...");


        Instance.level_intro.PlayIntro(data);
    }


    void Update()
    {
        if (fade_percent != i_blackScreen.color.a)
        {
            float new_alpha = i_blackScreen.color.a + Mathf.Clamp(fade_percent - i_blackScreen.color.a , -fade_speed* Time.deltaTime, fade_speed* Time.deltaTime);

            i_blackScreen.color = new Color(i_blackScreen.color.r, i_blackScreen.color.g, i_blackScreen.color.b, new_alpha);
        }
    }


    #region FADING

    public static void SetFadePercent(float value)
    {
        Instance.fade_percent = value;
        Instance.i_blackScreen.color = new Color(
            Instance.i_blackScreen.color.r,
            Instance.i_blackScreen.color.g,
            Instance.i_blackScreen.color.b,
            value
        );
    }
    
    public static void FadeOut()
    {
        Instance.fade_percent = 0;
    }
    public static void FadeIn()
    {
        Instance.fade_percent = 1;
    }


    #endregion





    #region OPEN/CLOSE

    public void ShowConsole()
    {
        g_console.gameObject.SetActive(true);
    }

    public void HideConsole()
    {
        g_console.gameObject.SetActive(false);
    }
    
    // these 2 are gonna be the most used
    // ***
    public static void ToggleConsole()
    {
        if (Instance.consoleTabs.connectedObjects[1].activeInHierarchy)
        {
            Instance.HideConsole();
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Instance.ShowConsole();
            Instance.consoleTabs.SetTabIndex(1);

            Cursor.lockState = CursorLockMode.None;
        }
    }
    public static void ToggleCheatMenu()
    {
        if (Instance.consoleTabs.connectedObjects[0].activeInHierarchy)
        {
            Instance.HideConsole();
            Cursor.lockState = CursorLockMode.Locked;
        } else
        {
            Instance.ShowConsole();
            Instance.consoleTabs.SetTabIndex(0);

            Cursor.lockState = CursorLockMode.None;
        }
    }
    // ***
    
    public void ToggleDebugMenu()
    {
        g_debugMenu.SetActive(!g_debugMenu.activeSelf);
    }

    public static void ShowPlayerHUD()
    {
        Instance.player_hud.gameObject.SetActive(true);
    }
    public static void HidePlayerHUD()
    {
        Instance.player_hud.gameObject.SetActive(false);
    }


    public void OpenInventory()
    {
        // making sure that we can SEE the widget
        inventory.gameObject.SetActive(true);

        // actually building the widget
        inventory.OpenPlayerInventory();

        // because the gameobject for the inventory stays active,
        // we need a tracking variable to keep up with it
        is_inventory_open = true;

        Player.LockAll();
    }
    public void CloseInventory()
    {
        //inventory.gameObject.SetActive(false);

        Player.UnlockAll();
        is_inventory_open = false;
    }
    public void ToggleInventory()
    {
        if (!is_inventory_open)
        {
            OpenInventory();
        } else
        {
            CloseInventory();
        }
    }



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
        g_console.gameObject.SetActive(false);
    }


    public void InMenuUpdate()
    {
        CameraController.Instance.UpdateCamera();
    }


    // not just 'update', because i only want to run this sometimes
    public void InGameUpdate()
    {
        CameraController.Instance.UpdateCamera();
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
