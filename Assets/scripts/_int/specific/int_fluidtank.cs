using UnityEngine;

// just a REALLY simplified version of LS's material/resource system,
// i guess



// side note:
// in My Summer/Winter car you can combine fluids (iirc) and ditto with The Long Drive
// this means you could have a gas tank full of water, alcohol, gas, oil, or all of the above at once
// we are NOT doing this because of the lesser information given to the player
// (they wouldn't know and would be stuck wondering what's wrong)


// anyways only one fluid at a time per tank

public class int_fluidtank : MonoBehaviour
{
    public float tank_fill_level; // how much fluid IS in the tank
    public float tank_capacity; // how much fluid can fit in the tank


    public string fluid_name;

    private int_item item_comp;

    void Awake()
    {
        item_comp = GetComponent<int_item>();

        if (item_comp != null)
        {
            item_comp.onDataUpdate.AddListener(UpdateFromItemData);
            item_comp.onInitialize.AddListener(UpdateItemData);
        }
    }

    public void AdjustFluidLevel(float amt)
    {
        tank_fill_level += amt;

        // make sure that the tank doesn't go negative or above cap
        tank_fill_level = Mathf.Clamp(tank_fill_level, 0, tank_capacity);


        UpdateItemData();
    }


    // if called we already know there is a comp,
    // so no need for a null chceck
    public void UpdateFromItemData()
    {
        tank_fill_level = item_comp.item_data.GetFloat("tank_fill_level");
    }

    private void UpdateItemData()
    {
        if (item_comp == null) {return;}
        item_comp.item_data.SetData("tank_fill_level", tank_fill_level);
    }
}
