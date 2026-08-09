using System.Collections.Generic;
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

    public Transform t_adjustmentContainer;
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

    void Start()
    {
        //GrabChunkAdjustments();


        PopulateChunkLocationMarkers();
    }

    public void InitializeChunkGeneration(NoiseProfile noise_data)
    {
        chunkGenerationActive = true;

        level_noise = noise_data;
    }

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
