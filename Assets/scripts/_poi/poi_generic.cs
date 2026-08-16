using UnityEngine;
using UnityEngine.Events;

public class poi_generic : MonoBehaviour
{
    [Header("MAP ICON SETTINGS")]
    public bool should_appear_on_map;

    // will choose a default map icon if null
    public Texture2D map_icon;



    [Header("PLACEMENT SETTINGS")]
    // if no, then the poi will be selected from the pool as normal (by every level)
    // if yes, then it will NOT be selected unless the level specifically requests it by name
    public bool is_special;
    // does the POI need to be placed on the shore of the lake?
    public bool must_be_lakeside;
    // does the POI HAVE to be placed? (meaning it is selected from the pool always)
    // if marked as 'special' and not requested, it will not be placed
    // the 'mandatory' only applies if its requested in that case
    public bool is_mandatory;

    public float exclusion_radius;
    public Transform exclusion_debug_object;

    
    [Header("UNITY EVENTS")]
    public UnityEvent onInitialize;


    // cached values
    public Vector2 map_position;

    

    void OnDrawGizmos()
    {
        if (exclusion_debug_object != null)
        {
            // visualizing the exclusion radius with a cirlce

            exclusion_debug_object.localScale = Vector3.one * exclusion_radius * 2;
        }
    }

    
    // called by the world manager when the object is spawned in
    public void Initialize()
    {
        HandlePlacement();

        HandleMapIcon();



        // some scripts rely on the POI being in its final position,
        // so this event needs to be called after ALL intialization logic is done
        onInitialize.Invoke();
    }

    // making sure that the POI appears on the in-game map
    // (if we want it to)
    private void HandleMapIcon()
    {
        if (!should_appear_on_map) {return;} // no map icon or anything at all

        Texture2D texture_to_write = (map_icon != null) ? map_icon : WorldManager.Instance.default_map_icon;

        WorldManager.Instance.map_texture = util_texture.WriteTextureOnTop(WorldManager.Instance.map_texture, util_map.MapUVToPixelPosition(map_position), texture_to_write, 0.125f/2f);
    }

    private void HandlePlacement()
    {
        // we start with just a random position
        map_position = util_map.GetRandomUVPositionInMap();

        transform.position = util_map.MapUVToWorldPosition(map_position);


        if (must_be_lakeside)
        {
            // every poi starts off with a random placement,
            // but some POIs NEED to be on the shoreline

            // so we gotta take the position that we're given and move it until its by the lake
            // we're doing this in a bit of a silly way
            
            transform.position = util_map.GetLakesidePosition(transform.position);
            map_position = util_map.WorldToMapUVPosition(transform.position);
        } else
        {
            // do nothing because we already got a random position
        }
    }
}
