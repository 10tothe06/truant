using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class medit_vertexslaver : MonoBehaviour
{
    public List<int> controllingVertices;
    public List<int> controllingTriangles;

    public Vector3 oldPosition;

    public void UpdatePosition()
    {
        Vector3 v = medit_mesh_processing.GetMidpoint(controllingVertices.ToArray(), GetComponent<medit_widget_generic>().obj.mf.sharedMesh);
        oldPosition =v;
        transform.localPosition  =v;
    }
}
