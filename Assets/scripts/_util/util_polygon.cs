// struct representing a piece of a polygon,
// so that we can keep track of vertex indices

// (UNUSED RIGHT NOW)
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class util_polygoncomponent
{
    public Vector2[] vertices;
    public Vector2[] actualVertexIndices;

    public util_polygoncomponent() {}

    public util_polygoncomponent(Vector2[] vertices, Vector2[] actualVertexIndices)
    {
        this.vertices = vertices;
        this.actualVertexIndices = actualVertexIndices;
    }
}

public class util_polygon
{
    public static Vector2[] Vector3ToVector2(Vector3[] raw)
    {
        Vector2[] result = new Vector2[raw.Length];

        for (int i = 0; i < raw.Length; i++)
        {
            result[i] = new Vector2(raw[i].x, raw[i].z);
        }

        return result;
    }
    public static Vector3[] Vector2ToVector3(Vector2[] raw)
    {
        Vector3[] result = new Vector3[raw.Length];

        for (int i = 0; i < raw.Length; i++)
        {
            result[i] = new Vector3(raw[i].x, 0, raw[i].y);
        }

        return result;
    }

    // count up the reflex angles either way, whichever way has less reflex angles is the proper way
    public static bool IsPolygonClockwise(Vector3[] verts)
    {
        bool[] reflexAnglesIfClockwise = IdentifyReflexAnglesWhenClockwise(verts);
        int trueSum = 0;
        int falseSum = 0;
        for (int i = 0; i < reflexAnglesIfClockwise.Length; i++)
        {
            if (reflexAnglesIfClockwise[i])
            {
                trueSum++;
            } else
            {
                falseSum++;
            }
        }

        if (trueSum > falseSum)
        {
            return false;
        } else
        {
            return true;
        }
    }

    public static bool[] IdentifyReflexAnglesWhenClockwise(Vector3[] verts)
    {
        bool[] result = new bool[verts.Length];

        Vector3 normal = Vector3.up;
        for (int i = 0; i < verts.Length; i++)
        {
            Vector3 v1 = GetPreceedingVertex(verts, i);
            Vector3 v2 = verts[i];
            Vector3 v3 = GetSucceedingVertex(verts, i);

            Vector3 a = v2 - v1;
            Vector3 b = v3 - v2;

            float cAngle = GetCounterClockwiseAngle(a, b, normal);

            if (cAngle >= 180)
            {
                result[i] = true;
            }
        }

        return result;
    }

    public static Vector3[] MakePolygon(int[] indices, Vector3[] pool)
    {
        Vector3[] result = new Vector3[indices.Length];

        for (int i = 0; i < result.Length; i++)
        {
            result[i] = pool[indices[i]];
        }

        return result;
    }

    // same data format as a mesh
    public static float CalculateArea(Mesh m)
    {
        return CalculateArea(m.vertices, m.triangles);
    }
    public static float CalculateArea(Vector3[] verts, int[] tris)
    {
        float sum = 0;

        for (int i = 0; i < tris.Length; i+=3)
        {
            Vector3 a = verts[tris[0]] - verts[tris[1]];
            Vector3 b = verts[tris[2]] - verts[tris[1]];

            float area = Vector3.Cross(a, b).magnitude / 2f;
            sum += area;
        }

        return sum;
    }

    public static int[] GetCollinears(Vector3[] points)
    {
        List<int> result = new List<int>();

        for (int i = 0; i < points.Length; i++)
        {
            Vector3 primary = points[i]; // just for readability

            for (int j = 0; j < points.Length; j++)
            {
                if (i != j) // can't be the same point
                {
                    Vector3 secondary = points[j]; // just for readability

                    for (int k = 0; k < points.Length; k++)
                    {
                        if (k != i && k != j)
                        {
                            if (!result.Contains(k) && Vector3.Dot((primary - points[k]).normalized, (secondary - points[k]).normalized) < -0.95f)
                            {
                                result.Add(k);
                            }
                        }
                    }
                }
            }
        }

        return result.ToArray();
    }

    public static Vector3[] RemoveCollinears(Vector3[] rawVerts)
    {
        List<Vector3> result = new List<Vector3>();

        int[] collinears = GetCollinears(rawVerts);
        for (int i = 0; i < rawVerts.Length; i++)
        {
            if (!collinears.Contains(i))
            {
                result.Add(rawVerts[i]);
            }
        }

        return result.ToArray();
    }


    // my own custom triangulation algorithm

    // * cut up the concave shape into a bunch of convex shapes
    // * triangulate all the convex shapes
    // * combine those triangulations into one final thing

