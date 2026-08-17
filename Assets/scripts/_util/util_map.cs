using UnityEngine;

public class util_map
{   
    public static Color ApplyHeightBanding(Color[] colors, float[] height_bands, float height_value)
    {
        if (height_bands.Length != colors.Length + 1) // wrong number of height bands
        {
            return colors[0];
        }

        for (int i = 0; i < colors.Length; i++)
        {
            if (height_value > height_bands[i] && height_value < height_bands[i+1])
            {
                return colors[i];
            }
        }


        // should only get here if the height bands were formatted wrong
        return colors[0];
    }



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
                Vector3 world_position = MapUVToWorldPosition(new Vector2(x/(float)(width-1), y/(float)(height-1)));

                if (util_mesh.IsPointInsidePolygon(WorldManager.Instance.lake_vertices, world_position))
                {
                    color_data[i] = Color.blue;
                    // color_data[i] = util_map.ApplyHeightBanding(
                    //     MapData.Instance.lake_height_colors, 
                    //     MapData.Instance.lake_height_values,
                        
                    //     util_mesh.DistanceInsidePolygon(WorldManager.Instance.lake_vertices, world_position));
                } else
                {
                    color_data[i] = util_map.ApplyHeightBanding(
                        MapData.Instance.terrain_height_colors, 
                        MapData.Instance.terrain_height_values,
                        // using level noise here isn't the best, because it doesn't take into account chunk adjustments
                        // but it should work fine because the main adjustment is for the lake, and that's a different color
                        WorldManager.level_noise.GetHeight(world_position)); 
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
        return new Vector2(map_position.x * MapData.pixel_width, map_position.y * MapData.pixel_height);
    }

    // takes in [0..1] coordinates (uv coordinates)
    public static Vector3 MapUVToWorldPosition(Vector2 map_position)
    {
        Vector3 up = WorldManager.Instance.d - WorldManager.Instance.a;
        Vector3 right = WorldManager.Instance.b - WorldManager.Instance.a;

        return WorldManager.Instance.a + right.normalized * map_position.x * MapData.world_width + up.normalized * map_position.y * MapData.world_height;
    }

    // ***



    // from world coordinates
    // ***

    public static Vector2 WorldToMapUVPosition(Vector3 world_position)
    {
        Vector3 from_map_origin = world_position - WorldManager.Instance.a;

        float right_component = util_math.ProjectedMagnitude(from_map_origin, WorldManager.Instance.b-WorldManager.Instance.a);
        float up_component = util_math.ProjectedMagnitude(from_map_origin, WorldManager.Instance.d-WorldManager.Instance.a);

        return new Vector2(right_component / MapData.world_width,
        up_component / MapData.world_height );
    }

    // ***

    #endregion
}
