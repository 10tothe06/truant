using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

// just a way of storing a bunch of static bools for tracking game events
// most of them are for the monster AI

// I don't want to say where this idea came from lol, if ykyk

[System.Serializable]
public class GameFlags
{
    public static GameFlags flags;


    // if they have NOT, they wil load into the tutorial upon starting the gamae
    // if they have finished it, they will load into the lobby

    // this ensures that they have to complete the tutorial
    public bool has_completed_tutorial;



    #region SAVE/LOAD


    // I COULD use plaintext here, or something human-readable
    // but I want it to be at least sort-of tamper resistant

    // TODO(?): make a plaintext version using File.WriteLines()
    public static void TryLoadFlags()
    {
        GameFlags flags_from_disk = null;

        string dir = util_file.GetWorkingDirectory() + "user.flags";

        if (File.Exists(dir))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(dir, FileMode.Open);

            GameFlags flags = formatter.Deserialize(stream) as GameFlags;

            // TODO: add try/catch block here
            stream.Close();


            flags_from_disk = flags;
        }
        
        if (flags_from_disk != null)
        {
            // we got some game flags, so we apply them
            flags = flags_from_disk;
        } else
        {
            // no data so we have to do factory defaults

            flags = GetFactoryDefaults();
        }
    }


    public static GameFlags GetFactoryDefaults()
    {
        GameFlags defaults = new GameFlags();

        defaults.has_completed_tutorial = false; // obviously

        return defaults;
    } 

    #endregion
    
}
