using UnityEngine;

public class lvl_basement : MonoBehaviour
{
    private lvl_generic gComp;
    void Awake()
    {
        gComp = GetComponent<lvl_generic>();
        gComp.onLevelEnter.AddListener(OnStartLevel);
    }

    public void OnStartLevel()
    {
        
    }
}