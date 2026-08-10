using UnityEngine;

[ExecuteAlways]
public class LakeTester2 : MonoBehaviour
{
    [Header("Spine & Dimensions")]
    public int vertexCount = 90;
    public float length = 30f;
    public float maxWidth = 10f;

    [Header("Character")]
    [Range(0.15f, 1.2f)] public float meander = 0.7f;
    [Range(0.2f, 1.2f)] public float widthVariation = 0.75f;
    public int seed = 98765;

    [Header("Visual")]
    public bool drawGizmos = true;
    public Color lineColor = new Color(0.1f, 0.4f, 0.8f, 0.95f);
    public Color pointColor = new Color(1f, 1f, 1f, 0.65f);
    public float pointSize = 0.16f;

    private Vector2[] vertices;
    private int lastHash;

    void OnValidate() => TryGenerate();
    void OnEnable() => TryGenerate();

    void TryGenerate()
    {
        int hash = vertexCount + seed + length.GetHashCode() + maxWidth.GetHashCode() +
                   meander.GetHashCode() + widthVariation.GetHashCode();
        if (hash == lastHash && vertices != null) return;

        vertices = LakeVertexGenerator2.Generate(
            vertexCount, length, maxWidth, meander, widthVariation, seed);
        lastHash = hash;
    }

    void OnDrawGizmos()
    {
        if (!drawGizmos || vertices == null || vertices.Length < 3) return;

        TryGenerate(); // keep live in editor

        Vector3 origin = transform.position;
        Gizmos.color = lineColor;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 a = origin + (Vector3)vertices[i];
            Vector3 b = origin + (Vector3)vertices[(i + 1) % vertices.Length];
            Gizmos.DrawLine(a, b);
        }

        Gizmos.color = pointColor;
        foreach (var v in vertices)
            Gizmos.DrawSphere(origin + (Vector3)v, pointSize);
    }

    [ContextMenu("Randomize Seed")]
    void Randomize()
    {
        seed = Random.Range(0, 999999);
        lastHash = 0;
        TryGenerate();
    }
}