using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

// note on exporting:
// for now I'm making it so that you export on a per-object basis, because doing otherwise would be too much work
// so yes this means FOR NOW we can only export single objects, not sets of them

// for the purposes of this engine that should be fine

public enum medit_genericmeshtype {
    Cube,
    Plane,
}

public enum medit_widgettype {
    Shape,
    Vertex,
    Edge,
    Face,
    Any,
}

public enum medit_meshrenderingmode
{
    Solid,
    Wireframe,
}

[ExecuteAlways]
public class medit_object : MonoBehaviour
{
    [Header("************")]
    [Header("CONSOLE")]

    // whether to enable the MeshRenderer component
    public bool renderMesh;
    public bool renderWidgets;
    private bool isRenderingWidgets;

    // how exactly to render the mesh
    // pretty WIP for now
    public medit_meshrenderingmode renderMode;
    private medit_meshrenderingmode lastRenderMode;

    // toggles the drawing of (raw) vertices of the mesh
    // (raw as in not affected by any code, just straight from the file)
    public bool showRawVertices;
    public float vertexRadius; // for above
    // toggles the drawing of (raw) edge lines of the mesh
    public bool drawEdgeLines;
    
    // flips the normals of all triangles in the object
    public bool flipAllNormals;

    public bool extrudeSelected;
    public bool fillSelected;

    // removes the entire mesh
    public bool clearMesh;
    
    // spawns in a mesh of a given type (cube, plane, etc.)
    public bool generateMesh;
    public medit_genericmeshtype type; // for above
    private medit_genericmeshtype setupFor; // storing the type that has been generated (not accessible by user)

    public bool resetShapeWidgets;

    // if toggled, the mesh will continuously re-generate itself EVERY FRAME
    // obviously this is usually a bad idea
    [Header("warning: laggy")]
    public bool generateContinuously;

    [Header("************")]

    [Space(24)]

    [Header("References")]
    public MeshFilter mf;
    public MeshRenderer mr;
    public List<medit_widget_generic> selected;

    // shape widgets affect the general shape of a mesh when spawning it in
    public Transform t_shapeWidgetContainer;
    public GameObject p_shapeWidget;

    public Transform t_vertexWidgetContainer;
    public GameObject p_vertexWidget;
    
    public Transform t_edgeWidgetContainer;
    public GameObject p_edgeWidget;

    public Transform t_faceWidgetContainer;
    public GameObject p_faceWidget;
    // no sense in having a delete button since you can just delete the gameobject, right?

    public Transform t_tempWidgetContainer; // WIP

    void OnDrawGizmos()
    {
        if (showRawVertices)
        {
            Gizmos.color = Color.lightCyan;
            for (int i = 0; i < mf.sharedMesh.vertices.Length; i++)
            {
                Gizmos.DrawSphere(transform.position + mf.sharedMesh.vertices[i], vertexRadius);
            }
        }

        if (drawEdgeLines)
        {
            if (mf.sharedMesh != null)
            {
                
                for (int i = 0; i < mf.sharedMesh.triangles.Length; i+=3)
                {
                    Vector3 a = transform.position + mf.sharedMesh.vertices[mf.sharedMesh.triangles[i]];
                    Vector3 b = transform.position + mf.sharedMesh.vertices[mf.sharedMesh.triangles[i+1]];
                    Vector3 c = transform.position + mf.sharedMesh.vertices[mf.sharedMesh.triangles[i+2]];
                    
                    Debug.DrawLine(a, b, Color.white);
                    Debug.DrawLine(b, c, Color.white);
                    Debug.DrawLine(c, a, Color.white);
                }
            }
        }
    }

    public void ResetShapeWidgets()
    {
        SetupForCube();
    }

