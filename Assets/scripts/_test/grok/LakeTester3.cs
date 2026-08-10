using UnityEngine;

[ExecuteAlways]
public class LakeTester3 : MonoBehaviour
{
    [Header("Generation")]
    public int vertexCount = 90;
    public float size = 20f;
    public float irregularity = 0.8f;
    public int seed = 42;

    [Header("Visual")]
    public bool drawGizmos = true;
    public Color lineColor = new Color(0.1f, 0.45f, 0.85f, 0.95f);
    public Color pointColor = new Color(1f, 1f, 1f, 0.6f);
    public float pointSize = 0.15f;

    private Vector2[] vertices;
    private int lastHash;

    void OnValidate() => TryGenerate();
    void OnEnable() => TryGenerate();

    void TryGenerate()
    {
        int hash = vertexCount + seed + size.GetHashCode() + irregularity.GetHashCode();
        if (hash == lastHash && vertices != null) return;

        vertices = LakeVertexGenerator.Generate(vertexCount, size, irregularity, seed);
        lastHash = hash;
    }

    void OnDrawGizmos()
    {
        if (!drawGizmos || vertices == null || vertices.Length < 3) return;

        TryGenerate();

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