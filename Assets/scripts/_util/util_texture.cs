using UnityEngine;

// unlike util_map,
// which is specifically for THIS game

// util_texture will contain generic functions for Texture2Ds,
//  that can be used for any project



// ***
// this script is brought to you by Grok
// ***

public class util_texture 
{
    public static Texture2D ScaleNearestNeighbour(Texture2D source, float scale)
    {
        int newWidth  = Mathf.Max(1, Mathf.RoundToInt(source.width  * scale));
        int newHeight = Mathf.Max(1, Mathf.RoundToInt(source.height * scale));
        return ScaleNearestNeighbour(source, newWidth, newHeight);
    }
    /// <summary>
    /// Scales a Texture2D up or down using nearest-neighbour sampling.
    /// Returns a new Texture2D (the original is left unchanged).
    /// 
    /// Requirements:
    /// - The source texture must have Read/Write Enabled in its import settings.
    /// </summary>
    public static Texture2D ScaleNearestNeighbour(Texture2D source, int newWidth, int newHeight)
    {
        if (source == null)
            throw new System.ArgumentNullException(nameof(source));

        if (newWidth <= 0 || newHeight <= 0)
            throw new System.ArgumentException("New dimensions must be greater than zero.");

        // Preserve format and whether the original had mipmaps
        Texture2D result = new Texture2D(newWidth, newHeight, source.format, source.mipmapCount > 1);

        Color32[] srcPixels = source.GetPixels32();
        Color32[] dstPixels = new Color32[newWidth * newHeight];

        float xRatio = (float)source.width  / newWidth;
        float yRatio = (float)source.height / newHeight;

        for (int y = 0; y < newHeight; y++)
        {
            // Nearest neighbour: floor the mapped coordinate
            int srcY = Mathf.Clamp(Mathf.FloorToInt(y * yRatio), 0, source.height - 1);

            for (int x = 0; x < newWidth; x++)
            {
                int srcX = Mathf.Clamp(Mathf.FloorToInt(x * xRatio), 0, source.width - 1);
                dstPixels[y * newWidth + x] = srcPixels[srcY * source.width + srcX];
            }
        }

        result.SetPixels32(dstPixels);
        result.Apply(true);          // regenerate mipmaps if the texture has them
        return result;
    }
    

    // ***
    // my (old) attempt at a texture scaling function
    // ****

    // public static Texture2D ScaleTexture(Texture2D old_texture, float scale_factor)
    // {
    //     int new_size_x = Mathf.RoundToInt(old_texture.width * scale_factor);
    //     int new_size_y  = Mathf.RoundToInt(old_texture.height * scale_factor);

    //     Texture2D result = new Texture2D(new_size_x, new_size_y, TextureFormat.RGBA32, false, true);

    //     Color[] colors = new Color[new_size_x * new_size_y];

    //     for (int y = 0, i = 0; y < new_size_x; y++)
    //     {
    //         for (int x = 0; x < new_size_y; x++, i++)
    //         {
    //             colors[i] = Color.red;
    //         }
    //     }

    //     result.SetPixels(colors);
    //     result.Apply(false, false);
    //     result.filterMode = FilterMode.Point;

    //     return result;
    // }

    public static Texture2D WriteTextureOnTop(Texture2D baseTexture, Vector2 pixelCoords, Texture2D overlayTexture, float overlay_scale = 1f)
    {
        if (baseTexture == null || overlayTexture == null)
        {
            Debug.LogError("WriteTextureOnTop: one or both textures are null.");
            return null;
        }

        // Ensure we can read the pixels
        if (!baseTexture.isReadable || !overlayTexture.isReadable)
        {
            Debug.LogError("WriteTextureOnTop: both textures must be readable (Read/Write Enabled in import settings).");
            return null;
        }

        int baseW = baseTexture.width;
        int baseH = baseTexture.height;
        int overlayW = Mathf.RoundToInt(overlayTexture.width * overlay_scale);
        int overlayH = Mathf.RoundToInt(overlayTexture.height * overlay_scale);

        // Create a working copy so we don't mutate the original
        Texture2D result = new Texture2D(baseW, baseH, baseTexture.format, baseTexture.mipmapCount > 1);
        result.SetPixels32(baseTexture.GetPixels32());
        result.Apply(false);

        Color32[] basePixels = result.GetPixels32();
        Color32[] overlayPixels = new Color32[]{};

        if (overlay_scale == 1)
        {
            overlayPixels = overlayTexture.GetPixels32();
        } else
        {
            overlayPixels  = ScaleNearestNeighbour(overlayTexture, overlay_scale).GetPixels32();
        }

        int startX = Mathf.RoundToInt(pixelCoords.x - overlayW / 2);
        int startY = Mathf.RoundToInt(pixelCoords.y - overlayH / 2);

        for (int oy = 0; oy < overlayH; oy++)
        {
            int by = startY + oy;
            if (by < 0 || by >= baseH) continue;

            for (int ox = 0; ox < overlayW; ox++)
            {
                int bx = startX + ox;
                if (bx < 0 || bx >= baseW) continue;

                Color32 src = overlayPixels[oy * overlayW + ox];

                // Fully transparent → write nothing
                if (src.a == 0) continue;

                int baseIndex = by * baseW + bx;
                Color32 dst = basePixels[baseIndex];

                // Simple alpha blend (src over dst)
                // Convert to float for correct blending, then back
                float srcA = src.a / 255f;
                float invA = 1f - srcA;

                byte r = (byte)Mathf.Clamp(Mathf.RoundToInt(src.r * srcA + dst.r * invA), 0, 255);
                byte g = (byte)Mathf.Clamp(Mathf.RoundToInt(src.g * srcA + dst.g * invA), 0, 255);
                byte b = (byte)Mathf.Clamp(Mathf.RoundToInt(src.b * srcA + dst.b * invA), 0, 255);
                byte a = (byte)Mathf.Clamp(Mathf.RoundToInt(src.a + dst.a * invA), 0, 255);

                basePixels[baseIndex] = new Color32(r, g, b, a);
            }
        }

        result.SetPixels32(basePixels);
        result.Apply(true); // regenerate mipmaps if present
        return result;
    }


    /// <summary>
    /// Draws a filled circle onto a Texture2D.
    /// The texture must have Read/Write Enabled in its import settings.
    /// Modifies the texture in-place and returns it.
    /// </summary>
    public static Texture2D DrawCircle(Texture2D texture, Vector2 center, Color color, float radius)
    {
        if (texture == null || radius <= 0f)
            return texture;

        int width  = texture.width;
        int height = texture.height;

        // Clamp the iteration bounds to the texture
        int minX = Mathf.Max(0, Mathf.FloorToInt(center.x - radius));
        int maxX = Mathf.Min(width  - 1, Mathf.CeilToInt(center.x + radius));
        int minY = Mathf.Max(0, Mathf.FloorToInt(center.y - radius));
        int maxY = Mathf.Min(height - 1, Mathf.CeilToInt(center.y + radius));

        float radiusSq = radius * radius;

        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                float dx = x - center.x;
                float dy = y - center.y;

                if (dx * dx + dy * dy <= radiusSq)
                {
                    texture.SetPixel(x, y, color);
                }
            }
        }

        texture.Apply();   // upload changes to the GPU
        return texture;
    }
}
