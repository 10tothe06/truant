using UnityEngine;

// just a way of storing a bunch of static bools for tracking game events
// most of them are for the monster AI

// I don't want to say where this idea came from lol, if ykyk

[System.Serializable]
public class GameFlags
{
    

    public static GameFlags GetCurrent()
    {
        GameFlags data = new GameFlags();

        return data;
    }

    public static void Apply(GameFlags data)
    {
        
    }

    public static void ApplyFactoryDefaults()
    {
         
    }
}