    // it was painful, but this is non-recursive (as in it doesn't call itself)
    public static int[] GenerateConcaveTriangulation(Vector2[] verts)
    {
        // gotta convert to 3D for our little angle-detection trick
        // pretty much everything up until the very end will reference this
        Vector3[] verts3D = Vector2ToVector3(verts);

        List<int> additionalTriangles = new List<int>();

        // to be filled with all of the convex pieces of our concave shape
        // think of these as 'fully processed'
        List<int[]> convexSlices = new List<int[]>(); // ints are vertex indices

        List<int[]> concaveSlices = new List<int[]>();
        // we add the shape as a whole as a concave slice, and begin
        concaveSlices.Add(GenerateAscending(verts.Length));

        Vector3 normal = Vector3.up;

        int iterationCount = 0;
        int safeIterations = 500;
        while (concaveSlices.Count > 0 && iterationCount < safeIterations)
        {
            iterationCount++;
            // we loop backwards cuz things will be getting deleted and we don't wanna mess up the index math
            for (int n = concaveSlices.Count - 1; n >= 0; n--)
            {
                // our job for each concave slice is to find a reflex angle
                // if we CAN'T find one then the slice is convex and we transfer it
                // if we CAN, then we slice it
                // in EITHER case we delete the slice

                Vector3[] v = GrabVertexSet(verts3D, concaveSlices[n]);

                // the loop that looks for a reflex angle
                int reflexIndex = -1;
                Vector2 bisector = Vector2.zero;
                for (int i = 0; i < concaveSlices[n].Length; i++)
                {
                    Vector3 v1 = GetPreceedingVertex(v, i);
                    Vector3 v2 = v[i];
                    Vector3 v3 = GetSucceedingVertex(v, i);

                    Vector3 a = v2 - v1;
                    Vector3 b = v3 - v2;

                    float angle = GetCounterClockwiseAngle(a, b, normal);

                    if (angle >= 180)
                    {
                        reflexIndex = i;

                        Vector2 a2D = new Vector2(-a.x, -a.z);
                        Vector2 b2D = new Vector2(b.x, b.z);

                        bisector = (a2D+b2D).normalized;
                        break;
                    }
                }
                //Debug.Log(iterationCount + "    " + reflexIndex);

                if (concaveSlices[n].Length > 2)
                {
                    if (reflexIndex == -1)
                    {
                        // the slice is already convex so we just add it
                        convexSlices.Add(concaveSlices[n]);
                    } else
                    {
                        List<int>[] slices = CutPolygon(bisector, verts[concaveSlices[n][reflexIndex]], GrabVertexSet(verts, concaveSlices[n]), concaveSlices[n]);

                        int[] extraTriangle = GetCutTriangle(bisector, concaveSlices[n][reflexIndex], verts[concaveSlices[n][reflexIndex]], GrabVertexSet(verts, concaveSlices[n]), concaveSlices[n]);
                        for (int i = 0; i < extraTriangle.Length; i++) {additionalTriangles.Add(extraTriangle[i]);}
                        
                        for (int i = 0; i < slices.Length; i++)
                        {
                            concaveSlices.Add(slices[i].ToArray());
                        }
                    }
                }
                
                // again, we delete the concave slice in either case
                concaveSlices.RemoveAt(n);
            }
        }

        if (iterationCount >= safeIterations)
        {
            Debug.Log("Triangulation failed!");
            return new int[0];
        }

        // so now the concave slices list is at 0,
        // and all we have are convex slices
        List<int[]> sliceTriangulations = new List<int[]>();
        int triSum = 0;
        for (int i = 0; i < convexSlices.Count; i++)
        {
            int[] newTriangulation = GenerateConvexTriangulation(convexSlices[i].Length, convexSlices[i]);

            triSum += newTriangulation.Length;
            sliceTriangulations.Add(newTriangulation);
        }

        // copying all of the different triangulations into one array
        int[] result = new int[triSum + additionalTriangles.Count];

        int localIndex = 0;
        int sliceIndex = 0;
        for (int i = 0; i < result.Length- additionalTriangles.Count; i++)
        {
            result[i] = sliceTriangulations[sliceIndex][localIndex];

            if (localIndex == sliceTriangulations[sliceIndex].Length - 1)
            {
                localIndex = 0;
                sliceIndex++;
            } else {localIndex++;}
        }

        for (int i = 0; i < additionalTriangles.Count; i++)
        {
            result[triSum + i] = additionalTriangles[i];
        }

        
        return result;
    }

