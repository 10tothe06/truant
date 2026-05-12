using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

// sort of a misc. class for mesh editing functions
public class medit_mesh_processing
{
    // needed for a lot of the boolean cut procedures
    public static float epsilon = 0.01f;

    // // assumes indices are [0..Length-1]
    // public static int[] GetInsetTriangulation(Vector3[] outerVertices, Vector3[] innerVertices)
    // {
    //     int[] outerIndices = new int[outerVertices.Length];
    //     int[] innerIndices = new int[innerVertices.Length];
    //     for (int i = 0; i < outerVertices.Length; i++) {outerIndices[i] = i;}
    //     for (int i = 0; i < innerVertices.Length; i++) {innerIndices[i] = i;}

    //     return GetInsetTriangulation(outerVertices, innerVertices, outerIndices, innerIndices);
    // }

    // // the algorithm developed by me and Mr. Spisak

    // // * loop through the outer vertices in any order
    // // * create an edge between
    // public static int[] GetInsetTriangulation(Vector3[] outerVertices, Vector3[] innerVertices, int[] outerIndices, int[] innerIndices)
    // {
        
    // }

    // returns degrees
    // works
    public static float GetClockwiseAngle(Vector3 a, Vector3 b, Vector3 normal)
    {
        float rawAngle = Vector3.Angle(a, b);
        if (Vector3.Dot(Vector3.Cross(a, b), normal) > 0)
        {
            return rawAngle;
        } else
        {
            return 360 - rawAngle;
        }
    }

    // works
    public static float GetCounterClockwiseAngle(Vector3 a, Vector3 b, Vector3 normal)
    {
        float rawAngle = Vector3.Angle(a, b);
        if (Vector3.Dot(Vector3.Cross(a, b), normal) > 0)
        {
            return 360 - rawAngle;
        } else
        {
            return rawAngle;
        }
    }

    public static Vector3[] ReOrderVectors(Vector3[] raw, int[] newIndices)
    {
        Vector3[] result = new Vector3[raw.Length];

        for (int i = 0; i < raw.Length; i++)
        {
            result[i] = raw[newIndices[i]];
        }

        return result;
    }

    // because we have the normal vector, the vertices can be in any order and will be sorted
    public static Mesh FillFace(Mesh m, Vector3[] verts, Vector3 normal)
    {
        Mesh result = new Mesh();

        List<int> tris = m.triangles.ToList();

        // sorting the vertices in a clockwise order, if they haven't been already
        verts = ReOrderVectors(verts, OrderVertices(verts, normal));

        int[] triangulation = TriangulateNGon(verts.Length);

        result.SetVertices(m.vertices);
        result.SetNormals(m.normals);
        result.SetUVs(0,m.uv);
        result.SetTriangles(tris,0);

        return  result;
    }

    public static int[] TriangulateNGon(int vertexCount)
    {
        List<int> result = new List<int>();

        

        return result.ToArray();
    }

    public static int[] OrderVertices(int[] verts, Vector3 normal, Mesh m)
    {
        Vector3[] vertPos = new Vector3[verts.Length];
        for (int i = 0; i < vertPos.Length; i++)
        {
            vertPos[i] = m.vertices[verts[i]];
        }

        int[] reordered = OrderVertices(vertPos, normal);

        int[] result = new int[verts.Length];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = verts[reordered[i]];
        }