    void Update()
    {
        mr.enabled = renderMesh;

        if (renderWidgets)
        {
            if (!isRenderingWidgets)
            {
                isRenderingWidgets = true;

                // show all widgets
                // TODO: separate function
                medit_widget_generic[] widgets = GetComponentsInChildren<medit_widget_generic>(true);
                for (int i = 0; i < widgets.Length; i++)
                {
                    widgets[i].gameObject.SetActive(true);
                }
            }
        } else
        {
            if (isRenderingWidgets)
            {
                // hide all widgets
                medit_widget_generic[] widgets = GetComponentsInChildren<medit_widget_generic>(true);
                for (int i = 0; i < widgets.Length; i++)
                {
                    widgets[i].gameObject.SetActive(false);
                }
                isRenderingWidgets = false;
            }
        }

        if (resetShapeWidgets)
        {
            resetShapeWidgets = false;
            ResetShapeWidgets();
        }
        if (flipAllNormals)
        {
            flipAllNormals = false;
            FlipAllMeshNormals();
        }

        if (lastRenderMode != renderMode)
        {
            UpdateMeshRenderMode();
            lastRenderMode = renderMode;
        }

        if (extrudeSelected)
        {
            extrudeSelected = false;
            ExtrudeSelected();
        }

        if (fillSelected)
        {
            fillSelected = false;
            FillSelected();
        }

        if (generateMesh)
        {
            ShowShapeWidgets();

            if (type == medit_genericmeshtype.Cube)
            {
                // this will initialize the widgets in the cube configuration
                if (setupFor != medit_genericmeshtype.Cube || t_shapeWidgetContainer.childCount != 4) {SetupForCube();}

                Vector3 midpoint = GetWidget(3);

                float x = (GetWidget(0).x - midpoint.x);
                float y = (GetWidget(1).y - midpoint.y);
                float z = (GetWidget(2).z - midpoint.z);
                
                Mesh newCube = medit_mesh_generation.CreateBox(Vector3.right * x, Vector3.up * y, Vector3.forward * z);
                newCube = medit_mesh_processing.ApplyMove(newCube, midpoint + new Vector3(x,y,z)/2f);

                // this function creates all of the necessary widgets
                ApplyMeshAndGenerateWidgets(newCube);

                for (int i =0; i < newCube.triangles.Length; i++)
                {
                    //Debug.Log(newCube.triangles[i]);
                }


                setupFor = medit_genericmeshtype.Cube;
            }
            if (type == medit_genericmeshtype.Plane)
            {
                if (setupFor != medit_genericmeshtype.Plane || t_shapeWidgetContainer.childCount != 2) {SetupForPlane();}

                Vector3 midpoint = GetWidget(2);

                float x = (GetWidget(0).x - midpoint.x);
                float y = (GetWidget(1).y - midpoint.y);
                
                Mesh newCube = medit_mesh_generation.CreateBox(Vector3.right * x, Vector3.up * y, Vector3.forward);
                newCube = medit_mesh_processing.ApplyMove(newCube, midpoint + new Vector3(x,y)/2f);

                // this function creates all of the necessary widgets
                ApplyMeshAndGenerateWidgets(newCube);
            }

            if (!generateContinuously) {generateMesh = false;}
        }

        if (clearMesh)
        {
            clearMesh = false;

            ClearAllWidgets();
            mf.mesh = null;
        }
    }

    // take two vertices, with different widgets, and make them the same widget
    public void MergeVertexWidgets(int a, int b)
    {
        // first, remove the existing vertex widgets
        DestroyWidgetControllingVertex(a, medit_widgettype.Vertex);
        DestroyWidgetControllingVertex(b, medit_widgettype.Vertex);

        // now a new widget that slaves both vertices
        SpawnNewVertexWidget(mf.sharedMesh.vertices[a], new int[] {a,b});
    }

    public void DestroyWidgetControllingVertex(int vertexId, medit_widgettype type)
    {
        if (type == medit_widgettype.Any || type == medit_widgettype.Vertex)
        {
            for (int i = t_vertexWidgetContainer.childCount - 1; i >= 0; i--)
            {
                if (t_vertexWidgetContainer.GetChild(i).GetComponent<medit_vertexslaver>().controllingVertices.Contains(vertexId))
                {
                    DestroyImmediate(t_vertexWidgetContainer.GetChild(i).gameObject);
                }
            }
        }

        if (type == medit_widgettype.Edge || type == medit_widgettype.Any)
        {
            
        }

        if (type == medit_widgettype.Face || type == medit_widgettype.Any)
        {
            
        }
    }

    // based on what the user has chosen in the inspector,
    // switch between rendering the mesh with an actual mesh and with just lines connecting the vertices
    public void UpdateMeshRenderMode()
    {
        if (renderMode == medit_meshrenderingmode.Solid)
        {
            mr.enabled = true;
            drawEdgeLines = false;

        } else if (renderMode == medit_meshrenderingmode.Wireframe)
        {
            mr.enabled = false;
            drawEdgeLines = true;
        }
    }

    public void Clearselected()
    {
        for (int i = 0; i < selected.Count; i++)
        {
            if (selected[i] != null)
            {
                selected[i].select = false;
            }
        }

        selected.Clear();
    }

    public void FillSelected()
    {
        List<int> involvedVertices = new List<int>();

        for (int i = 0; i < selected.Count; i++)
        {
            if (selected[i].type == medit_widgettype.Vertex)
            {
                involvedVertices.Add(selected[i].GetComponent<medit_vertexslaver>().controllingVertices[0]);
            }
        }

        int[] triangles = new int[mf.sharedMesh.triangles.Length + (involvedVertices.Count - 2) * 3];

        // the original triangles
        for (int i = 0; i < mf.sharedMesh.triangles.Length; i++)
        {
            triangles[i] = mf.sharedMesh.triangles[i];
        }

        // now the new ones (polygon fan)
        // TODO: SORT IT BY CLOCKWISE
        for (int i = 2,n = 0; i < involvedVertices.Count; i++,n+=3)
        {
            triangles[mf.sharedMesh.triangles.Length + n] = involvedVertices[0];
            triangles[mf.sharedMesh.triangles.Length + n+1] = involvedVertices[i-1];
            triangles[mf.sharedMesh.triangles.Length + n+2] = involvedVertices[i];
        }

        mf.sharedMesh.SetTriangles(triangles,0);
    }

