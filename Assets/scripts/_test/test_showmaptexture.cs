using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class test_showmaptexture : MonoBehaviour
{
    public float object_scale = 4f;

    void Awake()
    {
        GetComponent<RawImage>().enabled = false;
    }

    void Update()
    {
        if (Keyboard.current.jKey.wasPressedThisFrame)
        {
            GetComponent<RawImage>().enabled = true;
            GetComponent<RawImage>().texture = WorldManager.Instance.map_texture;

            GetComponent<RectTransform>().sizeDelta = new Vector2(WorldManager.Instance.map_texture.width * object_scale, WorldManager.Instance.map_texture.height * object_scale);
        }
    }
}
