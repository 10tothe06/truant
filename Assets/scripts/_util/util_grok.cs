using System.Collections.Generic;
using UnityEngine;

public class util_grok
{
    /// <summary>
    /// Cuts a mesh by a plane and returns two fully triangulated meshes.
    /// Positive side = side the normal points toward.
    /// </summary>
    public static (Mesh positive, Mesh negative) Cut(
        Mesh source,
        Vector3 planeNormal,
        Vector3 planePoint,
        bool addCap = true)
    {
        if (source == null || source.vertexCount == 0)
            return (null, null);

        planeNormal = planeNormal.normalized;

        float SignedDistance(Vector3 p) => Vector3.Dot(planeNormal, p - planePoint);

        var posVerts   = new List<Vector3>();
        var posNormals = new List<Vector3>();
        var posUVs     = new List<Vector2>();
        var posTris    = new List<int>();

        var negVerts   = new List<Vector3>();
        var negNormals = new List<Vector3>();
        var negUVs     = new List<Vector2>();
        var negTris    = new List<int>();

        var intersectionPoints = new List<Vector3>();

        Vector3[] verts   = source.vertices;
        Vector3[] normals = source.normals;
        Vector2[] uvs     = source.uv;
        int[]     tris    = source.triangles;

        bool hasNormals = normals != null && normals.Length == verts.Length;
        bool hasUVs     = uvs     != null && uvs.Length     == verts.Length;

        var edgeCache = new Dictionary<(int, int), int>();

        for (int t = 0; t < tris.Length; t += 3)
        {
            int i0 = tris[t], i1 = tris[t + 1], i2 = tris[t + 2];

            Vector3 v0 = verts[i0], v1 = verts[i1], v2 = verts[i2];
            float d0 = SignedDistance(v0);
            float d1 = SignedDistance(v1);
            float d2 = SignedDistance(v2);

            int s0 = d0 >  1e-5f ? 1 : (d0 < -1e-5f ? -1 : 0);
            int s1 = d1 >  1e-5f ? 1 : (d1 < -1e-5f ? -1 : 0);
            int s2 = d2 >  1e-5f ? 1 : (d2 < -1e-5f ? -1 : 0);

            (int posIdx, int negIdx) AddVertex(Vector3 pos, Vector3 norm, Vector2 uv)
            {
                int p = posVerts.Count;
                posVerts.Add(pos);
                if (hasNormals) posNormals.Add(norm);
                if (hasUVs)     posUVs.Add(uv);

                int n = negVerts.Count;
                negVerts.Add(pos);
                if (hasNormals) negNormals.Add(norm);
                if (hasUVs)     negUVs.Add(uv);

                return (p, n);
            }

            (int posIdx, int negIdx) GetIntersection(int ea, int eb, float da, float db)
            {
                var key = ea < eb ? (ea, eb) : (eb, ea);
                if (edgeCache.TryGetValue(key, out int cached))
                    return (cached, cached);

                float tt = da / (da - db);
                Vector3 pos = Vector3.Lerp(verts[ea], verts[eb], tt);
                Vector3 nrm = hasNormals ? Vector3.Lerp(normals[ea], normals[eb], tt).normalized : planeNormal;
                Vector2 uv  = hasUVs     ? Vector2.Lerp(uvs[ea], uvs[eb], tt) : Vector2.zero;

                var (p, n) = AddVertex(pos, nrm, uv);
                edgeCache[key] = p;
                intersectionPoints.Add(pos);
                return (p, n);
            }

            // ---------- all vertices on the same side ----------
            if (s0 == s1 && s1 == s2)
            {
                if (s0 >= 0)
                {
                    int baseIdx = posVerts.Count;
                    posVerts.Add(v0); posVerts.Add(v1); posVerts.Add(v2);
                    if (hasNormals) { posNormals.Add(normals[i0]); posNormals.Add(normals[i1]); posNormals.Add(normals[i2]); }
                    if (hasUVs)     { posUVs.Add(uvs[i0]);         posUVs.Add(uvs[i1]);         posUVs.Add(uvs[i2]); }
                    posTris.Add(baseIdx); posTris.Add(baseIdx + 1); posTris.Add(baseIdx + 2);
                }
                if (s0 <= 0)
                {
                    int baseIdx = negVerts.Count;
                    negVerts.Add(v0); negVerts.Add(v1); negVerts.Add(v2);
                    if (hasNormals) { negNormals.Add(normals[i0]); negNormals.Add(normals[i1]); negNormals.Add(normals[i2]); }
                    if (hasUVs)     { negUVs.Add(uvs[i0]);         negUVs.Add(uvs[i1]);         negUVs.Add(uvs[i2]); }
                    negTris.Add(baseIdx); negTris.Add(baseIdx + 1); negTris.Add(baseIdx + 2);
                }
                continue;
            }

            // ---------- straddling triangle ----------
            void EmitTri(bool toPositive,
                         Vector3 va, Vector3 vb, Vector3 vc,
                         Vector3 na, Vector3 nb, Vector3 nc,
                         Vector2 uva, Vector2 uvb, Vector2 uvc)
            {
                if (toPositive)
                {
                    int baseIdx = posVerts.Count;
                    posVerts.Add(va); posVerts.Add(vb); posVerts.Add(vc);
                    if (hasNormals) { posNormals.Add(na); posNormals.Add(nb); posNormals.Add(nc); }
                    if (hasUVs)     { posUVs.Add(uva);   posUVs.Add(uvb);   posUVs.Add(uvc); }
                    posTris.Add(baseIdx); posTris.Add(baseIdx + 1); posTris.Add(baseIdx + 2);
                }
                else
                {
                    int baseIdx = negVerts.Count;
                    negVerts.Add(va); negVerts.Add(vb); negVerts.Add(vc);
                    if (hasNormals) { negNormals.Add(na); negNormals.Add(nb); negNormals.Add(nc); }
                    if (hasUVs)     { negUVs.Add(uva);   negUVs.Add(uvb);   negUVs.Add(uvc); }
                    negTris.Add(baseIdx); negTris.Add(baseIdx + 1); negTris.Add(baseIdx + 2);
                }
            }

            int lonely = (s0 != s1 && s0 != s2) ? 0 : (s1 != s0 && s1 != s2) ? 1 : 2;
            int idxA = lonely;
            int idxB = (lonely + 1) % 3;
            int idxC = (lonely + 2) % 3;

            int[] idx = { i0, i1, i2 };
            Vector3[] v = { v0, v1, v2 };
            float[] d = { d0, d1, d2 };
            int[] s = { s0, s1, s2 };

            var (abPos, abNeg) = GetIntersection(idx[idxA], idx[idxB], d[idxA], d[idxB]);
            var (acPos, acNeg) = GetIntersection(idx[idxA], idx[idxC], d[idxA], d[idxC]);

            bool lonelyPositive = s[idxA] > 0;

            // Lonely side → single triangle
            EmitTri(lonelyPositive,
                    v[idxA], posVerts[abPos], posVerts[acPos],
                    hasNormals ? normals[idx[idxA]] : Vector3.zero,
                    hasNormals ? posNormals[abPos] : Vector3.zero,
                    hasNormals ? posNormals[acPos] : Vector3.zero,
                    hasUVs ? uvs[idx[idxA]] : Vector2.zero,
                    hasUVs ? posUVs[abPos] : Vector2.zero,
                    hasUVs ? posUVs[acPos] : Vector2.zero);

            // Other side → quad (two triangles)
            bool otherPositive = !lonelyPositive;

            EmitTri(otherPositive,
                    posVerts[abPos], v[idxB], v[idxC],
                    hasNormals ? posNormals[abPos] : Vector3.zero,
                    hasNormals ? normals[idx[idxB]] : Vector3.zero,
                    hasNormals ? normals[idx[idxC]] : Vector3.zero,
                    hasUVs ? posUVs[abPos] : Vector2.zero,
                    hasUVs ? uvs[idx[idxB]] : Vector2.zero,
                    hasUVs ? uvs[idx[idxC]] : Vector2.zero);

            EmitTri(otherPositive,
                    posVerts[abPos], v[idxC], posVerts[acPos],
                    hasNormals ? posNormals[abPos] : Vector3.zero,
                    hasNormals ? normals[idx[idxC]] : Vector3.zero,
                    hasNormals ? posNormals[acPos] : Vector3.zero,
                    hasUVs ? posUVs[abPos] : Vector2.zero,
                    hasUVs ? uvs[idx[idxC]] : Vector2.zero,
                    hasUVs ? posUVs[acPos] : Vector2.zero);
        }

        // ---------- optional cap (centroid fan) ----------
        if (addCap && intersectionPoints.Count >= 3)
        {
            var unique = new List<Vector3>();
            const float eps = 1e-4f;
            foreach (var p in intersectionPoints)
            {
                bool found = false;
                foreach (var u in unique)
                    if ((u - p).sqrMagnitude < eps * eps) { found = true; break; }
                if (!found) unique.Add(p);
            }

            if (unique.Count >= 3)
            {
                Vector3 centroid = Vector3.zero;
                foreach (var p in unique) centroid += p;
                centroid /= unique.Count;

                int posCent = posVerts.Count;
                posVerts.Add(centroid);
                if (hasNormals) posNormals.Add(planeNormal);
                if (hasUVs)     posUVs.Add(Vector2.zero);

                int negCent = negVerts.Count;
                negVerts.Add(centroid);
                if (hasNormals) negNormals.Add(-planeNormal);
                if (hasUVs)     negUVs.Add(Vector2.zero);

                Vector3 refDir = Vector3.Cross(planeNormal, unique[0] - centroid).normalized;
                if (refDir.sqrMagnitude < 1e-6f)
                    refDir = Vector3.Cross(planeNormal, Vector3.right).normalized;

                unique.Sort((pa, pb) =>
                {
                    Vector3 da = pa - centroid, db = pb - centroid;
                    float angA = Mathf.Atan2(Vector3.Dot(Vector3.Cross(refDir, da), planeNormal), Vector3.Dot(refDir, da));
                    float angB = Mathf.Atan2(Vector3.Dot(Vector3.Cross(refDir, db), planeNormal), Vector3.Dot(refDir, db));
                    return angA.CompareTo(angB);
                });

                for (int i = 0; i < unique.Count; i++)
                {
                    Vector3 pa = unique[i];
                    Vector3 pb = unique[(i + 1) % unique.Count];

                    // Positive side
                    int ia = posVerts.Count; posVerts.Add(pa);
                    int ib = posVerts.Count; posVerts.Add(pb);
                    if (hasNormals) { posNormals.Add(planeNormal); posNormals.Add(planeNormal); }
                    if (hasUVs)     { posUVs.Add(Vector2.zero);   posUVs.Add(Vector2.zero); }
                    posTris.Add(posCent);  posTris.Add(ib); posTris.Add(ia);

                    // Negative side (reversed winding)
                    int na = negVerts.Count; negVerts.Add(pa);
                    int nb = negVerts.Count; negVerts.Add(pb);
                    if (hasNormals) { negNormals.Add(-planeNormal); negNormals.Add(-planeNormal); }
                    if (hasUVs)     { negUVs.Add(Vector2.zero);    negUVs.Add(Vector2.zero); }
                    negTris.Add(negCent);  negTris.Add(na); negTris.Add(nb);
                }
            }
        }

        // ---------- build meshes ----------
        Mesh Build(List<Vector3> vList, List<Vector3> nList, List<Vector2> uvList, List<int> tList)
        {
            if (tList.Count == 0) return null;
            var m = new Mesh();
            m.indexFormat = vList.Count > 65535
                ? UnityEngine.Rendering.IndexFormat.UInt32
                : UnityEngine.Rendering.IndexFormat.UInt16;
            m.SetVertices(vList);
            if (hasNormals && nList.Count == vList.Count) m.SetNormals(nList);
            if (hasUVs     && uvList.Count == vList.Count) m.SetUVs(0, uvList);
            m.SetTriangles(tList, 0);
            m.RecalculateBounds();
            if (!hasNormals) m.RecalculateNormals();
            return m;
        }

        Mesh positive = Build(posVerts, posNormals, posUVs, posTris);
        Mesh negative = Build(negVerts, negNormals, negUVs, negTris);

        return (positive, negative);
    }




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
