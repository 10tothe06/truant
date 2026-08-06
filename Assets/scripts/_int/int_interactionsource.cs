using UnityEngine;

public class int_interactionsource : MonoBehaviour
{
    public Transform src;

    private bool isDraggingObject;
    private Rigidbody objectToDrag;

    public float draggingDistance = 0.1f;
    public float dragForce = 10f;

    public float min_drag_force;
    public float max_drag_force;

    public float max_drag_distance;

    void Update()
    {
        if (isDraggingObject)
        {
            Vector3 dragDir = src.position + src.forward * draggingDistance - objectToDrag.transform.position;

            if (dragDir.magnitude > max_drag_distance)
            {
                StopDraggingObject();
            }   else
            {
                Vector3 force = dragDir * dragForce / objectToDrag.mass;
                //force *= Mathf.Pow(force.magnitude, 4);

                if (force.magnitude > max_drag_force)
                {
                    force = force.normalized * max_drag_force;
                }
                if (force.magnitude < min_drag_force)
                {
                    force = force.normalized * min_drag_force;
                }

                objectToDrag.linearVelocity = force;
            }
        }
    }

    public void StartDraggingObject(GameObject obj)
    {
        if (isDraggingObject) {return;} // don't want to call repeatedly
        
        objectToDrag = obj.GetComponent<Rigidbody>();

        if (objectToDrag == null) {return;}
    
        if (objectToDrag.GetComponent<obj_applyphysics>() != null)
        {
            objectToDrag.GetComponent<obj_applyphysics>().useGravity = false;
        }

        isDraggingObject = true;
    }

    public void StopDraggingObject()
    {
        if (objectToDrag == null) {return;}
        if (!isDraggingObject) {return;}
        
        if (objectToDrag.GetComponent<obj_applyphysics>() != null)
        {
            objectToDrag.GetComponent<obj_applyphysics>().useGravity = true;
        }

        isDraggingObject = false;
        objectToDrag = null;
    }
}