    // todo: move this
    public static void LogArray(int[] toLog)
    {
        for (int i = 0; i < toLog.Length; i++)
        {
            Debug.Log(toLog[i]);
        }
    }

    public static Vector2[] GrabVertexSet(Vector2[] allVerts, int[] toGrab)
    {
        Vector2[] result = new Vector2[toGrab.Length];

        for (int i = 0; i < result.Length; i++)
        {
            result[i] = allVerts[toGrab[i]];
        }

        return result;
    }
    public static Vector3[] GrabVertexSet(Vector3[] allVerts, int[] toGrab)
    {
        Vector3[] result = new Vector3[toGrab.Length];

        for (int i = 0; i < result.Length; i++)
        {
            result[i] = allVerts[toGrab[i]];
        }

        return result;
    }

    // makes an array going [0, 1, 2, .... n] for n items
    public static int[] GenerateAscending(int length)
    {
        int[] result = new int[length];
        for (int i = 0; i < length; i++)
        {
            result[i] = i;
        }
        return result;
    }

    // just easier to package up this 2-case logic
    public static Vector2 GetPreceedingVertex(Vector2[] verts, int index)
    {
        if (index == 0)
        {
            return verts[verts.Length - 1];
        } else
        {
            return verts[index - 1];
        }
    }

    public static Vector2 GetSucceedingVertex(Vector2[] verts, int index)
    {
        if (index == verts.Length - 1)
        {
            return verts[0];
        } else
        {
            return verts[index + 1];
        }
    }

    public static Vector3 GetPreceedingVertex(Vector3[] verts, int index)
    {
        if (index == 0)
        {
            return verts[verts.Length - 1];
        } else
        {
            return verts[index - 1];
        }
    }

    public static Vector3 GetSucceedingVertex(Vector3[] verts, int index)
    {
        if (index == verts.Length - 1)
        {
            return verts[0];
        } else
        {
            return verts[index + 1];
        }
    }

    public static int[] GetCutTriangle(Vector2 cutLine, int sourceIndex, Vector2 source, Vector2[] verts, int[] indexMap)
    {
        Vector2 perpLine = new Vector2(cutLine.y, -cutLine.x);
        List<int> result = new List<int>();

        for (int i = 0; i < verts.Length; i++)
        {
            Vector2 v1 = verts[i];
            Vector2 v2 = GetSucceedingVertex(verts, i);

            if (Vector2.Dot(perpLine, v1 - source) > 0 && Vector2.Dot(perpLine, v2 - source) < 0)
            {
                result.Add(sourceIndex);
                result.Add(indexMap[i]);
                result.Add(indexMap[GetSucceedingIndex(indexMap.Length, i)]);
            } else if (Vector2.Dot(perpLine, v1 - source) < 0 && Vector2.Dot(perpLine, v2 - source) > 0)
            {
                result.Add(sourceIndex);
                result.Add(indexMap[GetSucceedingIndex(indexMap.Length, i)]);
                result.Add(indexMap[i]);
            }
        }

        return result.ToArray();
    }

    public static int GetSucceedingIndex(int length, int index)
    {
        if (index < length - 1)
        {
            return index+1;
        } else
        {
            return 0;
        }
    }

    public static int GetPreceedingIndex(int length, int index)
    {
        if (index > 0)
        {
            return index-1;
        } else
        {
            return length - 1;
        }
    }

