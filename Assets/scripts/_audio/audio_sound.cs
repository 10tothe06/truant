using UnityEngine;

[System.Serializable]
public class audio_sound
{
    public string name;
    public AudioClip clip;

    public audio_sound() {}

    public audio_sound(string name, AudioClip clip)
    {
        this.name = name;
        this.clip = clip;
    }
}