    public void ExtrudeSelected()
    {
        // temp!
        if (selected.Count < 2) {return;}

        Vector3 normal = Vector3.Cross(selected[0].GetComponent<medit_widget_edge>().GetDirection(),selected[1].GetComponent<medit_widget_edge>().GetDirection()).normalized;

        //Debug.Log(normal);
        List<int> toMerge = new List<int>();
        for (int i = 0; i < selected.Count; i++)
        {
            ExtrudeEdge(
                selected[i].GetComponent<medit_widget_edge>().slvr.controllingVertices[0],
                selected[i].GetComponent<medit_widget_edge>().slvr.controllingVertices[1],
                normal);

            // merging the newly created vertices based on their positions
            // the above function would have made 2 new vertices
            for (int j = t_vertexWidgetContainer.childCount - 3; j>= 0; j--)
            {
                int vertexA = t_vertexWidgetContainer.GetChild(j).GetComponent<medit_vertexslaver>().controllingVertices[0];
                int vertexB = t_vertexWidgetContainer.GetChild(t_vertexWidgetContainer.childCount - 1).GetComponent<medit_vertexslaver>().controllingVertices[0];
                int vertexC = t_vertexWidgetContainer.GetChild(t_vertexWidgetContainer.childCount - 2).GetComponent<medit_vertexslaver>().controllingVertices[0];

                if (mf.sharedMesh.vertices[vertexA] == mf.sharedMesh.vertices[vertexB])
                {
                    toMerge.Add(vertexA);
                    toMerge.Add(vertexB);
                }
                if (mf.sharedMesh.vertices[vertexA] == mf.sharedMesh.vertices[vertexC])
                {
                    toMerge.Add(vertexA);
                    toMerge.Add(vertexC);
                }
            }
        }

        // merging all the ones we found from earlier
        for (int i = 0; i < toMerge.Count; i+=2)
        {
            MergeVertexWidgets(toMerge[i],toMerge[i+1]);
        }

        Clearselected();
    }

    public void CreateInsetFace(int[] faceVertices)
    {
        Mesh m = medit_mesh_processing.RemoveTrianglesWithVertices(mf.sharedMesh, faceVertices);

        // make sure our vertices have a clockwise winding order
        Vector3 midPoint = medit_mesh_processing.GetMidpoint(faceVertices, m);
        Vector3 normal = Vector3.Cross((m.vertices[faceVertices[0]]-midPoint).normalized,
        (m.vertices[faceVertices[1]]-midPoint).normalized);

        faceVertices = medit_mesh_processing.OrderVertices(faceVertices, normal, m);

        // now we actually have to create the triangles
        // I'm cheating here and using an approach that ONLY WORKS FOR SQUARE FACES

        List<int> newFaceVertices = new List<int>();
        for (int i = 0; i < faceVertices.Length; i++)
        {
            newFaceVertices.Add(faceVertices[i]);
        }

        // the newer, inset vertices
        Vector2 uv = m.uv[faceVertices[0]] + (m.uv[faceVertices[2]] - m.uv[faceVertices[0]]).normalized * 0.25f;
        m = AddVertex(m.vertices[faceVertices[0]] + (midPoint - m.vertices[faceVertices[0]]).normalized * 0.25f,normal, m,uv);
        newFaceVertices.Add(m.vertices.Length - 1);

        uv = m.uv[faceVertices[1]] + (m.uv[faceVertices[3]] - m.uv[faceVertices[1]]).normalized * 0.25f;
        m = AddVertex(m.vertices[faceVertices[1]] + (midPoint - m.vertices[faceVertices[1]]).normalized * 0.25f,normal, m,uv);
        newFaceVertices.Add(m.vertices.Length - 1);

        uv = m.uv[faceVertices[2]] + (m.uv[faceVertices[0]] - m.uv[faceVertices[2]]).normalized * 0.25f;
        m = AddVertex(m.vertices[faceVertices[2]] + (midPoint - m.vertices[faceVertices[2]]).normalized * 0.25f,normal, m,uv);
        newFaceVertices.Add(m.vertices.Length - 1);

        uv = m.uv[faceVertices[3]] + (m.uv[faceVertices[1]] - m.uv[faceVertices[3]]).normalized * 0.25f;
        m = AddVertex(m.vertices[faceVertices[3]] + (midPoint - m.vertices[faceVertices[3]]).normalized * 0.25f,normal, m,uv);
        newFaceVertices.Add(m.vertices.Length - 1);

        // the vertex widgets will already be a thing, but now we need edge widgets
        SpawnNewEdgeWidget((m.vertices[newFaceVertices[4]] + m.vertices[newFaceVertices[5]])/2f, new int[] {newFaceVertices[4],newFaceVertices[5]});
        SpawnNewEdgeWidget((m.vertices[newFaceVertices[5]] + m.vertices[newFaceVertices[6]])/2f, new int[] {newFaceVertices[5],newFaceVertices[6]});
        SpawnNewEdgeWidget((m.vertices[newFaceVertices[6]] + m.vertices[newFaceVertices[7]])/2f, new int[] {newFaceVertices[6],newFaceVertices[7]});
        SpawnNewEdgeWidget((m.vertices[newFaceVertices[7]] + m.vertices[newFaceVertices[4]])/2f, new int[] {newFaceVertices[7],newFaceVertices[4]});

        int[] newTriangles = new int[] {
            newFaceVertices[0],newFaceVertices[1],newFaceVertices[4],
            newFaceVertices[4],newFaceVertices[1],newFaceVertices[5],

            newFaceVertices[1],newFaceVertices[2],newFaceVertices[5],
            newFaceVertices[5],newFaceVertices[2],newFaceVertices[6],
            
            newFaceVertices[2],newFaceVertices[3],newFaceVertices[6],
            newFaceVertices[6],newFaceVertices[3],newFaceVertices[7],

            newFaceVertices[3],newFaceVertices[0],newFaceVertices[7],
            newFaceVertices[7],newFaceVertices[0],newFaceVertices[4],};
        m = AddTriangles(newTriangles,m);

        ApplyMesh(m);
        // no widget updating yet!
    }

