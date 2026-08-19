using UnityEngine;
using UnityEngine.Events;

public enum CameraControlMode
{
    None,
    Freecam, // only a playtester/developer thing
    PlayerFirstPerson,
    MainMenu,
}

public class CameraController : MonoBehaviour
{
    private static CameraController _instance;

    public static CameraController Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
            {
                _instance = value;
            }
            else if (_instance != value)
            {
                Debug.Log("You messed up buddy.");
                Destroy(value);
            }
        }
    }

    void Awake()
    {
        Instance = this;

        t_cam = ins_t_cam;
        cam_main = ins_cam_main;
    }


    void Start()
    {
        default_fov = Settings.GetFloat("fov");

        target_fov = default_fov;
    }

    public Transform ins_t_cam;
    public static Transform t_cam;

    public static Camera cam_main;
    public Camera ins_cam_main;

    public ushort ins_controlMode;
    public static ushort controlMode;

    public static ushort previousControlMode;

    // to help with transitions
    public UnityEvent onChangeControlMode;
    public UnityEvent onCameraUpdate;



    // fov-related stuff
    // needs a bit of sysarch to make sure any sub-cameras obey the rules
    private float target_fov;
    [SerializeField]
    private float fov_lerp_speed;


    // TODO: make use of settings
    public static float default_fov = 60;


    public void UpdateCamera()
    { 
        onCameraUpdate.Invoke();


        // fov interpolation
        cam_main.fieldOfView = Mathf.Lerp(cam_main.fieldOfView, target_fov, fov_lerp_speed);
    }


    // basically a position/rotation reset command
    public static void ZeroOut()
    {
        Instance.transform.localPosition = Vector3.zero;
        t_cam.localPosition = Vector3.zero;

        Instance.transform.rotation =Quaternion.identity;
        t_cam.transform.rotation =Quaternion.identity;
    }
    public static void ZeroOutLocal()
    {
        Instance.transform.localPosition = Vector3.zero;
        t_cam.localPosition = Vector3.zero;

        Instance.transform.localRotation =Quaternion.identity;
        t_cam.transform.localRotation =Quaternion.identity;
    }


    public static void SetControlMode(CameraControlMode newMode)
    {
        SetControlMode((ushort)newMode);
    }
    public static void SetControlMode(ushort newMode)
    {
        if (newMode == 0) {Instance.transform.SetParent(null);}
        
        previousControlMode = controlMode;
        controlMode = newMode;
        Instance.ins_controlMode = controlMode;
        
        Instance.onChangeControlMode.Invoke();
    }

    public static void SetCameraFov(float target_fov, bool should_lerp = true)
    {
        Instance.target_fov = target_fov;

        if (!should_lerp)
        {
            // just set it immediately
            cam_main.fieldOfView = target_fov;
        }
    }
}
