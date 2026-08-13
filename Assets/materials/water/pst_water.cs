using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pst_water : MonoBehaviour
{
    private static pst_water _instance;

    public static pst_water Instance
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
        
        if (lightTransform == null)
        {
            lightTransform = GameObject.Find("<light>").transform;
        }
    }

    public Material m_water;

    public Color waterColor;

    public RenderTexture waterDepthTex;
    public RenderTexture waterColorTex;
    private float[] waveAngles;

    public MeshRenderer waterObj;
    public float baseWaveFrequency;
    public Transform lightTransform;
    public bool isUnderwater;

    void Start()
    {
        waveAngles = new float[] { -0.68f, 1.45f, -2.59f, 1, -2, 0.5f, 3, 5, 0.25f, -1.53f, 5, 9 };
    }
    
    void Update()
    {
        // m_water.SetVector("waterCol", new Vector3(waterColor.r, waterColor.g, waterColor.b));
        // m_water.SetTexture("_WaterColor", waterColorTex);
        // m_water.SetTexture("_WaterDepth", waterDepthTex);
        // m_water.SetFloatArray("waveAngles", waveAngles);
        // m_water.SetFloat("baseWaveFrequency", baseWaveFrequency);
        // m_water.SetVector("sunDir", -lightTransform.forward);
        // m_water.SetFloat("timeValue", Time.time);

        // if (GetHeight(transform.position) > transform.position.y)
        // {
        //     if (!isUnderwater) { Debug.Log("Went under!"); waterObj.GetComponent<PlaneGenerator>().Generate(true); }
        //     isUnderwater = true;
        // } else
        // {
        //     if (isUnderwater) { Debug.Log("Went out!"); waterObj.GetComponent<PlaneGenerator>().Generate(false);}
        //     isUnderwater = false;
        // }

        waterObj.sharedMaterial.SetFloatArray("waveAngles", waveAngles);
        waterObj.sharedMaterial.SetFloat("baseWaveFrequency", baseWaveFrequency);
        waterObj.sharedMaterial.SetVector("sunDir", -lightTransform.forward);

        waterObj.sharedMaterial.SetInt("isUnderWater", isUnderwater ? 1 : 0);
        
        waterObj.sharedMaterial.SetFloat("timeValue", Time.time);
    }

    void OnRenderImage(RenderTexture source, RenderTexture mod)
    {
        Graphics.Blit(source, mod, m_water);
    }

    public static float GetHeight(Vector3 pos) {
        float a = 50;
        float f = 0.05f;

        float sum = 0;
        int iterations = 12;

        for (int i = 0; i < iterations; i++) {
            float angle = Instance.waveAngles[i];

            float x = Mathf.Cos(angle) * (pos.x * Instance.baseWaveFrequency) - Mathf.Sin(angle) * (pos.z * Instance.baseWaveFrequency);
            x += Time.time*4;
            
            sum += Mathf.Pow(2.71828f, Mathf.Sin(x*f) - 1) * a;

            f *= 1.5f;
            a = Mathf.Lerp(a, 1, 0.5f);
        }

        return sum/iterations;
    }
}
