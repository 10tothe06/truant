using TMPro;
using UnityEngine;

public class ui_mainmenubutton : MonoBehaviour
{
    private ui_navigable nav;

    public ui_snappable arrow;
    public ui_button button;

    public TextMeshProUGUI tx;

    void Awake()
    {
        // better this, I think, than setting everything in the inspector

        // many silly GetComponent<>() calls, but since this runs once im okay with it

        nav = GetComponent<ui_navigable>();
        if (nav != null)
        {
            nav.onSelect.AddListener(Select);
            nav.onDeselect.AddListener(Deselect);
        }

        if (button != null)
        {
            button.onHoverEnter.AddListener(Select);
            button.onHoverExit.AddListener(Deselect);
        }

        arrow.SetSnappingPoint(1);
    }

    public void Select()
    {
        arrow.SetSnappingPoint(0);

        ui_navigator.Instance.Clear();
        ui_navigator.Instance.Set(nav);

        tx.fontStyle = FontStyles.Underline;
    }

    public void Deselect()
    {
        arrow.SetSnappingPoint(1);

        tx.fontStyle = FontStyles.Normal;
    }
}
