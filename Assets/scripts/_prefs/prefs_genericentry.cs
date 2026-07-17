using System.Collections.Generic;
using UnityEngine;

// the kind of variable that is being stored 
public enum prefs_entrytype
{
    Double,
    Float,
    Int,
    String,
    Bool,
}

// like everything else, the way this game handles settings is much more complicated than what I usually do

// like in Drivetools, I'm using strings as a sort of universal data type,
// which is made easier by the fact that 90% of all settings will be primitives - easy to make to/from strings

// TODO: split this into two classes, one for the value and one for the data?

[System.Serializable]
public class prefs_genericentry
{
    // some preferences do not appear in the settings menu, and can only be changed by editing the file
    public bool isHidden; // TODO

    
    public prefs_entrytype type;
    public uim_displaytype displayType;

    public bool isFilled; // a temporary flag that we use when reading the settings in
    public string key;
    public string value;
    public List<string> tags; // so that settings can be organized into categories
    public string lowerLimit;
    public string upperLimit;
    public string defaultValue;

    public prefs_genericentry() {tags = new List<string>();}
    

    public bool IsWithinLimits(string val)
    {
        if (lowerLimit == "" && upperLimit == "") {return true;}

        if (type == prefs_entrytype.Double)
        {
            double parsedVal;
            if (!double.TryParse(val, out parsedVal))
            {
                return false;
            }

            bool hasLower = false;
            bool hasUpper = false;
            double lower;
            double upper;
            if (double.TryParse(lowerLimit, out lower)) {hasLower = true;}
            if (double.TryParse(upperLimit, out upper)) {hasUpper = true;}

            if (hasLower)
            {
                if (parsedVal < lower) {return false;}
            }
            if (hasUpper)
            {
                if (parsedVal > upper) {return false;}
            }

            return true;

        }  else if (type == prefs_entrytype.Bool)
        {
            return val == "true" || val == "false";
        }else if (type == prefs_entrytype.Float)
        {
            // TODO: same thing as double
        } else if (type == prefs_entrytype.String)
        {
            return true;
        }

        return false;
    }

    public bool IsValidValue(string val)
    {
        // this also checks for the parsing to work
        return IsWithinLimits(val);
    }
}
