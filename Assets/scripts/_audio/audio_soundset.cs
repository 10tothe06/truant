using UnityEngine;

// used for when a bunch of sounds are playing repeatedly, like footsteps
// instead of just playing one over and over, we can store them in a pool and pick a random one

[System.Serializable]
public class audio_soundset
{
    public AudioClip[] versions;

    public audio_soundset() {}

    public audio_soundset(AudioClip[] versions)
    {
        this.versions = versions;
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
