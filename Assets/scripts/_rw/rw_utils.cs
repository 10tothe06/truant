using UnityEngine;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

// utility class, contains functions for modifying files/file paths
public class rw_utils : MonoBehaviour
{
    // this ends up being the appdata/locallow/10tothe6/etc. directory for windows
    public static string saveDirectory;

    void Awake()
    {
        rw_utils.saveDirectory = UnityEngine.Application.persistentDataPath;
    }

    // *****************************************************************************

    

    public static void SaveFlags(GameFlags _flags)
    {
        string savePath = saveDirectory + "/" + "v" + Program.Instance.version + "/";

        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(savePath + "flags", FileMode.Create);

        formatter.Serialize(stream, _flags);
        stream.Close();
    }

    public static GameFlags LoadFlags()
    {
        string loadPath = saveDirectory + "/" + "v" + Program.Instance.version + "/";

        if (File.Exists(loadPath + "flags"))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(loadPath + "flags", FileMode.Open);

            GameFlags flags = formatter.Deserialize(stream) as GameFlags;
            // TODO: add try/catch block here
            stream.Close();
            return flags;
        }
        else
        {
            return null;
        }
    }

    // *****************************************************************************

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