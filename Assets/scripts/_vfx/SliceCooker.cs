using NUnit.Framework;
using UnityEngine;

public enum vfx_meshslicemode
{
    Shards,
    Parts,
}

// what an awesome name for a script, no?

public class SliceCooker : MonoBehaviour
{
    [SerializeField]
    private EffectManager effect_manager;



    [SerializeField]
    private bool is_active;

    public vfx_meshslicemode slice_mode;

    [Header("Control Panel")]
    public string entry_name;
    public Mesh original_mesh;
    [SerializeField]
    private bool slice;

    void Awake()
    {
        is_active = false;
    }

    void OnDrawGizmos()
    {
        if (is_active)
        {
            if (slice)
            {
                Mesh[] new_slices = new Mesh[] {};

                if (slice_mode == vfx_meshslicemode.Shards)
                {
                    new_slices = util_mesh.DiceMesh(original_mesh);
                } else if (slice_mode == vfx_meshslicemode.Parts)
                {
                    new_slices = util_mesh.DissasembleMesh(original_mesh);
                }

                effect_manager.SetSliceEntry(entry_name, new_slices);

                slice = false;
            }
        }
    }
}
