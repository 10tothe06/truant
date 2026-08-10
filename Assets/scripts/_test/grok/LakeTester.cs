using UnityEngine;

[ExecuteAlways]
public class LakeTester : MonoBehaviour
{
    [Header("Generation")]
    public int vertexCount = 80;
    public float size = 18f;
    [Range(0.25f, 3f)] public float complexity = 0.75f;
    public int seed = 12345;

    [Header("Visual")]
    public bool drawGizmos = true;
    public Color lineColor = new Color(0.15f, 0.45f, 0.85f, 0.95f);
    public Color pointColor = new Color(1f, 1f, 1f, 0.7f);
    public float pointSize = 0.18f;

    [Header("Runtime")]
    public bool regenerateOnChange = true;

    private Vector2[] vertices;
    private int lastSeed;
    private float lastComplexity;
    private float lastSize;
    private int lastCount;

    void OnValidate()
    {
        if (regenerateOnChange)
            Generate();
    }

    void OnEnable() => Generate();

    public void Generate()
    {
        vertices = LakeVertexGenerator.Generate(vertexCount, size, complexity, seed);
        lastSeed = seed;
        lastComplexity = complexity;
        lastSize = size;
        lastCount = vertexCount;
    }

    void OnDrawGizmos()
    {
        if (!drawGizmos || vertices == null || vertices.Length < 3)
            return;

        // Auto-regenerate if parameters changed in play mode
        if (Application.isPlaying &&
            (seed != lastSeed || !Mathf.Approximately(complexity, lastComplexity) ||
             !Mathf.Approximately(size, lastSize) || vertexCount != lastCount))
        {
            Generate();
        }

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

    // Optional: force a new random lake from context menu
    [ContextMenu("Randomize Seed")]
    void Randomize()
    {
        seed = Random.Range(0, 999999);
        Generate();
    }
}