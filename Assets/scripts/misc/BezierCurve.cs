using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// CUBIC BEZIER CURVES
// so 4 control points

// this is how I'm making train tracks btw
[System.Serializable]
public class BezierCurve
{
    public List<Vector3> controlPoints;
    private float epsilon = 0.001f;

    // where is the curve located at a given percent
    public Vector3 GetPointFromPercent(float percent) {
        percent = Mathf.Clamp01(percent);

        List<Vector3> points = controlPoints;
        List<Vector3> midpoints = new List<Vector3>();
        while (points.Count > 1) {
            for (int i = 0; i < points.Count-1; i++) {
                midpoints.Add(Vector3.Lerp(points[i], points[i+1], percent));
            }

            points = midpoints;
            midpoints = new List<Vector3>();
        }

        return points[0];
    }

    // direct distance (not distance along curve) from one point to another
    public float DistanceBetweenPoints(float percent1, float percent2) {
        return Vector3.Distance(GetPointFromPercent(percent1), GetPointFromPercent(percent2));
    }

    // where is the curve "pointing" at a given percent
    public Vector3 GetForwardVectorFromPercent(float percent) {
        if (percent > 1 - epsilon) {
            return (GetPointFromPercent(percent) - GetPointFromPercent(percent - epsilon)).normalized;
        }
        else {
            return (GetPointFromPercent(percent + epsilon) - GetPointFromPercent(percent)).normalized;
        }
    }

    // approximating the curve using an array of points
    // useful when dealing with lengths and stuff
    public Vector3[] GetPointArray(int detailLevel) {
        Vector3[] toReturn = new Vector3[detailLevel];

        for (int i = 0; i < detailLevel; i++) {
            toReturn[i] = GetPointFromPercent(1f / (detailLevel - 1) * i);
        }

        return toReturn;
    }   

    public float GetLengthFromPercents(float percent1, float percent2, int detailLevel) {
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < detailLevel; i++) {
            points.Add(GetPointFromPercent(percent1 + (percent2 - percent1) / (detailLevel - 1) * i));
        }

        float length = 0;

        for (int i = 0; i < points.Count-1; i++) {
            length += Vector3.Distance(points[i], points[i+1]);
        }

        return length;
    }

    // this function sucks because of all the sqrt operations it has to do
    // TODO: either bake the lengths into the chunks so they're not calculated on the spot,
    // OR come up with a better solution
    public float GetLengthBad(int detailLevel) {
        Vector3[] points = GetPointArray(detailLevel);
        float length = 0;

        for (int i = 0; i < points.Length-1; i++) {
            length += Vector3.Distance(points[i], points[i+1]);
        }

        return length;
    }
}