    public void AddTriangles(int[] newEntries)
    {
        int[] newTriangleArray = new int[mf.sharedMesh.triangles.Length + newEntries.Length];
        for (int i = 0; i < mf.sharedMesh.triangles.Length; i++)
        {
            newTriangleArray[i] = mf.sharedMesh.triangles[i];
        }
        for (int i = 0; i < newEntries.Length; i++)
        {
            newTriangleArray[i + mf.sharedMesh.triangles.Length] = newEntries[i];
        }

        mf.sharedMesh.SetTriangles(newTriangleArray,0);
    }
    public Mesh AddTriangles(int[] newEntries, Mesh m)
    {
        int[] newTriangleArray = new int[m.triangles.Length + newEntries.Length];
        for (int i = 0; i < m.triangles.Length; i++)
        {
            newTriangleArray[i] = m.triangles[i];
        }
        for (int i = 0; i < newEntries.Length; i++)
        {
            newTriangleArray[i + m.triangles.Length] = newEntries[i];
        }

        m.SetTriangles(newTriangleArray,0);
        return m;
    }

    // TODO: have a look and make sure normals are ok
    // public void AddVertex(Vector3 pos, Vector3 normal)
    // {
    //     Vector3[] newVertices = new Vector3[mf.sharedMesh.vertices.Length +1 ];
    //     for (int i = 0 ;i < newVertices.Length - 1; i++)
    //     {
    //         newVertices[i] = mf.sharedMesh.vertices[i];
    //     }
    //     newVertices[newVertices.Length - 1] = pos;

    //     Vector3[] normals = new Vector3[mf.sharedMesh.normals.Length + 1];
    //     for (int i = 0 ;i < newVertices.Length - 1; i++)
    //     {
    //         normals[i] = mf.sharedMesh.normals[i];
    //     }
    //     normals[normals.Length - 1] = normal;
    //     mf.sharedMesh.SetNormals(normals);

    //     mf.sharedMesh.SetVertices(newVertices);
    // }

    public Mesh AddVertex(Vector3 pos, Vector3 normal, Mesh m, Vector2  newUV)
    {
        Vector3[] newVertices = new Vector3[m.vertices.Length +1 ];
        for (int i = 0 ;i < newVertices.Length - 1; i++)
        {
            newVertices[i] = m.vertices[i];
        }
        newVertices[newVertices.Length - 1] = pos;

        m.SetVertices(newVertices);

        Vector3[] normals = new Vector3[m.normals.Length];
        for (int i = 0 ;i < normals.Length - 1; i++)
        {
            normals[i] = m.normals[i];
        }
        normals[normals.Length - 1] = normal;
        m.SetNormals(normals);

        Vector2[] uvs = new Vector2[m.uv.Length];
        for (int i = 0 ;i < uvs.Length - 1; i++)
        {
            uvs[i] = m.uv[i];
        }
        
        uvs[uvs.Length - 1] = newUV;
        
        m.SetUVs(0,uvs);
        
        // also adding a new vertex widget
        SpawnNewVertexWidget(pos, m.vertices.Length - 1);

        return m;
    }

    public void RemoveFace(int[] faceVertices)
    {
        Mesh m = medit_mesh_processing.RemoveTrianglesWithVertices(mf.sharedMesh, faceVertices);
        ApplyMesh(m);
    }

