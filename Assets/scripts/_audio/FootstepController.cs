using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// separate script so that I can do fancy stuff like different sound effects for different materials

// INFO ON WHAT MATERIALS MAKE WHAT SOUNDS IS ALSO HERE
// im putting it here and not in a manager script because I should only need it here,
// i can just make this script singleton if I need to

public class FootstepController : MonoBehaviour
{
    public LayerMask validHits;
    
    public Transform ray_source;
    public float ray_distance;

    void Start()
    {
        if (GetComponent<PlayerController>() != null) {
            ray_distance = GetComponent<PlayerController>().raycastDistanceFromFoot + 0.1f;
            ray_source = GetComponent<PlayerController>().t_foot;
        }
    }

    // plays the step sound effect
    public void Step()
    {
        Material steppingOn = util_audio.GetMaterialFromRay(ray_source.position, -Vector3.up, ray_distance, validHits);

        // stacking should be controlled alr so i dont rlly care
        AudioManager.PlayAudioClip(util_audio.GetClipFromMaterial(steppingOn, AudioManager.Instance.footstep_sounds, AudioManager.GetSoundFromName(AudioManager.Instance.defaultStepSound)), 0.25f);
    }
}
