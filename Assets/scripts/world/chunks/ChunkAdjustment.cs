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

    public float noise_transition_width = 1f;
    public float foliage_transition_width = 1f;

    // ***
    
    public FoliageProfile foliage_overwrite;

    #region CONSTRUCTORS


    public ChunkAdjustment() {}

    public ChunkAdjustment(Vector3[] points)
    {
        this.points = points;

        this.noise_overwrite = null;
        this.foliage_overwrite = null;
    }

    public ChunkAdjustment(Vector3[] points, NoiseProfile noise_overwrite, float noise_transition_width)
    {
        this.points = points;

        this.noise_overwrite = noise_overwrite;
        this.foliage_overwrite = null;

        this.noise_transition_width = noise_transition_width;
    }

    public ChunkAdjustment(Vector3[] points, FoliageProfile foliage_overwrite, float foliage_transition_width)
    {
        this.points = points;

        this.noise_overwrite = null;
        this.foliage_overwrite = foliage_overwrite;

        this.foliage_transition_width = foliage_transition_width;
    }

    public ChunkAdjustment(Vector3[] points, NoiseProfile noise_overwrite, FoliageProfile foliage_overwrite, float noise_transition_width, float foliage_transition_width)
    {
        this.points = points;

        this.noise_overwrite = noise_overwrite;
        this.foliage_overwrite = foliage_overwrite;
        
        this.noise_transition_width = noise_transition_width;
        this.foliage_transition_width = foliage_transition_width;
    }


    #endregion


    // "hey, here's a terrain point, adjust it for me"
    public float AdjustTerrainHeight(Vector3 point, float old_height)
    {   
        if (noise_overwrite == null) {return old_height;}


        float adjusted_height = noise_overwrite.GetHeight(point);

        float dist = util_mesh.DistanceInsidePolygon(points, new Vector3(point.x, 0, point.z));

        return Mathf.Lerp(old_height, adjusted_height, Mathf.Clamp01(dist/noise_transition_width));
    }


    // 0 means the adjustment doesn't apply
    // 1 means it fully applies
    public float GetFoliageTransitionAmount(Vector3 sample_point)
    {
        float dist = util_mesh.DistanceInsidePolygon(points, new Vector3(sample_point.x, 0, sample_point.z));

        if (foliage_transition_width == 0)
        {
            return dist > 0 ? 1f : 0f;
        } else
        {
            return Mathf.Clamp01(dist/foliage_transition_width);
        }
    }

    public float GetNoiseTransitionAmount(Vector3 sample_point)
    {
        float dist = util_mesh.DistanceInsidePolygon(points, new Vector3(sample_point.x, 0, sample_point.z));

        if (noise_transition_width == 0)
        {
            return dist > 0 ? 1f : 0f;
        } else
        {
            return Mathf.Clamp01(dist/noise_transition_width);
        }
    }
}
