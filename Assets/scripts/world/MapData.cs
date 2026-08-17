using UnityEngine;

public class MapData : MonoBehaviour
{

    private static MapData _instance;

    // this is used for most things, static functions can also be used when verbosity is a concern
    public static MapData Instance
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

        pixel_width = ins_pixel_width;
        pixel_height = ins_pixel_height;

        world_width = ins_world_width;
        world_height = ins_world_height;
    }



    public Texture2D map_texture;

    [Header("DIMENSIONS")]
    // obviously the aspect ratio of the pixel dimensions 
    // and the world dimensions should be the same

    // the width, height of the map texture
    public int ins_pixel_width;
    public static int pixel_width;
    public int ins_pixel_height;
    public static int pixel_height;

    // width, height of the area the map takes up in the world
    public float ins_world_width;
    public static float world_width;
    public float ins_world_height;
    public static float world_height;




    // because of the way that we handled lake generation,
    // there is actually no height associated with the lake

    // instead we just take the distance to the edge of the lake,
    // so more interior regions are deeper (because that's usually how that works)
    public Color[] lake_height_colors;
    // the way these height values work is that the array has n+1 values,
    // and each terrain color is assigned the range of x to x+1
    // so the first value is the lowest, 
    // and the last value is the highest
    public float[] lake_height_values;


    // terrain does actually have height, so we'll use that
    public Color[] terrain_height_colors;
    public float[] terrain_height_values;
    

    [Space(20)]
    [Header("POIs")]
    public Texture2D default_map_icon;
}
