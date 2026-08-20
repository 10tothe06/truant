using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using UnityEngine;

public class util_mesh : MonoBehaviour
{
    public static Mesh[] DiceMesh(Mesh original, int num_iterations = 2)
    {
        List<Mesh> result = new List<Mesh>();
        List<Mesh> queue = new List<Mesh>() {original};
        List<Mesh> next_queue = new List<Mesh>() {};

        for (int n = 0; n < num_iterations; n++)
        {
            for (int q = 0; q < queue.Count; q++)
            {
                Vector3 point = GetMidpoint(queue[q].vertices);
                Vector3 normal = new Vector3(
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f)
                ).normalized;

                var(pos, neg) = util_grok.Cut(queue[q], normal, point, true);

                if (n == num_iterations - 1)
                {
                    result.Add(pos);
                    result.Add(neg);
                } else
                {
                    next_queue.Add(pos);
                    next_queue.Add(neg);
                }
            }

            queue.Clear();
            for (int i = 0; i < next_queue.Count; i++)
            {
                queue.Add(next_queue[i]);
            }
            next_queue.Clear();
        }



        return result.ToArray();
    }







    /// <summary>
    /// Generates a rectangular prism mesh that follows a path.
    /// Every face of every segment has its own independent 0-1 UV space.
    /// </summary>
    public static Mesh GenerateRectangularPrism(
        Vector3[] points,
        float width = 2f,
        float height = 0.5f,
        bool closed = false)
    {
        if (points == null || points.Length < 2)
        {
            Debug.LogError("Path must contain at least 2 points.");
            return null;
        }

        int pathCount = points.Length;
        int segmentCount = closed ? pathCount : pathCount - 1;

        var vertices  = new List<Vector3>();
        var normals   = new List<Vector3>();
        var uvs       = new List<Vector2>();
        var triangles = new List<int>();
        var top_triangles = new List<int>();

        // Pre-calculate forward directions
        Vector3[] forwards = new Vector3[pathCount];
        for (int i = 0; i < pathCount; i++)
        {
            if (closed)
            {
                Vector3 prev = points[(i - 1 + pathCount) % pathCount];
                Vector3 next = points[(i + 1) % pathCount];
                forwards[i] = (next - prev).normalized;
            }
            else
            {
                if (i == 0)
                    forwards[i] = (points[1] - points[0]).normalized;
                else if (i == pathCount - 1)
                    forwards[i] = (points[pathCount - 1] - points[pathCount - 2]).normalized;
                else
                    forwards[i] = (points[i + 1] - points[i - 1]).normalized;
            }
        }

        // ========== SIDE FACES (per segment, per face) ==========
        for (int seg = 0; seg < segmentCount; seg++)
        {
            int i0 = seg;
            int i1 = (seg + 1) % pathCount;

            // Get the two cross-sections
            GetCrossSection(points[i0], forwards[i0], width, height, out Vector3[] corners0, out Vector3[] _);
            GetCrossSection(points[i1], forwards[i1], width, height, out Vector3[] corners1, out Vector3[] _);

            // 4 faces per segment
            // Face order: 0=bottom, 1=right, 2=top, 3=left
            for (int face = 0; face < 4; face++)
            {
                int cA = face;
                int cB = (face + 1) % 4;

                // Four unique vertices for this single quad
                int baseVert = vertices.Count;

                Vector3 v0 = corners0[cA]; // start, side A
                Vector3 v1 = corners0[cB]; // start, side B
                Vector3 v2 = corners1[cB]; // end,   side B
                Vector3 v3 = corners1[cA]; // end,   side A

                vertices.Add(v0);
                vertices.Add(v1);
                vertices.Add(v2);
                vertices.Add(v3);

                // Face normal
                Vector3 normal = Vector3.Cross(v1 - v0, v3 - v0).normalized;
                // Ensure it points outward
                Vector3 faceCenter = (v0 + v1 + v2 + v3) * 0.25f;
                Vector3 pathCenter = (points[i0] + points[i1]) * 0.5f;
                if (Vector3.Dot(normal, faceCenter - pathCenter) < 0f)
                    normal = -normal;

                normals.Add(normal);
                normals.Add(normal);
                normals.Add(normal);
                normals.Add(normal);

                // Each face gets its own 0-1 UV space
                uvs.Add(new Vector2(0, 0)); // v0
                uvs.Add(new Vector2(1, 0)); // v1
                uvs.Add(new Vector2(1, 1)); // v2
                uvs.Add(new Vector2(0, 1)); // v3

                // Two triangles
                if (face == 2)
                {
                    // top face gets its own submesh
                    top_triangles.Add(baseVert + 0);
                    top_triangles.Add(baseVert + 1);
                    top_triangles.Add(baseVert + 2);

                    top_triangles.Add(baseVert + 0);
                    top_triangles.Add(baseVert + 2);
                    top_triangles.Add(baseVert + 3);
                } else
                {
                    // everything else is normal
                    triangles.Add(baseVert + 0);
                    triangles.Add(baseVert + 1);
                    triangles.Add(baseVert + 2);

                    triangles.Add(baseVert + 0);
                    triangles.Add(baseVert + 2);
                    triangles.Add(baseVert + 3);
                }
            }
        }

        // ========== END CAPS (only if open) ==========
        if (!closed)
        {
            AddCap(points[0], forwards[0], width, height, true, vertices, normals, uvs, triangles);
            AddCap(points[pathCount - 1], forwards[pathCount - 1], width, height, false, vertices, normals, uvs, triangles);
        }

        // Create mesh
        Mesh mesh = new Mesh();
        mesh.name = "RectangularPrismPath";
        mesh.subMeshCount = 2;
        mesh.SetVertices(vertices);
        mesh.SetNormals(normals);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(triangles, 0);
        mesh.SetTriangles(top_triangles, 1);
        mesh.RecalculateBounds();

        return mesh;
    }

    private static void GetCrossSection(
        Vector3 position,
        Vector3 forward,
        float width,
        float height,
        out Vector3[] corners,
        out Vector3[] normals)
    {
        Vector3 up = Vector3.up;
        if (Mathf.Abs(Vector3.Dot(forward, up)) > 0.99f)
            up = Vector3.right;

        Vector3 right = Vector3.Cross(up, forward).normalized;
        up = Vector3.Cross(forward, right).normalized;

        float halfW = width * 0.5f;
        float halfH = height * 0.5f;

        corners = new Vector3[4];
        corners[0] = position - right * halfW - up * halfH; // bottom-left
        corners[1] = position + right * halfW - up * halfH; // bottom-right
        corners[2] = position + right * halfW + up * halfH; // top-right
        corners[3] = position - right * halfW + up * halfH; // top-left

        normals = null; // not needed here
    }

    private static void AddCap(
        Vector3 position,
        Vector3 forward,
        float width,
        float height,
        bool isFront,
        List<Vector3> vertices,
        List<Vector3> normals,
        List<Vector2> uvs,
        List<int> triangles)
    {
        GetCrossSection(position, forward, width, height, out Vector3[] corners, out _);

        int baseVert = vertices.Count;
        Vector3 normal = isFront ? -forward : forward;

        for (int i = 0; i < 4; i++)
        {
            vertices.Add(corners[i]);
            normals.Add(normal);
        }

        // Own 0-1 UV island for the cap
        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(1, 0));
        uvs.Add(new Vector2(1, 1));
        uvs.Add(new Vector2(0, 1));

        if (isFront)
        {
            triangles.Add(baseVert + 0);
            triangles.Add(baseVert + 2);
            triangles.Add(baseVert + 1);

            triangles.Add(baseVert + 0);
            triangles.Add(baseVert + 3);
            triangles.Add(baseVert + 2);
        }
        else
        {
            triangles.Add(baseVert + 0);
            triangles.Add(baseVert + 1);
            triangles.Add(baseVert + 2);

            triangles.Add(baseVert + 0);
            triangles.Add(baseVert + 2);
            triangles.Add(baseVert + 3);
        }
    }


















    public static Vector3[] ToVector3(Vector2[] old)
    {
        Vector3[] toReturn = new Vector3[old.Length];

        for (int i = 0; i < old.Length; i++)
        {
            toReturn[i] = new Vector3(old[i].x, 0, old[i].y);
        }

        return toReturn;
    }

    public static AltMesh ToAlt(Mesh m)
    {
        AltMesh result = new AltMesh();

        result.vertices = m.vertices;
        result.normals = m.normals;
        result.uvs = m.uv;
        result.indices = m.triangles;

        return result;
    }
    public static Vector3[] CopyVectors(Vector3[] input)
    {
        Vector3[] result = new Vector3[input.Length];

        for (int i = 0; i < result.Length; i++)
        {
            result[i] = input[i];
        }

        return result;
    }

    public static float DistanceInsidePolygon(Vector3[] points1, Vector3 point1)
    {
        Vector3 point = new Vector3(point1.x, 0, point1.z);
        Vector3[] points = CopyVectors(points1);
        
        for (int i =0; i<points.Length; i++)
        {
            points[i] = points[i] - Vector3.up * points[i].y;
        }
        
        bool isInside = IsPointInsidePolygon(points, point);
        if (!isInside) return 0;

        Vector3 vert = point;
        float dist = 999;
        for (int n = 0; n < points.Length; n++)
        {   
            Vector3 dir1;
            Vector3 dir2;

            if (n < points.Length - 1)
            {
                dir1 = vert - points[n];
                dir2 = points[n+1] - points[n];
            } else
            {
                dir1 = vert - points[n];
                dir2 = points[0] - points[n];
            }

            if (Vector3.Dot(dir1, dir2) > 0)
            {
                Vector3 projectedDir = Vector3.Project(dir1, dir2);
                projectedDir = projectedDir.normalized * Mathf.Min(dir2.magnitude, projectedDir.magnitude);

                Vector3 clampedPoint = points[n] + projectedDir;
                clampedPoint = new Vector3(clampedPoint.x, 0, clampedPoint.z);
                vert = new Vector3(vert.x, 0, vert.z);

                float distToLine = Vector3.Distance(clampedPoint, vert);
                if (distToLine < dist) dist = distToLine;
            }
        } 

        return dist;
    }

    
    public static float DistanceToPolygon(Vector3[] points1, Vector3 point1)
    {
        Vector3 point = new Vector3(point1.x, 0, point1.z);
        Vector3[] points = CopyVectors(points1);
        
        for (int i =0; i<points.Length; i++)
        {
            points[i] = points[i] - Vector3.up * points[i].y;
        }
        
        bool isInside = IsPointInsidePolygon(points, point);
        if (isInside) return 0;

        Vector3 vert = point;
        float dist = 999;
        for (int n = 0; n < points.Length; n++)
        {   
            Vector3 dir1;
            Vector3 dir2;

            if (n < points.Length - 1)
            {
                dir1 = vert - points[n];
                dir2 = points[n+1] - points[n];
            } else
            {
                dir1 = vert - points[n];
                dir2 = points[0] - points[n];
            }

            if (Vector3.Dot(dir1, dir2) > 0)
            {
                Vector3 projectedDir = Vector3.Project(dir1, dir2);
                projectedDir = projectedDir.normalized * Mathf.Min(dir2.magnitude, projectedDir.magnitude);

                Vector3 clampedPoint = points[n] + projectedDir;
                clampedPoint = new Vector3(clampedPoint.x, 0, clampedPoint.z);
                vert = new Vector3(vert.x, 0, vert.z);

                float distToLine = Vector3.Distance(clampedPoint, vert);
                if (distToLine < dist) dist = distToLine;
            }
        } 

        return dist;
    }

    public static Vector3 GetMidpoint (Vector3[] points)
    {
        Vector3 m = Vector3.zero;

        for (int i = 0; i < points.Length; i++)
        {
            m+=points[i]/points.Length;
        }

        return m;
    }

    // convex OR concave vvvvv

    public static bool IsPointInsidePolygon(Vector3[] points, Vector3 point)
    {
        if (points == null || points.Length < 3)
            return false;

        bool inside = false;
        int j = points.Length - 1;

        for (int i = 0; i < points.Length; i++)
        {
            Vector3 pi = points[i];
            Vector3 pj = points[j];

            // Check if the ray from 'point' to +∞ on X crosses the edge (pj → pi)
            // We work in the XZ plane (Y is ignored)
            if (((pi.z > point.z) != (pj.z > point.z)) &&          // edge straddles the horizontal ray
                (point.x < (pj.x - pi.x) * (point.z - pi.z) / (pj.z - pi.z + float.Epsilon) + pi.x))
            {
                inside = !inside;
            }

            j = i;
        }

        return inside;
    }


    // convex only vvvv

    public static bool IsPointInsideConvexPolygon(Vector3[] points, Vector3 point)
    {
        int leftCount = 0;
        int rightCount = 0;

        for (int i = 0; i < points.Length; i++)
        {
            Vector3 toNext;
            Vector3 toPoint;
            if (i < points.Length-1)
            {
                toNext = points[i+1]-points[i];
                toNext = new Vector3(toNext.z, 0, -toNext.x);
                toPoint = point-points[i];
            } else
            {
                toNext = points[0]-points[i];
                toNext = new Vector3(toNext.z, 0, -toNext.x);
                toPoint = point-points[i];
            }

            if (Vector3.Dot(toNext, toPoint) < 0)
            {
                leftCount++;
            } else if (Vector3.Dot(toNext, toPoint) > 0) {rightCount++;}
        }

        if (leftCount == 0 || rightCount == 0)
        {
            return true;
        }
        return false;
    }

    public static bool IsPointInsidePolygon(Vector2[] points, Vector2 point)
    {
        int leftCount = 0;
        int rightCount = 0;

        for (int i = 0; i < points.Length; i++)
        {
            Vector2 toNext;
            Vector2 toPoint;
            if (i < points.Length-1)
            {
                toNext = points[i+1]-points[i];
                
                toPoint = point-points[i];
            } else
            {
                toNext = points[0]-points[i];
                
                toPoint = point-points[i];
            }

            if (Vector2.Dot(toNext, toPoint) < 0)
            {
                leftCount++;
            } else if (Vector2.Dot(toNext, toPoint) > 0) {rightCount++;}
        }

        if (leftCount == 0 || rightCount == 0)
        {
            return true;
        }
        return false;
    }

    public static Mesh GeneratePolygonMesh(Vector3[] points, Vector3 normalDirection, float uvScale)
    {
        Mesh result = new Mesh();

        Vector3[] verts = new Vector3[points.Length];
        Vector3[] norms = new Vector3[points.Length];
        Vector2[] uvs = new Vector2[points.Length];

        List<int> tris = new List<int>();

        for (int i = 0; i < points.Length; i++)
        {
            verts[i] = points[i];
            norms[i] = normalDirection;
            uvs[i] = new Vector2(verts[i].x, verts[i].z) * uvScale;

            if (i < points.Length - 2)
            {
                tris.Add(0);
                tris.Add(i+1);
                tris.Add(i+2);
            }
        }

        result.SetVertices(verts);
        result.SetNormals(norms);
        result.SetUVs(0, uvs);
        result.SetTriangles(tris, 0);

        return result;
    }
    public static Mesh GeneratePathMesh(Vector3[] points) // TODO: uvs
    {
        if (points.Length < 2) return null;
        Mesh result = new Mesh();

        float pathWidth = 2f;

        List<Vector3> verts = new List<Vector3>();
        List<Vector3> norms = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        Vector3 up = Vector3.up;
        Vector3 forward = (points[1]-points[0]).normalized;
        Vector3 right = Vector3.Cross(up,forward).normalized;

        float heightModifier = 0.05f;
        float uvScale = 0.15f;

        verts.Add(points[0]+right * pathWidth*-0.5f+Vector3.up * heightModifier);
        norms.Add(Vector3.up);
        uvs.Add(uvScale*new Vector2(verts[verts.Count-1].x,verts[verts.Count-1].z));
        verts.Add(points[0]+right * pathWidth*0.5f+Vector3.up * heightModifier);
        norms.Add(Vector3.up);
        uvs.Add(uvScale*new Vector2(verts[verts.Count-1].x,verts[verts.Count-1].z));

        for (int i = 1; i < points.Length; i++)
        {
            up = Vector3.up;
            forward = (points[i]-points[i-1]).normalized;
            right = Vector3.Cross(up,forward).normalized;

            verts.Add(points[i]+right * pathWidth*-0.5f+Vector3.up * heightModifier);
            
            uvs.Add(uvScale*new Vector2(verts[verts.Count-1].x,verts[verts.Count-1].z));
            verts.Add(points[i]+right * pathWidth*0.5f+Vector3.up * heightModifier);
            norms.Add(Vector3.up);
            norms.Add(Vector3.up);
            uvs.Add(uvScale*new Vector2(verts[verts.Count-1].x,verts[verts.Count-1].z));
                

            tris.Add(verts.Count-1);
            tris.Add(verts.Count-3);
            tris.Add(verts.Count-2);

            tris.Add(verts.Count-2);
            tris.Add(verts.Count-3);
            tris.Add(verts.Count-4);
        }
        

        result.SetVertices(verts.ToArray());
        result.SetNormals(norms.ToArray());
        result.SetUVs(0, uvs.ToArray());
        result.SetTriangles(tris, 0);

        return result;
    }

    public static Mesh CombineMeshes(Mesh a, Mesh b) {
        Mesh combinedMesh = new Mesh();

        combinedMesh.SetVertices(CombineVector3Arrays(a.vertices, b.vertices));
        combinedMesh.SetNormals(CombineVector3Arrays(a.normals, b.normals));
        combinedMesh.SetUVs(0, CombineVector2Arrays(a.uv, b.uv));
        
        int[] triangles = CombineIntArrays(a.triangles, b.triangles);

        for (int i = a.triangles.Length; i < triangles.Length; i++) {
            triangles[i] += a.vertices.Length;
        }

        combinedMesh.SetTriangles(triangles, 0);

        return combinedMesh;
    }

    public static Vector2[] CombineVector2Arrays(Vector2[] first, Vector2[] second)
    {
        Vector2[] result = new Vector2[first.Length + second.Length];

        for (int i = 0; i < result.Length; i++)
        {
            if (i < first.Length)
            {
                result[i] = first[i];
            }
            else
            {
                result[i] = second[i - first.Length];
            }


        }

        return result;
    }

    public static Vector3[] CombineVector3Arrays(Vector3[] first, Vector3[] second) {
        Vector3[] result = new Vector3[first.Length + second.Length];

        for (int i = 0; i < result.Length; i++) {
            if (i < first.Length) {
                result[i] = first[i];
            }
            else {
                result[i] = second[i-first.Length];
            }

            
        }

        return result;
    }

    public static int[] CombineIntArrays(int[] first, int[] second) {
        int[] result = new int[first.Length + second.Length];

        for (int i = 0; i < result.Length; i++) {
            if (i < first.Length) {
                result[i] = first[i];
            }
            else {
                result[i] = second[i-first.Length];
            }

            
        }

        return result;
    }

    public static float DistanceToRect(Transform rect, Vector3 point)
    {
        Vector3 localPoint = rect.InverseTransformPoint(point);
        
        Vector3 clampedPoint = new Vector3(
            Mathf.Clamp(localPoint.x, -0.5f, 0.5f),
            Mathf.Clamp(localPoint.y, -0.5f, 0.5f),
            Mathf.Clamp(localPoint.z, -0.5f, 0.5f));

            Vector3 dist = point - rect.TransformPoint(clampedPoint);

        return new Vector3(dist.x, 0, dist.z).magnitude;
    }

    public static Mesh GeneratePlane(int width, float worldScale, bool isReversed)
    {
        Mesh planeMesh = new Mesh();

        Vector3[] verts = new Vector3[width * width];
        Vector2[] uvs = new Vector2[width * width];
        Vector3[] norms = new Vector3[width * width];

        int[] tris = new int[(width - 1) * (width - 1) * 6];

        float scaleFactor = worldScale / (width-1);

        int triangleIndex = 0;
        for (int x = 0, i = 0; x < width; x++)
        {
            for (int y = 0; y < width; y++, i++)
            {
                verts[i] = new Vector3(x * scaleFactor - worldScale / 2, 0, y * scaleFactor - worldScale / 2);
                uvs[i] = new Vector2(x / (float)width, y / (float)width);

                norms[i] = Vector3.up;

                if (x > 0 && y > 0)
                {
                    if (!isReversed)
                    {
                        tris[triangleIndex] = i;
                        tris[triangleIndex + 1] = i - width - 1;
                        tris[triangleIndex + 2] = i - width;
                        tris[triangleIndex + 3] = i - 1;
                        tris[triangleIndex + 4] = i - width - 1;
                        tris[triangleIndex + 5] = i;
                    }
                    else
                    {
                        tris[triangleIndex] = i;
                        tris[triangleIndex + 1] = i - width;
                        tris[triangleIndex + 2] = i - width - 1;
                        tris[triangleIndex + 3] = i - 1;
                        tris[triangleIndex + 4] = i;
                        tris[triangleIndex + 5] = i - width - 1;
                    }

                    triangleIndex += 6;
                }
            }
        }

        planeMesh.SetVertices(verts);
        planeMesh.SetUVs(0, uvs);
        planeMesh.SetNormals(norms);

        planeMesh.SetTriangles(tris, 0);

        return planeMesh;
    }

    /* RANDOM IMPORTED STUFF FROM SUPPLYRUN */

    public static Vector3[] CreateRectVertices(Vector3 center, Vector3 dims, float headingAngle)
    {
        Vector3[] vertices = new Vector3[24];

        // plus signs added here for clarity

        // bottom face (-y)
        vertices[0] = center + util_math.ApplyRotationMatrix(new Vector3(-dims.x, -dims.y, -dims.z), 1, headingAngle);
        vertices[1] = center + util_math.ApplyRotationMatrix(new Vector3(+dims.x, -dims.y, -dims.z), 1, headingAngle);
        vertices[2] = center + util_math.ApplyRotationMatrix(new Vector3(+dims.x, -dims.y, +dims.z), 1, headingAngle);
        vertices[3] = center + util_math.ApplyRotationMatrix(new Vector3(-dims.x, -dims.y, +dims.z), 1, headingAngle);

        // top face (+y)
        vertices[4] = center + util_math.ApplyRotationMatrix(new Vector3(-dims.x, +dims.y, -dims.z), 1, headingAngle);
        vertices[5] = center + util_math.ApplyRotationMatrix(new Vector3(+dims.x, +dims.y, -dims.z), 1, headingAngle);
        vertices[6] = center + util_math.ApplyRotationMatrix(new Vector3(+dims.x, +dims.y, +dims.z), 1, headingAngle);
        vertices[7] = center + util_math.ApplyRotationMatrix(new Vector3(-dims.x, +dims.y, +dims.z), 1, headingAngle);

        // -x
        vertices[8] = center + util_math.ApplyRotationMatrix(new Vector3(-dims.x, -dims.y, +dims.z), 1, headingAngle);
        vertices[9] = center + util_math.ApplyRotationMatrix(new Vector3(-dims.x, +dims.y, +dims.z), 1, headingAngle);
        vertices[10] = center + util_math.ApplyRotationMatrix(new Vector3(-dims.x, +dims.y, -dims.z), 1, headingAngle);
        vertices[11] = center + util_math.ApplyRotationMatrix(new Vector3(-dims.x, -dims.y, -dims.z), 1, headingAngle);

        // +x
        vertices[12] = center + util_math.ApplyRotationMatrix(new Vector3(+dims.x, -dims.y, -dims.z), 1, headingAngle);
        vertices[13] = center + util_math.ApplyRotationMatrix(new Vector3(+dims.x, +dims.y, -dims.z), 1, headingAngle);
        vertices[14] = center + util_math.ApplyRotationMatrix(new Vector3(+dims.x, +dims.y, +dims.z), 1, headingAngle);
        vertices[15] = center + util_math.ApplyRotationMatrix(new Vector3(+dims.x, -dims.y, +dims.z), 1, headingAngle);

        // -z
        vertices[16] = center + util_math.ApplyRotationMatrix(new Vector3(-dims.x, -dims.y, -dims.z), 1, headingAngle);
        vertices[17] = center + util_math.ApplyRotationMatrix(new Vector3(-dims.x, +dims.y, -dims.z), 1, headingAngle);
        vertices[18] = center + util_math.ApplyRotationMatrix(new Vector3(+dims.x, +dims.y, -dims.z), 1, headingAngle);
        vertices[19] = center + util_math.ApplyRotationMatrix(new Vector3(+dims.x, -dims.y, -dims.z), 1, headingAngle);

        // +z
        vertices[20] = center + util_math.ApplyRotationMatrix(new Vector3(-dims.x, -dims.y, +dims.z), 1, headingAngle);
        vertices[21] = center + util_math.ApplyRotationMatrix(new Vector3(+dims.x, -dims.y, +dims.z), 1, headingAngle);
        vertices[22] = center + util_math.ApplyRotationMatrix(new Vector3(+dims.x, +dims.y, +dims.z), 1, headingAngle);
        vertices[23] = center + util_math.ApplyRotationMatrix(new Vector3(-dims.x, +dims.y, +dims.z), 1, headingAngle);

        return vertices;
    }

    // // a line in this case means a rectangle thats been extruded and follows some points
    // public static Mesh CreateLine(Vector3[] points, float width) {

    //     // this will be whats returned
    //     Mesh finalMesh = new Mesh();

    //     List<Vector3> vertices = new List<Vector3>();
    //     List<Vector3> normals = new List<Vector3>();
    //     List<int> triangles = new List<int>();

    //     for (int i = 0; i < points.Length - 1; i++) {
    //         // what I'm doing is rotating the forward vector 90 degrees and -90 degrees around the y axis, (PI / 2 radians)
    //         // to obtain two perpindicular and opposite vectors

    //         // inside rail side panels
    //         vertices.Add(points[i] + Vector3.forward * -width / 2f + -Vector3.right * width / 2f);
    //         vertices.Add(points[i] + Vector3.forward * -width / 2f + Vector3.right * width / 2f);
    //         vertices.Add(points[i+1] + Vector3.forward * -width / 2f + -Vector3.right * width / 2f);
    //         vertices.Add(points[i+1] + Vector3.forward * -width / 2f + Vector3.right * width / 2f);

    //         triangles.Add(vertices.Count - 4);
    //         triangles.Add(vertices.Count - 3);
    //         triangles.Add(vertices.Count - 1);
    //         triangles.Add(vertices.Count - 2);
    //         triangles.Add(vertices.Count - 4);
    //         triangles.Add(vertices.Count - 1);

    //         for (int j = 0; j < 4; j++) {normals.Add(-Vector3.forward);}

    //         // outside rail side panels
    //         vertices.Add(points[i] + Vector3.forward * width / 2f + -Vector3.right * width / 2f);
    //         vertices.Add(points[i] + Vector3.forward * width / 2f + Vector3.right * width / 2f);
    //         vertices.Add(points[i+1] + Vector3.forward * width / 2f + -Vector3.right * width / 2f);
    //         vertices.Add(points[i+1] + Vector3.forward * width / 2f + Vector3.right * width / 2f);
            
    //         triangles.Add(vertices.Count - 1);
    //         triangles.Add(vertices.Count - 3);
    //         triangles.Add(vertices.Count - 4);
    //         triangles.Add(vertices.Count - 1);
    //         triangles.Add(vertices.Count - 4);
    //         triangles.Add(vertices.Count - 2);

    //         for (int j = 0; j < 4; j++) {normals.Add(Vector3.forward);}

    //         // top rail panels
    //         vertices.Add(points[i] + Vector3.forward * width / 2f + Vector3.right * width / 2f);
    //         vertices.Add(points[i+1] + Vector3.forward * width / 2f + Vector3.right * width / 2f);
    //         vertices.Add(points[i] + Vector3.forward * -width / 2f + Vector3.right * width / 2f);
    //         vertices.Add(points[i+1] + Vector3.forward * -width / 2f + Vector3.right * width / 2f);

    //         triangles.Add(vertices.Count - 4);
    //         triangles.Add(vertices.Count - 3);
    //         triangles.Add(vertices.Count - 1);
    //         triangles.Add(vertices.Count - 2);
    //         triangles.Add(vertices.Count - 4);
    //         triangles.Add(vertices.Count - 1);

    //         for (int j = 0; j < 4; j++) {normals.Add(Vector3.up);}

    //         // bottom rail panels
    //         vertices.Add(points[i] + Vector3.forward * width / 2f + -Vector3.right * width / 2f);
    //         vertices.Add(points[i+1] + Vector3.forward * width / 2f + -Vector3.right * width / 2f);
    //         vertices.Add(points[i] + Vector3.forward * -width / 2f + -Vector3.right * width / 2f);
    //         vertices.Add(points[i+1] + Vector3.forward * -width / 2f + -Vector3.right * width / 2f);

    //         triangles.Add(vertices.Count - 1);
    //         triangles.Add(vertices.Count - 3);
    //         triangles.Add(vertices.Count - 4);
    //         triangles.Add(vertices.Count - 1);
    //         triangles.Add(vertices.Count - 4);
    //         triangles.Add(vertices.Count - 2);

    //         for (int j = 0; j < 4; j++) {normals.Add(-Vector3.up);}
    //     }

    //     // rail UVs
    //     Vector2[] uv = new Vector2[vertices.Count];
    //     // make all of the uvs one right now bc I don't care
    //     for (int i = 0; i < uv.Length; i++) {
    //         uv[i] = Vector2.one;
    //     }

    //     // assigning everything
    //     finalMesh.SetVertices(vertices);
    //     finalMesh.SetNormals(normals);
    //     finalMesh.SetUVs(0, uv);
    //     finalMesh.SetTriangles(triangles, 0);

    //     return finalMesh;
    // }
    
    // TODO: stop copying all this damn logic

    // create the mesh of a rectangular prism
    // only rotation in the y axis is supported
    public static Mesh CreateRect(Vector3 center, Vector3 dims, float headingAngle)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = CreateRectVertices(center, dims, headingAngle);

        Vector2[] uv = new Vector2[24];
        // make all of the uvs one right now bc I don't care
        for (int i = 0; i < 8; i++)
        {
            uv[i] = Vector2.one;
        }

        Vector3[] normals = new Vector3[24];
        // normals of each vertex point away from the center
        for (int i = 0; i < 24; i++)
        {
            if (i < 4)
            {
                normals[i] = util_math.ApplyRotationMatrix(-Vector3.up, 1, headingAngle);
            }
            else if (i < 8)
            {
                normals[i] = util_math.ApplyRotationMatrix(Vector3.up, 1, headingAngle);
            }
            else if (i < 12)
            {
                normals[i] = util_math.ApplyRotationMatrix(-Vector3.right, 1, headingAngle);
            }
            else if (i < 16)
            {
                normals[i] = util_math.ApplyRotationMatrix(Vector3.right, 1, headingAngle);
            }
            else if (i < 20)
            {
                normals[i] = util_math.ApplyRotationMatrix(-Vector3.forward, 1, headingAngle);
            }
            else if (i < 24)
            {
                normals[i] = util_math.ApplyRotationMatrix(Vector3.forward, 1, headingAngle);
            }
        }

        int[] triangles = new int[0];

        // bottom face (-y)
        triangles = CombineIntArrays(triangles, GetTrianglesForPlane(0, 1, 2, 3));
        // top face (+y)
        triangles = CombineIntArrays(triangles, GetTrianglesForPlane(4, 7, 6, 5));
        // -x face
        triangles = CombineIntArrays(triangles, GetTrianglesForPlane(8, 9, 10, 11));
        // +x face
        triangles = CombineIntArrays(triangles, GetTrianglesForPlane(12, 13, 14, 15));
        // -z face
        triangles = CombineIntArrays(triangles, GetTrianglesForPlane(16, 17, 18, 19));
        // +z face
        triangles = CombineIntArrays(triangles, GetTrianglesForPlane(20, 21, 22, 23));

        mesh.SetVertices(vertices);
        mesh.SetNormals(normals);
        mesh.SetUVs(0, uv);
        mesh.SetTriangles(triangles, 0);

        return mesh;
    }

    public static int[] GetTrianglesForPlane(int p1, int p2, int p3, int p4) {
        int[] triangles = new int[6];

        triangles[0] = p1;
        triangles[1] = p2;
        triangles[2] = p3;
        triangles[3] = p3;
        triangles[4] = p4;
        triangles[5] = p1;

        return triangles;
    }
}
