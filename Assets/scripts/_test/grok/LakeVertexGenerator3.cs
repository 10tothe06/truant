using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Fundamentally different approach: Union of irregularly placed circles.
/// 
/// We place several circles of varying size in a non-circular arrangement,
/// keep only the outer surface points (points not inside any other circle),
/// sort them angularly around the centroid, then smooth and resample.
/// 
/// This naturally produces peanut shapes, elongated multi-lobed lakes,
/// lumpy irregular outlines, etc. — far from simple perturbed circles.
/// Includes a basic self-intersection safety net with retries.
/// </summary>
public static class LakeVertexGenerator3
{
    public static Vector2[] Generate(
        int vertexCount = 80,
        float size = 18f,
        float irregularity = 0.75f,   // 0.4 = milder, 1.0 = wilder placement & size variation
        int seed = 42,
        int maxAttempts = 5)
    {
        Random.InitState(seed);

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            float attemptIrregularity = irregularity * (1f - attempt * 0.07f);
            Vector2[] result = GenerateOnce(vertexCount, size, attemptIrregularity, seed + attempt * 5501);

            if (result != null && result.Length >= 12 && !SelfIntersects(result))
                return result;
        }

        // Safe fallback: fewer, more tightly packed circles
        return GenerateOnce(vertexCount, size, irregularity * 0.45f, seed + 12345);
    }

    public static List<Vector2> GenerateList(
        int vertexCount = 80,
        float size = 18f,
        float irregularity = 0.75f,
        int seed = 42)
    {
        return new List<Vector2>(Generate(vertexCount, size, irregularity, seed));
    }

    // ------------------------------------------------------------------
    // Single generation attempt
    // ------------------------------------------------------------------
    private static Vector2[] GenerateOnce(int vertexCount, float size, float irregularity, int seed)
    {
        Random.InitState(seed);

        // ----- 1. Decide how many circles and place them irregularly -----
        int circleCount = Random.Range(3, 7); // 3–6 circles
        List<Vector2> centers = new List<Vector2>();
        List<float> radii = new List<float>();

        // First circle near origin
        centers.Add(Random.insideUnitCircle * size * 0.15f);
        radii.Add(size * Random.Range(0.35f, 0.65f));

        for (int i = 1; i < circleCount; i++)
        {
            // Place subsequent centers with a bias toward elongation + scatter
            Vector2 dir = Random.insideUnitCircle.normalized;
            // Stretch the distribution so the overall cloud is rarely round
            dir.x *= Random.Range(1f, 5f);
            dir.y *= Random.Range(0.6f, 1.4f);

            float dist = size * Random.Range(0.25f, 0.7f) * (0.6f + irregularity);
            Vector2 candidate = centers[Random.Range(0, centers.Count)] + dir * dist;

            // Mild separation so they overlap but don't completely coincide
            bool ok = true;
            for (int j = 0; j < centers.Count; j++)
            {
                if (Vector2.Distance(candidate, centers[j]) < (radii[j] + size * 0.15f) * 0.45f)
                {
                    ok = false;
                    break;
                }
            }
            if (!ok)
            {
                i--; // retry this circle
                continue;
            }

            centers.Add(candidate);
            float r = size * Random.Range(0.28f, 0.7f) * Random.Range(0.75f, 1.25f);
            // Occasional dominant circle or tiny one
            if (Random.value < 0.2f * irregularity) r *= Random.Range(1.3f, 1.7f);
            if (Random.value < 0.15f * irregularity) r *= Random.Range(0.45f, 0.7f);
            radii.Add(r);
        }

        // ----- 2. Sample points on every circle, keep only exterior ones -----
        List<Vector2> candidates = new List<Vector2>();
        int samplesPerCircle = 24 + Mathf.RoundToInt(18 * irregularity);

        for (int c = 0; c < centers.Count; c++)
        {
            for (int s = 0; s < samplesPerCircle; s++)
            {
                float angle = (s / (float)samplesPerCircle) * Mathf.PI * 2f;
                // Slight radius noise so the final edge isn't perfectly circular arcs
                float rad = radii[c] * (1f + (Random.value - 0.5f) * 0.08f);
                Vector2 p = centers[c] + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * rad;

                // Keep only if not inside any other circle
                bool insideOther = false;
                for (int o = 0; o < centers.Count; o++)
                {
                    if (o == c) continue;
                    if (Vector2.Distance(p, centers[o]) < radii[o] * 0.97f)
                    {
                        insideOther = true;
                        break;
                    }
                }
                if (!insideOther)
                    candidates.Add(p);
            }
        }

        if (candidates.Count < 12)
            return null;

        // ----- 3. Sort by angle around centroid (produces a simple closed loop) -----
        Vector2 centroid = Vector2.zero;
        foreach (var p in candidates) centroid += p;
        centroid /= candidates.Count;

        candidates.Sort((a, b) =>
        {
            float angA = Mathf.Atan2(a.y - centroid.y, a.x - centroid.x);
            float angB = Mathf.Atan2(b.y - centroid.y, b.x - centroid.x);
            return angA.CompareTo(angB);
        });

        // Remove consecutive points that are extremely close
        List<Vector2> cleaned = new List<Vector2>();
        cleaned.Add(candidates[0]);
        for (int i = 1; i < candidates.Count; i++)
        {
            if (Vector2.Distance(candidates[i], cleaned[cleaned.Count - 1]) > size * 0.02f)
                cleaned.Add(candidates[i]);
        }
        // Ensure closing doesn't duplicate
        if (Vector2.Distance(cleaned[0], cleaned[cleaned.Count - 1]) < size * 0.02f)
            cleaned.RemoveAt(cleaned.Count - 1);

        if (cleaned.Count < 12)
            return null;

        // ----- 4. Light smoothing + ensure clockwise + resample -----
        for (int s = 0; s < 3; s++)
            cleaned = SmoothClosed(cleaned, 0.32f);

        if (!IsClockwise(cleaned))
            cleaned.Reverse();

        return ResampleClosed(cleaned, vertexCount);
    }

    // ------------------------------------------------------------------
    // Self-intersection test
    // ------------------------------------------------------------------
    private static bool SelfIntersects(Vector2[] poly)
    {
        int n = poly.Length;
        for (int i = 0; i < n; i++)
        {
            Vector2 a1 = poly[i];
            Vector2 a2 = poly[(i + 1) % n];

            for (int j = i + 2; j < n; j++)
            {
                if (i == 0 && j == n - 1) continue;

                Vector2 b1 = poly[j];
                Vector2 b2 = poly[(j + 1) % n];

                if (SegmentsIntersect(a1, a2, b1, b2))
                    return true;
            }
        }
        return false;
    }

    private static bool SegmentsIntersect(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2)
    {
        float d1 = Cross(q2 - q1, p1 - q1);
        float d2 = Cross(q2 - q1, p2 - q1);
        float d3 = Cross(p2 - p1, q1 - p1);
        float d4 = Cross(p2 - p1, q2 - p1);

        return ((d1 > 0 && d2 < 0) || (d1 < 0 && d2 > 0)) &&
               ((d3 > 0 && d4 < 0) || (d3 < 0 && d4 > 0));
    }

    private static float Cross(Vector2 a, Vector2 b) => a.x * b.y - a.y * b.x;

    // ------------------------------------------------------------------
    // Helpers
    // ------------------------------------------------------------------
    private static List<Vector2> SmoothClosed(List<Vector2> src, float amount)
    {
        int n = src.Count;
        List<Vector2> dst = new List<Vector2>(n);
        for (int i = 0; i < n; i++)
        {
            Vector2 prev = src[(i - 1 + n) % n];
            Vector2 curr = src[i];
            Vector2 next = src[(i + 1) % n];
            dst.Add(curr * (1f - amount) + (prev + next) * (amount * 0.5f));
        }
        return dst;
    }

    private static bool IsClockwise(List<Vector2> pts)
    {
        float area = 0f;
        for (int i = 0; i < pts.Count; i++)
        {
            Vector2 a = pts[i];
            Vector2 b = pts[(i + 1) % pts.Count];
            area += (b.x - a.x) * (b.y + a.y);
        }
        return area > 0f;
    }

    private static Vector2[] ResampleClosed(List<Vector2> src, int targetCount)
    {
        if (src.Count == targetCount) return src.ToArray();

        float totalLen = 0f;
        float[] cum = new float[src.Count + 1];
        for (int i = 0; i < src.Count; i++)
        {
            totalLen += Vector2.Distance(src[i], src[(i + 1) % src.Count]);
            cum[i + 1] = totalLen;
        }

        Vector2[] result = new Vector2[targetCount];
        float step = totalLen / targetCount;

        for (int i = 0; i < targetCount; i++)
        {
            float d = i * step;
            int seg = 0;
            while (seg < src.Count - 1 && cum[seg + 1] < d) seg++;

            float segLen = cum[seg + 1] - cum[seg];
            float t = segLen > 1e-5f ? (d - cum[seg]) / segLen : 0f;
            result[i] = Vector2.Lerp(src[seg], src[(seg + 1) % src.Count], t);
        }
        return result;
    }
}