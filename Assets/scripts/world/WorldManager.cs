using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

// script that manages chunks and the procedurally-generated stuff

// this whole system is separate from the level system,
// so that I can avoid having a copy of all this per level
// instead, all level scripts just draw from here

public class WorldManager : MonoBehaviour
{
    private static WorldManager _instance;

    // this is used for most things, static functions can also be used when verbosity is a concern
    public static WorldManager Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
            {
                _instance = value;
            }
            else if (_instance != value)
            {
                Debug.Log("You messed up buddy.");
                Destroy(value);
            }
        }
    }

    void Awake()
    {
        Instance = this;
    }

    [Header("LEVEL DIMENSIONS")]
    // both in unity coordinate units
    public float level_width;
    public float level_height;
    public Texture2D map_texture;


    [Space(25)]
    [Header("POIs")]
    public string[] points_of_interest;


    [Space(25)]
    public List<ChunkAdjustment> globalChunkAdjustments;

    public bool chunkGenerationActive;

    public FoliageProfile chunkFoliage;

    [HideInInspector]
    public List<Transform> loadedChunks; // TODO: replace with a friendlier system for saving to disk
    public GameObject p_chunkPrefab;

    // all chunks will end up children of this obj
    [SerializeField]
    private Transform t_chunkContainer;

    public float chunkSize; // in world units
    public int chunkResolution; // # of vertices per side
    public int renderDistance; // works like in minecraft


    public Transform[] t_chunkLocationMarkers;
    public Transform t_locationMarkerContainer;

    public static NoiseProfile level_noise;

    // cached stuff
    // *****

    [HideInInspector]
    public Vector3[] lake_vertices;
    [HideInInspector]
    public GameObject[] poi_objects;

    [HideInInspector]
    public Vector3 a;
    [HideInInspector]
    public Vector3 b;
    [HideInInspector]
    public Vector3 c;
    [HideInInspector]
    public Vector3 d;



    // *****

    void Start()
    {
        //GrabChunkAdjustments();

        PopulateChunkLocationMarkers();
    }

    #region LEVEL GENERATION


    // what level scripts should be calling to make the map
    public static void InitializeLevelEnvironment(NoiseProfile terrain_noise)
    {
        if (Program.force_flat_terrain)
        {
            terrain_noise = NoiseProfile.Contstant(0);
        }

        // first, the lake
        InitializeLake();


        // now we have to decide where the dimensions of the map will go
        Vector3 lake_midpoint = util_mesh.GetMidpoint(Instance.lake_vertices);

        DebugManager.DrawSphere(lake_midpoint, 10f);

        // we're gonna try and find the long axis of the lake,
        // and use that as the bottom edge of the map
        // (not necessarily south btw)
        Vector3 long_lake_axis = util_world.GetWidestAxis(Instance.lake_vertices);

        DebugManager.DrawLine(lake_midpoint - long_lake_axis * 500f, lake_midpoint + long_lake_axis * 500f, Color.green);


        // now that we have the bottom edge, we can figure out the other ones

        Vector3 short_lake_axis = Vector3.Cross(long_lake_axis, Vector3.up).normalized;

        // making sure the level is actualy oriented towards land,
        // not the rest of the lake
        if (Vector3.Dot(short_lake_axis, lake_midpoint - Vector3.zero) > 0)
        {
            short_lake_axis *= -1;
        }

        // these are the four points that make up the map rectangle
        Instance.a = lake_midpoint - long_lake_axis * Instance.level_width/2f;
        Instance.b = lake_midpoint + long_lake_axis * Instance.level_width/2f;
        Instance.c = lake_midpoint + long_lake_axis * Instance.level_width/2f + short_lake_axis * Instance.level_height;
        Instance.d = lake_midpoint - long_lake_axis * Instance.level_width/2f + short_lake_axis * Instance.level_height;

        DebugManager.DrawLine(Instance.a, Instance.b, Color.cyan);
        DebugManager.DrawLine(Instance.b, Instance.c, Color.cyan);
        DebugManager.DrawLine(Instance.c, Instance.d, Color.cyan);
        DebugManager.DrawLine(Instance.d, Instance.a, Color.cyan);

        // finally, the chunks
        // we do these last so that we don't have to re-calculate all the chunk adjustments
        // (if we did level generation would take forever)
        InitializeChunkGeneration(terrain_noise);

        Instance.map_texture = util_map.GenerateMapTexture(128, 64);

        GenerateLevelPOIs();
    }


    // whole bunch of local variables in here that will be incorporated into sysarch later
    // (hopefully)
    private static void GenerateLevelPOIs()
    {
        // the number of poi instances we want in one level
        int poi_count = 10;
        Instance.poi_objects = new GameObject[poi_count];


        List<string> poi_pool = Instance.points_of_interest.ToList();

        // for the real game this should be false 
        bool allow_duplicate_pois = true;

        for (int i = 0; i < poi_count; i++)
        {
            if (poi_pool.Count == 0)
            {
                // we have no more pois left to add,
                // so we're done
                break;
            }

            // spawn the structure 
            // (these are spawned like any other object, because why SHOULD there be a difference?)
            int poi_index = Random.Range(0, poi_pool.Count);


            GameObject new_poi = ObjectManager.SpawnObject(poi_pool[poi_index], util_map.GetRandomMapPosition());

            poi_generic comp = new_poi.GetComponent<poi_generic>();

            comp.Initialize();

            Instance.poi_objects[i] = new_poi;

            // then we remove it from the pool
            // (this avoids duplicate structures)
            if (!allow_duplicate_pois)
            {
                poi_pool.RemoveAt(poi_index);
            }
        }

        RemoveInvalidPOIsFromLevel();
    }


    // takes out any POIs that are outside the map,
    // OR inside another POIs exclusion radius
    private static void RemoveInvalidPOIsFromLevel()
    {
        // first, checking if outside the map
        for (int i = Instance.poi_objects.Length - 1; i>= 0; i--)
        {
            if (!util_map.IsPositionInsideMap(Instance.poi_objects[i].transform.position))
            {
                Destroy(Instance.poi_objects[i]);
            }
        }
    }

    
    // called when a level wants to have chunk generation

    // even though most levels are going to use the same noise profile,
    // I still want support for different ones just in case
    public static void InitializeChunkGeneration(NoiseProfile noise_data)
    {
        Instance.chunkGenerationActive = true;

        level_noise = noise_data;
    }


    // called when a script wants to add a lake to the level
    // this handles the chunk adjustment,
    // the placement of the water mesh,
    // and everything else

    // note that 
    public static void InitializeLake()
    {
        // first, let's make the mesh itself
        Mesh lake_mesh = util_world.GenerateLakeMesh();

        GameObject lake_object = ObjectManager.SpawnObject("lake", new Vector3(-Instance.chunkSize/2f, -4f, -Instance.chunkSize/2f));

        lake_object.transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh = lake_mesh;

        // then, the chunk adjustment that will control the terrain
        AddChunkAdjustment(lake_mesh.vertices, NoiseProfile.Contstant(-10f), new FoliageProfile(), 30f, 0f);

        Instance.lake_vertices = lake_mesh.vertices;
    }

    public static void AddChunkAdjustment(Vector3[] points, NoiseProfile noise_overwrite, FoliageProfile foliage_overwrite, float noise_transition_width = 1f, float foliage_transition_width = 1f)
    {
        ChunkAdjustment new_adjust = new ChunkAdjustment(points, noise_overwrite, foliage_overwrite, noise_transition_width, foliage_transition_width);

        Instance.globalChunkAdjustments.Add(new_adjust);
    }

    #endregion

    

    // placing the gameobjects that will become the guides for chunk generation
    void PopulateChunkLocationMarkers()
    {
        t_chunkLocationMarkers = new Transform[(renderDistance * 2 + 1) * (renderDistance * 2 + 1)];
        for (int i = 0, x = 0; x < renderDistance*2+1; x++)
        {
            for (int y = 0; y < renderDistance*2+1; y++,i++)
            {
                t_chunkLocationMarkers[i] = new GameObject().transform;
                t_chunkLocationMarkers[i].SetParent(t_locationMarkerContainer);
            }
        }
    }

    void GrabChunkAdjustments()
    {
        // for (int i = 0; i < t_adjustmentContainer.childCount; i++)
        // {
        //     if (t_adjustmentContainer.GetChild(i).gameObject.activeSelf)
        //     {
        //         globalChunkAdjustments.Add(t_adjustmentContainer.GetChild(i).GetComponent<ChunkAdjustmentObject>().Get());

        //         if (t_adjustmentContainer.GetChild(i).GetComponent<ChunkAdjustmentObject>().type == ChunkAdjustmentType.Path && t_adjustmentContainer.GetChild(i).GetComponent<ChunkAdjustmentObject>().generateMesh)
        //         {
        //             // geneate the path if its a path type
        //             GameObject g_newPath = Instantiate(p_path, Vector3.zero, Quaternion.identity);
        //             g_newPath.transform.SetParent(t_pathMeshContainer);
        //             g_newPath.GetComponent<MeshFilter>().sharedMesh = util_mesh.GeneratePathMesh(t_adjustmentContainer.GetChild(i).GetComponent<ChunkAdjustmentObject>().Get().points);
        //         }
        //     }   
        // }
        //if (!Program.Instance.leaveAdjustmentsActive) t_adjustmentContainer.gameObject.SetActive(false);
    }

    void Update()
    {
        // moves the markers around so that they form a grid around the player
        UpdateChunkLocationMarkers();

        if (chunkGenerationActive) GenerateNewChunks();
    }

    public Transform FindChunkAtPosition(Vector3 position)
    {
        for (int i = 0; i < loadedChunks.Count; i++)
        {
            if (new Vector3(loadedChunks[i].position.x,0,loadedChunks[i].position.z) == new Vector3(position.x,0,position.z))
            {
                return loadedChunks[i];
            }
        }

        return null;
    }

    public void GenerateNewChunkAtPosition(Vector3 position)
    {
        GameObject newChunk = Instantiate(p_chunkPrefab, t_chunkContainer);

        newChunk.transform.position = position;
        newChunk.GetComponent<Chunk>().Initialize();

        loadedChunks.Add(newChunk.transform);
    }

    // if an empty space is detected make a new chunk
    void GenerateNewChunks()
    {
        for (int i = 0; i < t_chunkLocationMarkers.Length; i++)
        {
            if (FindChunkAtPosition(t_chunkLocationMarkers[i].position) == null)
            {
                GenerateNewChunkAtPosition(t_chunkLocationMarkers[i].position);
            }
        }
    }

    void UpdateChunkLocationMarkers()
    {
        for (int i = 0, x = 0; x < renderDistance*2+1; x++)
        {
            for (int y = 0; y < renderDistance*2+1; y++,i++)
            {
                Vector3 rawCameraPos = Player.Instance.transform.position;
                Vector3 cameraPos = new Vector3(Mathf.Round(rawCameraPos.x / chunkSize) * chunkSize, 0, Mathf.Round(rawCameraPos.z / chunkSize) * chunkSize);
                t_chunkLocationMarkers[i].position = new Vector3(cameraPos.x + (x - renderDistance)  * chunkSize, 0, cameraPos.z + (y-renderDistance) * chunkSize);
            }
        }
    }
}
