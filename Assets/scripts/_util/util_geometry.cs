using UnityEngine;

public class util_geometry
{
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
