using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ui_button : MonoBehaviour
{
    private bool isPressed;
    public int buttonToLookFor; // left, right, or middle click

    [Space(6)]
    [Header("Interactions")]
    public UnityEvent onPress; // Click (runs once)
    public UnityEvent onDrag; // Click and then move mouse away (runs once)
    public UnityEvent whilePress; // Hold click (repeats)

    [Space(6)]
    [Header("Color Settings")]
    public bool colorSwitch;
    public Color defaultColor;
    public Color hoverColor;
    public Color pressedColor;

    void Update() {
        if (Input.GetMouseButtonDown(buttonToLookFor) && ui_canvasutils.IsCursorInteract(gameObject, true)) {
            isPressed = true;
        }

        if (isPressed) {
            whilePress.Invoke(); // Invoke the event that runs when the button is held

             if (colorSwitch) { GetComponent<Image>().color = pressedColor; }
        }
        else if (ui_canvasutils.IsCursorInteract(gameObject, true)) {
            if (colorSwitch) { GetComponent<Image>().color = hoverColor; }
        }
        else {
            if (colorSwitch) { GetComponent<Image>().color = defaultColor; }
        }   

        if (!Input.GetMouseButton(buttonToLookFor)) {
            if (isPressed && ui_canvasutils.IsCursorInBounds(gameObject, true)) { onPress.Invoke(); }
            isPressed = false;
        }

        if (isPressed && !ui_canvasutils.IsCursorInteract(gameObject, true)) {
            onDrag.Invoke(); // Invoke the unity event that runs when you click and drag the button
            isPressed = false;
        }
    }

    public void TestButton() {
        Debug.Log("Test Succesful?");
    }
}