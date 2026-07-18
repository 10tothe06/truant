using UnityEngine;
using UnityEngine.Events;

public class ui_navigable : MonoBehaviour
{
    public bool is_default;

    public ui_navigable n_up; // w
    public ui_navigable n_down; // s
    public ui_navigable n_right; // d
    public ui_navigable n_left; // a

    [Space(20)]
    [Header("EVENTS")]
    public UnityEvent onSelect;
    public UnityEvent onDeselect;

    void Start()
    {
        if (is_default)
        {
            ui_navigator.Instance.Set(this);
        }
    }

    public void Select()
    {
        onSelect.Invoke();
    }
    public void Deselect()
    {
        onDeselect.Invoke();
    }
}
