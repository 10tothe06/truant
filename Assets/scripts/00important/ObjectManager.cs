using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    private static ObjectManager _instance;

    public static ObjectManager Instance
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

    public GameObject[] p_objects;

    public Transform t_currentObjectContainer;


    public static GameObject GetItemObject(inv_itemdata item_data)
    {
        if (item_data == null) {return null;}
        for (int i = 0; i < Instance.p_objects.Length; i++)
        {
            if (Instance.p_objects[i].name == item_data.item_name)
            {
                return Instance.p_objects[i];
            }
        }

        Debug.LogWarning("tried to get the object for an item and it didnt exist");
        return null;
    }



    // there are no prefixes or anything to worry about here,
    // the prefab gameobject names are VERBATIM the object names
    // (as opposed to doing 'e_' at the start or 'obj_')
    public static GameObject GetObjectPrefabFromName(string object_name)
    {
        for (int i = 0; i < Instance.p_objects.Length; i++)
        {
            if (Instance.p_objects[i].name == object_name)
            {
                return Instance.p_objects[i];
            }
        }

        Debug.LogWarning("Tried to spawn an object that (apparently) doesn't exist!");

        return null;
    }

    // does exactly what the name would suggest
    // even just having a comment here is futile, really

    // return value is just so that other scripts can run logic without having to find the object
    public static GameObject SpawnObject(string object_name, Vector3 spawn_position)
    {
        // just a wrapper
        return SpawnObject(object_name, spawn_position, Vector3.zero);
    }
    public static GameObject SpawnObject(string object_name, Vector3 spawn_position, Vector3 spawn_euler_angles)
    {
        if (GetObjectPrefabFromName(object_name) == null) {return null;}


        GameObject g_newObject = Instantiate(GetObjectPrefabFromName(object_name), Instance.t_currentObjectContainer);
        // get rid of the fucking '(Clone)' text i hate that shit
        g_newObject.name = GetObjectPrefabFromName(object_name).name;

        g_newObject.transform.position = spawn_position;

        return g_newObject;
    }
}
