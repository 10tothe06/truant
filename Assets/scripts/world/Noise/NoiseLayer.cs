using UnityEngine;

public enum NoiseLayerType
{
    Perlin,
    Constant,
}


[System.Serializable]
public class NoiseLayer
{
    public ushort layer_type;
    public float frequency;
    public float amplitude;

    public float value_adjustment;
    
    // applied before frequency
    public Vector3 offset;

    #region CONSTRUCTORS

    public NoiseLayer() {}

    public NoiseLayer(float value_adjustment)
    {
        layer_type = (ushort)NoiseLayerType.Constant;

        this.value_adjustment = value_adjustment;
    }

    // constructor where you pass in every value
    public NoiseLayer(ushort layer_type, float frequency, float amplitude, float value_adjustment, Vector3 offset)
    {
        this.layer_type = layer_type;

        this.frequency = frequency;
        this.amplitude = amplitude;

        this.offset = offset;

        this.value_adjustment = value_adjustment;
    }
    public NoiseLayer(ushort layer_type, float frequency, float amplitude, float value_adjustment)
    {
        this.layer_type = layer_type;

        this.frequency = frequency;
        this.amplitude = amplitude;

        this.offset = Vector3.zero;

        this.value_adjustment = value_adjustment;
    }
    public NoiseLayer(ushort layer_type, float frequency, float amplitude, Vector3 offset)
    {
        this.layer_type = layer_type;

        this.frequency = frequency;
        this.amplitude = amplitude;

        this.offset = offset;

        this.value_adjustment = 0;
    }
    public NoiseLayer(ushort layer_type, float frequency, float amplitude)
    {
        this.layer_type = layer_type;

        this.frequency = frequency;
        this.amplitude = amplitude;

        this.offset = Vector3.zero;

        this.value_adjustment = 0;
    }

    #endregion






    #region HELPERS

    // ***
    // THESE SHOULD BE USED INSTEAD OF THE RAW CONSTRUCTORS
    // ***

    public static NoiseLayer ConstantLayer(float value)
    {
        return new NoiseLayer(value);
    }



    public static NoiseLayer PerlinLayer(float frequency, float amplitude)
    {
        return new NoiseLayer((ushort)NoiseLayerType.Perlin, frequency, amplitude, 0, Vector3.zero);
    }
    public static NoiseLayer PerlinLayer(float frequency, float amplitude, float value_adjustment)
    {
        return new NoiseLayer((ushort)NoiseLayerType.Perlin, frequency, amplitude, value_adjustment, Vector3.zero);
    }
    public static NoiseLayer PerlinLayer(float frequency, float amplitude, float value_adjustment, Vector3 offset)
    {
        return new NoiseLayer((ushort)NoiseLayerType.Perlin, frequency, amplitude, value_adjustment, offset);
    }


    #endregion
    



    

    public float GetValueAt(Vector3 point)
    {
        if (layer_type == (ushort)NoiseLayerType.Perlin)
        {
            Vector3 p = (point + offset) * frequency;
            return Perlin.Noise(p.x, p.y, p.z) * amplitude + value_adjustment;
        } else if (layer_type == (ushort)NoiseLayerType.Constant)
        {
            return value_adjustment;
        }

        // should never be getting here
        return 0;
    }
}
