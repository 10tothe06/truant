using UnityEngine;

public class int_breakable : MonoBehaviour
{
    public float break_threshold;

    void Awake()
    {
        GetComponent<InteractableObject3D>().onImpact.AddListener(TryBreak);
    }

    void TryBreak(float force)
    {
        if (force > break_threshold)
        {
            AudioManager.PlaySound("ping");

            // spawn all of the broken pieces
            EffectManager.SpawnSlices(gameObject.name, transform.position, transform.GetChild(0).rotation, GetComponent<Rigidbody>().linearVelocity, transform.GetChild(0).localScale.x);


            // TODO: maybe spawn a particle too? idk if that will hinder the realism

            // destroy the main object
            Destroy(gameObject);
        }
    }
}
