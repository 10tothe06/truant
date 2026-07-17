using UnityEngine;
using UnityEngine.UI;

// weird title - but specific

public class ui_imagewave : MonoBehaviour
{
    public Sprite[] images;
    public GameObject p_img;
    public Transform t_imageContainer;
    
    public float imageSpacing;
    public Vector3 axis;

    public float freq;
    public float amp;

    public float spin_freq;
    public float spin_amp;

    public float scale_freq;
    public float scale_amp;

    void Start()
    {
        axis = axis.normalized;
        Initialize();
    }

    public void Initialize()
    {
        for (int i = 0; i < images.Length; i++)
        {
            GameObject g_newImg = Instantiate(p_img, t_imageContainer);

            g_newImg.GetComponent<Image>().sprite = images[i];
            g_newImg.GetComponent<RectTransform>().sizeDelta = new Vector2(images[i].bounds.size.x, images[i].bounds.size.y)*100f;

            g_newImg.transform.localPosition = axis * imageSpacing * i;

            ui_bob comp = g_newImg.GetComponent<ui_bob>();

            comp.freq = freq;
            comp.amp = amp;

            comp.bobAxis = Vector3.up;
            comp.animate = true;

            comp.offset = Random.Range(-10f, 10f);

            ui_bobspin comp2 = g_newImg.GetComponent<ui_bobspin>();
            comp2.freq = spin_freq;
            comp2.amp = spin_amp;
            comp2.offset = Random.Range(-10f, 10f);
            comp2.animate = true;

            ui_bobstretch comp3 = g_newImg.GetComponent<ui_bobstretch>();
            comp3.freq = scale_freq;
            comp3.amp = scale_amp;
            comp3.offset = Random.Range(-10f, 10f);
            comp3.animate = true;
        }
    }
}
