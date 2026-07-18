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
    public audio_soundmaterial[] materials;
    public audio_soundset defaultStepSound;

    private float rayDistance;

    void Start()
    {
        if (GetComponent<PlayerController>() != null) {
            rayDistance =GetComponent<PlayerController>().raycastDistanceFromFoot;
        }
    }

    // plays the step sound effect
    public void Step()
    {
        Material steppingOn = util_audio.GetMaterialFromRay(transform.position, -Vector3.up, rayDistance, validHits);

        // stacking should be controlled alr so i dont rlly care
        //AudioManager.Instance.PlaySound(util_audio.GetClipFromMaterial(steppingOn, materials, defaultStepSound));
    }
}
