using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

// TODO: figure out how to group settings by category

// 07/06:
// achievements are a part of this now cuz im not making another one of these scripts for that

public class Settings : MonoBehaviour
{
    private static Settings _instance;

    public static Settings Instance
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
        settings = ins_settings; // this sets the parameters of the settings, we'll have to read the file to get the actual values

        advancementData = ins_advancementData;

        // just in case
        for (int i = 0; i < settings.Count;i++)
        {
            settings[i].isFilled = false;
        }
    }

    void Start()
    {
        ReadSettingsFile();

        if (Program.Instance.resetAdvancements)
        {
            trackedAdvancements = new List<adv_trackedadvancement>();
            SaveTrackedAdvancements();
        } else
        {
            LoadTrackedAdvancements();
        }
    }

    public static string emptyString = "err";
    public static double emptyDouble = -999d;
    public static float emptyFloat = -999f;

    // to be edited by the user
    public  List<prefs_genericentry> ins_settings;
    // realistically, this will never be accessed directly
    public static List<prefs_genericentry> settings;


    // the hard-coded data
    public List<adv_advancementdata> ins_advancementData;
    public static List<adv_advancementdata> advancementData;

    // the "soft" data (which ones the player has gotten)
    // this is separate for ease of use (and better version control)
    public static List<adv_trackedadvancement> trackedAdvancements;

    #region ADVANCEMENTS


    public static void UnlockAdvancement(string advancement_name)
    {
        // if we have already unlocked the advancement, no need to do any of this
        if (DoesPlayerHaveAdvancement(advancement_name))
        {
            // may as well log it still
            cmd.LogRaw("ALREADY UNLOCKED: " + advancement_name, Color.skyBlue);
            return;
        }

        cmd.LogRaw("UNLOCKED ADVANCEMENT: " + advancement_name, Color.skyBlue);

        adv_trackedadvancement newData = new adv_trackedadvancement();

        newData.hasGotten = true;
        newData.advancement_name = advancement_name;

        trackedAdvancements.Add(newData);
        // file will be written when game closes, 
        // but we do it now to be safe (making sure a crash doesn't erase progress)
        Instance.SaveTrackedAdvancements();

        Instance.StartCoroutine(UIManager.Instance.ShowAdvancementPopup(Instance.GetAdvancementDataFromName(advancement_name)));
    }

    public adv_advancementdata GetAdvancementDataFromName(string name)
    {
        for (int i = 0; i < advancementData.Count; i++)
        {
            if (advancementData[i].name == name)
            {
                return advancementData[i];
            }
        }

        return null;
    }

    public static bool DoesPlayerHaveAdvancement(string advancement_name)
    {
        for (int i = 0; i < trackedAdvancements.Count; i++)
        {
            if (trackedAdvancements[i].advancement_name == advancement_name)
            {
                return true;
            }
        }

        return false;
    }

    private void LoadTrackedAdvancements()
    {
        trackedAdvancements = new List<adv_trackedadvancement>();

        string actualFilePath = "";
        string theoreticalFilePath = util_file.GetWorkingDirectory() + "user.advancements";

        if (File.Exists(theoreticalFilePath))
        {
            // oh cool we found the file, read it
            actualFilePath = theoreticalFilePath;
        } else
        {
            // ah well we didn't find the file
            // now we look in the previous version
            string backupPath = util_file.GetRawWorkingDirectory() + Program.Instance.GetPreviousVersion() + "/user.advancements";

            if (File.Exists(backupPath)) {actualFilePath = backupPath;}
        }

        if (actualFilePath.Length > 0)
        {
            // somewhere we found a file, so let's read what we can
            string[] lines = File.ReadLines(actualFilePath).ToArray();

            for (int i = 0; i < lines.Length; i++)
            {
                string[] elements = util_string.SplitByChar(lines[i],',');

                adv_trackedadvancement newData = new adv_trackedadvancement();

                newData.advancement_name = elements[0];
                bool parsedValue = false;
                if (bool.TryParse(elements[1], out parsedValue))
                {
                    newData.hasGotten = parsedValue;
                }

                trackedAdvancements.Add(newData);
            }
        }
    }

    public void SaveTrackedAdvancements()
    {
        if (!Directory.Exists(util_file.GetWorkingDirectory())) {Directory.CreateDirectory(util_file.GetWorkingDirectory());}
        string filePath = util_file.GetWorkingDirectory() + "user.advancements";

        if (!File.Exists(filePath))
        {
            File.Create(filePath);
        }

        List<string> lines = new List<string>();

        for (int i = 0; i < trackedAdvancements.Count; i++)
        {
            lines.Add(trackedAdvancements[i].advancement_name  + "," + trackedAdvancements[i].hasGotten);
        }

        File.WriteAllLines(filePath, lines.ToArray());
    }


    #endregion


    // annoying conversion, but the modular menu system does need to be standardized
    public static uim_modularmenuentry[] GetModularEntries()
    {
        List<uim_modularmenuentry> result = new List<uim_modularmenuentry>();

        for (int i = 0; i < settings.Count; i++)
        {
            // first, add the title
            uim_modularmenuentry newTitle = new uim_modularmenuentry();

            newTitle.data = settings[i].key;
            newTitle.displayInfo = "";
            newTitle.displayType = (ushort)uim_displaytype.Text;

            // then the data itself
            uim_modularmenuentry newEntry = new uim_modularmenuentry();

            int j = i;
            newEntry.data = settings[j].value;
            newEntry.onDataUpdate.AddListener((x) => {settings[j].value = x;});
            newEntry.displayInfo = settings[j].lowerLimit + "," + settings[j].upperLimit;
            newEntry.displayType = (ushort)settings[j].displayType;

            result.Add(newTitle);
            result.Add(newEntry);
        }

        return result.ToArray();
    }

    // this part works similarly to the OLD WPILib communications protocol
    // ************************************
    
    public static string GetString(string key)
    {
        prefs_genericentry entry = Instance.GetEntryByName(key);
        if (entry == null) {return emptyString;}

        return entry.value;
    }

    public static bool GetBool(string key)
    {
        prefs_genericentry entry = Instance.GetEntryByName(key);
        if (entry == null) {return false;}

        bool parsedValue = false;
        if (bool.TryParse(entry.value,out parsedValue))
        {
            return parsedValue;
        }
        return false;
    }


    public static double GetDouble(string key)
    {
        prefs_genericentry entry = Instance.GetEntryByName(key);
        if (entry == null) {return emptyDouble;}

        double parsedValue = 0;
        if (double.TryParse(entry.value,out parsedValue))
        {
            return parsedValue;
        }
        return emptyDouble;
    }
    public static float GetFloat(string key)
    {
        prefs_genericentry entry = Instance.GetEntryByName(key);
        if (entry == null) {return emptyFloat;}

        float parsedValue = 0;
        if (float.TryParse(entry.value,out parsedValue))
        {
            return parsedValue;
        }
        return emptyFloat;
    }
    //***************************************

    public prefs_genericentry GetEntryByName(string key)
    {
        for (int i = 0; i < settings.Count; i++)
        {
            if (settings[i].key == key)
            {
                return settings[i];
            }
        }

        return null;
    }

    // unlike before, this is read line-by-line
    // we don't actually need to pass in a file path, because we know where the file will be
    public void ReadSettingsFile()
    {
        string actualFilePath = "";
        string theoreticalFilePath = util_file.GetWorkingDirectory() + "user.settings";

        if (File.Exists(theoreticalFilePath))
        {
            // oh cool we found the file, read it
            actualFilePath = theoreticalFilePath;
        } else
        {
            // ah well we didn't find the file
            // now we look in the previous version
            string backupPath = util_file.GetRawWorkingDirectory() + Program.Instance.GetPreviousVersion() + "/user.settings";

            if (File.Exists(backupPath)) {actualFilePath = backupPath;}
        }

        if (actualFilePath.Length > 0)
        {
            // somewhere we found a file, so let's read what we can
            string[] lines = File.ReadLines(actualFilePath).ToArray();

            for (int i = 0; i < lines.Length; i++)
            {
                // keep in mind any accidental spaces will be read too, causing potential issues
                // I'm not dealing with them
                // if the user edits the settings file they better be careful
                string[] elements = util_string.SplitByChar(lines[i],':');

                // first element is always the key
                prefs_genericentry entry = GetEntryByName(elements[0]);
                if (entry == null) {continue;} // that key doesn't match anything

                if (entry.IsValidValue(elements[1]))
                {
                    entry.value = elements[1]; 
                    entry.isFilled = true;
                }
            }
        }

        // here we fill in any data that wasn't found in the file
        for (int i = 0; i < settings.Count; i++)
        {
            if (!settings[i].isFilled)
            {
                settings[i].value = settings[i].defaultValue;
            }
        }

        // we're not going to bother with writing anything to disk, that's another function's job
    }

    public void WriteToSettingsFile()
    {
        if (!Directory.Exists(util_file.GetWorkingDirectory())) {Directory.CreateDirectory(util_file.GetWorkingDirectory());}
        string filePath = util_file.GetWorkingDirectory() + "user.settings";

        if (!File.Exists(filePath))
        {
            File.Create(filePath);
        }

        List<string> lines = new List<string>();

        for (int i = 0; i < settings.Count; i++)
        {
            lines.Add(settings[i].key + ":" + settings[i].value);
        }

        File.WriteAllLines(filePath, lines.ToArray());
    }
}
