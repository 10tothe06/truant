using UnityEngine;

public class test_cutmesh : MonoBehaviour
{
    public MeshFilter original_mesh;

    public GameObject p_slice;

    void Start()
    {
        Mesh[] slices = util_mesh.DiceMesh(original_mesh.mesh, 3);

        for (int i = 0; i < slices.Length; i++)
        {
            GameObject g_newSlice = Instantiate(p_slice);
            g_newSlice.GetComponent<MeshFilter>().sharedMesh = slices[i];
        }
    }
}
