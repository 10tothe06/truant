using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteAlways]
public class medit_widget_face : MonoBehaviour
{
    [HideInInspector]
    public medit_widget_generic g;
    public medit_vertexslaver slvr;

    [Header("CONSOLE")]
    public bool flipNormals;

    // works the same as blender, pretty much
    // gonna use it to create doorways and such
    public bool inset; 
    // creating a wall from a doorway
    public bool extrude;

    // literally just remove the triangulation for the face
    public bool removeFace;
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
        if (g == null) {g = GetComponent<medit_widget_generic>();g.type = medit_widgettype.Face;slvr.oldPosition = transform.localPosition;}

        if (flipNormals)
        {
            flipNormals = false;
            g.obj.FlipTriangleNormals(slvr.controllingTriangles.ToArray());
        }

        if (slvr.oldPosition != transform.localPosition && !awakeLock)
        {
            // the vertex has been moved, so we need to tell the object that in order to get everything updated
            g.obj.MoveVertices(slvr.controllingVertices.ToArray(), transform.localPosition - slvr.oldPosition);

            g.refreshWidgets = true;
        }
        slvr.oldPosition = transform.localPosition;

        if (inset)
        {
            inset = false;
            g.obj.CreateInsetFace(new int[] {slvr.controllingVertices[0],slvr.controllingVertices[1],slvr.controllingVertices[2],slvr.controllingVertices[3]});
            
            slvr.controllingVertices.Add(g.obj.mf.sharedMesh.vertices.Length - 4);
            slvr.controllingVertices.Add(g.obj.mf.sharedMesh.vertices.Length - 3);
            slvr.controllingVertices.Add(g.obj.mf.sharedMesh.vertices.Length - 2);
            slvr.controllingVertices.Add(g.obj.mf.sharedMesh.vertices.Length - 1);
        }

        if (removeFace)
        {
            removeFace = false;
            g.obj.RemoveFace(slvr.controllingVertices.ToArray());
        }

        if (extrude)
        {
            extrude = false;
            
        }


        awakeLock = false;
    }

    // forced
    public void ControlVertices(int[] firstTriangleIndices, int[] controllingVertices)
    {
        slvr.controllingTriangles = firstTriangleIndices.ToList();
        slvr.controllingVertices = controllingVertices.ToList();
    }

    public void ControlVertices(int[] firstTriangleIndices)
    {
        slvr.controllingTriangles = firstTriangleIndices.ToList();
        
        List<int> verts = new List<int>();

        int n = 0;
        for (int i = 0; i < firstTriangleIndices.Length; i++,n+=3)
        {
            if (!verts.Contains(g.obj.mf.sharedMesh.triangles[firstTriangleIndices[i]])) {verts.Add(g.obj.mf.sharedMesh.triangles[firstTriangleIndices[i]]);}
            if (!verts.Contains(g.obj.mf.sharedMesh.triangles[firstTriangleIndices[i]+1])) {verts.Add(g.obj.mf.sharedMesh.triangles[firstTriangleIndices[i]+1]);}
           if (!verts.Contains(g.obj.mf.sharedMesh.triangles[firstTriangleIndices[i]+2]))  {verts.Add(g.obj.mf.sharedMesh.triangles[firstTriangleIndices[i]+2]);}
        }

        slvr.controllingVertices = verts;
    }

    public void ReplaceExistingEdgeWidgets()
    {
        
        for (int i = g.obj.t_edgeWidgetContainer.childCount-1; i >=0; i--)
        {
            if (Vector3.Distance(g.obj.t_edgeWidgetContainer.GetChild(i).localPosition, transform.localPosition) < 0.01f)
            {
                DestroyImmediate(g.obj.t_edgeWidgetContainer.GetChild(i).gameObject);
            }
        }
    }
}
