using UnityEngine;

public class int_colliderlist : MonoBehaviour
{
    public Collider[] colliders;


    // these 2 are shortcut functions
    public void DisableAll()
    {
        SetCollidersActive(false);
    }
    public void EnableAll()
    {
        SetCollidersActive(true);
    }



    public void SetCollidersActive(bool active)
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = active;
        }
    }
}
