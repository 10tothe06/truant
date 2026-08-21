using UnityEngine;

// the int_physicslever component handles the actual movement of the door,
// but opening/closing is done by this script

public class int_door : MonoBehaviour
{
    private InteractableObject3D io;

    private audio_channel creak_channel;

    void Awake()
    {
        io = GetComponent<InteractableObject3D>();
    }

    void Start()
    {
        // this sound will persist throughout,
        // it should ONLY be destroyed when the door is
        // (has to be in Start(), not Awake(), because the AudioManager needs time to initialize)
        creak_channel = AudioManager.PlayDynamicSound("door_creak", () => GetCreakSpeed(), transform.position);
    }


    void OnDestroy()
    {
        AudioManager.DestroyChannel(creak_channel);
    }


    // goofy ahh function name
    public float GetCreakSpeed()
    {
        return GetComponent<Rigidbody>().angularVelocity.y;
    }
}
