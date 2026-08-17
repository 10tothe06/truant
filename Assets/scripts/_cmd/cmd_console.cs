using System.Linq;
using TMPro;
using UnityEngine;

public enum cmd_consoleerror
{
    WrongNumArgs,
}

public enum cmd_commandarg
{
    DecimalNumber,
    IntegerNumber,
    Text,
    PlayerName,
}

[System.Serializable]
public class cmd_consolecommand
{
    // we have multiple so that commands like 'teleport'
    // can also have shorthands ('tp')
    public string[] names;
    public bool canBeRunLocally;

    public cmd_commandarg[] args;

    public cmd_consolecommand() {}
    public cmd_consolecommand(string[] names)
    {
        this.names = names;
    }

    public bool IsValid(string name)
    {
        return names.Contains(name);
    }
}

public class cmd_console : MonoBehaviour
{
    private static cmd_console _instance;
    public static cmd_console Instance
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

    public ui_console menu;

    public static cmd_consolecommand[] possibleCommands = new cmd_consolecommand[]
    {
        new cmd_consolecommand(new string[]{"tp"}), // teleport
        
        new cmd_consolecommand(new string[]{"summon"}), // spawns objects
        new cmd_consolecommand(new string[]{"give"}), // give item to player

        new cmd_consolecommand(new string[]{"tpcar"}), // go to the car, like, RIGHT NOW
        new cmd_consolecommand(new string[]{"caritems"}), // spawn every car-relevant item

        new cmd_consolecommand(new string[]{"help"}), // show every console command
    };
    
    public static cmd_consolecommand GetCommandData(string name)
    {
        for (int i = 0; i < possibleCommands.Length; i++)
        {
            if (possibleCommands[i].names.Contains(name))
            {
                return possibleCommands[i];
            }
        }

        return null;
    }


    // this is called from the UI
    // it will call ProcessMessage(), and then call ShipMessageToServer() if needed
    public void TryRunCommand(TMP_InputField input)
    {
        ProcessMessage(input.text);
    }

    public cmd_consolecommand GetSelectedCommand(string commandName)
    {
        for (int i = 0; i < possibleCommands.Length; i++)
        {
            if (possibleCommands[i].names.Contains(commandName))
            {
                return possibleCommands[i];
            }
        }
        return null;
    }

    public void ProcessMessage(string text)
    {
        string[] items = util_string.SplitIntoWords(text);

        if (items.Length == 0) {return;}

        // the VERY FIRST THING WE HAVE TO DO IS CHECK IF THE COMMAND CAN BE LOCAL
        cmd_consolecommand selectedCommand = GetSelectedCommand(items[0]);
        if (selectedCommand == null)
        {
            // no command means text
            PostToConsole(text);
        } 
        
        // tp
        else if (selectedCommand == possibleCommands[0])
        {
            
        }

        // summon (object)
        else if (selectedCommand == possibleCommands[1])
        {
            ObjectManager.SpawnObject(items[1], Player.Instance.transform.position);
        }

        // give (item)
        else if (selectedCommand == possibleCommands[2])
        {
            // the arguments for this command MUST be as follows:
            // 1 - the command name itself "give"
            // 2 - the ITEM NAME
            // 3 - the item count

            string item_name = items[1];
            int parsed_item_count = 1;

            if (int.TryParse(items[2], out parsed_item_count))
            {
                Player.GiveItem(new inv_itemstack(item_name, parsed_item_count, -1));
            }
        }

        // tpcar
        else if (selectedCommand == possibleCommands[3])
        {
            GameObject g_car = GameObject.Find("car_1");

            if (g_car != null)
            {
                Player.TeleportTo(g_car.transform.position + Vector3.forward * 4f);
            }
        }

        //caritems
        else if (selectedCommand == possibleCommands[4])
        {
            ObjectManager.SpawnObject("gascan", Player.Instance.transform.position);


            ObjectManager.SpawnObject("car_tire", Player.Instance.transform.position);
            ObjectManager.SpawnObject("car_tire", Player.Instance.transform.position);
            ObjectManager.SpawnObject("car_tire", Player.Instance.transform.position);
            ObjectManager.SpawnObject("car_tire", Player.Instance.transform.position);

            ObjectManager.SpawnObject("car_headlight", Player.Instance.transform.position);
            ObjectManager.SpawnObject("car_headlight", Player.Instance.transform.position);

            ObjectManager.SpawnObject("car_battery", Player.Instance.transform.position);
        }

        // help
        else if (selectedCommand == possibleCommands[5])
        {
            PostToConsole("list of commands:", Color.yellow);

            PostToConsole("tp", Color.yellow);
            PostToConsole("summon {object_name}", Color.yellow);
            PostToConsole("give {item_name} {item_count}", Color.yellow);

            PostToConsole("tpcar", Color.yellow);
            PostToConsole("caritems", Color.yellow);
        }
    }

    public static void MakeError()
    {
        Debug.LogError("TEST ERROR");
    }

    public static void MakeException()
    {
        string[] items = new string[1];

        items[15] = "test"; // this throws an error
    }

    bool ArgCheck(string[] items, cmd_consolecommand command)
    {
        bool validCount = items.Length == command.args.Length + 1;
        if (!validCount)
        {
            PostErrorToConsole(cmd_consoleerror.WrongNumArgs);
        }

        bool validTypes = true;

        return validCount && validTypes;
    }

    public void PostErrorToConsole(cmd_consoleerror error)
    {
        if (error == cmd_consoleerror.WrongNumArgs)
        {
            PostToConsole("Invalid number of arguments!");
        }
    }

    // replaces "debug.log", making it so messages appear in the in-game console as well as the unity one
    public void DebugLog(string msg)
    {
        // our console
        PostToConsole(msg);

        // unity console
        Debug.Log(msg);
    }
    public void DebugLog(string msg, Color col)
    {
        // our console
        PostToConsole(msg, col);

        // unity console
        Debug.Log(msg);
    }

    public void PostToConsole(string msg)
    {
        menu.PostMessage(msg);
    }
    public void PostToConsole(string msg, Color col)
    {
        menu.PostMessage(msg, col);
    }
}