    // TODO: actually test this
    public void ExportObject()
    {
        string path = "C:/users/maxim/Desktop/";
        medit_utils_objexporter.ExportMeshToObj(mf.sharedMesh, path);
    }

    // for QOL, we get rid of the shape widgets once we've made edits to the mesh
    // yk cuz they don't do anything at that point
    // also these are both wrappers in case i need more logic
    public void HideShapeWidgets()
    {
        medit_utils.SetChildrenActive(t_shapeWidgetContainer, false);
    }
    // we need to show them again once the user regens the mesh ofc
    public void ShowShapeWidgets()
    {
        medit_utils.SetChildrenActive(t_shapeWidgetContainer, true);
    }

    public void ApplyMesh(Mesh m)
    {
        mf.sharedMesh = m;
    }

    
    public void ApplyMeshAndGenerateWidgets(Mesh m)
    {
        ApplyMesh(m);
        GenerateMeshWidgets(); // fully delete and remake the widgets
        selected.Clear();
    }

    // don't add/delete anything, just move the objects
    public void UpdateMeshWidgetPositions(Mesh m, medit_widgettype type, int childIndex)
    {
        // vertex first
        for (int i = 0; i< t_vertexWidgetContainer.childCount; i++)
        {
            t_vertexWidgetContainer.GetChild(i).GetComponent<medit_vertexslaver>().UpdatePosition();
        }

        // then edge
        for (int i = 0; i< t_edgeWidgetContainer.childCount; i++)
        {
            t_edgeWidgetContainer.GetChild(i).GetComponent<medit_vertexslaver>().UpdatePosition();
        }

        // then face
        for (int i = 0; i< t_faceWidgetContainer.childCount; i++)
        {
            t_faceWidgetContainer.GetChild(i).GetComponent<medit_vertexslaver>().UpdatePosition();

            
        }
    }

    // DO add/delete, but only whats needed
    public void UpdateMeshWidgets(Mesh m, medit_widgettype type, int childIndex)
    {
        if (type != medit_widgettype.Vertex)
        {
            MakeVertexWidgetsFromType(-1);
        } else
        {
            MakeVertexWidgetsFromType(childIndex);
        }


        
        if (type != medit_widgettype.Edge)
        {
            MakeEdgeWidgetsFromType(-1);
        } else
        {
            MakeEdgeWidgetsFromType(childIndex);
        }


        if (type != medit_widgettype.Face)
        {
            MakeFaceWidgetsFromType(-1);
        } else
        {
            MakeFaceWidgetsFromType(childIndex);
        }
    }

    // creates and initalizes all vertex,edge,face widgets for a mesh
    public void GenerateMeshWidgets()
    {
        MakeVertexWidgetsFromType(-1);
        MakeEdgeWidgetsFromType(-1);
        MakeFaceWidgetsFromType(-1);
    }

    public void MakeVertexWidgetsFromType(int toAvoid)
    {
        Mesh m = mf.sharedMesh;
        if (type == medit_genericmeshtype.Cube)
        {
            MakeVertexWidgetsForCube(toAvoid);
        } else
        {
            MakeVertexWidgets(m,toAvoid);
        }
    }

    public void MakeEdgeWidgetsFromType(int toAvoid)
    {
        Mesh m = mf.sharedMesh;
        if (type == medit_genericmeshtype.Cube)
        {
            MakeEdgeWidgetsForCube(toAvoid);
        } else
        {
            MakeEdgeWidgets(m,toAvoid);
        }
        
    }
    public void MakeFaceWidgetsFromType(int toAvoid)
    {
        Mesh m = mf.sharedMesh;
        if (type == medit_genericmeshtype.Cube)
        {
            MakeFaceWidgetsForCube(toAvoid);
        } else
        {
           MakeFaceWidgets(m,toAvoid);
        }
           
    }

    // works for any mesh
    void MakeFaceWidgets(Mesh m, int toAvoid)
    {
        medit_utils.ImmediateDestroy(t_faceWidgetContainer, toAvoid);
        for (int i = 0, n=0; i < m.triangles.Length; i+=3,n++)
        {
            if (toAvoid == n) {continue;}

            // one new widget for every tri
            Vector3 midpoint = (m.vertices[m.triangles[i]]+m.vertices[m.triangles[i+1]]+m.vertices[m.triangles[i+2]]) / 3f;
            SpawnNewFaceWidget(midpoint, new int[]{i,i+1,i+2});
        }
    }

    void MakeFaceWidgetsForPlane()
    {
        
    }

