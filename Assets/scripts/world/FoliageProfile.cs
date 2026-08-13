using UnityEngine;

[System.Serializable]
public class FoliageProfile
{
    public float grass_density;
    
    public FoliageType[] types;
    public int totalCount;

    public int GetCount(int id)
    {
        return Mathf.RoundToInt(types[id].percent * totalCount);
    }

    public FoliageProfile()
    {
        totalCount = 0;
        types = new FoliageType[0];
    }
}

[System.Serializable]
public class FoliageType
{
    public float percent; // [0..1]
    public string object_name;
}
