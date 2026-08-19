using UnityEngine;
using UnityEngine.UI;

public class int_crosshair : MonoBehaviour
{
    [Header("CONFIG")]
    public bool color_switch = true;
    public Color interactColor;
    public Color defaultColor;

    [Space(8)]
    public bool sprite_switch = true;
    public Sprite sp_default;
    public Sprite sp_interactable;


    [SerializeField]
    private Image i_crosshair;

    void Awake()
    {
        if (i_crosshair == null)
        {
            // by default we just use the image component attached to the gameobject
            i_crosshair = GetComponent<Image>();
        }
    }

    public void SetInteractable()
    {
        if (sprite_switch) {i_crosshair.sprite = sp_interactable;}
        
        if (color_switch) {i_crosshair.color = interactColor;}
    }

    public void SetDefault()
    {
        if (sprite_switch) {i_crosshair.sprite = sp_default;}

        if (color_switch) {i_crosshair.color = defaultColor;}
    }
}
