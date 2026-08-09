using UnityEngine;

[System.Serializable]
public class FoliageProfile
{
    public FoliageType[] types;
    public int totalCount;

    public int GetCount(int id)
    {
        return Mathf.RoundToInt(types[id].percent * totalCount);
    }
}

[System.Serializable]
public class FoliageType
{
    public float percent; // [0..1]
    public string object_name;
}
