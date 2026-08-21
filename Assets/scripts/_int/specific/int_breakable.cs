using UnityEngine;

public class int_breakable : MonoBehaviour
{
    public float break_threshold;
    public string break_sound;

    void Awake()
    {
        GetComponent<InteractableObject3D>().onImpact.AddListener(TryBreak);
    }

    void TryBreak(float force)
    {
        if (force > break_threshold)
        {
            AudioManager.PlaySound(break_sound, transform.position);

            // spawn all of the broken pieces
            EffectManager.SpawnSlices(gameObject.name, GetComponentInChildren<MeshRenderer>().sharedMaterial, transform.position, transform.GetChild(0).rotation, GetComponent<Rigidbody>().linearVelocity, transform.GetChild(0).localScale.x, 2f);


            // TODO: maybe spawn a particle too? idk if that will hinder the realism

            // destroy the main object
            Destroy(gameObject);
        }
    }
}
