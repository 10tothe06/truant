using UnityEngine;

public class int_door : MonoBehaviour
{
    void Start()
    {
        AudioManager.PlayDynamicSound("door_creak", () => GetCreakSpeed(), transform.position);
    }



    // goofy ahh function name
    public float GetCreakSpeed()
    {
        return GetComponent<Rigidbody>().angularVelocity.y;
    }
}
