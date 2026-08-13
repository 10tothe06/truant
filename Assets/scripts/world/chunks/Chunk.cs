using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Chunk : MonoBehaviour
{
    public GrassRenderer grass;
    public List<ChunkAdjustment> localChunkAdjustments;
    private MeshFilter meshComp;
    private MeshCollider colliderComp;

    private Vector3[] v;

    public Texture2D chunk_tex;


    void CalculateVertices(AltMesh planeMesh, Transform t, Vector3 p)
    {
        int res = WorldManager.Instance.chunkResolution;

        //float noiseScale = 0.1f;
        //float noiseAmplitude = 3;

        v = new Vector3[res * res];

        for (int x = 0, i = 0; x < res; x++)
        {
            for (int z = 0; z < res; z++, i++)
            {
                // this is lying
                // this is the WRONG vertex position

                // but DONT FUCKING FIX IT
                // because im lying the same way elsewhere and it works
                float noiseX = (p.x + x / ((float)res-1) * WorldManager.Instance.chunkSize);
                float noiseY = (p.z + z / ((float)res-1) * WorldManager.Instance.chunkSize);

                Vector3 noise_sample_point = new Vector3(noiseX, 0, noiseY);

                float targetHeight = WorldManager.level_noise.GetHeight(noise_sample_point);
                
                // adjusting the terrain based on chunk adjustments
                // ***

                for (int c = 0; c < localChunkAdjustments.Count; c++)
                {
                    targetHeight = localChunkAdjustments[c].AdjustTerrainHeight(noise_sample_point, targetHeight);
                }

                // ***
                

                v[i] = planeMesh.vertices[i] + Vector3.up * targetHeight;
            }
        }
    }
    public async void Initialize()
    {
        for (int i = 0; i < WorldManager.Instance.globalChunkAdjustments.Count; i++)
        {
            localChunkAdjustments.Add(WorldManager.Instance.globalChunkAdjustments[i]);
        }


        meshComp = GetComponent<MeshFilter>();
        colliderComp = GetComponent<MeshCollider>();

        Mesh planeMesh = util_mesh.GeneratePlane(WorldManager.Instance.chunkResolution, WorldManager.Instance.chunkSize, false);

        Transform t = transform;
        Vector3 p = t.position;
        AltMesh a = util_mesh.ToAlt(planeMesh);

        chunk_tex = WorldManager.level_noise.GenerateTexture(64, transform.position);

        //GetComponent<MeshRenderer>().material.mainTexture = chunk_tex;

        await Task.Run(() => CalculateVertices(a,t,p));


        planeMesh.SetVertices(v);
        planeMesh.RecalculateBounds();
        meshComp.sharedMesh = planeMesh;
        
        colliderComp.sharedMesh = planeMesh;

        // draw grass
        grass.Initialize();

        SpawnFoliage();
    }


    // TODO: sample from a heightmap instead of raycasting?
    private void SpawnFoliage()
    {
        // now the foliage
        for (int i = 0; i < WorldManager.Instance.chunkFoliage.types.Length; i++)
        {
            int count = WorldManager.Instance.chunkFoliage.GetCount(i);
            for (int j = 0; j < count; j++)
            {
                // TODO: spawn them in like other objects??
                Transform t_newFoliage = Instantiate(ObjectManager.GetObjectPrefabFromName(WorldManager.Instance.chunkFoliage.types[i].object_name), transform).transform;

                // TODO: seeded
                float minX = transform.position.x - WorldManager.Instance.chunkSize/2f;
                float maxX = transform.position.x + WorldManager.Instance.chunkSize/2f;

                float minY = transform.position.z - WorldManager.Instance.chunkSize/2f;
                float maxY = transform.position.z + WorldManager.Instance.chunkSize/2f;

                float x = Random.Range(0f, 1f);
                float z = Random.Range(0f, 1f);
                float height = (chunk_tex.GetPixel(Mathf.RoundToInt(x * 64), Mathf.RoundToInt(z * 64)).r - 0.5f) * WorldManager.level_noise.noise_range;
                t_newFoliage.position = new Vector3(Mathf.Lerp(minX, maxX, x), height, Mathf.Lerp(minY, maxY, z));

                bool placedSucessfully = true;

                // for (int n = 0; n < localChunkAdjustments.Count;n++)
                // {
                //     if (localChunkAdjustments[n].type == ChunkAdjustmentType.Foliage_Break || localChunkAdjustments[n].type == ChunkAdjustmentType.Flat_Area)
                //     {
                        
                //         float dist = util_mesh.DistanceToPolygon(localChunkAdjustments[n].points, t_newFoliage.position);
                //         if (dist < 0.1f) placedSucessfully = false;
                //     }
                //     else if (localChunkAdjustments[n].type == ChunkAdjustmentType.Path)
                //     {
                //         // paths are a bit different, instead of getting the distance to a rect we're grabbing the distance to the line
                //         if (localChunkAdjustments[n].points.Length > 1)
                //         {
                //             Vector3 vert = t_newFoliage.position;
                //             for (int k = 0; k < localChunkAdjustments[n].points.Length - 1; k++)
                //             {   
                //                 Vector3 dir1 = vert - localChunkAdjustments[n].points[k];
                //                 Vector3 dir2 = localChunkAdjustments[n].points[k+1] - localChunkAdjustments[n].points[k];
                                
                //                 if (Vector3.Dot(dir1, dir2) > 0)
                //                 {
                //                     Vector3 projectedDir = Vector3.Project(dir1, dir2);
                //                     projectedDir = projectedDir.normalized * Mathf.Min(dir2.magnitude, projectedDir.magnitude);

                //                     Vector3 clampedPoint = localChunkAdjustments[n].points[k] + projectedDir;
                //                     clampedPoint = new Vector3(clampedPoint.x, 0, clampedPoint.z);
                //                     vert = new Vector3(vert.x, 0, vert.z);

                //                     float distToLine = Vector3.Distance(clampedPoint, vert);
                //                     if (distToLine < 4f) placedSucessfully = false;
                //                 }
                //             }
                //         }
                //     }
                // }

                if (!placedSucessfully)
                {
                    Destroy(t_newFoliage.gameObject);
                }
            }
        }
    }
}
