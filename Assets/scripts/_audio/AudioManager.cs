using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;

    public static AudioManager Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
            {
                _instance = value;
            }
            else if (_instance != value)
            {
                Debug.Log("You messed up buddy.");
                Destroy(value);
            }
        }
    }

    void Awake()
    {
        Instance = this;
    }

    public audio_musictrack[] musicTracks;

    public GameObject p_audioChannel;

    public audio_soundset[] variableSounds;
    public AudioClip[] staticSounds;

    // making this its own function so if I ever want to change the sound index I can
    public void ButtonClick()
    {
        // TODO: this
    }

    public void PlayStaticSound(int index, bool loop = false)
    {
        SpawnAudioTrack(staticSounds[index]);
    }
    public void PlayVariableSound(int index, bool loop = false)
    {
        SpawnAudioTrack(variableSounds[index].Get());
    }

    public void SpawnAudioTrack(AudioClip clip)
    {
        GameObject g_newChannel = Instantiate(p_audioChannel, transform);

        AudioSource comp = g_newChannel.GetComponent<AudioSource>();
        comp.clip = clip;
        comp.loop = false;
        comp.volume = Settings.GetFloat("vol_master") * Settings.GetFloat("vol_sfx");
        comp.Play();
    }

    public void PlayMusic(int index)
    {
        // cant just spawn an audio channel because we need a parent for organization purposes


        // dont want 2 or more tracks at the same time
        StopAllMusic();

        // creating a parent audio channel
        GameObject g_channelParent = new GameObject();
        g_channelParent.transform.SetParent(transform);
        g_channelParent.name = "music " + index;

        for (int i = 0; i < musicTracks[index].layers.Length; i++)
        {
            GameObject g_newChannel = Instantiate(p_audioChannel, g_channelParent.transform);

            AudioSource comp = g_newChannel.GetComponent<AudioSource>();
            comp.clip = musicTracks[index].layers[i];
            comp.volume = Settings.GetFloat("vol_master") * Settings.GetFloat("vol_music");

            comp.Play();
        }
    }

    public void StopAllMusic()
    {
        for (int i = transform.childCount - 1; i>=0;i--)
        {
            if (transform.GetChild(i).gameObject.name.Contains("music"))
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
    }
}
