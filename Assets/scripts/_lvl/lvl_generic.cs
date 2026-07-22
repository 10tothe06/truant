using UnityEngine;
using UnityEngine.Events;

public class lvl_generic : MonoBehaviour
{
    public UnityEvent onLevelEnter;

    // called by either booting into a level directly from the editor, 
    // or loading into one in-game
    public void EnterLevel()
    {
        onLevelEnter.Invoke();
    }

    public void ExitLevel()
    {
        
    }
}
