using System;
using System.IO;
using UnityEngine;
using System.Diagnostics;

public class util_file : MonoBehaviour
{
    public static string workingDir = Application.persistentDataPath;



    public static void OpenUrl(string url)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }
    


    public static string GetWorkingDirectory()
    {
        return EnsureTrailingSlash(workingDir) + Program.Instance.version + "/";
    }
    public static string GetRawWorkingDirectory()
    {
        // i ensure trailing slash because I don't trust how unity formats the directory
        // too lazy to look
        return EnsureTrailingSlash(workingDir);
    }

    public static string EnsureTrailingSlash(string str)
    {
        char lastChar = str[str.Length - 1];

        if (lastChar == '/' || lastChar == '\\')
        {
            return str;
        } else
        {
            return str + "\\";
        }
    }

    public static Texture2D LoadPNG(string filePath)
    {

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        return tex;
    }

    // replaces one line of a text file with a string
    //returns null if goes right, return error message if goes wrong
    public static string ModifyTxtWithNoSurprises(string file, int lineIndex, string newText)
    {
        try
        {
            var lines = File.ReadAllLines(file);
            lines[lineIndex] = newText;
            File.WriteAllLines(file, lines);
        }
        catch (Exception e)
        {
            return e.Message;
        }
        return null;
    }
}
