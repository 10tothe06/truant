using UnityEngine;

public class util_geometry
{

    /// <summary>
    /// Returns the shortest distance from a point to a polyline (line made of consecutive segments).
    /// </summary>
    /// <param name="linePoints">Array of points defining the line (must have at least 2 points).</param>
    /// <param name="testPoint">The point to measure distance from.</param>
    /// <returns>Minimum distance to any segment of the line. Returns float.MaxValue if the array is invalid.</returns>
    public static float DistanceToLine(Vector3[] linePoints, Vector3 testPoint)
    {
        if (linePoints == null || linePoints.Length < 2)
            return float.MaxValue;

        float minDistance = float.MaxValue;

        for (int i = 0; i < linePoints.Length - 1; i++)
        {
            float dist = DistancePointToSegment(testPoint, linePoints[i], linePoints[i + 1]);
            if (dist < minDistance)
                minDistance = dist;
        }

        return minDistance;
    }

    /// <summary>
    /// Distance from a point to a line segment (clamped).
    /// </summary>
    private static float DistancePointToSegment(Vector3 point, Vector3 a, Vector3 b)
    {
        Vector3 ab = b - a;
        float abSqrMagnitude = ab.sqrMagnitude;

        // Degenerate segment (a == b)
        if (abSqrMagnitude < Mathf.Epsilon)
            return Vector3.Distance(point, a);

        // Project point onto the infinite line, then clamp to the segment
        float t = Vector3.Dot(point - a, ab) / abSqrMagnitude;
        t = Mathf.Clamp01(t);

        Vector3 closestPoint = a + t * ab;
        return Vector3.Distance(point, closestPoint);
    }




    
    public static Vector3[] ScaleVertices(Vector3[] raw, float scale_factor)
    {
        for (int i = 0; i < raw.Length; i++)
        {
            raw[i] *= scale_factor;
        }

        return raw;
    }

    public static Vector3[] ScaleVerticesByConstant(Vector3[] raw, float constant_scale_factor)
    {
        for (int i = 0; i < raw.Length; i++)
        {
            raw[i] += raw[i].normalized * constant_scale_factor;
        }

        return raw;
    }



    public static Vector2[] Vector3ToVector2(Vector3[] raw)
    {
        Vector2[] result = new Vector2[raw.Length];

        for (int i = 0; i < raw.Length; i++)
        {
            result[i] = new Vector2(raw[i].x, raw[i].z);
        }

        return result;
    }
    public static Vector3[] Vector2ToVector3(Vector2[] raw)
    {
        Vector3[] result = new Vector3[raw.Length];

        for (int i = 0; i < raw.Length; i++)
        {
            result[i] = new Vector3(raw[i].x, 0, raw[i].y);
        }

        return result;
    }
}
