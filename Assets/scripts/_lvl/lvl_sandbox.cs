using UnityEngine;

public class lvl_sandbox : MonoBehaviour
{
    private lvl_generic g;

    public Transform t_playerSpawn;

    void Awake()
    {
        g =GetComponent<lvl_generic>();

        g.onLevelEnter.AddListener(OnLevelEnter);
    }


    public void OnLevelEnter()
    {
        UIManager.SwitchMenu("");
        // set the camera mode
        CameraController.SetControlMode(CameraControlMode.PlayerFirstPerson);

        UIManager.ShowPlayerHUD();

        Player.TeleportTo(t_playerSpawn.position);
    }
}
