using UnityEngine;

// script to handle SPECIFICALLY LEVEL-RELATED things,
// because I forsee the GameManager script getting too full of shit

// and I like manager scripts

public class LevelManager : MonoBehaviour
{
    private static LevelManager _instance;

    public static LevelManager Instance
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
    }
}
