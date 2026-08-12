using UnityEngine;

public class util_array
{
    public static double[] FillArrayWith(double array_item, int array_length)
    {
        double[] result=  new double[array_length];

        for (int i = 0; i <result.Length; i++)
        {
            result[i] = array_item;
        }

        return result;
    }
    public static float[] FillArrayWith(float array_item, int array_length)
    {
        float[] result=  new float[array_length];

        for (int i = 0; i <result.Length; i++)
        {
            result[i] = array_item;
        }

        return result;
    }
    public static int[] FillArrayWith(int array_item, int array_length)
    {
        int[] result=  new int[array_length];

        for (int i = 0; i <result.Length; i++)
        {
            result[i] = array_item;
        }

        return result;
    }

    public static Vector3[] FillArrayWith(Vector3 array_item, int array_length)
    {
        Vector3[] result=  new Vector3[array_length];

        for (int i = 0; i <result.Length; i++)
        {
            result[i] = array_item;
        }

        return result;
    }

    public static Vector2[] FillArrayWith(Vector2 array_item, int array_length)
    {
        Vector2[] result=  new Vector2[array_length];

        for (int i = 0; i <result.Length; i++)
        {
            result[i] = array_item;
        }

        return result;
    }
}
