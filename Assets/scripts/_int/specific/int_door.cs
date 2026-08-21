using UnityEngine;

// the int_physicslever component handles the actual movement of the door,
// but opening/closing is done by this script

public class int_door : MonoBehaviour
{
    private InteractableObject3D io;
    private int_physicslever lever;
    private int_jiggle jiggle;
    private audio_channel creak_channel;
    
    public bool start_locked;
    
    public bool is_locked {get; private set;}



    void Awake()
    {
        io = GetComponent<InteractableObject3D>();
        lever = GetComponent<int_physicslever>();
        jiggle = GetComponent<int_jiggle>();

        io.onInteract.AddListener(Interact);
    }

    void Start()
    {
        // this sound will persist throughout,
        // it should ONLY be destroyed when the door is
        // (has to be in Start(), not Awake(), because the AudioManager needs time to initialize)
        creak_channel = AudioManager.PlayDynamicSound("door_creak", () => GetCreakSpeed(), transform.position);

        if (start_locked)
        {
            Lock();
        }
    }

    private void Rattle()
    {
        AudioManager.PlaySound("door_rattle", transform.position);

        // little bit of jiggle too
        jiggle.Jiggle(0.01f);
    }

    public void Interact()
    {
        // TODO: have a dialog line that says that door is locked?

        if (is_locked)
        {
            Rattle();
        }
    }

    public void Lock()
    {
        AudioManager.PlaySound("door_lock", transform.position);
        is_locked = true;

        // we stop the int_physics lever from moving
        lever.Freeze();
    }
    public void Unlock()
    {
        AudioManager.PlaySound("door_unlock", transform.position);
        is_locked = false;

        lever.UnFreeze();
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
