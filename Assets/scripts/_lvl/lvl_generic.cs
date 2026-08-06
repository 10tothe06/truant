using UnityEngine;
using UnityEngine.Events;

public class lvl_generic : MonoBehaviour
{
    public UnityEvent onLevelEnter;
    public UnityEvent onLevelExit;

    // called by either booting into a level directly from the editor, 
    // or loading into one in-game
    public void EnterLevel()
    {
        gameObject.SetActive(true);

        onLevelEnter.Invoke();

        UIManager.Instance.inventory.OpenPlayerInventory();
    }

    public void ExitLevel(bool was_completed = false)
    {
        onLevelExit.Invoke();

        if (was_completed)
        {
            Debug.Log("💖 Level completed!");
        } else
        {
            Debug.Log("💔 Level exited, incomplete.");
        }
    }
}
