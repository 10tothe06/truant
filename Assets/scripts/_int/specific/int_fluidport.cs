using UnityEngine;
using UnityEngine.InputSystem;

public class int_fluidport : MonoBehaviour
{
    public int_fluidtank connected_tank; // the tank that the port is attached to
    private int_fluidtank temporary_tank; // the tank that the player is holding

    private bool is_adding_fluid; // is the player in the process of pouring

    public float fluid_transfer_per_second = 25;

    void Awake()
    {
        GetComponent<InteractableObject3D>().onInteract.AddListener(TryAddingFluidFromPlayer);
    }

    public void TryAddingFluidFromPlayer()
    {
        // first, get the object that the player is holding
        InteractableObject3D io = Player.GetHeldObject().GetComponent<InteractableObject3D>();

        // note that the object the player is holding should have the proper data,
        // as it syncs with the object in the player's inventory

        if (io == null) {return;}

        int_fluidtank comp = io.GetComponent<int_fluidtank>();

        if (comp != null)
        {
            // fluids have to be the same type
            if (comp.fluid_name != connected_tank.fluid_name) {return;}

            StartAddingFluid(comp);
        }
    }

    public void StopAddingFluid()
    {
        is_adding_fluid = false;
    }

    public void StartAddingFluid(int_fluidtank tank_to_add_from)
    {
        is_adding_fluid = true;

        temporary_tank = tank_to_add_from;
    }
    
    private void Update()
    {
        if (is_adding_fluid)
        {
            if (!Keyboard.current.eKey.isPressed)
            {
                // player no longer holding the button, so we stop
                StopAddingFluid();
            }

            // pick whichever is less: the space we have or the fluid we have
            float fluid_to_transfer = Mathf.Min(connected_tank.tank_capacity - connected_tank.tank_fill_level, temporary_tank.tank_fill_level);

            // rate limiting
            fluid_to_transfer = Mathf.Min(fluid_to_transfer, fluid_transfer_per_second * Time.deltaTime);

            temporary_tank.AdjustFluidLevel(-fluid_to_transfer);
            connected_tank.AdjustFluidLevel(fluid_to_transfer);
        }
    }
}
