using UnityEngine;

[System.Serializable]
public class vfx_slicedmesh
{
    public string name;

    public Mesh[] slices;


    public vfx_slicedmesh() {}


    public vfx_slicedmesh(string name, Mesh[] slices)
    {
        this.name = name;
        this.slices = slices;
    }
}
