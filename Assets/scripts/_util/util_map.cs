using UnityEngine;

public class util_map
{   

    // called right after the level has been generated,
    // shouldn't really change over the course of the level
    public static Texture2D GenerateMapTexture(int width, int height)
    {   
        Color[] color_data = new Color[width * height];

        for (int y = 0, i = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++, i++)
            {
                if (util_mesh.IsPointInsidePolygon(WorldManager.Instance.lake_vertices, MapToWorldPosition(new Vector2(x/(float)(width-1), y/(float)(height-1)))))
                {
                    color_data[i] = Color.blue;
                } else
                {
                    color_data[i] = Color.darkGreen;
                }


                //color_data[i] = new Color(x/(float)(width-1), y/(float)(height-1), 0, 1);
            }
        }


        Texture2D result = new Texture2D(width, height, TextureFormat.RGBA32, false, true);
        result.SetPixels(color_data);
        result.Apply(false, false);
        result.filterMode = FilterMode.Point;

        return result;
    }


    // takes in [0..1] coordinates (uv coordinates)
    public static Vector3 MapToWorldPosition(Vector2 map_position)
    {
        Vector3 up = WorldManager.Instance.d - WorldManager.Instance.a;
        Vector3 right = WorldManager.Instance.b - WorldManager.Instance.a;

        return WorldManager.Instance.a + right.normalized * map_position.x * WorldManager.Instance.level_width + up.normalized * map_position.y * WorldManager.Instance.level_height;
    }
}
