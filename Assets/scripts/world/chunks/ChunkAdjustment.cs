using UnityEngine;

// different types of adjustments
public enum ChunkAdjustmentType
{
    
}

// this is probably destined to be the messiest class of the whole project
// i refuse to make a modular system for this kind of thing

[System.Serializable]
public class ChunkAdjustment
{
    // the polygon shape of the chunk adjustment
    public Vector3[] points;


    // data for the adjustment of the terrain mesh
    // ***

    // adding or subtracting from terrain values
    public NoiseProfile noise_overwrite;

    // ***


    // data for how widely the adjustment is applied
    // ***

    public float transition_width = 1f;

    // ***

    #region CONSTRUCTORS


    public ChunkAdjustment() {}

    public ChunkAdjustment(Vector3[] points, NoiseProfile noise_overwrite, float transition_width)
    {
        this.points = points;

        this.noise_overwrite = noise_overwrite;
        this.transition_width = transition_width;
    }


    #endregion


    // "hey, here's a terrain point, adjust it for me"
    public float AdjustTerrainHeight(Vector3 point, float old_height)
    {   
        if (noise_overwrite == null) {return old_height;}


        float adjusted_height = noise_overwrite.GetHeight(point);

        float dist = util_mesh.DistanceToPolygon(points, new Vector3(point.x, 0, point.z));

        return Mathf.Lerp(old_height, adjusted_height, Mathf.Clamp01(1 - (dist/transition_width)));
    }
}
