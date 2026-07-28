using UnityEngine;
using UnityEngine.Events;

// a generic controller for accepting a player_keypresspacket, 
// which gets then sent to the ACTUAL player controller

// this script doesn't need to do much other than store the packet

public class player_genericcontroller : MonoBehaviour
{   
    [Header("CONSOLE")]
    public bool auto_update;

    public player_keypresspacket ins_mostRecentPacket;
    public player_keypresspacket mostRecentPacket {get; private set;}
    public player_keypresspacket oldPacket {get; private set;}



    public UnityEvent onPacketUpate;

    void Update()
    {
        if (auto_update)
        {
            AcceptKeyPresses(Input.GetKeypressPacket());
        }
    }

    public void AcceptKeyPresses(player_keypresspacket packet)
    {
        oldPacket = mostRecentPacket;

        
        mostRecentPacket = packet;
        ins_mostRecentPacket = packet;

        //Debug.Log("got key presses");

        onPacketUpate.Invoke(); // tell whoever that we got key presses
    }
}
