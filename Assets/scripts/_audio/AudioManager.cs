using System;
using System.Collections.Generic;
using TreeEditor;
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
        dynamic_sound_instances = new List<audio_dynamicsoundinstance>();
    }   


    [Header("CHANNEL UPDATES")]
    public float channel_list_update_interval = 2f;
    private float last_channel_list_update; // Time.time value



    public audio_musictrack[] musicTracks;

    public GameObject p_audioChannel;

    public audio_soundgroup[] variableSounds;
    public audio_sound[] staticSounds;

    // these are special,
    // and are not treated like other sounds (called by different functions, not playable via 'playsound')
    public audio_dynamicsound[] dynamic_sounds;
    private List<audio_dynamicsoundinstance> dynamic_sound_instances;

    // for use with things like footsteps (player and mosnter)
    [Space(15)]
    public audio_soundmaterial[] footstep_sounds;
    public string defaultStepSound;

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



    private void Update()
    {
        // deleting channels that aren't playing anymore
        // (if they're not dynamic)
        // if one IS dynamic, then it'll have to be destroyed by the object that created it in the first place
        // *****

        if (Time.time > last_channel_list_update + channel_list_update_interval)
        {
            last_channel_list_update = Time.time;

            for (int i = 0; i < transform.childCount; i++)
            {
                if (!transform.GetChild(i).GetComponent<AudioSource>().isPlaying)
                {
                    if (!transform.GetChild(i).GetComponent<audio_channel>().is_dynamic)
                    {
                        DestroyChannel(transform.GetChild(i).GetComponent<audio_channel>());
                    }
                }
            }
        }

        // *****



        // updating the AudioSource components associated with dynamic sounds
        // ****

        // this ONE loop may be the most complicated audio-related piece of code that I've ever written!
        // it's so bad it's actually scary!
        // am I going to comment it? fuck no!
        // future me is going to probably just have to burn it!


        for (int i = 0; i < dynamic_sound_instances.Count; i++)
        {
            float raw_progress_speed = dynamic_sound_instances[i].progress_speed.Invoke();
            float processed_progress_speed = Mathf.Clamp(Mathf.Abs(raw_progress_speed) - 0.2f, 0.75f, 1.25f);

            if (processed_progress_speed > dynamic_sound_instances[i].max_recorded_processed_speed)
            {
                dynamic_sound_instances[i].max_recorded_processed_speed = processed_progress_speed;

                
                dynamic_sound_instances[i].pitch_offset = UnityEngine.Random.Range(-0.25f, 0.25f);
                dynamic_sound_instances[i].src.Play();
            }

            if (processed_progress_speed == 0)
            {
                dynamic_sound_instances[i].src.Stop();
                dynamic_sound_instances[i].is_playing = false;
                dynamic_sound_instances[i].max_recorded_processed_speed = 0;
            } else
            {
                if (!dynamic_sound_instances[i].is_playing)
                {
                    if (raw_progress_speed > 0)
                    {
                        dynamic_sound_instances[i].src.clip = dynamic_sound_instances[i].sound_data.forwards;
                    } else
                    {
                        dynamic_sound_instances[i].src.clip = dynamic_sound_instances[i].sound_data.backwards;
                    }

                    dynamic_sound_instances[i].pitch_offset = UnityEngine.Random.Range(-0.25f, 0.25f);

                    dynamic_sound_instances[i].src.Play();
                    dynamic_sound_instances[i].is_playing = true;
                } else
                {
                    if (!dynamic_sound_instances[i].src.isPlaying)
                    {
                        if (Mathf.Abs(raw_progress_speed) < dynamic_sound_instances[i].max_recorded_processed_speed/2f)
                        {
                            dynamic_sound_instances[i].is_playing = false;
                            dynamic_sound_instances[i].max_recorded_processed_speed = 0;
                        }
                    }

                    if (dynamic_sound_instances[i].src.clip == dynamic_sound_instances[i].sound_data.forwards && raw_progress_speed < 0)
                    {
                        dynamic_sound_instances[i].src.Stop();
                        dynamic_sound_instances[i].src.clip = dynamic_sound_instances[i].sound_data.backwards;

                        dynamic_sound_instances[i].pitch_offset = UnityEngine.Random.Range(-0.25f, 0.25f);
                        dynamic_sound_instances[i].src.Play();
                    } else if (dynamic_sound_instances[i].src.clip == dynamic_sound_instances[i].sound_data.backwards && raw_progress_speed > 0)
                    {
                        dynamic_sound_instances[i].src.Stop();
                        dynamic_sound_instances[i].src.clip = dynamic_sound_instances[i].sound_data.forwards;

                        dynamic_sound_instances[i].pitch_offset = UnityEngine.Random.Range(-0.25f, 0.25f);
                        dynamic_sound_instances[i].src.Play();
                    }
                }

                dynamic_sound_instances[i].src.volume = Mathf.Abs(raw_progress_speed);
                dynamic_sound_instances[i].src.pitch = processed_progress_speed + dynamic_sound_instances[i].pitch_offset;
            }
        }


        // ****
    }



    #region SOUNDS


    public static void DestroyChannel(audio_channel channel)
    {
        if (channel.dynamic_instance != null)
        {
            Instance.dynamic_sound_instances.Remove(channel.dynamic_instance);
        }

        if (channel.gameObject != null) // this null check is needed to stop a weird error when closing the game in the editor
        {
            Destroy(channel.gameObject);
        }
    }

    public static audio_dynamicsound GetDynamicSoundFromName(string name)
    {
        for (int i = 0; i < Instance.dynamic_sounds.Length; i++)
        {
            if (Instance.dynamic_sounds[i].name == name)
            {
                return Instance.dynamic_sounds[i];
            }
        }

        return null;
    }

    
    public static audio_channel PlayDynamicSound(string sound_name, Func<float> progress_speed, Vector3 position)
    {
        AudioSource src = Instantiate(Instance.p_audioChannel, Instance.transform).GetComponent<AudioSource>();
        src.transform.position = position;
        src.spatialBlend = 1f;
        src.volume = 1f; // TEMP
        src.loop = false;

        src.GetComponent<audio_channel>().is_dynamic = true;

        Instance.dynamic_sound_instances.Add(new audio_dynamicsoundinstance(GetDynamicSoundFromName(sound_name), progress_speed, src));

        src.GetComponent<audio_channel>().dynamic_instance = Instance.dynamic_sound_instances[Instance.dynamic_sound_instances.Count - 1];




        return src.GetComponent<audio_channel>();
    }


    public static void PlayAudioClip(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f)
    {
        if (clip == null) {return;}
        Instance.SpawnAudioTrack(clip, position, volume, pitch);
    }
    public void SpawnAudioTrack(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f)
    {
        if (clip == null) {return;}
        GameObject g_newChannel = Instantiate(p_audioChannel, transform);

        AudioSource comp = g_newChannel.GetComponent<AudioSource>();
        comp.transform.position = position;
        if (position != Vector3.zero)
        {
            comp.spatialBlend = 1f;
        } else
        {
            comp.spatialBlend = 0f;
        }
        comp.clip = clip;
        comp.loop = false;
        comp.volume = volume;
        comp.pitch = pitch;
        comp.Play();
    }

    private float GetMinPitch(string sound_name)
    {
        // first, check variable sounds
        for (int i = 0; i < Instance.variableSounds.Length; i++)
        {
            if (Instance.variableSounds[i].name == sound_name)
            {
                return Instance.variableSounds[i].min_pitch;
            }
        }

        // then, static sounds
        for (int i = 0; i < Instance.staticSounds.Length; i++)
        {
            if (Instance.staticSounds[i].name == sound_name)
            {
                return Instance.staticSounds[i].min_pitch;
            }
        }

        return 1f;
    }
    private float GetMaxPitch(string sound_name)
    {
        // first, check variable sounds
        for (int i = 0; i < Instance.variableSounds.Length; i++)
        {
            if (Instance.variableSounds[i].name == sound_name)
            {
                return Instance.variableSounds[i].max_pitch;
            }
        }

        // then, static sounds
        for (int i = 0; i < Instance.staticSounds.Length; i++)
        {
            if (Instance.staticSounds[i].name == sound_name)
            {
                return Instance.staticSounds[i].max_pitch;
            }
        }

        return 1f;
    }


    public static void PlaySound(string sound_name, float volume = 1f)
    {
        PlaySound(sound_name, Vector3.zero, volume);
    }
    public static void PlaySound(string sound_name, Vector3 position, float volume = 1f)
    {
        float pitch = UnityEngine.Random.Range(Instance.GetMinPitch(sound_name), Instance.GetMaxPitch(sound_name));
        PlayAudioClip(GetSoundFromName(sound_name), position, volume, pitch);
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
        Instance.SpawnAudioTrack(Instance.staticSounds[index].clip, Vector3.zero);
    }
    [Obsolete]
    public static void PlayVariableSound(int index, bool loop = false)
    {
        Instance.SpawnAudioTrack(Instance.variableSounds[index].Get(), Vector3.zero);
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
