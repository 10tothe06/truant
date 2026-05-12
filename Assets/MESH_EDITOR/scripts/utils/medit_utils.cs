using UnityEngine;

public class medit_utils : MonoBehaviour
{
    // lowkey just a duplicate of the method inside of CanvasUtils.cs, which is the one I normally use,
    // but I need the mesh editor to be self sufficient (so no referencing outside scripts)
    public static void ImmediateDestroy(Transform input) {
        Transform[] toDestroy = input.GetComponentsInChildren<Transform>(true);
        for (int i = toDestroy.Length - 1; i >= 0; i--) {
            if (toDestroy[i] != input) {
                DestroyImmediate(toDestroy[i].gameObject);
            }
            toDestroy[i] = null;
        }
    }
    public static void ImmediateDestroy(Transform input, int toAvoid) {
        Transform[] toDestroy = input.GetComponentsInChildren<Transform>(true);
        for (int i = toDestroy.Length - 1; i >= 0; i--) {
            if (i-1 == toAvoid) {continue;}
            if (toDestroy[i] != input) {
                DestroyImmediate(toDestroy[i].gameObject);
            }
            toDestroy[i] = null;
        }
    }
    public static void ImmediateDestroy(GameObject input) {
        Transform[] toDestroy = input.GetComponentsInChildren<Transform>(true);
        for (int i = toDestroy.Length - 1; i >= 0; i--) {
            if (toDestroy[i].gameObject != input) {
                DestroyImmediate(toDestroy[i].gameObject);
            }
            toDestroy[i] = null;
        }
    }
    public static void ImmediateDestroy(GameObject input, int toAvoid) {
        Transform[] toDestroy = input.GetComponentsInChildren<Transform>(true);
        for (int i = toDestroy.Length - 1; i >= 0; i--) {
            if (i-1 == toAvoid) {continue;}
            if (toDestroy[i].gameObject != input) {
                DestroyImmediate(toDestroy[i].gameObject);
            }
            toDestroy[i] = null;
        }
    }

    public static void SetChildrenActive(Transform input, bool active) {
        Transform[] toDestroy = input.GetComponentsInChildren<Transform>(true);
        for (int i = toDestroy.Length - 1; i >= 0; i--) {
            if (toDestroy[i] != input) {
                toDestroy[i].gameObject.SetActive(active);
            }
            toDestroy[i] = null;
        }
    }
    public static void SetChildrenActive(GameObject input, bool active) {
        Transform[] toDestroy = input.GetComponentsInChildren<Transform>(true);
        for (int i = toDestroy.Length - 1; i >= 0; i--) {
            if (toDestroy[i].gameObject != input) {
                toDestroy[i].gameObject.SetActive(active);
            }
            toDestroy[i] = null;
        }
    }
}