    // this one does special face widgets for the quads, which would normally be two faces
    void MakeFaceWidgetsForCube(int toAvoid)
    {
        Mesh m = mf.sharedMesh;
        medit_utils.ImmediateDestroy(t_faceWidgetContainer, toAvoid);
        // unit cube, for now

        if (toAvoid!=0) {SpawnNewFaceWidget((m.vertices[0]+m.vertices[1]+m.vertices[2]+m.vertices[3])/4f, new int[]{0, 3},new int[]{0,1,2,3,       8,11,    13,14,   17,18,   20,23});}
        if (toAvoid!=1) {SpawnNewFaceWidget((m.vertices[4]+m.vertices[5]+m.vertices[6]+m.vertices[7])/4f, new int[]{6, 9},new int[]{4,5,6,7,        9,10,    12,15,   16,19,   21,22});}

        if (toAvoid!=2) {SpawnNewFaceWidget((m.vertices[8]+m.vertices[9]+m.vertices[10]+m.vertices[11])/4f, new int[]{12, 15},new int[]{8,9,10,11,  1, 2,   4, 7,   18,19,  22,23});}
        if (toAvoid!=3) {SpawnNewFaceWidget((m.vertices[12]+m.vertices[13]+m.vertices[14]+m.vertices[15])/4f, new int[]{18, 21},new int[]{12,13,14,15,    0,3,   5,6,   16,17,   20,21   });}

        if (toAvoid!=4) {SpawnNewFaceWidget((m.vertices[16]+m.vertices[17]+m.vertices[18]+m.vertices[19])/4f, new int[]{24, 27},new int[]{16,17,18,19,      2,3,    6, 7,    14,15,   10,11});}
        if (toAvoid!=5) {SpawnNewFaceWidget((m.vertices[20]+m.vertices[21]+m.vertices[22]+m.vertices[23])/4f, new int[]{30,33},new int[]{20,21,22,23,     0,1,    4,5,   8,9,   12,13});}
    }

    void MakeVertexWidgetsForCube(int toAvoid)
    {
        Mesh m = mf.sharedMesh;
        medit_utils.ImmediateDestroy(t_vertexWidgetContainer, toAvoid);

        if (toAvoid!=0) {SpawnNewVertexWidget(m.vertices[0], new int[]{0,13,20});}
        if (toAvoid!=1) {SpawnNewVertexWidget(m.vertices[1], new int[]{1,8,23});}
        if (toAvoid!=2) {SpawnNewVertexWidget(m.vertices[2], new int[]{2,11,18});}
        if (toAvoid!=3) {SpawnNewVertexWidget(m.vertices[3], new int[]{3,14,17});}
        if (toAvoid!=4) {SpawnNewVertexWidget(m.vertices[4], new int[]{4,9,22});}
        if (toAvoid!=5) {SpawnNewVertexWidget(m.vertices[5], new int[]{5,12,21});}
        if (toAvoid!=6) {SpawnNewVertexWidget(m.vertices[6], new int[]{6,15,16});}
        if (toAvoid!=7) {SpawnNewVertexWidget(m.vertices[7], new int[]{7,10,19});}
    }

    void MakeEdgeWidgetsForCube(int toAvoid)
    {
        Mesh m = mf.sharedMesh;
        medit_utils.ImmediateDestroy(t_edgeWidgetContainer, toAvoid);

        if (toAvoid!=0) {SpawnNewEdgeWidget((m.vertices[0]+m.vertices[1])/2, new int[] {0,1,    13,20,  8,23     });}
        if (toAvoid!=1) {SpawnNewEdgeWidget((m.vertices[1]+m.vertices[2])/2, new int[] {1,2,    8,23,   11,18     });}
        if (toAvoid!=2) {SpawnNewEdgeWidget((m.vertices[2]+m.vertices[3])/2, new int[] {2,3,  11,18, 14,17     });}
        if (toAvoid!=3) {SpawnNewEdgeWidget((m.vertices[3]+m.vertices[0])/2, new int[] {3,0,    14,17,  13,20,     });}

        if (toAvoid!=4) {SpawnNewEdgeWidget((m.vertices[4]+m.vertices[5])/2, new int[] {4,5,   9,22,   12,21,     });}
        if (toAvoid!=5) {SpawnNewEdgeWidget((m.vertices[5]+m.vertices[6])/2, new int[] {5,6,   12,21,  15,16,     });}
        if (toAvoid!=6) {SpawnNewEdgeWidget((m.vertices[6]+m.vertices[7])/2, new int[] {6,7,   15,16,  10,19,     });}
        if (toAvoid!=7) {SpawnNewEdgeWidget((m.vertices[7]+m.vertices[4])/2, new int[] {7,4,   10,19,  9,22,     });}

        if (toAvoid!=8) {SpawnNewEdgeWidget((m.vertices[1]+m.vertices[4])/2, new int[] {1,4,   8,23,   9,22      });}
        if (toAvoid!=9) {SpawnNewEdgeWidget((m.vertices[5]+m.vertices[0])/2, new int[] {5,0,  12,21,  13,20,     });}
        if (toAvoid!=10) {SpawnNewEdgeWidget((m.vertices[2]+m.vertices[7])/2, new int[] {2,7, 11,18,  10,19,     });}
        if (toAvoid!=11) {SpawnNewEdgeWidget((m.vertices[6]+m.vertices[3])/2, new int[] {6,3,   15,16,  14,17,     });}
    }

