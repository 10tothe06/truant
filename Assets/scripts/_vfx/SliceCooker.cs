using NUnit.Framework;
using UnityEngine;

// what an awesome name for a script, no?

public class SliceCooker : MonoBehaviour
{
    [SerializeField]
    private EffectManager effect_manager;



    [SerializeField]
    private bool is_active;

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
                Mesh[] new_slices = util_mesh.DiceMesh(original_mesh);

                effect_manager.SetSliceEntry(entry_name, new_slices);

                slice = false;
            }
        }
    }
}
