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
    private Color[] chunk_tex_data;


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

        for (int i = 0; i < chunk_tex_data.Length; i++)
        {
            float noise_x = (i % 64) / (float)64;
            float noise_y = 0;
            float noise_z = (i / 64) / (float)64;

            Vector3 noise_sample_point = p + new Vector3(noise_x, noise_y, noise_z) * WorldManager.Instance.chunkSize;


            float targetHeight = WorldManager.level_noise.GetHeight(noise_sample_point);

            for (int c = 0; c < localChunkAdjustments.Count; c++)
            {
                targetHeight = localChunkAdjustments[c].AdjustTerrainHeight(noise_sample_point, targetHeight);
            }

            chunk_tex_data[i] = new Color(targetHeight / WorldManager.level_noise.noise_range + 0.5f, 0, 0, 0);
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

        chunk_tex_data = WorldManager.level_noise.GenerateTextureData(64, transform.position);

        //GetComponent<MeshRenderer>().material.mainTexture = chunk_tex;

        await Task.Run(() => CalculateVertices(a,t,p));

        chunk_tex = new Texture2D(64, 64, TextureFormat.RGBA32, false, true);
        chunk_tex.SetPixels(chunk_tex_data);
        chunk_tex.Apply(false, false);
        chunk_tex.filterMode = FilterMode.Point;

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
            }
        }
    }
}
