using UnityEngine;

// used for when a bunch of sounds are playing repeatedly, like footsteps
// instead of just playing one over and over, we can store them in a pool and pick a random one

[System.Serializable]
public class audio_soundgroup
{
    public string name;
    public AudioClip[] versions;


    public float min_pitch = 1f;
    public float max_pitch = 1f;

    public audio_soundgroup() {}

    public audio_soundgroup(string name, AudioClip[] versions)
    {
        this.name = name;
        this.versions = versions;
    }

    public audio_soundgroup(string name, AudioClip[] versions, float pitch_range)
    {
        this.name = name;
        this.versions = versions;

        this.max_pitch = 1 + pitch_range;
        this.min_pitch = 1 - pitch_range;
    }

    public audio_soundgroup(string name, AudioClip[] versions, float min_pitch, float max_pitch)
    {
        this.name = name;
        this.versions = versions;

        this.max_pitch = max_pitch;
        this.min_pitch = min_pitch;
    }

    // grab a random sound
    // TODO: add a system so the same index isn't gotten twice?
    public AudioClip Get() {
        return versions[Random.Range(0, versions.Length)];
    }

    public bool Contains(AudioClip clip) {
        for (int i = 0; i < versions.Length; i++) {
            if (versions[i] == clip) {
                return true;
            }
        }

        return false;
    }
}
