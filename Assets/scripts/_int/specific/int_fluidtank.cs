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
    public float tank_capacity;
    public string fluid_name;
}
