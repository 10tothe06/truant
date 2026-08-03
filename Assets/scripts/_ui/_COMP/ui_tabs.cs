using UnityEngine;
using UnityEngine.UI;

// TODO: make this more automatic, generating the tabs for you

public class ui_tabs : MonoBehaviour
{
    public Button[] tabObjects;

    public GameObject[] connectedObjects; // the things to actually hide and show based on what tab is active

    public int currentIndex;

    public bool useColorSwap;
    public Color selectedColor;
    public Color defaultColor;

    void Awake()
    {
        AssignTabActions();

        SetTabIndex(currentIndex);  // making sure it is visually set up properly
        if (useColorSwap)
        {
             
            UpdateTabColors();
        }
    }

    public void AssignTabActions()
    {
        for (int i = 0; i < tabObjects.Length; i++)
        {
            int j = i;
            tabObjects[i].onClick.AddListener(() => SetTabIndex(j));
        }
    }

    public void UpdateTabColors()
    {
        for (int i = 0; i < tabObjects.Length; i++)
        {
            if (i == currentIndex)
            {
                tabObjects[i].GetComponent<Image>().color = selectedColor;
            } else
            {
                tabObjects[i].GetComponent<Image>().color = defaultColor;
            }
        }
    }

    public void SetTabIndex(int index)
    {
        for (int i = 0; i < connectedObjects.Length; i++)
        {
            if (i == index)
            {
                connectedObjects[i].SetActive(true);
            } else
            {
                connectedObjects[i].SetActive(false);
            }
        }

        currentIndex = index;

        UpdateTabColors();
    }
}
