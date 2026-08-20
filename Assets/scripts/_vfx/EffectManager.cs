using UnityEngine;

// central, organized script for managing particle effects
// (something that I weirdly have not done before)

public class EffectManager : MonoBehaviour
{
    private static EffectManager _instance;

    public static EffectManager Instance {
        get => _instance;
        private set {
            if (_instance == null) {
                _instance = value;
            }
            else if (_instance != value) {
                Debug.Log("You messed up buddy.");
                Destroy(value);
            }
        }
    }

    void Awake()
    {
        Instance = this;
    }

    public static void Play(string effect_name, Vector3 effect_position)
    {
        
    }
}
