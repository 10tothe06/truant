using UnityEngine;

public class util_map
{   
    // just uses the "is point inside polygon" test
    // maybe not the fastest? idk and idc
    public static bool IsPositionInsideMap(Vector3 position_to_test)
    {
        return util_mesh.IsPointInsideConvexPolygon(new Vector3[]{
            WorldManager.Instance.a,
            WorldManager.Instance.b,
            WorldManager.Instance.c,
            WorldManager.Instance.d,
            },
            position_to_test);
    }


    // I was originally planning on doing a binary search to get to the lake,
    // but then I realized I was stupid and can just teleport to the closest lake vertex
    
    // provided that the lake vertices are close enough together, 
    // this will be close enough to what I want

    
    // TODO: ensure that the lakeside position is INSIDE THE MAP AREA
    // (otherwise the POI gets deleted bc its outside the map)
    public static Vector3 GetLakesidePosition(Vector3 original_position)
    {
        Vector3 result = original_position;

        int king_index = 0;
        float king_distance = Vector3.Distance(original_position, WorldManager.Instance.lake_vertices[0]);

        for (int i = 1; i < WorldManager.Instance.lake_vertices.Length; i++) {
            float new_distance = Vector3.Distance(original_position, WorldManager.Instance.lake_vertices[i]);

            if (new_distance < king_distance)
            {
                king_distance = new_distance;
                king_index = i;
            }
        }

        result = WorldManager.Instance.lake_vertices[king_index] - new Vector3(10f, 0, 10f);
        

        return result;
    }



    // called right after the level has been generated,
    // shouldn't really change over the course of the level
    public static Texture2D GenerateMapTexture(int width, int height)
    {   
        Color[] color_data = new Color[width * height];

        for (int y = 0, i = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++, i++)
            {
                if (util_mesh.IsPointInsidePolygon(WorldManager.Instance.lake_vertices, MapUVToWorldPosition(new Vector2(x/(float)(width-1), y/(float)(height-1)))))
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

    public static Vector3 GetRandomWorldPositionInMap()
    {
        return MapUVToWorldPosition(new Vector2(Random.Range(0f,1f),Random.Range(0f,1f)));
    }

    public static Vector2 GetRandomUVPositionInMap()
    {
        return new Vector2(Random.Range(0f,1f),Random.Range(0f,1f));
    }


    

    #region CONVERSION FUNCTIONS

    // from uv coordinates
    // ***

    public static Vector2 MapUVToPixelPosition(Vector2 map_position)
    {
        return new Vector2(map_position.x * 128, map_position.y * 64);
    }

    // takes in [0..1] coordinates (uv coordinates)
    public static Vector3 MapUVToWorldPosition(Vector2 map_position)
    {
        Vector3 up = WorldManager.Instance.d - WorldManager.Instance.a;
        Vector3 right = WorldManager.Instance.b - WorldManager.Instance.a;

        return WorldManager.Instance.a + right.normalized * map_position.x * WorldManager.Instance.level_width + up.normalized * map_position.y * WorldManager.Instance.level_height;
    }

    // ***



    // from world coordinates
    // ***

    public static Vector2 WorldToMapUVPosition(Vector3 world_position)
    {
        Vector3 from_map_origin = world_position - WorldManager.Instance.a;

        float right_component = util_math.ProjectedMagnitude(from_map_origin, WorldManager.Instance.b-WorldManager.Instance.a);
        float up_component = util_math.ProjectedMagnitude(from_map_origin, WorldManager.Instance.d-WorldManager.Instance.a);

        return new Vector2(right_component / WorldManager.Instance.level_width,
        up_component / WorldManager.Instance.level_height);
    }

    // ***

    #endregion
}
