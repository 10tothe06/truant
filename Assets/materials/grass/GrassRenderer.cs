using UnityEngine;
using UnityEngine.Rendering;

public class GrassRenderer : MonoBehaviour
{
    [Header("References")]
    public Mesh grassMesh;
    public Material grassMaterial;
    public ComputeShader grassCompute;

    [Header("Settings")]
    public int bladeCount = 150000;
    public float drawDistance = 70f;
    public float windStrength = 0.45f;
    public Vector2 windDirection = new Vector2(1f, 0.35f);

    ComputeBuffer allBladesBuffer;
    ComputeBuffer visibleBladesBuffer;
    ComputeBuffer argsBuffer;

    int kernelGenerate;
    int kernelCull;

    uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
    Bounds renderBounds;

    MaterialPropertyBlock propertyBlock;

    void OnEnable()
    {
        if (grassMesh == null)
            grassMesh = GrassBladeMeshGenerator.GenerateBlade();

        kernelGenerate = grassCompute.FindKernel("CSMain");
        kernelCull     = grassCompute.FindKernel("CSCull");

        propertyBlock = new MaterialPropertyBlock();
    }

    public void Initialize()
    {
        InitBuffers();
        GenerateGrass();
    }

    void OnDisable() => ReleaseBuffers();

    void InitBuffers()
    {
        ReleaseBuffers();

        int stride = sizeof(float) * 8;
        allBladesBuffer     = new ComputeBuffer(bladeCount, stride);
        visibleBladesBuffer = new ComputeBuffer(bladeCount, stride, ComputeBufferType.Append);
        argsBuffer          = new ComputeBuffer(1, sizeof(uint) * 5, ComputeBufferType.IndirectArguments);

        args[0] = grassMesh.GetIndexCount(0);
        args[1] = 0;
        args[2] = grassMesh.GetIndexStart(0);
        args[3] = grassMesh.GetBaseVertex(0);
        args[4] = 0;
        argsBuffer.SetData(args);

        renderBounds = new Bounds(transform.position, Vector3.one * WorldManager.Instance.chunkSize);
    }

    void GenerateGrass()
    {
        grassCompute.SetBuffer(kernelGenerate, "_AllBlades", allBladesBuffer);
        grassCompute.SetInt("_BladeCount", bladeCount);
        grassCompute.SetFloat("_AreaSize", WorldManager.Instance.chunkSize);
        
        grassCompute.SetFloat("_NoiseRange", WorldManager.level_noise.noise_range);

        grassCompute.SetVector("_Position", new Vector3(transform.position.x, 0, transform.position.z));
        Texture2D tex = WorldManager.level_noise.GenerateTexture(64, transform.position);
        grassCompute.SetTexture(kernelGenerate, "_ChunkTexture", tex);

        //transform.parent.GetComponent<MeshRenderer>().material.mainTexture = tex;

        int groups = Mathf.CeilToInt(bladeCount / 256f);
        grassCompute.Dispatch(kernelGenerate, groups, 1, 1);
    }

    void Update()
    {
        if (allBladesBuffer == null || grassMaterial == null || Camera.main == null)
            return;

        // ---- Cull ----
        visibleBladesBuffer.SetCounterValue(0);

        Matrix4x4 vp = Camera.main.projectionMatrix * Camera.main.worldToCameraMatrix;
        grassCompute.SetMatrix("_VPMatrix", vp);
        grassCompute.SetVector("_CameraPos", Camera.main.transform.position);
        grassCompute.SetFloat("_DrawDistance", drawDistance);
        grassCompute.SetInt("_BladeCount", bladeCount);
        grassCompute.SetBuffer(kernelCull, "_AllBlades", allBladesBuffer);
        grassCompute.SetBuffer(kernelCull, "_VisibleBlades", visibleBladesBuffer);

        int groups = Mathf.CeilToInt(bladeCount / 256f);
        grassCompute.Dispatch(kernelCull, groups, 1, 1);

        ComputeBuffer.CopyCount(visibleBladesBuffer, argsBuffer, sizeof(uint));

        // ---- Material ----
        propertyBlock.Clear();                                      
        propertyBlock.SetBuffer("_GrassBlades", visibleBladesBuffer);
        propertyBlock.SetFloat("_WindStrength", windStrength);
        propertyBlock.SetVector("_WindDirection", windDirection);

        // ---- Draw ----
        Graphics.DrawMeshInstancedIndirect(
            grassMesh,
            0,
            grassMaterial,
            renderBounds,
            argsBuffer,
            0,
            propertyBlock,
            ShadowCastingMode.On,
            true,
            gameObject.layer
        );
    }

    void ReleaseBuffers()
    {
        allBladesBuffer?.Release();
        visibleBladesBuffer?.Release();
        argsBuffer?.Release();
        allBladesBuffer = visibleBladesBuffer = argsBuffer = null;
    }
}