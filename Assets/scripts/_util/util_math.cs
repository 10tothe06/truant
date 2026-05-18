using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class util_math : MonoBehaviour
{
    public static Vector3 RotateVector(Vector3 vector, Vector3 axis, float angle) {
        Vector3 rotated = new Vector3(0,0,0);
        rotated.x = vector.x * (     (axis.x * axis.x) * (1 - Mathf.Cos(angle)) + Mathf.Cos(angle)                  ) +   vector.y * (        (axis.y * axis.x) * (1 - Mathf.Cos(angle)) - (axis.z * Mathf.Sin(angle))         ) + vector.z * (        (axis.z * axis.x) * (1 - Mathf.Cos(angle)) + (axis.y * Mathf.Sin(angle))     );
        rotated.y = vector.x * (     (axis.x * axis.y) * (1 - Mathf.Cos(angle)) + (axis.z * Mathf.Sin(angle))       ) +   vector.y * (        (axis.y * axis.y) * (1 - Mathf.Cos(angle)) + Mathf.Cos(angle)                    ) + vector.z * (        (axis.z * axis.y) * (1 - Mathf.Cos(angle)) - (axis.x * Mathf.Sin(angle))     );
        rotated.z = vector.x * (     (axis.x * axis.z) * (1 - Mathf.Cos(angle)) - (axis.y * Mathf.Sin(angle))       ) +   vector.y * (        (axis.y * axis.z) * (1 - Mathf.Cos(angle)) + (axis.x * Mathf.Sin(angle))         ) + vector.z * (        (axis.z * axis.z) * (1 - Mathf.Cos(angle)) + Mathf.Cos(angle)                );

        return rotated;
    }

    // input angle in RADIANS
    // for the axis variable x is 0, y is 1, z is 2
    public static Vector3 ApplyRotationMatrix(Vector3 a, int axis, float theta) {
        float sinHeading = Mathf.Sin(theta);
        float cosHeading = Mathf.Cos(theta);

        // right now this ONLY WORKS FOR ROTATION ON THE Y AXIS
        if (axis == 1) {
            return new Vector3(a.x * cosHeading + a.z * -sinHeading, a.y, a.x * sinHeading + a.z * cosHeading);
        } else {
            return a;
        }
    }

    public static int[] RandomIndices(int length)
    {
        List<int> pool = new List<int>();
        for (int i = 0; i < length; i++) {pool.Add(i);}

        int[] result = new int[length];
        for (int i = 0; i < result.Length; i++)
        {
            int poolIndex = Random.Range(0, pool.Count);
            result[i] = pool[poolIndex];
            pool.RemoveAt(poolIndex);
        }

        return result;
    }
}
