using UnityEngine;
using UnityEngine.Events;

public class ui_navigable : MonoBehaviour
{
    public bool is_default;
    [SerializeField]
    public bool is_selected {get; private set;}

    public ui_navigable n_up; // w
    public ui_navigable n_down; // s
    public ui_navigable n_right; // d
    public ui_navigable n_left; // a

    [Space(20)]
    [Header("EVENTS")]
    public UnityEvent onSelect;
    public UnityEvent onDeselect;
    public UnityEvent onConfirmSelection;

    public ui_button button;

    void Start()
    {
        Initialize();
    }

    private void OnEnable()
    {
        Initialize();
    }

    private void Initialize()
    {
        Deselect();

        if (is_default)
        {
            if (ui_navigator.Instance != null) {ui_navigator.Instance.Set(this);}
        }
    }

    public void ConfirmSelection()
    {
        onConfirmSelection.Invoke();

        if (button != null)
        {
            button.onPress.Invoke();
        }
    }


    public void Select()
    {
        onSelect.Invoke();
        is_selected = true;
    }
    public void Deselect()
    {
        onDeselect.Invoke();
        is_selected = false;
    }
}
