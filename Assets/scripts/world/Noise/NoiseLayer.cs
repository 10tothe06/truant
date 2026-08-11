using UnityEngine;


[System.Serializable]
public class NoiseLayer
{
    public float frequency;
    public float amplitude;
    
    // applied before frequency
    public Vector3 offset;

    public NoiseLayer() {}

    public NoiseLayer(float frequency, float amplitude, Vector3 offset)
    {
        this.frequency = frequency;
        this.amplitude = amplitude;

        this.offset = offset;
    }
}
