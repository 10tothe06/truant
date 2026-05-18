using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// NEEDS REWORK

// I have an AudioManager in pretty much every project, but as of 07/14 this version has some important upgrades
// use this version

// the MAIN UPGRADE is that instead of an index system, we're now using a hashcode (string) system
// this is better because the channel list shuffles around A LOT, so indices are largely useless
// hashcodes allow effective referencing of audio channels

// yes okay IN THEORY my approach could result in equal hascodes, but with a length of 24 chars (which is what its set to) I don't see that happening

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

    // NEW FUNCTIONS ****

    public UnityEvent<Vector3, float> onPhysicalNoise;

    // the volume is for in-game entities, not the actual volume played through the speakers
    // it's like "how loud was the sound just played?"
    public void PlayPhysicalSound(int id, Vector3 pos, float vol) 
    {
        PlaySoundNoReturn(id, pos);

        onPhysicalNoise.Invoke(pos, vol);
    }

    public void PlaySoundNoReturn(int id, Vector3 pos)
    {
        PlaySound(soundTracks[id].Get(), pos, true, 1, true);
    }

    // a lot of these functions do return the string code, but ofc u can just call them without messing with the return var

    // grabbing the file from the array using an index
    public string PlaySound(int soundId, bool allowStacking)
    {
        return PlaySound(soundTracks[soundId].Get(), Vector3.zero, allowStacking, 1, false);
    }

    public string PlaySound(int soundId)
    {
        return PlaySound(soundTracks[soundId].Get(), Vector3.zero, true, 1, false);
    }

    public void PlaySoundNoReturn(int soundId)
    {
        PlaySound(soundTracks[soundId].Get(), Vector3.zero, true, 1, false);
    }

    // defaults to true for stacking
    public string PlaySound(AudioClip sound)
    {
        return PlaySound(sound, Vector3.zero, true, 1, false);
    }

    public string PlaySound(AudioClip sound, float volume)
    {
        return PlaySound(sound, Vector3.zero, true, volume, false);
    }

    public string PlaySound(int soundId, float volume)
    {
        return PlaySound(soundTracks[soundId].Get(), Vector3.zero, true, volume, false);
    }

    // this is the base version of the function
    public string PlaySound(AudioClip sound, Vector3 pos, bool allowStacking, float volume, bool is3D)
    {
        string channelName = "sfx" + "_" + GenerateHashcode();
        // stacking IS ALLOWED
        GameObject newChannel = Instantiate(channelPrefab, Vector3.zero, Quaternion.identity);
        newChannel.GetComponent<AudioSource>().clip = sound;
        newChannel.transform.SetParent(transform);

        newChannel.transform.position = pos;

        newChannel.name = channelName;

        channels.Add(newChannel.GetComponent<AudioSource>());

        newChannel.GetComponent<AudioSource>().volume = 1f * volume;
        if (is3D)
        {
            newChannel.GetComponent<AudioSource>().spatialBlend = 1;
        }
        newChannel.GetComponent<AudioSource>().Play();

        return channelName;
    }

    // stopping a channel using the hashcode
    public void StopChannel(string code)
    {
        // better, I suppose, than just killing the object from elsewhere

        for (int i = 0; i < channels.Count; i++)
        {
            // the reason I'm using Contains() and not == is because of prefixes like sfx_ and music_
            // should work the same (hopefully)
            if (channels[i].name.Contains(code))
            {
                Destroy(channels[i].gameObject);
                channels.RemoveAt(i);
                break;
            }
        }
    }

    public AudioSource GetChannel(string code)
    {
        // better, I suppose, than just killing the object from elsewhere

        for (int i = 0; i < channels.Count; i++)
        {
            // the reason I'm using Contains() and not == is because of prefixes like sfx_ and music_
            // should work the same (hopefully)
            if (channels[i].name.Contains(code))
            {
                return channels[i].GetComponent<AudioSource>();
            }
        }

        return null;
    }


    // ****

    private void Awake()
    {
        Instance = this;

        channels = new List<AudioSource>();
        ambientLowPassCutoff = 20000;
    }

    // the main idea behind this script (and the reason I need a centralized audio system in the first place) 
    // is channel management. Picking which objects to run the audio source components on, essentially.
    private List<AudioSource> channels;
    public GameObject channelPrefab;

    public audio_soundset[] soundTracks; // using a custom class here because some sounds need variations (usually pitched up/down)
    public AudioClip[] musicTracks; // just using the AudioClip class here because music can only be played one way, no pitches
    public AudioClip[] ambientTracks; // wind and such

    // (this is the target)
    public float ambientLowPassCutoff; // bit of a specific feature but eh
    private float currentCutoff;

    void Update()
    {
        // feel the jank
        if (ambientLowPassCutoff > currentCutoff) {
            currentCutoff = Mathf.Lerp(currentCutoff, ambientLowPassCutoff, 0.05f);
        } else {
            currentCutoff = Mathf.Lerp(currentCutoff, ambientLowPassCutoff, 0.15f);
        }

        for (int i = channels.Count - 1; i >= 0; i--)
        {
            if (channels[i] == null) { Debug.Log("Sir! A problem with a null audio channel!"); }

            if (!channels[i].isPlaying)
            {
                Destroy(channels[i].gameObject);
                channels.RemoveAt(i);
            }
            else
            {
                if (channels[i].gameObject.name.Substring(0, 3) == "amb")
                {
                    channels[i].GetComponent<AudioLowPassFilter>().cutoffFrequency = currentCutoff;
                }
            }
        }
    }

    void FixedUpdate()
    {
        Transform playerTransform = Player.t;

        // this is the bit of code that determines whether the player is "inside" or not
        // that's then used to put a low pass filter over all the outside sounds (wind mainly)

        // originally this was either an "in" or "out" thing, which was determined by shooting a ray up at the ceiling
        // turns out, fairly inaccurate

        // so now we shoot 5 rays and sum them up to create an inside "score" and move the cutoff freq. based on that

        float score = 0; // low means outside
        float defaultVal = 3000;
        float m = 50;

        RaycastHit hit;
        if (Physics.Raycast(playerTransform.position, playerTransform.up, out hit))
        {
            // change the cutoff frequency if we're inside, essentially
            score += defaultVal - hit.distance * m;
        }
        if (Physics.Raycast(playerTransform.position, playerTransform.right, out hit))
        {
            // change the cutoff frequency if we're inside, essentially
            score += defaultVal - hit.distance * m;
        }
        if (Physics.Raycast(playerTransform.position, -playerTransform.right, out hit))
        {
            // change the cutoff frequency if we're inside, essentially
            score += defaultVal - hit.distance * m;
        }
        if (Physics.Raycast(playerTransform.position, playerTransform.forward, out hit))
        {
            // change the cutoff frequency if we're inside, essentially
            score += defaultVal - hit.distance * m;
        }
        if (Physics.Raycast(playerTransform.position, -playerTransform.forward, out hit))
        {
            // change the cutoff frequency if we're inside, essentially
            score += defaultVal - hit.distance * m;
        }

        ambientLowPassCutoff = 15000 - score;
    }

    public string PlayAmbience(AudioClip sound, bool allowStacking)
    {
        string channelName = "amb" + "_" + GenerateHashcode();
        // stacking IS ALLOWED
        GameObject newChannel = Instantiate(channelPrefab, Vector3.zero, Quaternion.identity);
        newChannel.GetComponent<AudioSource>().clip = sound;
        newChannel.transform.SetParent(transform);

        newChannel.name = channelName;

        channels.Add(newChannel.GetComponent<AudioSource>());

        newChannel.GetComponent<AudioSource>().volume = 1f;
        newChannel.GetComponent<AudioSource>().Play();

        return channelName;
    }

    // defaults to true for stacking
    public string PlayAmbience(AudioClip sound)
    {
        return PlayAmbience(sound, true);
    }
    
    public string PlayAmbience(int soundId)
    {
        return PlayAmbience(ambientTracks[soundId], true);
    }

    // music channels, specifically, have their GameObjects labelled "music"
    public string PlayMusic(int musicId)
    {
        StopAllMusic();

        string channelName = "music" + "_" + GenerateHashcode();

        GameObject newChannel = Instantiate(channelPrefab, Vector3.zero, Quaternion.identity);
        newChannel.GetComponent<AudioSource>().clip = musicTracks[musicId];
        newChannel.name = channelName;
        newChannel.transform.SetParent(transform);

        channels.Add(newChannel.GetComponent<AudioSource>());

        newChannel.GetComponent<AudioSource>().Play();

        return channelName;
    }

    public void StopAllMusic()
    {
        for (int i = channels.Count - 1; i >= 0; i--)
        {
            if (channels[i].gameObject.name.Substring(0, 5) == "music")  // the substring call here is okay, because all hascodes are at least 5 chars
            {
                Destroy(channels[i].gameObject);
                channels.RemoveAt(i);
            }
        }
    }

    // okay so-
    // I feel like I've written a function that does this exact thing
    // I can't bother to find it tho
    string GenerateHashcode()
    {
        return GenerateHashcode(24);
    }
    string GenerateHashcode(int len)
    {
        string toReturn = "";

        for (int i = 0; i < len; i++)
        {
            toReturn += Random.Range(0, 10).ToString();
        }

        return toReturn;
    }
}