using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class medit_mesh_intersection
{
    // needed for a lot of the boolean cut procedures
    public static float epsilon = 0.01f;

    // public static Vector2 LineLineIntersection2D(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
    // {
    //     Vector2 a = a2-a1;
    //     Vector2 b = b2-b1;

    //     // slopes
    //     float ma = a.y/a.x;
    //     float mb = b.y/b.x;

    //     // intercepts
    //     float 
    // }
    
    // tested 12/08/2025, works
    // ONLY WORKS FOR CONVEX MESHES, BUT ITS WAYY FASTER
    public static bool IsPointInsideMesh(Mesh m, Vector3 p)
    {
        for (int i = 0; i < m.vertices.Length; i++)
        {
            if (Vector3.Dot((p-m.vertices[i]).normalized, m.normals[i]) >= 0)
            {
                return false;
            }
        }

        return true;
    }

    public static bool LineLineIntersection(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
    {
        Vector2 a = a2-a1;
        Vector2 b = b2-b1;

        return LineLineIntersection(
            new Vector3(a1.x, a1.y, 0), 
            new Vector3(a.x, a.y, 0), 
            new Vector3(b1.x, b2.y, 0), 
            new Vector3(b.x, b.y, 0));
    }

    // rA and rB LENGTH MATTERS
    public static bool LineLineIntersection(Vector3 pA1, Vector3 rA, Vector3 pB1, Vector3 rB)
    {
        Vector3 w0 = pA1-pB1;
        Vector3 N = Vector3.Cross(rA, rB);

        if (N.magnitude == 0) return false; // parallel lines

        float dist = Mathf.Abs(Vector3.Dot(w0,N)) / N.magnitude;

        if (dist != 0) return false; // skew lines

        return true;
    }

    public static Vector3 LineLineIntersectionPoint(Vector3 pA1, Vector3 rA, Vector3 pB1, Vector3 rB)
    {
        Vector3 w0 = pA1-pB1;
        Vector3 N = Vector3.Cross(rA, rB);

        if (N.magnitude < epsilon) return Vector3.zero; // parallel lines

        float dist = Mathf.Abs(Vector3.Dot(w0,N)) / N.magnitude;

        if (dist != 0) return Vector3.zero; // skew lines

        float s = Vector3.Dot((pB1-pA1), Vector3.Cross(rB,N)) / Vector3.Dot(rA, Vector3.Cross(rB,N));

        Vector3 I = pA1 + s * rA;

        return I;
    }


    // another cv, but also tiny

    // rA and rB MUST BE NORMALIZED
    public static float LineLineIntersectionDistance(Vector3 pA1, Vector3 rA, Vector3 pB1, Vector3 rB)
    {
        if (Vector3.Angle(rA, rB) < epsilon || Vector3.Angle(rA, rB) > 180- epsilon)
        {
            return 0;
        }
        Vector3 w0 = pA1-pB1;
        Vector3 N = Vector3.Cross(rA, rB);
            
        if (N.magnitude < epsilon) return 0; // parallel lines

        float dist = Mathf.Abs(Vector3.Dot(w0,N)) / N.magnitude;

        if (dist != 0) return 0; // skew lines

        float s = Vector3.Dot((pB1-pA1), Vector3.Cross(rB,N)) / Vector3.Dot(rA, Vector3.Cross(rB,N));

        return s;
    }

    // yes its a cv, but its small so its okay
    public static List<mesh_triangle> MeshIntersectTriangle(Mesh m, Vector3 p, Vector3 r)
    {
        List<mesh_triangle> result = new List<mesh_triangle>();
        int[] tris = m.GetTriangles(0);

        for (int i = 0; i < tris.Length; i+=3)
        {
            Vector3 res = TriangleIntersectPoint(m.vertices[tris[i]], m.vertices[tris[i+1]], m.vertices[tris[i+2]], p, r.normalized);
            if (res != Vector3.zero&& (res-p).magnitude <= r.magnitude - epsilon) 
            {
                result.Add(new mesh_triangle());
                result[result.Count - 1].v1 = m.vertices[tris[i]];
                result[result.Count - 1].v2 = m.vertices[tris[i+1]];
                result[result.Count - 1].v3 = m.vertices[tris[i+2]];

                result[result.Count - 1].n1 = tris[i];
                result[result.Count - 1].n2 = tris[i+1];
                result[result.Count - 1].n3 = tris[i+2];
                
            }
        }

        return result;
    }

    public static Vector3 MeshIntersectPoint(Mesh m, Vector3 p, Vector3 r)
    {
        int[] tris = m.GetTriangles(0);

        for (int i = 0; i < tris.Length; i+=3)
        {
            Vector3 res = TriangleIntersectPoint(m.vertices[tris[i]], m.vertices[tris[i+1]], m.vertices[tris[i+2]], p, r.normalized);
            if (res != Vector3.zero&& (res-p).magnitude <= r.magnitude - epsilon)
            {
                return res;
            }
        }

        return Vector3.zero;
    }

    public static bool MeshIntersect(Mesh m, Vector3 p, Vector3 r)
    {
        Vector3 res = MeshIntersectPoint(m, p, r.normalized);
        if (res != Vector3.zero)
        {
            if (Vector3.Distance(p, res) < r.magnitude - epsilon)
            {
                return true;
            }
            return false;
        }
        return false;
    }

    // tested 12/08/2025, works
    // r MUST BE NORMALIZED
    public static Vector3 TriangleIntersectPoint(Vector3 ta, Vector3 tb, Vector3 tc, Vector3 p, Vector3 r)
    {
        Vector3 triangleNormal = Vector3.Cross(tc-ta,tb-ta).normalized;
        Vector3 triangleCenter = (ta+tb+tc)/3f;

        float planeIntersectionDistance = Vector3.Dot(triangleCenter - p, triangleNormal)
            / Vector3.Dot(r, triangleNormal);

        if (planeIntersectionDistance > 0) // can't be negative
        {
            Vector3 planeIntersectionPoint = p + r.normalized*planeIntersectionDistance;
            if (IsPointInsideTriangle3D(planeIntersectionPoint, ta, tb, tc))
            {
                return planeIntersectionPoint;
            }
        }

        return Vector3.zero;
    }

    public static bool TriangleIntersect(Vector3 ta, Vector3 tb, Vector3 tc, Vector3 p, Vector3 r)
    {
        return TriangleIntersectPoint(ta, tb, tc, p, r) != Vector3.zero;
    }


    // tested 12/08/2025, works
    public static bool IsPointInsideTriangle3D(Vector3 p, Vector3 ta, Vector3 tb, Vector3 tc)
    {
        Vector3 m = (ta+tb+tc)/3f;

        // inflate
        ta += (ta-m).normalized * epsilon;
        tb += (tb-m).normalized * epsilon;
        tc += (tc-m).normalized * epsilon;

        Vector3 triangleNormal = Vector3.Cross(tc-ta,tb-ta).normalized;
        
        Vector3 normalTest = Vector3.Cross(tc-p,tb-p).normalized;
        if (Vector3.Angle(triangleNormal, normalTest) > epsilon && Vector3.Angle(triangleNormal, -normalTest) > epsilon)
        {
            // if we get here, the point is outside of the plane
            return false;
        }
        else
        {
            Vector3 abRotated = RotateAround(tb-ta, triangleNormal, Mathf.PI/2f);
            Vector3 bcRotated = RotateAround(tc-tb, triangleNormal, Mathf.PI/2f);
            Vector3 caRotated = RotateAround(ta-tc, triangleNormal, Mathf.PI/2f);

            Vector3 ap = p-ta;
            Vector3 bp = p-tb;
            Vector3 cp = p-tc;

            float tolerance = epsilon;
            if (Vector3.Dot(ap, abRotated) >= -tolerance && Vector3.Dot(bp, bcRotated) >= -tolerance && Vector3.Dot(cp, caRotated) >= -tolerance)
            {
                return true;
            } else if (Vector3.Dot(ap, abRotated) <= tolerance && Vector3.Dot(bp, bcRotated) <= tolerance && Vector3.Dot(cp, caRotated) <= tolerance)
            {
                return true;
            }
        }

        return false;
    }

    // weird ass function name
    public static bool IsLineValidForMesh(Mesh m, Vector3 newv1, Vector3 newv2)
    {
        return LineIntersectForMesh(m, newv1, newv2) == -1;
    }

    // checking to see if a line CROSSES existing triangle lines, 
    // in which case it is invalid
    public static int LineIntersectForMesh(Mesh m, Vector3 newv1, Vector3 newv2)
    {
        int[] existingTris = m.GetTriangles(0);

        for (int i = 0; i < existingTris.Length; i+=3)
        {
            Vector3 v1 = m.vertices[existingTris[i]];
            Vector3 v2 = m.vertices[existingTris[i+1]];
            Vector3 v3 = m.vertices[existingTris[i+2]];

            float i1 = LineLineIntersectionDistance(newv1, (newv2-newv1).normalized, v1, (v2-v1).normalized);
            float i2 = LineLineIntersectionDistance(newv1, (newv2-newv1).normalized, v2, (v3-v2).normalized);
            float i3 = LineLineIntersectionDistance(newv1, (newv2-newv1).normalized, v3, (v1-v3).normalized);

            float threshold = 1f; // TODO: look into this
            if (i1 > threshold && i1 < (newv2-newv1).magnitude-threshold) {return existingTris[i];}
            if (i2 > threshold && i2 < (newv2-newv1).magnitude-threshold) { return existingTris[i+1];} // Debug.Log(existingTris[i] + "   " + existingTris[i+1] + "    " + existingTris[i+2]);
            if (i3 > threshold && i3 < (newv2-newv1).magnitude-threshold) {return existingTris[i+2];}
        }


        // no intersections were found, so the line is valid
        return -1;
    }

    // holy fucking trig math
    // I don't even have a link for this one, I stole it from my python 3D game engine
    // yes im aware the brackets aren't necessary

    // tested 12/08/2025, works
    public static Vector3 RotateAround(Vector3 v, Vector3 a, float theta)
    {
        return new Vector3(
            v.x * (     (a.x * a.x) * (1 - Mathf.Cos(theta)) + Mathf.Cos(theta)             ) + v.y * (        (a.y * a.x) * (1 - Mathf.Cos(theta)) - (a.z * Mathf.Sin(theta))         ) + v.z * (        (a.x * a.z) * (1 - Mathf.Cos(theta)) + (a.y * Mathf.Sin(theta))     ),
            v.x * (     (a.x * a.y) * (1 - Mathf.Cos(theta)) + (a.z * Mathf.Sin(theta))     ) + v.y * (        (a.y * a.y) * (1 - Mathf.Cos(theta)) + Mathf.Cos(theta)                 ) + v.z * (        (a.y * a.z) * (1 - Mathf.Cos(theta)) - (a.x * Mathf.Sin(theta))     ),
            v.x * (     (a.x * a.z) * (1 - Mathf.Cos(theta)) - (a.y * Mathf.Sin(theta))     ) + v.y * (        (a.y * a.z) * (1 - Mathf.Cos(theta)) + (a.x * Mathf.Sin(theta))         ) + v.z * (        (a.z * a.z) * (1 - Mathf.Cos(theta)) + Mathf.Cos(theta)              )
        );
    }
}