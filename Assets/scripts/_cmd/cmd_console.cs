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

    // the difference between operator and admin commands
    public bool needsAdmin;
    public bool canBeRunLocally;

    public cmd_commandarg[] args;

    public cmd_consolecommand() {}
    public cmd_consolecommand(string[] names)
    {
        this.names = names;
        needsAdmin = false;
        canBeRunLocally = false;
    }
    public cmd_consolecommand(string[] names, bool needsAdmin, bool canBeRunLocally)
    {
        this.names = names;
        this.needsAdmin = needsAdmin;
        this.canBeRunLocally = canBeRunLocally;
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
        // CURRENT:
        new cmd_consolecommand(new string[]{"tp"},false,false), // teleport
        new cmd_consolecommand(new string[]{"systp"},false,false), // (planetary) system teleport

        new cmd_consolecommand(new string[]{"fspeed"},false,true), // freecam speed

        new cmd_consolecommand(new string[]{"whitelist","wlist"},true,false), // allow a player on a server
        new cmd_consolecommand(new string[]{"blacklist","blist"},true,false), // block a player from a server
        new cmd_consolecommand(new string[]{"kick","k"},true,false), // remove a player from a server
        new cmd_consolecommand(new string[]{"ban","b"},true,false), // kick + blacklist

        new cmd_consolecommand(new string[]{"spawn"},false,false), // spawn entity

        new cmd_consolecommand(new string[]{"chat","c"},false,false), // big text for all players

        new cmd_consolecommand(new string[]{"p","perm"},true,false), // change permission

        new cmd_consolecommand(new string[]{"sandbox","sbox"},true,false), // go in/out of sandbox

        // for debugging purposes
        new cmd_consolecommand(new string[]{"error","err"},false,true),
        new cmd_consolecommand(new string[]{"exception","exc"},false,true),


        new cmd_consolecommand(new string[]{"kill"},false,false), // killing an entity


        // FUTURE:
        new cmd_consolecommand(new string[]{"timeset","t"},false,false), // set time 
        new cmd_consolecommand(new string[]{"title"},false,false), // big text for all players
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
        if (selectedCommand == null) {return;}

        else
        {
            PostToConsole(items[0]);
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