    public void ClearAllWidgets()
    {
        medit_utils.ImmediateDestroy(t_faceWidgetContainer);
        medit_utils.ImmediateDestroy(t_edgeWidgetContainer);
        medit_utils.ImmediateDestroy(t_vertexWidgetContainer);
    }

    void MakeEdgeWidgets(Mesh m, int toAvoid)
    {
        medit_utils.ImmediateDestroy(t_edgeWidgetContainer, toAvoid);

        // now edges, these are done using the triangle array
        for (int i = 0, n=0; i < m.triangles.Length; i+=3, n=0)
        {
            if (toAvoid == n) {continue;}
            Vector3 midpointAB = (m.vertices[m.triangles[i]]+m.vertices[m.triangles[i+1]])/2f;
            Vector3 midpointBC = (m.vertices[m.triangles[i+1]]+m.vertices[m.triangles[i+2]])/2f;
            Vector3 midpointCA = (m.vertices[m.triangles[i+2]]+m.vertices[m.triangles[i]])/2f;

            SpawnNewEdgeWidget(midpointAB, m.triangles[i], m.triangles[i+1]);
            SpawnNewEdgeWidget(midpointBC, m.triangles[i+1], m.triangles[i+2]);
            SpawnNewEdgeWidget(midpointCA, m.triangles[i+2], m.triangles[i]);
        }
    }

    void MakeVertexWidgets(Mesh m, int toAvoid)
    {
        medit_utils.ImmediateDestroy(t_vertexWidgetContainer, toAvoid);

        // first the vertex widgets
        for (int i = 0; i < m.vertices.Length; i++)
        {
            if (toAvoid == i) {continue;}
            SpawnNewVertexWidget(m.vertices[i], i);
        }
    }

    public void FlipTriangleNormals(int[] firstTriangleIndices)
    {
        for (int i = 0; i < firstTriangleIndices.Length; i++)
        {
            ApplyMesh(medit_mesh_processing.FlipTriangle(mf.sharedMesh, firstTriangleIndices[i]));
        }
    }

    public void MoveVertices(int[] verts, Vector3 offset)
    {
        HideShapeWidgets();
        ApplyMesh(medit_mesh_processing.OffsetAllVertices(mf.sharedMesh, verts, offset));
    }

    public void MoveVertex(int vertexIndex, Vector3 p)
    {
        HideShapeWidgets();
        ApplyMesh(medit_mesh_processing.MoveVertices(mf.sharedMesh, new int[]{vertexIndex}, new Vector3[] {p}));
    }

    public void MoveEdge(Vector3 newMidpoint, int v1, int v2)
    {
        HideShapeWidgets();
        ApplyMesh(medit_mesh_processing.MoveEdge(mf.sharedMesh, newMidpoint, v1,v2));
    }

    // widget spawning functions
    // ** 

    void SpawnNewFaceWidget(Vector3 p, int[] firstTriangleIndices)
    {
        medit_widget_generic newWidget = Instantiate(p_faceWidget, t_faceWidgetContainer).GetComponent<medit_widget_generic>();
        newWidget.obj = this;
        newWidget.SetWidgetIcon(medit_main.widgetIcons[8]);
        newWidget.transform.localPosition = p;

        // assigning the vertices that the widget has control of
        newWidget.GetComponent<medit_widget_face>().slvr.oldPosition =  newWidget.transform.localPosition;
        newWidget.GetComponent<medit_widget_face>().ControlVertices(firstTriangleIndices);
        // removing any edge widgets that occupy the same space
        newWidget.GetComponent<medit_widget_face>().ReplaceExistingEdgeWidgets();
        
    }

    void SpawnNewFaceWidget(Vector3 p, int[] firstTriangleIndices, int[] controllingVertices)
    {
        medit_widget_generic newWidget = Instantiate(p_faceWidget, t_faceWidgetContainer).GetComponent<medit_widget_generic>();
        newWidget.obj = this;
        newWidget.SetWidgetIcon(medit_main.widgetIcons[8]);
        newWidget.transform.localPosition = p;
        // assigning the vertices that the widget has control of
        newWidget.GetComponent<medit_widget_face>().slvr.oldPosition =  newWidget.transform.localPosition;
        newWidget.GetComponent<medit_widget_face>().ControlVertices(firstTriangleIndices,controllingVertices);
        // removing any edge widgets that occupy the same space
        newWidget.GetComponent<medit_widget_face>().ReplaceExistingEdgeWidgets();
        
    }

    void SpawnNewShapeWidget(int iconIndex, Vector3 p)
    {
        medit_widget_generic newWidget = Instantiate(p_shapeWidget, t_shapeWidgetContainer).GetComponent<medit_widget_generic>();
        newWidget.obj = this;
        newWidget.SetWidgetIcon(medit_main.widgetIcons[iconIndex]);
        newWidget.transform.localPosition = p;
    }

