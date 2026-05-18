using UnityEngine;

// for now, just a bunch of shortcut functions

public class cmd
{
    public static void Log(string msg)
    {
        cmd_console.Instance.DebugLog("[DEBUG] " + msg);
    }
    public static void Log(string msg, Color col)
    {
        cmd_console.Instance.DebugLog("[DEBUG] " + msg, col);
    }
    public static void LogError(string msg)
    {
        cmd_console.Instance.DebugLog("[ERROR] " + msg, Color.red);
    }


    public static void LogRaw(string msg)
    {
        cmd_console.Instance.DebugLog(msg);
    }
    public static void LogRaw(string msg, Color col)
    {
        cmd_console.Instance.DebugLog(msg, col);
    }
}
