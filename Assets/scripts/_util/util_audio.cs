using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class util_audio
{
    // returns a default clip if no match is found
    public static AudioClip GetClipFromMaterial(Material mat, audio_soundmaterial[] materials, audio_soundset defaultSet)
    {
        if (mat == null) { return defaultSet.Get(); }
        for (int i = 0; i < materials.Length; i++)
        {
            if (materials[i].applicableMaterials.Contains(mat))
            {
                return materials[i].sound.Get();
            }
        }

        return defaultSet.Get();
    }

    public static Material GetMaterialFromRay(Vector3 p, Vector3 r, float d, LayerMask mask)
    {
        // figuring out what material we're stepping on
        RaycastHit hit;
        if (Physics.Raycast(p, r, out hit, d, mask))
        {
            if (hit.collider.GetComponent<MeshCollider>() != null)
            {
                // mesh colliders can have multiple materials, so we need to look at submeshes
                Mesh m = hit.collider.GetComponent<MeshFilter>().sharedMesh;
                int desiredIndex = -1;

                if (m.subMeshCount == 1)
                {
                    desiredIndex = 0;
                }
                else
                {
                    for (int i = 0; i < m.subMeshCount; i++)
                    {
                        if (m.GetSubMesh(i).indexStart > hit.triangleIndex * 3)
                        {
                            desiredIndex = i - 1;
                            break;
                        }
                    }
                }

                if (desiredIndex != -1)
                {
                    return hit.collider.GetComponent<MeshRenderer>().sharedMaterials[desiredIndex];
                }

                // if the desired index remains -1 the material will stay null
            }
            else
            {
                // no mesh collider is gonna mean 1 material, so our job is easier

                return hit.collider.GetComponent<MeshRenderer>().sharedMaterial;
            }
        }
        else
        {
            // something has gone wrong (there is no floor??)
            return null;
        }

        return null;
    }
}
