using UnityEngine;

[ExecuteAlways]
public class medit_widget_vertex : MonoBehaviour
{
    private medit_widget_generic g;
    public medit_vertexslaver slvr;

    [Header("CONSOLE")]
    public bool extrude; // click this to extrude the vertex
    private bool awakeLock;

    void Awake()
    {
        slvr = GetComponent<medit_vertexslaver>();
        awakeLock = true;
    }

    // not sure how exactly Awake() and Start() work while in edit mode,
    // so I'm just making sure references are assigned periodically
    public void Update()
    {
        if (g == null) {g = GetComponent<medit_widget_generic>();g.type = medit_widgettype.Vertex;}

        if (extrude)
        {
            extrude = false;
            g.obj.ExtrudeVertex(slvr.controllingVertices[0]);
            return;
        }

        if (slvr.oldPosition != transform.localPosition && !awakeLock)
        {
            // the vertex has been moved, so we need to tell the object that in order to get everything updated
            for (int i = 0; i < slvr.controllingVertices.Count; i++)
            {
                g.obj.MoveVertex(slvr.controllingVertices[i], transform.localPosition);
            }
            g.refreshWidgets = true;
        }
        slvr.oldPosition = transform.localPosition;

        awakeLock = false;
    }
}