    public static bool SegmentsIntersect(Vector3 a1, Vector3 a2, Vector3 b1, Vector3 b2)
    {
        Vector3 dirA = a2 - a1;
        Vector3 dirB = b2 - b1;

        // Direction of the line connecting the two start points
        Vector3 dirAB = b1 - a1;

        Vector3 crossAB = Vector3.Cross(dirA, dirB);
        float crossSqrMag = crossAB.sqrMagnitude;

        // Lines are parallel (or nearly parallel)
        if (crossSqrMag < 1e-8f)
            return false;

        // Check if the lines are coplanar
        float planarFactor = Vector3.Dot(dirAB, crossAB);
        if (Mathf.Abs(planarFactor) > 1e-5f)
            return false; // Not coplanar → cannot intersect in 3D

        // Calculate the intersection parameters
        Vector3 crossABandB = Vector3.Cross(dirAB, dirB);
        float t = Vector3.Dot(crossABandB, crossAB) / crossSqrMag;

        Vector3 crossABandA = Vector3.Cross(dirAB, dirA);
        float u = Vector3.Dot(crossABandA, crossAB) / crossSqrMag;

        // Check if the intersection point lies on both segments
        // (using a small epsilon for floating-point tolerance)
        const float epsilon = -0.01f;
        return t >= -epsilon && t <= 1f + epsilon &&
               u >= -epsilon && u <= 1f + epsilon;
    }

    
    // Vector3.zero is being used as a stand-in for a null value type
    public static Vector3 SegmentsIntersectPoint(Vector3 a1, Vector3 a2, Vector3 b1, Vector3 b2)
    {
        Vector3 dirA = a2 - a1;
        Vector3 dirB = b2 - b1;
        Vector3 dirAB = b1 - a1;

        Vector3 crossAB = Vector3.Cross(dirA, dirB);
        float crossSqrMag = crossAB.sqrMagnitude;

        // Parallel (or nearly parallel)
        if (crossSqrMag < 1e-8f)
            return Vector3.zero;

        // Not coplanar
        float planarFactor = Vector3.Dot(dirAB, crossAB);
        if (Mathf.Abs(planarFactor) > 1e-5f)
            return Vector3.zero;

        // Intersection parameters
        Vector3 crossABandB = Vector3.Cross(dirAB, dirB);
        float t = Vector3.Dot(crossABandB, crossAB) / crossSqrMag;

        Vector3 crossABandA = Vector3.Cross(dirAB, dirA);
        float u = Vector3.Dot(crossABandA, crossAB) / crossSqrMag;

        // Same tolerance as your original
        const float epsilon = -0.01f;
        if (t < -epsilon || t > 1f + epsilon ||
            u < -epsilon || u > 1f + epsilon)
            return Vector3.zero;

        // Point of intersection (using parameter t on segment A)
        return a1 + t * dirA;
    }

    // returns a 2 by n array that represents the two pieces of the cut polygon

    // very useful for turning a concave polygon into 2 convex gons

    // we don't need to return a util_polygoncomponent[] because we can just keep the original indices
    public static List<int>[] CutPolygon(Vector2 cutLine, Vector2 source, Vector2[] verts, int[] indexMap)
    {
        // ***
        // splitting up the vertices in two
        // ***

        // using the idea that taking the negative rociprocal of the slope of a 2D line yields the perpindicular slope
        Vector2 perpLine = new Vector2(cutLine.y, -cutLine.x);
        // there are 2 possible perp lines, which one this ends up being doesn't matter


        // we start with these two unseparated pieces
        List<List<int>> queue = new List<List<int>>();
        // one for each side of the cut line
        queue.Add(new List<int>());
        queue.Add(new List<int>());

        for (int i = 0; i < verts.Length; i++)
        {
            if (Vector2.Dot(perpLine, verts[i]-source) > 0)
            {
                queue[0].Add(indexMap[i]);
            } else if (Vector2.Dot(perpLine, verts[i]-source) < 0)
            {
                queue[1].Add(indexMap[i]);
            } else // lies right on the line, so 2 copies (bc we need the resulting copies to be complete polygons)
            {
                queue[0].Add(indexMap[i]);
                queue[1].Add(indexMap[i]);
            }
        }


        return queue.ToArray();
    }
    // with no map
    // public static List<int>[] CutPolygon(Vector2 cutLine, Vector2[] verts)
    // {
    //     return CutPolygon(cutLine, verts, GenerateAscending(verts.Length));
    // }

    // just a triangle fan, technically my own idea but many people have used this
    
    // not sure whether this is clockwise or not?
    public static int[] GenerateConvexTriangulation(int length, int[] indexMap)
    {
        int[] tris = new int[(length - 2) * 3];

        for (int i = 0, n = 1; i < tris.Length; i+= 3, n++)
        {
            tris[i] = indexMap[0];
            tris[i+1] = indexMap[n+1];
            tris[i+2] = indexMap[n];
            
        }

        return tris;
    }
    // when there isn't a mapping
    public static int[] GenerateConvexTriangulation(int length)
    {
        return GenerateConvexTriangulation(length, GenerateAscending(length));
    }

    // returns degrees
    public static float GetClockwiseAngle(Vector3 a, Vector3 b, Vector3 normal)
    {
        float rawAngle = Vector3.Angle(a, b);
        if (Vector3.Dot(Vector3.Cross(a, b), normal) > 0)
        {
            return rawAngle;
        } else
        {
            return 360 - rawAngle;
        }
    }

    // returns degrees
    public static float GetCounterClockwiseAngle(Vector3 a, Vector3 b, Vector3 normal)
    {
        float rawAngle = Vector3.Angle(a, b);
        if (Vector3.Dot(Vector3.Cross(a, b), normal) > 0)
        {
            return 360 - rawAngle;
        } else
        {
            return rawAngle;
        }
    }
}
