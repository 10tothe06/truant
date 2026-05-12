using System.Collections.Generic;
using System.IO.Compression;
using UnityEngine;

public class medit_mesh_generation
{
    // why is it called 'normal' and not 'forward'?

    public static Mesh CreateTrueBox(Vector3 right, Vector3 up, Vector3 normal)
    {
        Mesh result = new Mesh();

        Vector3[] verts = new Vector3[6];
        Vector3[] norms = new Vector3[6];
        Vector2[] uvs = new Vector2[6];
        int[] tris = new int[36];

        verts[0] = new Vector3(-right.x/2f,-up.y/2f,-normal.z/2f);
        verts[1] = new Vector3(right.x/2f,-up.y/2f,-normal.z/2f);
        verts[2] = new Vector3(right.x/2f,-up.y/2f,normal.z/2f);
        verts[3] = new Vector3(-right.x/2f,-up.y/2f,normal.z/2f);

        verts[4] = new Vector3(-right.x/2f,up.y/2f,-normal.z/2f);
        verts[5] = new Vector3(right.x/2f,up.y/2f,-normal.z/2f);
        verts[6] = new Vector3(right.x/2f,up.y/2f,normal.z/2f);
        verts[7] = new Vector3(-right.x/2f,up.y/2f,normal.z/2f);

        for (int i = 0; i < 6; i++)
        {
            norms[i] = Vector3.up;
            uvs[i] = Vector2.one; // don't really care about either of these rn
        }

        // -z
        tris[0] = 0;
        tris[1] = 1;
        tris[2] = 2;
        
        tris[3] = 0;
        tris[4] = 2;
        tris[5] = 3;

        // +x
        tris[6] = 1;
        tris[7] = 2;
        tris[8] = 6;

        tris[9] = 1;
        tris[10] = 6;
        tris[11] = 5;

        // -x
        tris[12] = 0;
        tris[13] = 3;
        tris[14] = 7;

        tris[15] = 0;
        tris[16] = 7;
        tris[17] = 4;
        
        // z+
        tris[18] = 4;
        tris[19] = 5;
        tris[20] = 6;

        tris[21] = 4;
        tris[22] = 6;
        tris[23] = 7;

        // -y
        tris[24] = 1;
        tris[25] = 5;
        tris[26] = 4;

        tris[27] = 1;
        tris[28] = 4;
        tris[29] = 0;

        // +y
        tris[30] = 2;
        tris[31] = 3;
        tris[32] = 7;

        tris[33] = 2;
        tris[34] = 7;
        tris[35] = 6;

        result.SetVertices(verts);
        result.SetNormals(norms);
        result.SetUVs(0,uvs);
        result.SetTriangles(tris,0);

        return result;
    }

    public static Mesh CreateBox(Vector3 right, Vector3 up, Vector3 normal)
    {
        Mesh result = new Mesh();

        Mesh xUp = CreatePlane(right/2,normal,up);
        Mesh xDown = CreatePlane(- right/2,normal,-up);

        Mesh yUp = CreatePlane(up/2,normal,-right);
        Mesh yDown = CreatePlane(- up/2,normal,right);

        Mesh zUp = CreatePlane(normal/2,up, right);
        Mesh zDown = CreatePlane(- normal/2,up, -right);

        Mesh[] faces = new Mesh[]
        {
            xUp,xDown,yUp,yDown,zUp,zDown
        };

        result = CombineMeshes(faces);

        return result;
    }

    public static Mesh CreatePlane(Vector3 center, Vector3 up, Vector3 right)
    {
        Mesh result = new Mesh();

        Vector3[] vertices = new Vector3[]
        {
            center - up/2-right/2,
            center - up/2+right/2,
            center + up/2+right/2,
            center + up/2-right/2,
        };

        int[] tris = new int[]
        {
            0,1,2,
            0,2,3,
        };

        Vector3 n = -Vector3.Cross(up,right).normalized;
        Vector3[] normals = new Vector3[]
        {
            n,n,n,n
        };

        // THESE ARE PROPER TEXTURE COORDS, NOT PLACEHOLDER
        Vector2[] uvs = new Vector2[]
        {
            new Vector2(0,0),
            new Vector2(0,1),
            new Vector2(1,1),
            new Vector2(1,0),
        };

        result.SetVertices(vertices);
        result.SetNormals(normals);
        result.SetUVs(0, uvs);
        result.SetTriangles(tris, 0);

        return result;
    }

    public static Mesh CombineMeshes(Mesh[] input)
    {
        return CombineMeshes(input, 0, input.Length-1);
    }

    public static Mesh CombineMeshes(Mesh[] input, int startIndex, int endIndex)
    {
        Mesh result = new Mesh();

        List<Vector3> verts = new List<Vector3>();
        List<Vector3> norms = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        int triangleIndexOffset = 0;
        for (int i = startIndex; i < endIndex + 1; i++)
        {
            List<Vector2> currentUVs = new List<Vector2>();
            input[i].GetUVs(0, currentUVs);

            for (int j = 0; j < input[i].vertices.Length; j++)
            {
                verts.Add(input[i].vertices[j]);
                norms.Add(input[i].normals[j]);
                uvs.Add(currentUVs[j]);
            }

            int[] currentTris = input[i].GetTriangles(0);
            for (int j = 0; j < currentTris.Length; j++)
            {
                tris.Add(currentTris[j] + triangleIndexOffset);
            }

            triangleIndexOffset = verts.Count;
        }

        result.SetVertices(verts);
        result.SetNormals(norms);
        result.SetUVs(0, uvs);
        result.SetTriangles(tris, 0);

        return result;
    }
}
