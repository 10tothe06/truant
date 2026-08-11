using UnityEngine;

public class test_lakemesh : MonoBehaviour
{
    public bool calculate;

    void OnDrawGizmos()
    {
        if (calculate)
        {
            calculate = false;

            GenerateLakeMesh();
        }
    }

    private void GenerateLakeMesh()
    {
        int[] verts = util_grok.Triangulate(GetComponent<test_lakevertices>().vertices);

        Mesh m = new Mesh();

        m.SetVertices(util_mesh.ToVector3(GetComponent<test_lakevertices>().vertices));
        m.SetTriangles(verts,0);

        GetComponent<MeshFilter>().sharedMesh = m;
    }
}
