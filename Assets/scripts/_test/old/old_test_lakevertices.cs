using System.Collections.Generic;
using UnityEngine;

public class old_test_lakevertices : MonoBehaviour
{
    public float noise_frequency;
    public float noise_amplitude;
    public float step_length;
    public int vertex_count;
    public float point_radius;
    private List<Vector3> vertices;


    [Space(30)]
    public bool regen;

    void Awake()
    {   
        Generate();
    }
    
    void Update()
    {
        if (regen)
        {
            Generate();

            regen = false;
        }
    }

    private void Generate()
    {
        vertices = new List<Vector3>();

        // first, we come up with a random point to start off at
        Vector3 start_position = Vector3.zero;
        Vector2 noise_offset = new Vector2(Random.Range(-100f, 100f), Random.Range(-100f, 100f));

        float gradient_z = 0;
        float gradient_x = (Perlin.Noise(start_position.x*noise_frequency+noise_offset.x + 0.01f, 0, start_position.z*noise_frequency+noise_offset.y)*noise_amplitude - Perlin.Noise(start_position.x*noise_frequency+noise_offset.x, 0, start_position.z*noise_frequency+noise_offset.y)*noise_amplitude) / 0.01f;

        int safe_iterations = 100;
        int num_iterations = 0;

        while (Mathf.Abs(gradient_x) < 0.9f && num_iterations < safe_iterations)
        {
            num_iterations++;

            start_position += Vector3.right * 0.632f;
            gradient_x = (Perlin.Noise(start_position.x*noise_frequency+noise_offset.x + 0.01f, 0, start_position.z*noise_frequency+noise_offset.y)*noise_amplitude - Perlin.Noise(start_position.x*noise_frequency+noise_offset.x, 0, start_position.z*noise_frequency+noise_offset.y)*noise_amplitude) / 0.01f;
        }

        vertices.Add(start_position);

        // then we figure out the gradient vector,
        // make a new vertex,
        // and repeat
        for (int i = 0; i < vertex_count - 1; i++)
        {
            Vector3 last_vertex = vertices[vertices.Count - 1];

            gradient_x = (Perlin.Noise(last_vertex.x*noise_frequency+noise_offset.x + 0.01f, 0, last_vertex.z*noise_frequency+noise_offset.y)*noise_amplitude - Perlin.Noise(last_vertex.x*noise_frequency+noise_offset.x, 0, last_vertex.z*noise_frequency+noise_offset.y)*noise_amplitude) / 0.01f;
            gradient_z = (Perlin.Noise(last_vertex.x*noise_frequency+noise_offset.x, 0, last_vertex.z*noise_frequency+noise_offset.y + 0.01f)*noise_amplitude - Perlin.Noise(last_vertex.x*noise_frequency+noise_offset.x, 0, last_vertex.z*noise_frequency+noise_offset.y)*noise_amplitude) / 0.01f;

            Vector3 slope_x = Vector3.right * gradient_x;
            Vector3 slope_z = Vector3.forward * gradient_z;

            Vector3 normal = Vector3.up;

            Vector3 zero_gradient = Vector3.Cross((slope_x + slope_z).normalized, normal);

            // After computing zero_gradient and before adding the new vertex:
            Vector3 candidate = last_vertex + zero_gradient.normalized * step_length;

            // Project back onto the original isovalue
            float target = Perlin.Noise(start_position.x * noise_frequency + noise_offset.x,
                                        0,
                                        start_position.z * noise_frequency + noise_offset.y) * noise_amplitude;

            float current = Perlin.Noise(candidate.x * noise_frequency + noise_offset.x,
                                        0,
                                        candidate.z * noise_frequency + noise_offset.y) * noise_amplitude;

            Vector3 grad = new Vector3(gradient_x, 0, gradient_z); // reuse the last gradient
            if (grad.sqrMagnitude > 1e-8f)
            {
                candidate -= (current - target) / grad.sqrMagnitude * grad;
            }

            vertices.Add(candidate);

            if (Vector3.Distance(candidate, vertices[0]) < 1f && i > 20f)
            {
                return;
            }
        }
    }

    void OnDrawGizmos()
    {
        if (vertices == null) {return;}


        Gizmos.color = Color.lightCyan;
        for (int i = 0; i < vertices.Count; i++)
        {
            Gizmos.DrawSphere(vertices[i], point_radius);
        }
    }
}
