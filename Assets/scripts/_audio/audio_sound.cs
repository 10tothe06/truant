using UnityEngine;

[System.Serializable]
public class audio_sound
{
    public string name;
    public AudioClip clip;

    public float min_pitch = 1f;
    public float max_pitch = 1f;

    public audio_sound() {}

    public audio_sound(string name, AudioClip clip)
    {
        this.name = name;
        this.clip = clip;
    }

    public audio_sound(string name, AudioClip clip, float min_pitch, float max_pitch)
    {
        this.name = name;
        this.clip = clip;

        this.min_pitch = min_pitch;
        this.max_pitch = max_pitch;
    }
}
