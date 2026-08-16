using UnityEngine;

public class int_interactionsource : MonoBehaviour
{
    public int_objectcarrier carry_point;
    public Transform src;

    private bool isDraggingObject;
    public Rigidbody objectToDrag {get; private set;}

    public float draggingDistance = 0.1f;
    public float dragForce = 10f;

    public float min_drag_force;
    public float max_drag_force;

    public float max_drag_distance;

    public float object_rotation_speed = 0.5f;

    void Update()
    {
        Player.isDraggingObject = isDraggingObject;
        
        if (isDraggingObject)
        {
            Vector3 dragDir = src.position + src.forward * draggingDistance - objectToDrag.transform.position;

            if (Input.mouseButtonDownRight)
            {
                Player.LockCamera();
            }

            if (Input.mouseButtonRight)
            {
                // right-clicking to rotate the object

                Vector3 rot = CameraController.t_cam.right * Input.mouseMovement.y + CameraController.t_cam.up * -Input.mouseMovement.x;

                objectToDrag.transform.Rotate(rot * object_rotation_speed * Time.deltaTime / objectToDrag.mass, Space.World);
            } else {
                Player.UnlockAll();
            }

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
                objectToDrag.angularVelocity = Vector3.zero;
            }
        }
    }

    public void StartDraggingObject(GameObject obj)
    {
        if (carry_point.isCarryingObject) {return;}
        if (isDraggingObject) {return;} // don't want to call repeatedly
        
        objectToDrag = obj.GetComponent<Rigidbody>();

        if (objectToDrag == null) {return;}
    
        objectToDrag.useGravity = false;

        isDraggingObject = true;
    }

    public void StopDraggingObject()
    {
        if (objectToDrag == null) {return;}
        if (!isDraggingObject) {return;}
        
        objectToDrag.useGravity = true;

        isDraggingObject = false;
        objectToDrag = null;
    }
}
