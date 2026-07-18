using UnityEngine;
using UnityEngine.UI;

public class ui_sliderdisplay : MonoBehaviour
{
    public Slider sComp;
    public ui_instantiatable iComp;
    public float min;
    public float max;

    public void Setup(float min, float max)
    {
        this.min = min;
        this.max = max;
    }

    // updates the slider from the data that the instantiable component has been given
    // why does this function say DisplayData? what is the display?
    public void SetupFromDisplayData(string data)
    {
        string[] split = util_string.SplitByChar(iComp.displayInfo,',');

        min = float.Parse(split[0]);
        max = float.Parse(split[1]);

        sComp.value = (float.Parse(data) - min) / (max - min);
    }

    // setting up the unity events that pass data both ways
    void Awake()
    {
        // update settings comp from slider
        sComp.onValueChanged.AddListener((x) => {iComp.onDataUpdate.Invoke((min + x*(max-min)).ToString());});
        // update slider from settings comp
        iComp.onDataUpdate.AddListener((x) => SetupFromDisplayData(x));
    }
}
