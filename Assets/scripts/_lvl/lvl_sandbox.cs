using UnityEngine;

public class lvl_sandbox : MonoBehaviour
{
    private lvl_generic g;

    void Awake()
    {
        g =GetComponent<lvl_generic>();

        g.onLevelEnter.AddListener(OnLevelEnter);
    }


    public void OnLevelEnter()
    {
        
    }
}
