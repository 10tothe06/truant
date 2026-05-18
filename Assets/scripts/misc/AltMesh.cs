using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this class is my solution to the issue that you can't create Mesh objects outside of the main thread
// hopefully it works?

public class AltMesh
{
    public Vector3[] vertices;
    public Vector3[] normals;
    public Vector2[] uvs;
    public int[] indices;
}
