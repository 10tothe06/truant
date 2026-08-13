using UnityEngine;

[ExecuteInEditMode]
public class AreaDebugger : MonoBehaviour
{
    // [Header("MESH GENERATION")]
    // public Material m_area;
    // public bool logArea;
    // public bool reverseOrder;

    // void Update()
    // {
    //     if (logArea)
    //     {
    //         logArea = false;
    //         GenerateAreaMesh();
    //     }
    // }

    // public void GenerateAreaMesh()
    // {
    //     Vector3[] points = new Vector3[transform.childCount];
    //     for (int i = 0; i < transform.childCount; i++)
    //     {
    //         if (reverseOrder)
    //         {
    //             points[i] = transform.InverseTransformPoint(transform.GetChild(transform.childCount - 1-i).position);
    //         } else {points[i] = transform.InverseTransformPoint(transform.GetChild(i).position);}
    //     }
    //     GetComponent<MeshFilter>().sharedMesh = util_meshgeneration.GeneratePolygonMesh(points, Vector3.up, 1);
    //     GetComponent<MeshRenderer>().sharedMaterial = m_area;
    // }
}
