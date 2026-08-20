using UnityEngine;

public class util_array
{
    public static string[] Combine(string[] a, string[] b)
    {
        string[] result = new string[a.Length +b.Length];

        for (int i = 0; i < a.Length; i++)
        {
            result[i] = a[i];
        }
        for (int i = a.Length; i < b.Length + a.Length; i++)
        {
            result[i] = b[i-a.Length];
        }

        return result;
    }
    public static int[] Combine(int[] a, int[] b)
    {
        int[] result = new int[a.Length +b.Length];

        for (int i = 0; i < a.Length; i++)
        {
            result[i] = a[i];
        }
        for (int i = a.Length; i < b.Length + a.Length; i++)
        {
            result[i] = b[i-a.Length];
        }

        return result;
    }
    public static double[] Combine(double[] a, double[] b)
    {
        double[] result = new double[a.Length +b.Length];

        for (int i = 0; i < a.Length; i++)
        {
            result[i] = a[i];
        }
        for (int i = a.Length; i < b.Length + a.Length; i++)
        {
            result[i] = b[i-a.Length];
        }

        return result;
    }
    public static Vector2[] Combine(Vector2[] a, Vector2[] b)
    {
        Vector2[] result = new Vector2[a.Length +b.Length];

        for (int i = 0; i < a.Length; i++)
        {
            result[i] = a[i];
        }
        for (int i = a.Length; i < b.Length + a.Length; i++)
        {
            result[i] = b[i-a.Length];
        }

        return result;
    }
    public static Vector3[] Combine(Vector3[] a, Vector3[] b)
    {
        Vector3[] result = new Vector3[a.Length +b.Length];

        for (int i = 0; i < a.Length; i++)
        {
            result[i] = a[i];
        }
        for (int i = a.Length; i < b.Length + a.Length; i++)
        {
            result[i] = b[i-a.Length];
        }

        return result;
    }
    public static bool[] Combine(bool[] a, bool[] b)
    {
        bool[] result = new bool[a.Length +b.Length];

        for (int i = 0; i < a.Length; i++)
        {
            result[i] = a[i];
        }
        for (int i = a.Length; i < b.Length + a.Length; i++)
        {
            result[i] = b[i-a.Length];
        }

        return result;
    }


    



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
