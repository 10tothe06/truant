using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class test_lakevertices : MonoBehaviour
{
    public float noise_amp;
    [Header("Spine & Dimensions")]
    public int vertexCount = 90;
    public float length = 30f;
    public float maxWidth = 10f;

    [Header("Character")]
    [Range(0.15f, 1.2f)] public float meander = 0.7f;
    [Range(0.2f, 1.2f)] public float widthVariation = 0.75f;
    public int seed = 98765;

    [Header("Visual")]
    public Color lineColor = new Color(0.1f, 0.4f, 0.8f, 0.95f);
    public Color pointColor = new Color(1f, 1f, 1f, 0.65f);
    public float pointSize = 0.16f;

    private Vector2[] vertices;
    public List<int> bad_lines;
    private double lastHash;

    public NoiseProfile prof;

    void TryGenerate()
    {
        double hash = vertexCount + seed + length.GetHashCode() + maxWidth.GetHashCode() +
                   meander.GetHashCode() + widthVariation.GetHashCode() + noise_amp;
        //if (hash == lastHash && vertices != null) return;

        vertices = AddNoiseToVertices(LakeVertexGenerator2.Generate(
            vertexCount, length, maxWidth, meander, widthVariation, seed));

        CheckForBadVertices();
            
        lastHash = hash;
    }

    private void CheckForBadVertices()
    {
        bad_lines =  new List<int>();
        bad_lines.Clear();

        Vector3 origin = transform.position;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 a1 = origin + (Vector3)vertices[i];
            Vector3 a2 = origin + (Vector3)vertices[(i + 1) % vertices.Length];

            for (int j = 0; j < vertices.Length; j++)
            {
                if (j == i) {continue;}

                Vector3 b1 = origin + (Vector3)vertices[j];
                Vector3 b2 = origin + (Vector3)vertices[(j + 1) % vertices.Length];

                if (SegmentsIntersect(a1, a2, b1, b2))
                {
                    bad_lines.Add(i);
                }
            }
        }
    }
    


    public static bool SegmentsIntersect(Vector3 a1, Vector3 a2, Vector3 b1, Vector3 b2)
    {
        Vector3 dirA = a2 - a1;
        Vector3 dirB = b2 - b1;

        // Direction of the line connecting the two start points
        Vector3 dirAB = b1 - a1;

        Vector3 crossAB = Vector3.Cross(dirA, dirB);
        float crossSqrMag = crossAB.sqrMagnitude;

        // Lines are parallel (or nearly parallel)
        if (crossSqrMag < 1e-8f)
            return false;

        // Check if the lines are coplanar
        float planarFactor = Vector3.Dot(dirAB, crossAB);
        if (Mathf.Abs(planarFactor) > 1e-5f)
            return false; // Not coplanar → cannot intersect in 3D

        // Calculate the intersection parameters
        Vector3 crossABandB = Vector3.Cross(dirAB, dirB);
        float t = Vector3.Dot(crossABandB, crossAB) / crossSqrMag;

        Vector3 crossABandA = Vector3.Cross(dirAB, dirA);
        float u = Vector3.Dot(crossABandA, crossAB) / crossSqrMag;

        // Check if the intersection point lies on both segments
        // (using a small epsilon for floating-point tolerance)
        const float epsilon = -0.01f;
        return t >= -epsilon && t <= 1f + epsilon &&
               u >= -epsilon && u <= 1f + epsilon;
    }




    void OnDrawGizmos()
    {
        TryGenerate(); // keep live in editor

        Vector3 origin = transform.position;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 a = origin + (Vector3)vertices[i];
            Vector3 b = origin + (Vector3)vertices[(i + 1) % vertices.Length];

            if (bad_lines.Contains(i))
            {
                Gizmos.color = Color.red;
            } else
            {
                Gizmos.color = lineColor;
            }

            Gizmos.DrawLine(a, b);
        }

        Gizmos.color = pointColor;
        for (int i =0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(origin + (Vector3)vertices[i], pointSize);
        }  
    }

    [ContextMenu("Randomize Seed")]
    void Randomize()
    {
        seed = Random.Range(0, 999999);
        lastHash = 0;
        TryGenerate();
    }



    private Vector2[] AddNoiseToVertices(Vector2[] vertices)
    {
        List<Vector2> toReturn = new List<Vector2>();

        int count_per_segment = 10;
        for (int n = 1; n < vertices.Length; n++)
        {
            for (int i = 1; i < count_per_segment; i++)
            {
                Vector2 new_v = Vector2.Lerp(vertices[n-1], vertices[n], i/(float)(count_per_segment-1));

                toReturn.Add(new_v);
            }
        }

        Vector2[] v = new Vector2[toReturn.Count];

        for (int i = 0; i < toReturn.Count; i++)
        {
            Vector3 normal = Vector3.zero;

            if (i > 0)
            {
                normal = Vector3.Cross(toReturn[i]-toReturn[i-1], Vector3.forward);
            } else
            {
                normal = Vector3.Cross(toReturn[i+1]-toReturn[i], Vector3.forward);
            }
            

            v[i] = toReturn[i] + new Vector2(normal.x, normal.y).normalized * prof.GetHeight(new Vector3(toReturn[i].x, toReturn[i].y, 0)) * noise_amp;
        }

        return v;
    }







    public static Vector2[] Generate(
        int vertexCount = 80,
        float length = 28f,
        float maxWidth = 9f,
        float meander = 0.65f,
        float widthVariation = 0.7f,
        int seed = 42)
    {
        Random.InitState(seed);

        // -------------------------------------------------
        // 1. Build a meandering spine (open polyline)
        // -------------------------------------------------
        int spinePoints = Mathf.Clamp(Mathf.RoundToInt(length / 2.5f), 6, 18);
        List<Vector2> spine = BuildMeanderingSpine(spinePoints, length, meander);

        // Densify the spine so width sampling is smooth
        spine = DensifyOpen(spine, Mathf.Max(spinePoints * 3, 24));

        // -------------------------------------------------
        // 2. Compute varying half-width at every spine point
        // -------------------------------------------------
        float[] halfWidths = new float[spine.Count];
        for (int i = 0; i < spine.Count; i++)
        {
            float t = i / (float)(spine.Count - 1); // 0 → 1 along spine

            // Base profile: wider in the middle, tapers toward both ends
            float profile = Mathf.Sin(t * Mathf.PI);               // 0 at ends, 1 in middle
            profile = Mathf.Pow(profile, Random.Range(0.7f, 1.4f)); // vary the fullness

            // Large-scale width modulation
            float large = 1f + (Mathf.PerlinNoise(t * 3.5f + seed * 0.1f, 17.3f) - 0.5f) * 2f * widthVariation;

            // Medium detail
            float medium = 1f + (Mathf.PerlinNoise(t * 9f + 40f, seed * 0.17f) - 0.5f) * 1.4f * widthVariation;

            halfWidths[i] = (maxWidth * 0.5f) * profile * large * medium;

            // Occasional local widening / narrowing (bays / pinches)
            if (Random.value < 0.12f * widthVariation)
                halfWidths[i] *= Random.Range(0.45f, 0.7f);
            else if (Random.value < 0.10f * widthVariation)
                halfWidths[i] *= Random.Range(1.25f, 1.6f);

            halfWidths[i] = Mathf.Max(halfWidths[i], maxWidth * 0.08f);
        }

        // Light smoothing of the width array so we don't get sudden spikes
        halfWidths = SmoothArray(halfWidths, 2);

        // -------------------------------------------------
        // 3. Extrude left and right sides
        // -------------------------------------------------
        List<Vector2> left = new List<Vector2>();
        List<Vector2> right = new List<Vector2>();

        for (int i = 0; i < spine.Count; i++)
        {
            Vector2 tangent;
            if (i == 0)
                tangent = (spine[1] - spine[0]).normalized;
            else if (i == spine.Count - 1)
                tangent = (spine[i] - spine[i - 1]).normalized;
            else
                tangent = (spine[i + 1] - spine[i - 1]).normalized;

            Vector2 normal = new Vector2(-tangent.y, tangent.x); // perpendicular

            left.Add(spine[i] + normal * halfWidths[i]);
            right.Add(spine[i] - normal * halfWidths[i]);
        }

        // -------------------------------------------------
        // 4. Build closed ring: left side → end cap → reversed right side → start cap
        // -------------------------------------------------
        List<Vector2> ring = new List<Vector2>();

        // Left side (start → end)
        ring.AddRange(left);

        // Simple rounded end cap at the far end
        AddEndCap(ring, left[left.Count - 1], right[right.Count - 1], spine[spine.Count - 1], true);

        // Right side reversed (end → start)
        for (int i = right.Count - 1; i >= 0; i--)
            ring.Add(right[i]);

        // Start cap
        AddEndCap(ring, right[0], left[0], spine[0], false);

        // -------------------------------------------------
        // 5. Final light smoothing + ensure clockwise + resample
        // -------------------------------------------------
        for (int s = 0; s < 2; s++)
            ring = SmoothClosed(ring, 0.28f);

        if (!IsClockwise(ring))
            ring.Reverse();

        return ResampleClosed(ring, vertexCount);
    }

    public static List<Vector2> GenerateList(
        int vertexCount = 80,
        float length = 28f,
        float maxWidth = 9f,
        float meander = 0.65f,
        float widthVariation = 0.7f,
        int seed = 42)
    {
        return new List<Vector2>(Generate(vertexCount, length, maxWidth, meander, widthVariation, seed));
    }

    // ------------------------------------------------------------------
    // Helpers
    // ------------------------------------------------------------------

    private static List<Vector2> BuildMeanderingSpine(int count, float length, float meander)
    {
        List<Vector2> pts = new List<Vector2>(count);
        Vector2 pos = Vector2.zero;
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float step = length / (count - 1);

        pts.Add(pos);

        float turnPersistence = 0f; // gives the meander some "memory"

        for (int i = 1; i < count; i++)
        {
            // Persistent turning force + noise
            turnPersistence += (Random.value - 0.5f) * 1.8f * meander;
            turnPersistence = Mathf.Clamp(turnPersistence, -1.4f * meander, 1.4f * meander);

            float turn = turnPersistence + (Random.value - 0.5f) * 0.9f * meander;
            angle += turn;

            pos += new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * step;
            pts.Add(pos);
        }

        // Center the whole spine around origin
        Vector2 centroid = Vector2.zero;
        foreach (var p in pts) centroid += p;
        centroid /= pts.Count;
        for (int i = 0; i < pts.Count; i++)
            pts[i] -= centroid;

        return pts;
    }

    private static List<Vector2> DensifyOpen(List<Vector2> src, int target)
    {
        if (src.Count >= target) return new List<Vector2>(src);

        // Cumulative length
        float total = 0f;
        float[] cum = new float[src.Count];
        cum[0] = 0f;
        for (int i = 1; i < src.Count; i++)
        {
            total += Vector2.Distance(src[i - 1], src[i]);
            cum[i] = total;
        }

        List<Vector2> dst = new List<Vector2>(target);
        float step = total / (target - 1);

        for (int i = 0; i < target; i++)
        {
            float d = i * step;
            int seg = 0;
            while (seg < src.Count - 2 && cum[seg + 1] < d) seg++;

            float segLen = cum[seg + 1] - cum[seg];
            float t = segLen > 1e-5f ? (d - cum[seg]) / segLen : 0f;
            dst.Add(Vector2.Lerp(src[seg], src[seg + 1], t));
        }
        return dst;
    }

    private static void AddEndCap(List<Vector2> ring, Vector2 from, Vector2 to, Vector2 center, bool farEnd)
    {
        // Simple semicircle-ish cap using a few points
        Vector2 dir = (to - from).normalized;
        Vector2 outward = farEnd
            ? ((from + to) * 0.5f - center).normalized
            : (center - (from + to) * 0.5f).normalized;

        // If the outward calculation is degenerate, fall back
        if (outward.sqrMagnitude < 0.01f)
            outward = new Vector2(-dir.y, dir.x);

        int capPoints = 5;
        for (int i = 1; i < capPoints; i++)
        {
            float t = i / (float)capPoints;
            // Lerp from → to while bulging outward
            Vector2 p = Vector2.Lerp(from, to, t);
            float bulge = Mathf.Sin(t * Mathf.PI) * Vector2.Distance(from, to) * 0.35f;
            p += outward * bulge;
            ring.Add(p);
        }
    }

    private static float[] SmoothArray(float[] src, int passes)
    {
        float[] a = (float[])src.Clone();
        for (int p = 0; p < passes; p++)
        {
            float[] b = new float[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                float prev = a[Mathf.Max(i - 1, 0)];
                float next = a[Mathf.Min(i + 1, a.Length - 1)];
                b[i] = a[i] * 0.5f + (prev + next) * 0.25f;
            }
            a = b;
        }
        return a;
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
