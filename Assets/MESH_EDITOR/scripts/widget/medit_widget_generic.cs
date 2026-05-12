using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
// in case I need an interface between other scripts and widgets
public class medit_widget_generic : MonoBehaviour
{
    [Header("CONSOLE")]
    public bool refreshWidgets;
    public medit_object obj;
    public medit_widgettype type;
    public bool select;
    private bool isSelected;

    void Update()
    {
        if (select && !isSelected)
        {
            isSelected = true;
            // select the widget
            obj.selectedEdges.Add(this);

            SetWidgetIcon(medit_main.Instance.ins_widgetIcons[(ushort)type + 8]);
        } else if (!select && isSelected)
        {
            isSelected = false;
            // un-select the widget on the object
            obj.selectedEdges.Remove(this);

            ushort numType = (ushort)type;
            if (numType == 1)
            {
                SetWidgetIcon(medit_main.Instance.ins_widgetIcons[1]);
            } else if (numType == 2)
            {
                SetWidgetIcon(medit_main.Instance.ins_widgetIcons[7]);
            } else if (numType == 3)
            {
                SetWidgetIcon(medit_main.Instance.ins_widgetIcons[8]);
            }
        }



        if (refreshWidgets)
        {
            refreshWidgets = false;
            obj.UpdateMeshWidgetPositions(obj.mf.sharedMesh, type, transform.GetSiblingIndex());
        }
    }

    // wrapper in case i need more logic here, which is very possible
    public void SetWidgetIcon(Texture2D icon)
    {
        EditorGUIUtility.SetIconForObject(gameObject, icon);
    }
}
