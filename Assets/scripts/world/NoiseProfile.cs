using UnityEngine;

// because it was so helpful as a way to universally store data in the past,
// I'm bringing back an updated version of the idea of a 'noise profile'

[System.Serializable]
public class NoiseProfile : MonoBehaviour
{
    public NoiseLayer[] layers;

    public NoiseProfile()
    {
        // default, test information
        layers = new NoiseLayer[]
        {
            new NoiseLayer(0.03f, 4f, Vector3.zero),
            new NoiseLayer(0.3f, 1f, Vector3.zero),
        };
    }

    public float GetHeight(Vector3 position)
    {
        float height = 0;

        for (int i = 0; i < layers.Length; i++)
        {
            Vector3 p = (position + layers[i].offset) * layers[i].frequency;
            height += Perlin.Noise(p.x, p.y, p.z) * layers[i].amplitude;
        }

        return height;
    }
}
