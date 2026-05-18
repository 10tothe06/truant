using UnityEngine;

// using this instead of a dictionary or smth, because i might need to expand things later
[System.Serializable]
public class audio_soundmaterial
{
    public Material[] applicableMaterials;
    public audio_soundset sound;
}
