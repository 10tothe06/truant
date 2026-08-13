using System.IO;
using UnityEngine;

// just allows me to see the progress on the chunk paths

[ExecuteInEditMode]
public class PathDebugger : MonoBehaviour
{
    [Header("LINE")]
    public Material m_path;

    public bool logPath;
    private LineRenderer rend;

    [Header("POINTS")]
    public bool togglePoints;
    private bool showPoints;

    void Update()
    {
        if (togglePoints)
        {
            togglePoints = false;

            showPoints = !showPoints;
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<MeshRenderer>().enabled = showPoints;
            }
        }

        if (logPath)
        {
            if (rend==null) rend = GetComponent<LineRenderer>();

            logPath = false;

            Vector3[] points = new Vector3[transform.childCount];

            for (int i = 0; i < transform.childCount; i++)
            {
                points[i] = transform.GetChild(i).position;
            }

            rend.positionCount = transform.childCount;
           
            rend.sharedMaterial = m_path;
            rend.SetPositions(points);
        }
    }
}
