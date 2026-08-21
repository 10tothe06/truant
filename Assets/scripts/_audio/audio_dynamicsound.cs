using UnityEngine;

// this is a really dumb idea and I love it



[System.Serializable]
public class audio_dynamicsound
{
    public string name;
    public AudioClip forwards;
    public AudioClip backwards;

    public audio_dynamicsound() {}

    public audio_dynamicsound(string name, AudioClip forwards, AudioClip backwards)
    {
        this.name = name;
        this.forwards = forwards;
        this.backwards = backwards;
    }
}
