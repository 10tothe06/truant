using UnityEngine;
using UnityEngine.Events;

public class obj_generic : MonoBehaviour
{
    // called when UpdateEntity() is called
    // which is called every time UpdateGame() is called on the GameManager
    public UnityEvent onEntityUpdate;
}
