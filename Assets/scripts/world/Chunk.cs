using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public List<ChunkAdjustment> localChunkAdjustments;
    private MeshFilter meshComp;
    private MeshFilter grassComp;
    private MeshCollider colliderComp;
    public Material m_grassBlades;

    private Vector3[] v;

    private Texture2D grassMap;
    private Color32[] grassColors;

    void CalculateVertices(AltMesh planeMesh, Transform t, Vector3 p)
    {
        int res = WorldManager.Instance.chunkResolution;
        grassColors = new Color32[res*res];

        float noiseScale = 0.1f;
        float noiseAmplitude = 3;

        v = new Vector3[res * res];

        for (int x = 0, i = 0; x < res; x++)
        {
            for (int z = 0; z < res; z++, i++)
            {
                float noiseX = noiseScale * (p.x + x / ((float)res-1) * WorldManager.Instance.chunkSize);
                float noiseY = noiseScale * (p.z + z / ((float)res-1) * WorldManager.Instance.chunkSize);

                //v[i] = planeMesh.vertices[i] + Vector3.up * ((float)Perlin.Noise(noiseX, 0, noiseY) * noiseAmplitude * (float)Perlin.Noise(noiseX / 10f, 0, noiseY / 10f) - noiseAmplitude * 2 * Mathf.Abs(Mathf.Pow((float)Perlin.Noise(noiseX/5, 0, noiseY/5) + 0.4f, 8f)));
                float dist = 999f;
                float targetHeight = 0;
                bool actingPath = false;
                grassColors[i] = Color.white;
                for (int j = 0; j < localChunkAdjustments.Count;j++)
                {
                    if (localChunkAdjustments[j].type == ChunkAdjustmentType.Terrain_Adjust || localChunkAdjustments[j].type == ChunkAdjustmentType.Flat_Area)
                    {
                        if (!actingPath)
                        {
                            float tHeight = localChunkAdjustments[j].points[0].y;
                            float newDist = util_mesh.DistanceToPolygon(localChunkAdjustments[j].points, planeMesh.vertices[i]+p);
                            if (newDist < dist) {
                                dist = newDist; targetHeight = tHeight;
                                if (localChunkAdjustments[j].grassBan && newDist == 0)
                                {
                                    grassColors[i] = Color.black;
                                }
                            }
                        }
                    }
                    else if (localChunkAdjustments[j].type == ChunkAdjustmentType.Foliage_Break)
                    {
                        if (!actingPath)
                        {

                            float newDist = util_mesh.DistanceToPolygon(localChunkAdjustments[j].points, planeMesh.vertices[i]+p);
                            if (localChunkAdjustments[j].grassBan && newDist == 0) {grassColors[i] = Color.black;}
                        }
                    }
                    else if (localChunkAdjustments[j].type == ChunkAdjustmentType.Path)
                    {
                        // paths are a bit different, instead of getting the distance to a rect we're grabbing the distance to the line
                        if (localChunkAdjustments[j].points.Length > 1)
                        {
                            Vector3 vert = planeMesh.vertices[i]+p;
                            for (int n = 0; n < localChunkAdjustments[j].points.Length - 1; n++)
                            {   
                                Vector3 dir1 = vert - localChunkAdjustments[j].points[n];
                                Vector3 dir2 = localChunkAdjustments[j].points[n+1] - localChunkAdjustments[j].points[n];
                                
                                if (Vector3.Dot(dir1, dir2) > 0)
                                {
                                    Vector3 projectedDir = Vector3.Project(dir1, dir2);
                                    projectedDir = projectedDir.normalized * Mathf.Min(dir2.magnitude, projectedDir.magnitude);

                                    Vector3 clampedPoint = localChunkAdjustments[j].points[n] + projectedDir;
                                    clampedPoint = new Vector3(clampedPoint.x, 0, clampedPoint.z);
                                    vert = new Vector3(vert.x, 0, vert.z);

                                    float distToLine = Vector3.Distance(clampedPoint, vert)/2f;
                                    float height = Mathf.Lerp(localChunkAdjustments[j].points[n].y, localChunkAdjustments[j].points[n+1].y, projectedDir.magnitude / dir2.magnitude);
                                    if (distToLine < dist && distToLine < 2) {
                                        dist = distToLine; targetHeight = height;actingPath = true;
                                        if (distToLine < 1)
                                        {
                                            grassColors[i] = Color.black;
                                        }
                                        }
                                }
                            }
                        }
                    }
                }

                float term = actingPath ? dist-2 : (5+dist)/20f;
                v[i] = planeMesh.vertices[i] + Vector3.Lerp(Vector3.up * targetHeight, Vector3.up * Perlin.Noise(noiseX, 0, noiseY) * noiseAmplitude, Mathf.Min(term, 1));
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
        grassComp = transform.GetChild(0).GetComponent<MeshFilter>();
        colliderComp = GetComponent<MeshCollider>();
        grassMap = new Texture2D(WorldManager.Instance.chunkResolution,WorldManager.Instance.chunkResolution);
        

        Mesh planeMesh = util_mesh.GeneratePlane(WorldManager.Instance.chunkResolution, WorldManager.Instance.chunkSize, false);

        Transform t = transform;
        Vector3 p = t.position;
        AltMesh a = util_mesh.ToAlt(planeMesh);
        await Task.Run(() => CalculateVertices(a,t,p));
        
        grassMap.SetPixels32(grassColors);
        grassMap.Apply();

        planeMesh.SetVertices(v);
        planeMesh.RecalculateBounds();
        meshComp.sharedMesh = planeMesh;

        // grassComp.gameObject.SetActive(rw_utils.prefs.enableGrass);

        // if (grassComp.gameObject.activeSelf)
        // {
        //     grassComp.sharedMesh = planeMesh;
        //     grassComp.GetComponent<MeshRenderer>().material = m_grassBlades;
        //     grassComp.GetComponent<MeshRenderer>().material.SetFloat("CENTER_X", transform.position.x);
        //     grassComp.GetComponent<MeshRenderer>().material.SetFloat("CENTER_Y", transform.position.z);
        //     grassComp.GetComponent<MeshRenderer>().material.SetTexture("_NoiseTexture", grassMap);
        // }
        
        colliderComp.sharedMesh = planeMesh;

        // now the foliage
        for (int i = 0; i < WorldManager.Instance.chunkFoliage.types.Length; i++)
        {
            int count = WorldManager.Instance.chunkFoliage.GetCount(i);
            for (int j = 0; j < count; j++)
            {
                Transform t_newFoliage = Instantiate(WorldManager.Instance.chunkFoliage.types[i].prefab, transform).transform;

                // TODO: seeded
                float minX = transform.position.x - WorldManager.Instance.chunkSize/2f;
                float maxX = transform.position.x + WorldManager.Instance.chunkSize/2f;

                float minY = transform.position.z - WorldManager.Instance.chunkSize/2f;
                float maxY = transform.position.z + WorldManager.Instance.chunkSize/2f;

                Vector3 rPos = new Vector3(Random.Range(minX, maxX), 100, Random.Range(minY, maxY));

                bool placedSucessfully = false;
                RaycastHit hit;
                if (Physics.Raycast(rPos, -Vector3.up, out hit))
                {
                    // this gives the tree a random position, and makes sure that it lies on the terrain
                    if (hit.collider.gameObject == gameObject)
                    {
                        t_newFoliage.position = hit.point;
                        placedSucessfully = true;
                    }

                }

                for (int n = 0; n < localChunkAdjustments.Count;n++)
                {
                    if (localChunkAdjustments[n].type == ChunkAdjustmentType.Foliage_Break || localChunkAdjustments[n].type == ChunkAdjustmentType.Flat_Area)
                    {
                        
                        float dist = util_mesh.DistanceToPolygon(localChunkAdjustments[n].points, hit.point);
                        if (dist < 0.1f) placedSucessfully = false;
                    }
                    else if (localChunkAdjustments[n].type == ChunkAdjustmentType.Path)
                    {
                        // paths are a bit different, instead of getting the distance to a rect we're grabbing the distance to the line
                        if (localChunkAdjustments[n].points.Length > 1)
                        {
                            Vector3 vert = hit.point;
                            for (int k = 0; k < localChunkAdjustments[n].points.Length - 1; k++)
                            {   
                                Vector3 dir1 = vert - localChunkAdjustments[n].points[k];
                                Vector3 dir2 = localChunkAdjustments[n].points[k+1] - localChunkAdjustments[n].points[k];
                                
                                if (Vector3.Dot(dir1, dir2) > 0)
                                {
                                    Vector3 projectedDir = Vector3.Project(dir1, dir2);
                                    projectedDir = projectedDir.normalized * Mathf.Min(dir2.magnitude, projectedDir.magnitude);

                                    Vector3 clampedPoint = localChunkAdjustments[n].points[k] + projectedDir;
                                    clampedPoint = new Vector3(clampedPoint.x, 0, clampedPoint.z);
                                    vert = new Vector3(vert.x, 0, vert.z);

                                    float distToLine = Vector3.Distance(clampedPoint, vert);
                                    if (distToLine < 4f) placedSucessfully = false;
                                }
                            }
                        }
                    }
                }

                if (!placedSucessfully)
                {
                    Destroy(t_newFoliage.gameObject);
                }
            }
        }
    }
}
