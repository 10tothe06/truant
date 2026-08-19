using TMPro;
using Unity.VisualScripting;
using UnityEngine;

// all notes, papers, etc. in the game are going to be run through this one class
// just makes life easier

public class int_note : MonoBehaviour
{
    private int_item item_comp;

    public string note_text;

    public TextMeshPro tx;

    void Awake()
    {
        if (note_text != null)
        {
            UpdateText(note_text);
        }

        item_comp = GetComponent<int_item>();

        if (item_comp != null)
        {
            item_comp.onDataUpdate.AddListener(UpdateFromItemData);
            item_comp.onInitialize.AddListener(UpdateItemData);
        }

        if (Player.item_holder != null)
        {
            // checking IF the item is being held by the player,
            // and IF SO, pass the note information to the note HUD
            Player.item_holder.onUpdateHeldObject.AddListener(OnItemHeld);
        }
    }

    // apply a new message to the note
    public void UpdateText(string text)
    {
        note_text = text;
        tx.text = note_text;
    }

    void UpdateFromItemData()
    {
        string text_data = item_comp.item_data.GetString("message");

        if (string.IsNullOrEmpty(text_data)) {return;}

        UpdateText(text_data);
    }

    void UpdateItemData()
    {
        if (item_comp == null) {return;}
        item_comp.item_data.SetData("message", note_text);
    }



    void OnItemHeld()
    {
        if (Player.GetHeldObject() == gameObject)
        {
            // make sure that the note HUD gets the right information
            ui_notehud.Instance.PassNoteText(gameObject, note_text);
        }
    }
}
