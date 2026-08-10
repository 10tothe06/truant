using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Generates realistic, varied lake shorelines that are NOT just noisy / stretched circles.
/// 
/// Method: Start with a strongly irregular simple polygon (random control points with
/// non-uniform angles + high radius variance), then apply multiple generations of
/// closed midpoint displacement. This creates natural bays, peninsulas and asymmetry.
/// Final light smoothing keeps the shape clean while preserving character.
/// Vertices are returned in clockwise order.
/// </summary>
public static class LakeVertexGenerator
{
    /// <param name="vertexCount">Approximate final number of vertices (will be close to a power of 2 from the iterations)</param>
    /// <param name="size">Rough overall size of the lake</param>
    /// <param name="complexity">How wild the shape is (0.3 = gentle, 1.0 = very irregular with deep bays)</param>
    /// <param name="seed">Random seed</param>
    public static Vector2[] Generate(
        int vertexCount = 64,
        float size = 15f,
        float complexity = 0.7f,
        int seed = 42)
    {
        Random.InitState(seed);

        // -------------------------------------------------
        // 1. Create a strongly non-circular base polygon
        // -------------------------------------------------
        int controlCount = Mathf.Clamp(Random.Range(7, 13), 6, 14);
        List<Vector2> points = CreateIrregularBasePolygon(controlCount, size, complexity);

        // -------------------------------------------------
        // 2. Closed midpoint displacement (the key step)
        // -------------------------------------------------
        // We keep subdividing until we are at or above the desired vertex count
        int generations = 0;
        while (points.Count < vertexCount && generations < 6)
        {
            points = MidpointDisplaceClosed(points, complexity, generations);
            generations++;
        }

        // -------------------------------------------------
        // 3. Optional final light smoothing (preserves shape, removes jaggedness)
        // -------------------------------------------------
        for (int i = 0; i < 2; i++)
            points = SmoothClosed(points, 0.35f);

        // -------------------------------------------------
        // 4. Ensure clockwise winding
        // -------------------------------------------------
        if (!IsClockwise(points))
            points.Reverse();

        // -------------------------------------------------
        // 5. Resample to exact desired count (uniform-ish arc length)
        // -------------------------------------------------
        return ResampleClosed(points, vertexCount);
    }

    public static List<Vector2> GenerateList(
        int vertexCount = 64,
        float size = 15f,
        float complexity = 0.7f,
        int seed = 42)
    {
        return new List<Vector2>(Generate(vertexCount, size, complexity, seed));
    }

    // ------------------------------------------------------------------
    // Core helpers
    // ------------------------------------------------------------------

    private static List<Vector2> CreateIrregularBasePolygon(int count, float size, float complexity)
    {
        List<Vector2> pts = new List<Vector2>(count);

        // Non-uniform angular spacing + strong radius variation
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float[] radii = new float[count];

        for (int i = 0; i < count; i++)
        {
            // Strongly varying radius – this is what kills the "circle" look
            float r = size * Random.Range(0.35f, 1.15f);
            // Occasional deep bay / peninsula
            if (Random.value < 0.25f * complexity)
                r *= Random.Range(0.4f, 0.7f);
            else if (Random.value < 0.2f * complexity)
                r *= Random.Range(1.15f, 1.45f);

            radii[i] = r;
        }

        // Light smoothing of the radius array so we don't get completely chaotic spikes
        for (int pass = 0; pass < 2; pass++)
        {
            float[] smoothed = new float[count];
            for (int i = 0; i < count; i++)
            {
                float prev = radii[(i - 1 + count) % count];
                float next = radii[(i + 1) % count];
                smoothed[i] = radii[i] * 0.5f + (prev + next) * 0.25f;
            }
            radii = smoothed;
        }

        for (int i = 0; i < count; i++)
        {
            // Variable step size – clusters points and creates longer straightish shores
            float step = (Mathf.PI * 2f / count) * Random.Range(0.55f, 1.55f);
            angle += step;

            float r = radii[i];
            pts.Add(new Vector2(Mathf.Cos(angle) * r, Mathf.Sin(angle) * r));
        }

        return pts;
    }

    private static List<Vector2> MidpointDisplaceClosed(List<Vector2> src, float complexity, int generation)
    {
        List<Vector2> dst = new List<Vector2>(src.Count * 2);
        float maxDisplace = (0.55f * complexity) / (generation + 1.1f); // falls off each generation

        for (int i = 0; i < src.Count; i++)
        {
            Vector2 a = src[i];
            Vector2 b = src[(i + 1) % src.Count];

            dst.Add(a);

            Vector2 mid = (a + b) * 0.5f;
            Vector2 edge = b - a;
            Vector2 normal = new Vector2(-edge.y, edge.x).normalized; // perpendicular

            // Random displacement along the normal (can go in or out)
            float displace = (Random.value * 2f - 1f) * edge.magnitude * maxDisplace;
            mid += normal * displace;

            dst.Add(mid);
        }

        return dst;
    }

    private static List<Vector2> SmoothClosed(List<Vector2> src, float amount)
    {
        int n = src.Count;
        List<Vector2> dst = new List<Vector2>(n);

        for (int i = 0; i < n; i++)
        {
            Vector2 prev = src[(i - 1 + n) % n];
            Vector2 curr = src[i];
            Vector2 next = src[(i + 1) % n];

            Vector2 smoothed = curr * (1f - amount) + (prev + next) * (amount * 0.5f);
            dst.Add(smoothed);
        }
        return dst;
    }

    private static bool IsClockwise(List<Vector2> pts)
    {
        // Signed area
        float area = 0f;
        for (int i = 0; i < pts.Count; i++)
        {
            Vector2 a = pts[i];
            Vector2 b = pts[(i + 1) % pts.Count];
            area += (b.x - a.x) * (b.y + a.y);
        }
        return area > 0f; // positive = clockwise in Unity's Y-up 2D
    }

    private static Vector2[] ResampleClosed(List<Vector2> src, int targetCount)
    {
        if (src.Count == targetCount)
            return src.ToArray();

        // Compute cumulative lengths
        float totalLen = 0f;
        float[] cum = new float[src.Count + 1];
        for (int i = 0; i < src.Count; i++)
        {
            Vector2 a = src[i];
            Vector2 b = src[(i + 1) % src.Count];
            float len = Vector2.Distance(a, b);
            totalLen += len;
            cum[i + 1] = totalLen;
        }

        Vector2[] result = new Vector2[targetCount];
        float step = totalLen / targetCount;

        for (int i = 0; i < targetCount; i++)
        {
            float d = i * step;
            // Find segment
            int seg = 0;
            while (seg < src.Count - 1 && cum[seg + 1] < d)
                seg++;

            float segStart = cum[seg];
            float segLen = cum[seg + 1] - segStart;
            float t = segLen > 0.0001f ? (d - segStart) / segLen : 0f;

            Vector2 a = src[seg];
            Vector2 b = src[(seg + 1) % src.Count];
            result[i] = Vector2.Lerp(a, b, t);
        }

        return result;
    }
}