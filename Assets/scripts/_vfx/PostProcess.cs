using UnityEngine;

// literally everything that goes on the camera,
// from fog to the drunk effect

public class PostProcess : MonoBehaviour
{
    private static PostProcess _instance;

    public static PostProcess Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
            {
                _instance = value;
            }
            else if (_instance != value)
            {
                Debug.Log("You messed up buddy.");
                Destroy(value);
            }
        }
    }

    void Awake()
    {
        Instance = this;

        Player.drunk_level = 1;
    }

    public ImageEffect effect_drunk;
    public ImageEffect effect_fog;

    void Update()
    {
        effect_drunk.effect.SetFloat("_Amt", Player.drunk_level);
    }
}
