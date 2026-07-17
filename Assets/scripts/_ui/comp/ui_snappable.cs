using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ui_snappable : MonoBehaviour
{
    public bool canBeInteractedWith = true;
    public bool canBeCycledThrough = false; // Tab to cycle right now
    public Transform[] snappingPoints;

    public UnityEvent<int> onChangeIndex;

    private bool isHolding;
    private Vector3 targetPos;
    public float handleMoveSpeed;

    public int current_index;

    void Awake()
    {
        targetPos = transform.position;
    }

    public void SetSnappingPoint(int index)
    {
        if (index < 0 || index > snappingPoints.Length-1) {return;}

        current_index = index;
        targetPos = snappingPoints[index].position;
        onChangeIndex.Invoke(current_index);
    }

    private void Update()
    {
        if (canBeCycledThrough)
        {
            if (Keyboard.current.tabKey.wasPressedThisFrame)
            {
                if (current_index < snappingPoints.Length - 1)
                {
                    SetSnappingPoint(current_index + 1);
                } else
                {
                    SetSnappingPoint(0);
                }
            }
        }

        if (canBeInteractedWith)
        {
            if (Input.mouseButtonDownLeft && ui_canvasutils.IsCursorInteract(gameObject, true))
            {
                isHolding = true;
            }
            
            if (!Input.mouseButtonLeft)
            {
                isHolding = false;
            }
        } else
        {
            isHolding = false;
        }

        if (isHolding)
        {
            Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);

            // basically: teleport the handle to the nearest snapping point
            int kingIndex = 0;
            float kingDist = 9999;
            for (int i = 0; i < snappingPoints.Length; i++)
            {
                if (Vector3.Distance(mousePos, snappingPoints[i].position) < kingDist)
                {
                    kingIndex = i;
                    current_index = i;
                    onChangeIndex.Invoke(current_index);
                    kingDist = Vector3.Distance(mousePos, snappingPoints[i].position);
                }
            }

            Vector3 drag = (mousePos - snappingPoints[kingIndex].position);
            drag -= Vector3.Project(drag, Vector3.up);
            targetPos = snappingPoints[kingIndex].position +  drag* 0.25f;
        } else
        {
            targetPos = snappingPoints[current_index].position;
        }

        transform.position = Vector3.Lerp(transform.position, targetPos, handleMoveSpeed);
    }
}
