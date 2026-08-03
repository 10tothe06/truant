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
    public List<Vector2> blacklistedChunks; // chunk coords, obv
    [HideInInspector]
    public List<Transform> loadedChunks; // TODO: replace with a friendlier system for saving to disk
    public GameObject p_chunkPrefab;
    public Transform t_chunkContainer;

    public float chunkSize; // in world units
    public int chunkResolution; // # of vertices per side
    public int renderDistance; // works like in minecraft

    public bool showChunkLocationMarkers; // debug feature
    private bool showingLocationMarkers;
    public Transform[] t_chunkLocationMarkers;
    public Transform t_locationMarkerContainer;
    public GameObject p_path;

    public Transform t_pathMeshContainer;

    void Start()
    {
        GrabChunkAdjustments();
        PopulateChunkLocationMarkers();
    }

    void GrabChunkAdjustments()
    {
        for (int i = 0; i < t_adjustmentContainer.childCount; i++)
        {
            if (t_adjustmentContainer.GetChild(i).gameObject.activeSelf)
            {
                globalChunkAdjustments.Add(t_adjustmentContainer.GetChild(i).GetComponent<ChunkAdjustmentObject>().Get());

                if (t_adjustmentContainer.GetChild(i).GetComponent<ChunkAdjustmentObject>().type == ChunkAdjustmentType.Path && t_adjustmentContainer.GetChild(i).GetComponent<ChunkAdjustmentObject>().generateMesh)
                {
                    // geneate the path if its a path type
                    GameObject g_newPath = Instantiate(p_path, Vector3.zero, Quaternion.identity);
                    g_newPath.transform.SetParent(t_pathMeshContainer);
                    g_newPath.GetComponent<MeshFilter>().sharedMesh = util_mesh.GeneratePathMesh(t_adjustmentContainer.GetChild(i).GetComponent<ChunkAdjustmentObject>().Get().points);
                }
            }   
        }
        //if (!Program.Instance.leaveAdjustmentsActive) t_adjustmentContainer.gameObject.SetActive(false);
    }
    public void BlacklistChunks(Vector2 start)
    {
        BlacklistChunks(start, start);
    }
    public void BlacklistChunks(Vector2 start, Vector2 end) // end (x and y) has to be GREATER THAN START
    {
        for (int x = Mathf.RoundToInt(start.x); x <= end.x; x++)
        {
            for (int y = Mathf.RoundToInt(start.y); y <= end.y; y++)
            {
                blacklistedChunks.Add(new Vector2(x, y));
            }
        }
    }

    // how much energy the environment is pulling from the player
    // being in water is calculated separately, as is everything else
    public static float GetAmbientTemperatureFlux(Vector3 samplePosition)
    {
        return -0.01f;
    }

    void Update()
    {
        UpdateChunkLocationMarkers();

        // if (Keyboard.current.minusKey.wasPressedThisFrame)
        // {
        //     showChunkLocationMarkers = !showChunkLocationMarkers;
        // }

        if (showChunkLocationMarkers && !showingLocationMarkers)
        {
            for (int i = 0; i < t_chunkLocationMarkers.Length; i++)
            {
                //DebugManager.DrawUISphere(t_chunkLocationMarkers[i]);
            }
            showingLocationMarkers = true; // making sure we only call this once
        }
        if (!showChunkLocationMarkers && showingLocationMarkers)
        {
            for (int i = 0; i < t_chunkLocationMarkers.Length; i++)
            {
                //DebugManager.ClearUISphere(t_chunkLocationMarkers[i]);
            }
            showingLocationMarkers = false; // making sure we only call this once
        }

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
                if (IsChunkPositionValid(t_chunkLocationMarkers[i].position))
                {
                    GenerateNewChunkAtPosition(t_chunkLocationMarkers[i].position);
                }
            }
        }
    }

    bool IsChunkPositionValid(Vector3 position)
    {
        for (int i = 0; i < blacklistedChunks.Count; i++)
        {
            if (new Vector3(blacklistedChunks[i].x, 0, blacklistedChunks[i].y) * chunkSize == position)
            {
                return false;
            }
        }
        return true;
    }

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