        return result;
    }

    // works
    // orders them clockwise
    public static int[] OrderVertices(Vector3[] verts, Vector3 normal)
    {
        Vector3 midpoint = GetMidpoint(verts);
        float[] angles = new float[verts.Length];

        Vector3 toZero = verts[0] - midpoint;

        angles[0] = 0; // we know the first angle is zero,
        // so we skip it and only run the loop on the others
        for (int i = 1; i < angles.Length; i++)
        {
            Vector3 fromMid = verts[i] - midpoint;
            angles[i] = GetClockwiseAngle(toZero, fromMid, normal);
        }

        List<int> orderedIndices = new List<int>();

        // time for another god-fucking-awful sorting algorithm
        // a.k.a king of the hill
        for (int n = 0; n < angles.Length; n++)
        {
            int smallestIndex = -1;
            for (int i = 0; i < angles.Length; i++)
            {
                if (orderedIndices.Contains(i)) {continue;}
                if (smallestIndex == -1)
                {
                    smallestIndex = i;
                }
                if (angles[i] < angles[smallestIndex])
                {
                    smallestIndex = i;
                }
            }
            orderedIndices.Add(smallestIndex);
        }

        return orderedIndices.ToArray();
    }

    public static Vector3 GetMidpoint(Vector3[] verts)
    {
        Vector3 midpoint = Vector3.zero;

        for (int i = 0; i < verts.Length; i++)
        {
            midpoint += verts[i]/(float)verts.Length;
        }

        return midpoint;
    }
    public static Vector3 GetMidpoint(int[] verts, Mesh m)
    {
        Vector3 midpoint = Vector3.zero;

        for (int i = 0; i < verts.Length; i++)
        {
            midpoint += m.vertices[verts[i]]/(float)verts.Length;
        }

        return midpoint;
    }

    // replacing one of the faces of a mesh
    public static Mesh InsetFace(Mesh m, int[] faceVertices, int[] newVertices)
    {
        Mesh result = new Mesh();

        return result;
    }

    public static bool IsTriangleClockwise(Vector3[] points, Vector3 normal) {

        if (Vector3.Angle(Vector3.Cross(points[1]-points[0],points[2]-points[0]), normal)<1f)
        {
            return true;
        } else
        {
            return false;
        }
    }

    // this is why I have to go to university for computer science
    // this is the worst damn algorithm I have ever written lol
    public static int[] GetCollinears(List<Vector3> points)
    {
        List<int> result = new List<int>();

        for (int i = 0; i < points.Count; i++)
        {
            Vector3 primary = points[i]; // just for readability

            for (int j = 0; j < points.Count; j++)
            {
                if (i != j) // can't be the same point
                {
                    Vector3 secondary = points[j]; // just for readability

                    for (int k = 0; k < points.Count; k++)
                    {
                        if (k != i && k != j)
                        {
                            if (!result.Contains(k) && Vector3.Dot((primary - points[k]).normalized, (secondary - points[k]).normalized) == -1)
                            {
                                result.Add(k);
                            }
                        }
                    }
                }
            }
        }

        return result.ToArray();
    }

    public static Mesh ReduceMesh(Mesh m)
    {
        Mesh result = new Mesh();

        List<Vector3> slimVertices = new List<Vector3>();
        List<Vector3> norms = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();

        List<int> conversionOld = new List<int>();
        List<int> conversionNew = new List<int>();

        for (int i = 0; i < m.vertices.Length; i++)
        {
            int match = FindFirstMatch(slimVertices, m.vertices[i]);
            if (match == -1)
            {
                slimVertices.Add(m.vertices[i]);
                norms.Add(m.normals[i]);
                uv.Add(Vector2.one); // not bothering with texture coords
            } else
            {
                conversionNew.Add(match);
                conversionOld.Add(i);
            }
        }

        int[] tris = m.GetTriangles(0);
        for (int i = 0; i < tris.Length; i++)
        {
            if (conversionOld.Contains(tris[i]))
            {
                tris[i] = conversionNew[conversionOld.IndexOf(tris[i])];
            }
        }

        result.SetVertices(slimVertices);
        result.SetNormals(norms);
        result.SetUVs(0, uv);
        result.SetTriangles(tris, 0);

        return result;
    }

    public static int FindFirstMatch(List<Vector3> toSearch, Vector3 target)
    {
        for (int i = 0; i < toSearch.Count; i++)
        {
            if (Vector3.Distance(toSearch[i], target) < epsilon)
            {
                return i;
            }
        }

        return -1;
    }

    public static line_segment[] FindConnectingVertices(Vector3[] actuals, int[] sources1, Mesh m, Mesh m_reduced)
    {
        List<line_segment> result = new List<line_segment>();

        int[] triArray = m_reduced.GetTriangles(0);
        
        for (int i = 0; i < sources1.Length; i++)
        {
            for (int j = 0; j < sources1.Length; j++)
            {
                // the vertex we're looking at cannot be the one that the ray came from, because we already handled that
                // we also can't be looking at the original vertex either
                if (sources1[i] != sources1[j]) 
                {
                    //Debug.Log("sv   " + sources1[i] + "    " + sources1[j]);
                    if (AreVerticesConnected(triArray, FindVertexId(m_reduced, m.vertices[sources1[i]]), FindVertexId(m_reduced, m.vertices[sources1[j]])))
                    {
                        Vector3 v1 = actuals[i];
                        Vector3 v2 = actuals[j];
                        //Test.Instance.interruptPoints.Add(v1 + (v2-v1)*0.5f);
                        if (!medit_mesh_intersection.IsPointInsideMesh(m, v1 + (v2-v1)*0.5f))
                        {
                            // so we've found a connected vertex
                            result.Add(new line_segment(sources1[i], sources1[j]));
                        }
                    }
                }
            }
        }

        return result.ToArray();
    }

    public static int FindVertexId(Mesh m, Vector3 vPos)
    {
        for (int i = 0; i < m.vertices.Length; i++)
        {
            if (Vector3.Distance(m.vertices[i], vPos) < 0.01f)
            {
                return i;
            }
        }

        return -1;
    }

    public static bool TriangleExistsInMesh(Mesh m, int a, int b, int c)
    {
        int[] tris = m.GetTriangles(0);
        for (int i = 0; i < tris.Length; i+=3)
        {
            List<int> temp = new List<int>();

            temp.Add(tris[i]);
            temp.Add(tris[i+1]);
            temp.Add(tris[i+2]);

            if (temp.Contains(a) && temp.Contains(b) && temp.Contains(c)) {
                return true;
            }
        }

        return false;
    }

    public static bool TriangleExistsInMesh(int[] tris, int a, int b, int c)
    {
        for (int i = 0; i < tris.Length; i+=3)
        {
            List<int> temp = new List<int>();

            temp.Add(tris[i]);
            temp.Add(tris[i+1]);
            temp.Add(tris[i+2]);

            if (temp.Contains(a) && temp.Contains(b) && temp.Contains(c)) {
                return true;
            }
        }

        return false;
    }

    public static bool AreVerticesConnected(int[] tris, int a, int b)
    {
        for (int i = 0; i < tris.Length; i+=3)
        {
            if (tris[i] == a && tris[i+1] == b) return true; 
            if (tris[i] == b && tris[i+1] == a) return true; 
            if (tris[i+1] == a && tris[i+2] == b) return true;
            if (tris[i+1] == b && tris[i+2] == a) return true;
            if (tris[i+2] == a && tris[i] == b) return true;
            if (tris[i+2] == b && tris[i] == a) return true;
        }

        return false;
    }

    public static int[] GetTrianglesThatContainVertices(Mesh m, int[] vertices)
    {
        List<int> result = new List<int>();

        for (int i = 0; i < m.triangles.Length; i+= 3)
        {
            if (vertices.Contains(m.triangles[i]) || vertices.Contains(m.triangles[i+1]) || vertices.Contains(m.triangles[i+2]))
            {
                result.Add(i);
            }
        }

        return result.ToArray();
    }

    // adding a new edge and also the a face to connect the edge to the old one
    public static Mesh AddExtrudedEdge(Mesh m, Vector3 newMidpoint, int v1, int v2)
    {
        // easiest way to add items imo
        List<Vector3> verts = m.vertices.ToList();
        List<Vector3> normals = m.normals.ToList();
        List<Vector2> uvs = m.uv.ToList();
        List<int> tris = m.triangles.ToList();

        Vector3 oldMidpoint = (m.vertices[v1]+m.vertices[v2])/2f;
        Vector3 diff = newMidpoint - oldMidpoint;

        verts.Add(m.vertices[v1]+diff);
        verts.Add(m.vertices[v2]+diff);
        normals.Add(m.normals[v1]);
        normals.Add(m.normals[v2]);
        uvs.Add(m.uv[v1]);
        uvs.Add(m.uv[v2]);

        // here we're just triangulating the four 
        // the winding order doesn't matter since the user can just flip it
        tris.Add(v1);
        tris.Add(v2);
        tris.Add(verts.Count - 2);

        tris.Add(v2);
        tris.Add(verts.Count - 1);
        tris.Add(verts.Count - 2);

        m.SetVertices(verts);
        m.SetNormals(normals);
        m.SetUVs(0,uvs);
        m.SetTriangles(tris,0);
        return m;
    }

    // TODO: make a plural version of this with arrays, that might make doing faces easier
    // maybe
    public static Mesh MoveEdge(Mesh m, Vector3 newMidpoint, int v1, int v2)
    {
        Vector3 oldMidpoint = (m.vertices[v1]+m.vertices[v2])/2f;
        Vector3 diff = newMidpoint - oldMidpoint;

        Vector3[] verts = m.vertices;
        verts[v1]+=diff;
        verts[v2]+=diff;
        m.SetVertices(verts);

        return m;
    }

    public static Mesh OffsetAllVertices(Mesh m, int[] vertexIndices, Vector3 offset)
    {
        Vector3[] verts = m.vertices;
        for (int i = 0; i < vertexIndices.Length; i++)
        {
            verts[vertexIndices[i]] += offset;
        }

        m.SetVertices(verts);
        return m;
    }

    public static Mesh MoveVertices(Mesh m, int[] vertexIndices, Vector3[] newPositions)
    {
        Vector3[] verts = m.vertices;
        for (int i = 0; i < vertexIndices.Length; i++)
        {
            verts[vertexIndices[i]] = newPositions[i];
        }

        m.SetVertices(verts);
        return m;
    }

    // tested 12/08/2025, works
    public static Mesh ApplyTransform(Mesh m, Transform t)
    {
        Mesh result = new Mesh();

        Vector3[] verts = new Vector3[m.vertices.Length];
        Vector3[] norms = new Vector3[m.vertices.Length];

        List<Vector2> oldUvs = new List<Vector2>();
        m.GetUVs(0, oldUvs);// these stay EXACTLY the same
        int[] oldTris = m.GetTriangles(0); // these stay EXACTLY the same

        for (int i = 0; i < verts.Length; i++)
        {
            verts[i] = t.TransformPoint(m.vertices[i]);
            norms[i] = t.TransformDirection(m.normals[i]);
        }

        
        
        
        
        result.SetVertices(verts);
        result.SetNormals(norms);
        result.SetUVs(0, oldUvs);
        result.SetTriangles(oldTris, 0);

        return result;
    }

    // given the first index in the triangle you want to remove,
    // take it out of the mesh
    public static Mesh RemoveTriangle(Mesh m, int triangleFirstIndex)
    {
        Mesh result = new Mesh();
        result.SetVertices(m.vertices);
        result.SetNormals(m.normals);
        result.SetUVs(0,m.uv);

        List<int> tris = new List<int>();
        for (int i = 0; i < m.triangles.Length; i+=3)
        {
            if (i != triangleFirstIndex)
            {
                tris.Add(m.triangles[i]);
                tris.Add(m.triangles[i+1]);
                tris.Add(m.triangles[i+2]);
            }
        }

        result.SetTriangles(tris, 0);
        return result;
    }

    // same thing but with multiple indices
    // kind of an unecessary cv here
    public static Mesh RemoveTriangles(Mesh m, int[] triangleFirstIndices)
    {
        Mesh result = new Mesh();
        result.SetVertices(m.vertices);
        result.SetNormals(m.normals);
        result.SetUVs(0,m.uv);

        List<int> tris = new List<int>();
        for (int i = 0; i < m.triangles.Length; i+=3)
        {
            if (!triangleFirstIndices.Contains(i))
            {
                tris.Add(m.triangles[i]);
                tris.Add(m.triangles[i+1]);
                tris.Add(m.triangles[i+2]);
            }
        }

        result.SetTriangles(tris, 0);
        return result;
    }

    // similar to above two, but this time the goal is to remove triangles that ONLY  
    // contain vertices in the array
    public static Mesh  RemoveTrianglesWithVertices(Mesh m, int[] vertexIndices)
    {
        Mesh result = new Mesh();
        result.SetVertices(m.vertices);
        result.SetNormals(m.normals);
        result.SetUVs(0,m.uv);

        List<int> tris = new List<int>();
        for (int i = 0; i < m.triangles.Length; i+=3)
        {
            if (!vertexIndices.Contains(m.triangles[i]) &&
            !vertexIndices.Contains(m.triangles[i+1]) &&
            !vertexIndices.Contains(m.triangles[i+2]))
            {
                tris.Add(m.triangles[i]);
                tris.Add(m.triangles[i+1]);
                tris.Add(m.triangles[i+2]);
            }
        }

        result.SetTriangles(tris, 0);
        return result;
    }

    public static Mesh ApplyMove(Mesh m, Vector3 v)
    {
        Mesh result = new Mesh();

        Vector3[] verts = new Vector3[m.vertices.Length];
        Vector3[] norms = new Vector3[m.vertices.Length];

        List<Vector2> oldUvs = new List<Vector2>();
        m.GetUVs(0, oldUvs);// these stay EXACTLY the same
        int[] oldTris = m.GetTriangles(0); // these stay EXACTLY the same

        for (int i = 0; i < verts.Length; i++)
        {
            verts[i] = m.vertices[i] + v;
            norms[i] = m.normals[i];
        } 
        
        result.SetVertices(verts);
        result.SetNormals(norms);
        result.SetUVs(0, oldUvs);
        result.SetTriangles(oldTris, 0);

        return result;
    }

    public static Mesh FlipTriangle(Mesh m, int triIndex)
    {
        int[] tris = m.triangles;

        // simplest way I know to flip triangle order
        int b = tris[triIndex+1];
        int c = tris[triIndex+2];

        tris[triIndex+1]=c;
        tris[triIndex+2]=b;

        m.SetTriangles(tris, 0);
        return m;
    }

    // since this function is relatively simple (doesn't change vertex or triangle count),
    // I'm not creating a new mesh object at the start
    // hopefully this doesn't cause any issues
    public static Mesh FlipAllTriangles(Mesh m)
    {
        int[] tris = m.triangles;

        for (int i = 0; i < tris.Length; i+=3)
        {
            // simplest way I know to flip triangle order
            int b = tris[i+1];
            int c = tris[i+2];

            tris[i+1]=c;
            tris[i+2]=b;
        }

        m.SetTriangles(tris, 0);
        return m;
    }

    public static Mesh FlipAllNormals(Mesh m)
    {
        Vector3[] norms = m.normals;

        for (int i = 0; i < norms.Length; i++)
        {
            norms[i] = -norms[i];
        }

        m.SetNormals(norms);
        return m;
    }

    // terrible function name
    public static List<mesh_triangle> AddToList(List<mesh_triangle> a, List<mesh_triangle> b)
    {
        List<mesh_triangle> result = new List<mesh_triangle>();

        for (int i = 0; i < a.Count; i++)
        {
            result.Add(a[i]);
        }

        for (int i = 0; i < b.Count; i++)
        {
            result.Add(b[i]);
        }

        return result;
    }
}