    void SpawnNewVertexWidget(Vector3 p, int vertexIndex)
    {
        medit_widget_generic newWidget = Instantiate(p_vertexWidget, t_vertexWidgetContainer).GetComponent<medit_widget_generic>();
        newWidget.obj = this;
        newWidget.SetWidgetIcon(medit_main.widgetIcons[1]);
        newWidget.transform.localPosition = p;

        newWidget.GetComponent<medit_widget_vertex>().slvr.controllingVertices = new List<int>() {vertexIndex};
    }

    void SpawnNewVertexWidget(Vector3 p, int[] vertexIndices)
    {
        medit_widget_generic newWidget = Instantiate(p_vertexWidget, t_vertexWidgetContainer).GetComponent<medit_widget_generic>();
        newWidget.obj = this;
        newWidget.SetWidgetIcon(medit_main.widgetIcons[1]);
        newWidget.transform.localPosition = p;

        newWidget.GetComponent<medit_widget_vertex>().slvr.controllingVertices = vertexIndices.ToList();
    }

    void SpawnNewEdgeWidget(Vector3 p, int vertexIndex1, int vertexIndex2)
    {
        medit_widget_generic newWidget = Instantiate(p_edgeWidget, t_edgeWidgetContainer).GetComponent<medit_widget_generic>();
        newWidget.obj = this;
        newWidget.SetWidgetIcon(medit_main.widgetIcons[7]);
        newWidget.transform.localPosition = p;

        newWidget.GetComponent<medit_widget_edge>().slvr.controllingVertices = new List<int>() {vertexIndex1, vertexIndex2};
    }

    void SpawnNewEdgeWidget(Vector3 p, int[] controllingVertices)
    {
        medit_widget_generic newWidget = Instantiate(p_edgeWidget, t_edgeWidgetContainer).GetComponent<medit_widget_generic>();
        newWidget.obj = this;
        newWidget.SetWidgetIcon(medit_main.widgetIcons[7]);
        newWidget.transform.localPosition = p;

        newWidget.GetComponent<medit_widget_edge>().slvr.controllingVertices = controllingVertices.ToList();
    }

    // **

    void SetupForCube()
    {
        // resetting first, to remove any existing shape widgets
        medit_utils.ImmediateDestroy(t_shapeWidgetContainer);

        // we need to have FOUR shape widgets, that represent the center and bounds in each axis
            
        SpawnNewShapeWidget(4, Vector3.right); // x
        SpawnNewShapeWidget(5, Vector3.up); // y
        SpawnNewShapeWidget(6, Vector3.forward); // z
        SpawnNewShapeWidget(3, Vector3.zero); // center
    }

    void SetupForPlane()
    {
        // resetting first, to remove any existing shape widgets
        medit_utils.ImmediateDestroy(t_shapeWidgetContainer);
    
        SpawnNewShapeWidget(4, Vector3.right); // x
        SpawnNewShapeWidget(5, Vector3.up); // y
        SpawnNewShapeWidget(3, Vector3.up); // y
    }

    public Vector3 GetWidget(int index)
    {
        return t_shapeWidgetContainer.GetChild(index).localPosition;
    }

    // see this doesn't just flip the normals, the only reason I named this function that is bc of blender
    // like in blender, what this ACTUALLY does is change the winding order of the triangles
    public void FlipAllMeshNormals()
    {
        Mesh m = mf.sharedMesh;
        m = medit_mesh_processing.FlipAllNormals(medit_mesh_processing.FlipAllTriangles(m));
    }

    public void ExtrudeEdge(int v1, int v2, Vector3 dir)
    {
        Mesh m = mf.sharedMesh;
        Vector3 midpoint = (m.vertices[v1]+m.vertices[v2])/2f;
        Vector3 newMidpoint = midpoint+dir;
        m = medit_mesh_processing.AddExtrudedEdge(m, newMidpoint, v1, v2);

        // widgets
        int newV1 = m.vertices.Length - 2;
        int newV2 = m.vertices.Length - 1;
        SpawnNewVertexWidget(m.vertices[newV1], newV1);
        SpawnNewVertexWidget(m.vertices[newV2], newV2);

        SpawnNewEdgeWidget(newMidpoint, new int[] {newV1,newV2});

        SpawnNewEdgeWidget(m.vertices[v1] + (newMidpoint - midpoint)/2f, new int[] {v1, newV1});
        SpawnNewEdgeWidget(m.vertices[v2] + (newMidpoint - midpoint)/2f, new int[] {v2, newV2});

        SpawnNewFaceWidget(midpoint + (newMidpoint - midpoint)/2f, new int[] {m.triangles.Length - 3, m.triangles.Length - 6});

        // update the actual mesh
        ApplyMesh(m);
    }

    // spawn a new line and vertex, off of an old one
    public void ExtrudeVertex(int vertexId)
    {
        
    }
}
