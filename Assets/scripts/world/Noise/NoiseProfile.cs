using UnityEngine;

// because it was so helpful as a way to universally store data in the past,
// I'm bringing back an updated version of the idea of a 'noise profile'

[System.Serializable]
public class NoiseProfile
{
    public NoiseLayer[] layers;
    public float noise_range = 20f;

    #region CONSTRUCTORS

    public NoiseProfile()
    {
        // default, test information
        layers = new NoiseLayer[]
        {
            NoiseLayer.PerlinLayer(0.03f, 4f),
           NoiseLayer.PerlinLayer(0.3f, 1f),
        };

        noise_range = 20f;
    }

    public NoiseProfile(NoiseLayer[] layers)
    {
        this.layers = layers;
        noise_range = 20f;
    }

    #endregion




    public static NoiseProfile Contstant(float constant_value)
    {
        return new NoiseProfile(new NoiseLayer[] {NoiseLayer.ConstantLayer(constant_value)});
    }


    public Color[] GenerateTextureData(int resolution, Vector3 center_position)
    {
        Color[] colors = new Color[resolution * resolution];

        for (int i = 0; i < colors.Length; i++)
        {
            float x = (i % resolution) / (float)resolution;
            float y = 0;
            float z = (i / resolution) / (float)resolution;

            //colors[i] = new Color(x, z, 0, 1f);

            colors[i] = new Color(GetHeight(center_position + new Vector3(x * WorldManager.Instance.chunkSize,y,z * WorldManager.Instance.chunkSize)) / noise_range + 0.5f, 0f, 0, 0);
        }

        return colors;
    }

    public Texture2D GenerateTexture(int resolution, Vector3 center_position)
    {
        Texture2D toReturn = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false, true);

        Color[] colors = GenerateTextureData(resolution, center_position);

        toReturn.SetPixels(colors);
        toReturn.Apply(false, false);
        toReturn.filterMode = FilterMode.Point;

        return toReturn;
    }

    public float GetHeight(Vector3 position)
    {
        float height = 0;

        for (int i = 0; i < layers.Length; i++)
        {
            height += layers[i].GetValueAt(position);
        }

        return height;
    }
}
