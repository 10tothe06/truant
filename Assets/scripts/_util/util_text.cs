using TMPro;
using UnityEngine;

// not strings,
// this is specifically for UI text stuff

public class util_text
{
    public static FontStyles RemoveStyle(FontStyles old, FontStyles toRemove)
    {
        return old ^ (old & toRemove);
    }

    public static FontStyles AddStyle(FontStyles old, FontStyles toRemove)
    {
        return old | toRemove;
    }
}
