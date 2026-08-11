using System.Collections.Generic;
using UnityEngine;

public class test_triangulate : MonoBehaviour
{
    private List<Vector3> bisector_points;
    private List<Vector3> bisector_directions;

    private Vector2[] vertices;
    private int[] triangles;

    public bool triangulate;

    void Update()
    {
        if (triangulate)
        {
            triangulate = false;

            // clearing the debug information
            bisector_points = new List<Vector3>();
            bisector_directions = new List<Vector3>();

            UpdateVertices();

            triangles = util_grok.Triangulate(vertices);

            Mesh m = new Mesh();

            m.SetVertices(util_polygon.Vector2ToVector3(vertices));
            m.SetTriangles(triangles,0);

            GetComponent<MeshFilter>().sharedMesh = m;
        }

        if (bisector_directions != null)
        {
            for (int i = 0; i < bisector_directions.Count; i++)
            {
                Debug.DrawLine(bisector_points[i] - bisector_directions[i] * 10f, bisector_points[i] + bisector_directions[i] * 10f);
            }
        }
    }

    private void UpdateVertices()
    {
        vertices = new Vector2[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            vertices[i] = new Vector3(transform.GetChild(i).position.x, transform.GetChild(i).position.z);
        }
    }
}
