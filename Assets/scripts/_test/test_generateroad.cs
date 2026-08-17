using UnityEngine;

public class test_generateroad : MonoBehaviour
{
    void Start()
    {
        Vector3[] path = new Vector3[]
        {
            new Vector3(0, 0, 0),
            new Vector3(5, 0, 2),
            new Vector3(10, 1, 5),
            new Vector3(15, 0, 3),
            new Vector3(20, 0, 0)
        };

        Mesh mesh = util_mesh.GenerateRectangularPrism(
            path,
            width: 3f,
            height: 0.4f,
            closed: false
        );

        GetComponent<MeshFilter>().mesh = mesh;
        // Don't forget a MeshCollider if you need physics
    }
}
