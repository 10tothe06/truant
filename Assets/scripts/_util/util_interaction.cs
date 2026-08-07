using UnityEngine;

public class util_interaction
{
    public static InteractableObject3D FindInteractionComponent(GameObject g)
    {
        if (g.GetComponent<InteractableObject3D>() != null)
        {
            return g.GetComponent<InteractableObject3D>();
        }
        if (g.GetComponent<InteractCollider>() != null)
        {
            return g.GetComponent<InteractCollider>().parentObject;
        }

        return null;
    }
}
