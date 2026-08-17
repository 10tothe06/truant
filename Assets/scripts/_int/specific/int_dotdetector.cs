using UnityEngine;
using UnityEngine.Events;

/*

INFO:

so the name of this script may be misleading,
what this is is a trigger that works by calculating a dot product

basically it can tell what "side" of an object the player is on

*/

public class int_dotdetector : MonoBehaviour
{
    public float detection_range = 15f;

    public bool is_active {get; private set;}
    public bool value {get; private set;}
    private bool old_value;



    [Header("UNITY EVENTS")]
    public UnityEvent onValueTrue;
    public UnityEvent onValueFalse;

    public void Activate() // turns the detecting logic on
    {
        
    }
    public void Deactivate() // turns the detecting logic off
    {
        
    }

    void Update() // the detecting logic in question
    {
        old_value = value;
        value = Vector3.Dot(Player.t.position - transform.position, transform.forward) > 0 && Vector3.Distance(Player.t.position, transform.position) < detection_range;

        if (value && !old_value)
        {
            onValueTrue.Invoke();
        }
        if (!value && old_value)
        {
            onValueFalse.Invoke();
        }
    }
}
