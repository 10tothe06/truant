using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

// central, organized script for managing particle effects
// (something that I weirdly have not done before)

public class EffectManager : MonoBehaviour
{
    private static EffectManager _instance;

    public static EffectManager Instance {
        get => _instance;
        private set {
            if (_instance == null) {
                _instance = value;
            }
            else if (_instance != value) {
                Debug.Log("You messed up buddy.");
                Destroy(value);
            }
        }
    }

    
    public List<vfx_slicedmesh> slice_database;
    public GameObject p_meshSlice;



    void Awake()
    {
        Instance = this;
    }

    public void SetSliceEntry(string name, Mesh[] slices)
    {
        bool foundExistingEntry = false;
        for (int i = 0; i < slice_database.Count; i++)
        {
            if (slice_database[i].name == name)
            {
                foundExistingEntry = true;
                slice_database[i].slices = slices;
            }
        }

        if (!foundExistingEntry)
        {
            slice_database.Add(new vfx_slicedmesh(name, slices));
        }
    }

    public static void SpawnSlices(string object_name, Vector3 object_position, Quaternion object_rotation, Vector3 linear_velocity, float scale = 1f)
    {
        vfx_slicedmesh toSpawn = null;
        for (int i = 0; i < Instance.slice_database.Count; i++)
        {
            if (Instance.slice_database[i].name == object_name)
            {
                toSpawn = Instance.slice_database[i];
            }
        }


        if (toSpawn == null) {return;}


        for (int i = 0; i < toSpawn.slices.Length; i++)
        {
            GameObject g_newSlice = Instantiate(Instance.p_meshSlice);

            g_newSlice.GetComponent<MeshFilter>().sharedMesh = toSpawn.slices[i];
            g_newSlice.GetComponent<MeshCollider>().sharedMesh = toSpawn.slices[i];

            g_newSlice.GetComponent<Rigidbody>().linearVelocity = linear_velocity;

            g_newSlice.transform.position = object_position;
            g_newSlice.transform.rotation = object_rotation;
            g_newSlice.transform.localScale = Vector3.one * scale;
        }
    }
    

    public static void Play(string effect_name, Vector3 effect_position)
    {
        
    }
}
