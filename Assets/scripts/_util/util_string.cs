using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class util_string
{   
    public static string ParseQuaternion(Quaternion q)
    {
        string result = "";

        result += q.x;
        result += ",";
        result += q.y;
        result += ",";
        result += q.z;
        result += ",";
        result += q.w;

        return result;
    }
    public static Quaternion ParseQuaternion(string s)
    {
        string[] split = SplitByChar(s,',');
        return new Quaternion(
            float.Parse(split[0]),
            float.Parse(split[1]),
            float.Parse(split[2]),
            float.Parse(split[3])
            );
    }
    public static string Vector3ToString(Vector3 v)
    {
        string result = "";

        result += v.x.ToString();
        result += ",";
        result += v.y.ToString();
        result += ",";
        result += v.z.ToString();

        return result;
    }
    public static Vector3 StringToVector3(string s)
    {
        string[] split = SplitByChar(s, ',');
        return new Vector3(float.Parse(split[0]), float.Parse(split[1]), float.Parse(split[2]));
    }



    public static string[] AddToArray(string[] old, string toAdd)
    {
        string[] result = new string[old.Length + 1];

        for (int i = 0; i < old.Length; i++)
        {
            result[i] = old[i];
        }
        result[result.Length - 1] = toAdd;

        return result;
    }
    public static string RemoveChars(string raw, char[] toRemove)
    {
        string result = "";

        for (int i = 0; i < raw.Length; i++)
        {
            if (!toRemove.Contains(raw[i]))
            {
                result += raw[i];
            }
        }

        return result;
    }
    public static string[] SplitByChar(string raw, char c)
    {
        string current = "";
        List<string> results = new List<string>();

        for (int i = 0; i < raw.Length; i++)
        {
            if (raw[i] == c)
            {
                if (current.Length > 0) {results.Add(current);}
                current = "";
            } else if (i == raw.Length - 1)
            {
                current += raw[i];
                if (current.Length > 0) {results.Add(current);}
            } else
            {
                current += raw[i];
            }
        }

        return results.ToArray();
    }
    
    // uses spaces ' ' as the split character, doesn't include them in the result
    public static string[] SplitIntoWords(string raw)
    {
        return SplitByChar(raw, ' ');
    }

    public static string CharsToString(char[] chars)
    {
        string result = "";

        for (int i = 0; i < chars.Length; i++)
        {
            result+=chars[i];
        }

        return result;
    }

    public static int FindFirstOccurance(string input, char toLookFor)
    {
        return FindOccurance(input, toLookFor, 0);
    }

    public static int FindOccurance(string input, char toLookFor, int numSkips)
    {
        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] == toLookFor)
            {
                if (numSkips == 0)
                {
                    return i;
                }
                else
                {
                    numSkips--;
                }
            }
        }

        return -1;
    }
}
