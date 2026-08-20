using UnityEngine;

public class test_cutmesh : MonoBehaviour
{
    public vfx_meshslicemode cut_mode;
    public MeshFilter original_mesh;

    public GameObject p_slice;

    void Start()
    {
        Mesh[] slices = new Mesh[] {};

        if (cut_mode == vfx_meshslicemode.Shards)
        {
            slices = util_mesh.DiceMesh(original_mesh.mesh, 3);
        } else if (cut_mode == vfx_meshslicemode.Parts)
        {
            slices = util_mesh.DissasembleMesh(original_mesh.mesh);
        }

        for (int i = 0; i < slices.Length; i++)
        {
            GameObject g_newSlice = Instantiate(p_slice);
            g_newSlice.GetComponent<MeshFilter>().sharedMesh = slices[i];
        }
    }
}
