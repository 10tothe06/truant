using UnityEngine;

public class poi_roadboundary : MonoBehaviour
{
    public Transform collider_a;
    public Transform collider_b;



    void Awake()
    {
        GetComponent<poi_generic>().onInitialize.AddListener(Initialize);
    }
    
    // keep in mind this is called RIGHT AFTER the generic script initializes
    public void Initialize()
    {
        // the main goal here is to place the two colliders (children of this object),
        // at either end of the road

        // to do this we simply split the road up into 2 segments,
        // and handle the intersections between each segment and the level boundary
        
        // the intersection points are where the colliders go

        Vector3 left_road_edge = MapData.road_points[0];
        Vector3 right_road_edge = MapData.road_points[1];
        Vector3 road_midpoint = (MapData.road_points[0] + MapData.road_points[1])/2f;

        // left point
        Vector3 point_a;
        if (util_polygon.SegmentsIntersect(left_road_edge, road_midpoint, WorldManager.Instance.d, WorldManager.Instance.c))
        {
            point_a = util_polygon.SegmentsIntersectPoint(left_road_edge, road_midpoint, WorldManager.Instance.d, WorldManager.Instance.c);
        } else
        {
            point_a = util_polygon.SegmentsIntersectPoint(left_road_edge, road_midpoint, WorldManager.Instance.d, WorldManager.Instance.a);
        }

        // right point
        Vector3 point_b;
        if (util_polygon.SegmentsIntersect(right_road_edge, road_midpoint, WorldManager.Instance.c, WorldManager.Instance.d))
        {
            point_b = util_polygon.SegmentsIntersectPoint(right_road_edge, road_midpoint, WorldManager.Instance.c, WorldManager.Instance.d);
        } else
        {
            point_b = util_polygon.SegmentsIntersectPoint(right_road_edge, road_midpoint, WorldManager.Instance.c, WorldManager.Instance.b);
        }
        

        collider_a.position = point_a;
        collider_b.position = point_b;
    }
}
