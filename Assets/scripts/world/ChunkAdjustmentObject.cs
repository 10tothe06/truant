using UnityEngine;
using UnityEngine.UIElements;

public class ChunkAdjustmentObject : MonoBehaviour
{
    public ChunkAdjustmentType type;
    public bool generateMesh = true; // ONLY FOR PATHS
    public bool grassBan;

    public ChunkAdjustment Get()
    {
        ChunkAdjustment result = new ChunkAdjustment();

        Vector3 m = Vector3.zero;

        result.type = type;
        result.generateMesh = generateMesh;
        result.grassBan = grassBan;

        result.points = new Vector3[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            result.points[i] = transform.GetChild(i).position;
            m += result.points[i]/transform.childCount;
        }
        
        if (type == ChunkAdjustmentType.Terrain_Adjust || type == ChunkAdjustmentType.Foliage_Break)
        {
            result.normals = new Vector3[transform.childCount];
            
            for (int i = 0; i < transform.childCount; i++)
            {
                result.normals[i] = result.points[i] - m;
            }
        } else
        {
            result.normals = new Vector3[0];
        }

        return result;
    }
}
