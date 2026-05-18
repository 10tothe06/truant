using UnityEngine;

public class framelocker : MonoBehaviour
{
    public int framerate;

    void Start()
    {
        Application.targetFrameRate = framerate;
    }
}
