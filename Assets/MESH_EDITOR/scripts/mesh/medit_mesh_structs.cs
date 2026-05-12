using UnityEngine;

// data struct for holding all information about a triangle, not just indices
[System.Serializable]
public class mesh_triangle
{
    // the vertex positions
    public Vector3 v1;
    public Vector3 v2;
    public Vector3 v3;

    public int n1;
    public int n2;
    public int n3;

    public mesh_triangle() {}

    public mesh_triangle(Vector3 v1,Vector3 v2,Vector3 v3,int n1,int n2,int n3)
    {
        this.v1 = v1;
        this.v2 = v2;
        this.v3 = v3;
        this.n1 = n1;
        this.n2 = n2;
        this.n3 = n3;
    }
}

// I cannot sufficiently express my distaste of this struct
[System.Serializable]
public class secondary_vertex
{
    public int gatewayVertex;
    public int secondaryVertex;

    public secondary_vertex(int gatewayVertex, int secondaryVertex)
    {
        this.gatewayVertex = gatewayVertex;
        this.secondaryVertex = secondaryVertex;
    }
}

// this one too, truly disgusting
// in part because its THE EXACT SAME as secondary_vertex
[System.Serializable]
public class line_segment
{
    public int a;
    public int b;

    public line_segment(int a, int b)
    {
        this.a = a;
        this.b = b;
    }
}