using System;
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

    public audio_soundgroup[] variableSounds;
    public audio_sound[] staticSounds;

    // for use with things like footsteps (player and mosnter)
    [Space(15)]
    public audio_soundmaterial[] materials;
    public audio_soundgroup defaultStepSound;

    #region UI


    // making this its own function so if I ever want to change the sound index I can
    public void ButtonClick()
    {
        // TODO: this
    }

    // not clicking, but hovering over a button
    public void ButtonSelect()
    {
        
    }

    #endregion







    #region SOUNDS

    public static void PlayAudioClip(AudioClip clip, float volume = 1f)
    {
        if (clip == null) {return;}
        Instance.SpawnAudioTrack(clip, volume);
    }
    public void SpawnAudioTrack(AudioClip clip, float volume = 1f)
    {
        if (clip == null) {return;}
        GameObject g_newChannel = Instantiate(p_audioChannel, transform);

        AudioSource comp = g_newChannel.GetComponent<AudioSource>();
        comp.clip = clip;
        comp.loop = false;
        comp.volume = Settings.GetFloat("vol_master") * Settings.GetFloat("vol_sfx") * volume;
        comp.Play();
    }


    public static void PlaySound(string sound_name, float volume = 1f)
    {
        PlayAudioClip(GetSoundFromName(sound_name), volume);
    }

    public static AudioClip GetSoundFromName(string sound_name)
    {
        // first, check variable sounds
        for (int i = 0; i < Instance.variableSounds.Length; i++)
        {
            if (Instance.variableSounds[i].name == sound_name)
            {
                return Instance.variableSounds[i].Get();
            }
        }

        // then, static sounds
        for (int i = 0; i < Instance.staticSounds.Length; i++)
        {
            if (Instance.staticSounds[i].name == sound_name)
            {
                return Instance.staticSounds[i].clip;
            }
        }

        // if we dont find it,
        // we have nothing to return
        // (functions have built-in null checks to deal with this)
        return null;
    }


    #endregion




    #region MUSIC

    public static void StopAllMusic()
    {
        for (int i = Instance.transform.childCount - 1; i>=0;i--)
        {
            if (Instance.transform.GetChild(i).gameObject.name.Contains("music"))
            {
                Destroy(Instance.transform.GetChild(i).gameObject);
            }
        }
    }


    #endregion



    // basically everything below here is deprecated:

    #region OLD FUNCTIONS

    [Obsolete]
    public static void PlayStaticSound(int index, bool loop = false)
    {
        Instance.SpawnAudioTrack(Instance.staticSounds[index].clip);
    }
    [Obsolete]
    public static void PlayVariableSound(int index, bool loop = false)
    {
        Instance.SpawnAudioTrack(Instance.variableSounds[index].Get());
    } 

    [Obsolete]
    public static void PlayMusic(int index, float volume)
    {
        // cant just spawn an audio channel because we need a parent for organization purposes


        // dont want 2 or more tracks at the same time
        StopAllMusic();

        // creating a parent audio channel
        GameObject g_channelParent = new GameObject();
        g_channelParent.transform.SetParent(Instance.transform);
        g_channelParent.name = "music " + index;

        for (int i = 0; i < Instance.musicTracks[index].layers.Length; i++)
        {
            GameObject g_newChannel = Instantiate(Instance.p_audioChannel, g_channelParent.transform);

            AudioSource comp = g_newChannel.GetComponent<AudioSource>();
            comp.clip = Instance.musicTracks[index].layers[i];
            comp.volume = Settings.GetFloat("vol_master") * Settings.GetFloat("vol_music") * volume;

            comp.Play();
        }
    }

    #endregion
}
