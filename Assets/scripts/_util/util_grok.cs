using System.Collections.Generic;
using UnityEngine;

public class util_grok
{
    /// <summary>
    /// Triangulates a simple polygon (convex or concave) given in counter-clockwise order.
    /// Returns an int[] of triangle indices suitable for Mesh.triangles
    /// (groups of 3 indices into the original vertices array).
    /// Does NOT support self-intersecting polygons or holes.
    /// </summary>
    public static int[] Triangulate(Vector2[] vertices)
    {
        if (vertices == null || vertices.Length < 3)
            return new int[0];

        int n = vertices.Length;
        if (n == 3)
            return new int[] { 0, 1, 2 };

        // Working list of vertex indices (we remove ear tips from this)
        var indices = new List<int>(n);
        for (int i = 0; i < n; i++)
            indices.Add(i);

        var triangles = new List<int>((n - 2) * 3);

        // Ear-clipping loop
        while (indices.Count > 3)
        {
            bool earFound = false;

            for (int i = 0; i < indices.Count; i++)
            {
                int prev = indices[(i - 1 + indices.Count) % indices.Count];
                int curr = indices[i];
                int next = indices[(i + 1) % indices.Count];

                if (IsEar(vertices, indices, prev, curr, next))
                {
                    // Record the triangle (CCW order)
                    triangles.Add(prev);
                    triangles.Add(next);
                    triangles.Add(curr);

                    // Clip the ear tip
                    indices.RemoveAt(i);
                    earFound = true;
                    break;
                }
            }

            // Safety: if no ear was found the polygon is likely self-intersecting
            // or has numerical issues. Bail out to avoid infinite loop.
            if (!earFound)
            {
                Debug.LogWarning("PolygonTriangulator: Could not find an ear. " +
                                 "Polygon may be self-intersecting or degenerate.");
                break;
            }
        }

        // Final remaining triangle
        if (indices.Count == 3)
        {
            triangles.Add(indices[0]);
            triangles.Add(indices[2]);
            triangles.Add(indices[1]);
        }

        return triangles.ToArray();
    }

    // ------------------------------------------------------------------
    // Helpers
    // ------------------------------------------------------------------

    private static bool IsEar(Vector2[] verts, List<int> indices, int prev, int curr, int next)
    {
        // Must be a convex vertex (for CCW polygon the cross product is positive)
        if (!IsConvex(verts[prev], verts[curr], verts[next]))
            return false;

        // No other remaining vertices may lie inside the triangle
        for (int i = 0; i < indices.Count; i++)
        {
            int idx = indices[i];
            if (idx == prev || idx == curr || idx == next)
                continue;

            if (PointInTriangle(verts[idx], verts[prev], verts[curr], verts[next]))
                return false;
        }

        return true;
    }

    private static bool IsConvex(Vector2 a, Vector2 b, Vector2 c)
    {
        // Cross product (b-a) × (c-b). Positive for left turn (CCW).
        float cross = (b.x - a.x) * (c.y - b.y) - (b.y - a.y) * (c.x - b.x);
        return cross > 1e-8f;   // small epsilon to reject near-collinear points
    }

    private static bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
    {
        // Barycentric technique (same orientation test as IsConvex)
        float d1 = Sign(p, a, b);
        float d2 = Sign(p, b, c);
        float d3 = Sign(p, c, a);

        bool hasNeg = (d1 < 0) || (d2 < 0) || (d3 < 0);
        bool hasPos = (d1 > 0) || (d2 > 0) || (d3 > 0);

        // Strict interior (points exactly on an edge are considered outside
        // so we don't reject valid ears that share an edge)
        return !(hasNeg && hasPos);
    }

    private static float Sign(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
    }
}
