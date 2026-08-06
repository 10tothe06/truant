using UnityEngine;

public class GrassBladeMeshGenerator : MonoBehaviour
{
    public static Mesh GenerateBlade(int segments = 2, float height = 1.5f, float baseWidth = 0.15f, float tipWidth = 0.15f)
    {
        Mesh mesh = new Mesh();
        mesh.name = "GrassBlade";

        int vertCount = (segments + 1) * 2;
        Vector3[] vertices = new Vector3[vertCount];
        Vector2[] uvs = new Vector2[vertCount];
        Vector3[] normals = new Vector3[vertCount];
        int[] triangles = new int[segments * 6];

        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;
            float y = t * height;
            float currentWidth = Mathf.Lerp(baseWidth, tipWidth, t);

            // Left and right vertices
            vertices[i * 2 + 0] = new Vector3(-currentWidth * 0.5f, y, 0);
            vertices[i * 2 + 1] = new Vector3( currentWidth * 0.5f, y, 0);

            uvs[i * 2 + 0] = new Vector2(0, t);
            uvs[i * 2 + 1] = new Vector2(1, t);

            normals[i * 2 + 0] = Vector3.forward;
            normals[i * 2 + 1] = Vector3.forward;
        }

        int triIndex = 0;
        for (int i = 0; i < segments; i++)
        {
            int baseIndex = i * 2;

            // First triangle
            triangles[triIndex++] = baseIndex;
            triangles[triIndex++] = baseIndex + 2;
            triangles[triIndex++] = baseIndex + 1;

            // Second triangle
            triangles[triIndex++] = baseIndex + 1;
            triangles[triIndex++] = baseIndex + 2;
            triangles[triIndex++] = baseIndex + 3;
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.normals = normals;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        return mesh;
    }
}