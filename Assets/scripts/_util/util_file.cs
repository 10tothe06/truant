using UnityEngine;

public class util_file : MonoBehaviour
{
    public static string workingDir = Application.persistentDataPath;

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
}
