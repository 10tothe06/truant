using UnityEngine;

public enum ChunkAdjustmentType
{
    Path, // for trails and such
    Terrain_Adjust, // changing the height of the terrain, mostly used by the lake
    Foliage_Break, // removing foliage in an area
    Flat_Area,
}

// this is probably destined to be the messiest class of the whole project
// i refuse to make a modular system for this kind of thing

[System.Serializable]
public class ChunkAdjustment
{
    public bool grassBan;
    public bool generateMesh;
    public ChunkAdjustmentType type;
    public Vector3[] points;
    public Vector3[] normals; 
}
