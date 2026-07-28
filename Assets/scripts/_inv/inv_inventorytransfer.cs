using UnityEngine;

// struct for storing inventory transfers (sent across the network)
// this is the ONLY way that inventories are updated

[System.Serializable]
public class inv_inventorytransfer
{
    int origin_entity_index;
    int origin_part_index;
    int origin_cell_index;
    int origin_item_count;



    int destination_entity_index;
    int destination_part_index;
    int destination_cell_index;
    int destination_rotation_index;
}
