using UnityEngine;

// using this instead of a dictionary or smth, because i might need to expand things later
[System.Serializable]
public class audio_soundmaterial
{
    public string name;
    public Material[] applicableMaterials;
    public string sound_name;

    public audio_soundmaterial() {}

    public audio_soundmaterial(string name, Material[] applicableMaterials, string sound_name)
    {
        this.name = name;
        this.applicableMaterials = applicableMaterials;
        this.sound_name = sound_name;
    }
}
