using Unity.VisualScripting;
using UnityEngine;

public class cam_firstperson : MonoBehaviour
{
    public LayerMask cullingMask; 

    [HideInInspector]
    public Transform t_controlling;
    [HideInInspector]
    public PlayerController controller;

    void Start()
    {
        CameraController.Instance.onChangeControlMode.AddListener(ProcessChangeInControlMode);
        CameraController.Instance.onCameraUpdate.AddListener(CameraUpdate);
    }

    // ************************************
    public void ProcessChangeInControlMode()
    {
        if (CameraController.controlMode == (ushort)CameraControlMode.PlayerFirstPerson)
        {
            EnterControl();
        } else if (CameraController.previousControlMode == (ushort)CameraControlMode.PlayerFirstPerson){ExitControl();}
    }

    public void SetControllingObject(GameObject g_toControl)
    {
        t_controlling = g_toControl.transform;
        controller = g_toControl.GetComponent<PlayerController>();
    }

    public void EnterControl()
    {
        SetControllingObject(Player.t.gameObject);

        CameraController.cam_main.cullingMask = cullingMask;
        transform.SetParent(t_controlling.GetChild(0));
        
        CameraController.ZeroOutLocal();

        Cursor.lockState = CursorLockMode.Locked;
        
    }
    public void ExitControl()
    {
        
    }
    // ************************************

    // because Update() isn't rlly available for anyone except Program.cs
    void CameraUpdate()
    {
        // make sure the camera mode is right
        // otherwise we'd just be running this constantly
        if (CameraController.Instance.ins_controlMode == (ushort)CameraControlMode.PlayerFirstPerson)
        {

            // the zooming in can easily just be a local thing,
            // not network-relevant
            if (Input.mouseButtonRight)
            {
                CameraController.SetCameraFov(20);
            } else
            {
                CameraController.SetCameraFov(60);
            }
        }
    }
}
