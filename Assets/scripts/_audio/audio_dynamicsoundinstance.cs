using System;
using UnityEngine;

[System.Serializable]
public class audio_dynamicsoundinstance
{
    public audio_dynamicsound sound_data;
    public Func<float> progress_speed;
    public AudioSource src;
    public bool is_playing;
    public float pitch_offset;
    public float max_recorded_processed_speed;

    public audio_dynamicsoundinstance() {}

    public audio_dynamicsoundinstance(audio_dynamicsound sound_data, Func<float> progress_speed, AudioSource src)
    {
        this.sound_data = sound_data;
        this.progress_speed = progress_speed;
        this.src = src;
    }
}
