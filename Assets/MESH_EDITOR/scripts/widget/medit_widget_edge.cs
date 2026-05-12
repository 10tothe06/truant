using UnityEngine;

[ExecuteAlways]
public class medit_widget_edge : MonoBehaviour
{   
    private medit_widget_generic g;
    public medit_vertexslaver slvr;

    [Header("CONSOLE")]
    public bool extrude;
    private bool awakeLock;

    void Awake()
    {
        slvr = GetComponent<medit_vertexslaver>();
        slvr.oldPosition = transform.localPosition;
        awakeLock = true;
    }

    public Vector3 GetDirection()
    {
        return (g.obj.mf.sharedMesh.vertices[slvr.controllingVertices[1]] - g.obj.mf.sharedMesh.vertices[slvr.controllingVertices[0]]).normalized;
    }

    // not sure how exactly Awake() and Start() work while in edit mode,
    // so I'm just making sure references are assigned periodically
    public void Update()
    {
        if (g == null) {g = GetComponent<medit_widget_generic>(); g.type = medit_widgettype.Edge; slvr.oldPosition = transform.localPosition;}

        if (extrude)
        {
            extrude = false;
            g.obj.ExtrudeEdge(slvr.controllingVertices[0], slvr.controllingVertices[1], Vector3.up);
            return;
        }
        
        if (slvr.oldPosition != transform.localPosition && !awakeLock)
        {
            // the vertex has been moved, so we need to tell the object that in order to get everything updated
            for (int i = 0; i < slvr.controllingVertices.Count; i++)
            {
                g.obj.MoveVertex(slvr.controllingVertices[i], g.obj.mf.sharedMesh.vertices[slvr.controllingVertices[i]] + transform.localPosition - slvr.oldPosition);
            }
            g.refreshWidgets = true;
        }
        slvr.oldPosition = transform.localPosition;

        awakeLock = false;
    }
}
