using UnityEngine;

// mono component for storing data on the audio channel objects

public class audio_channel : MonoBehaviour
{
    public bool is_dynamic;

    // only useful if the channel is dynamic, obviously
    public audio_dynamicsoundinstance dynamic_instance;
}
