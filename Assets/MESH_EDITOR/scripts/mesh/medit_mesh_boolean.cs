
// seriously doubt any of these are going to be used for the mesh editor, but I'm leaving them here in case they are


// class for boolean operations (Union, Intersect, Cut, etc.)
public class medit_mesh_boolean
{
    // this may very well be the longest function I've ever written
    // it's almost 500 lines
    
    // (commented bc of the sheer amount of wavy red lines that I don't want to fix)

    
    // a less-gutted version of this function with more debug features is available in the Drivetrain project
    // public static Mesh BooleanCut(Mesh cutter, Mesh piece)
    // {
    //     // I. define the variables that we're going to be using

    //     Mesh result = new Mesh();
    //     // VERY IMPORTANT FIRST STEP
    //     // we have to "reduce" the input meshes so that there are no duplicate vertices
    //     // cube meshes will have multiple vertices (24 instead of 8) to allow for proper shading
    //     // we don't want this
    //     Mesh cutter_reduced = ReduceMesh(cutter);
    //     Mesh piece_reduced = ReduceMesh(piece);

    //     // for the new, resulting mesh
    //     List<Vector3> newVertices = new List<Vector3>();
    //     // these will help us point to the new vertex indices later
    //     // we have one int per vertex of the cutter/piece, and the value of that int is the new vertex index
    //     int[] cNewVerts= new int[cutter.vertices.Length];
    //     int[] pNewVerts = new int[piece.vertices.Length];
    //     for (int i = 0; i < cNewVerts.Length; i++) { cNewVerts[i] = -1;}
    //     for (int i = 0; i < pNewVerts.Length; i++) { pNewVerts[i] = -1;}

    //     List<Vector3> newNormals = new List<Vector3>();
    //     List<int> newTris = new List<int>();
    //     // we don't care about uvs, like at all

    //     // the old, existing triangles
    //     int[] cutter_triangles = cutter.GetTriangles(0);
    //     int[] reduced_cutter_triangles = cutter_reduced.GetTriangles(0);
    //     int[] piece_triangles = piece.GetTriangles(0);
    //     int[] reduced_piece_triangles = piece_reduced.GetTriangles(0);

    //     // the (cutter) vertices that end up intersecting the piece
    //     List<Vector3> hitVertices = new List<Vector3>(); // positions 
    //     List<int> hitVertexIndices = new List<int>(); // new indices
    //     List<int> hitSourceVertexIndices = new List<int>(); // old indices
    //     List<mesh_triangle[]> hitTriangles = new List<mesh_triangle[]>(); // trianges that are affected

    //     // II. keeping any vertices, normals and tris that aren't affected by the cut at all

    //     // cutter first, verts/normals
    //     for (int i = 0; i < cutter.vertices.Length; i++)
    //     {
    //         // we keep these automatically if they are outside of the piece
    //         if (medit_mesh_intersection.IsPointInsideMesh(piece, cutter.vertices[i]))
    //         {
    //             newVertices.Add(cutter.vertices[i]);
    //             newNormals.Add(cutter.normals[i]);
    //             cNewVerts[i] = newVertices.Count - 1;
    //         }
    //     }
    //     // then tris
    //     // notice that here we have some extra code to grab the intersecting vertices while we're here
    //     for (int i = 0; i < cutter_triangles.Length; i+=3)
    //     {
    //         // the triangle we're dealing with
    //         // for a CUTTER triangle to be valid, it has to be fully inside in the piece
    //         Vector3 v1 = cutter.vertices[cutter_triangles[i]];
    //         Vector3 v2 = cutter.vertices[cutter_triangles[i+1]];
    //         Vector3 v3 = cutter.vertices[cutter_triangles[i+2]];

    //         bool inOutCheck = true;

    //         // here we want to make sure that all vertices are INSIDE the piece,
    //         // otherwise the triangle is not preserved

    //         if (!medit_mesh_intersection.IsPointInsideMesh(piece, v1)) {inOutCheck = false;}
    //         if (!medit_mesh_intersection.IsPointInsideMesh(piece, v2)) {inOutCheck = false;}
    //         if (!medit_mesh_intersection.IsPointInsideMesh(piece, v3)) {inOutCheck = false;}

    //         bool rayCheck = true;

    //         // if we find lines (v1->v2, v2->v3, v3->v1) that INTERSECT the piece,
    //         // then we add them to a list to keep track of

    //         Vector3 ray1 = medit_mesh_intersection.MeshIntersectPoint(piece, v1, (v2-v1));
    //         Vector3 ray2 = medit_mesh_intersection.MeshIntersectPoint(piece, v2, (v3-v2)); // these directions shouldn't be normalized
    //         Vector3 ray3 = medit_mesh_intersection.MeshIntersectPoint(piece, v3, (v1-v3));

    //         // tests for each ray individually
    //         // we're looking for 'out' vertices going 'in'
    //         if (!medit_mesh_intersection.IsPointInsideMesh(piece, v1) && ray1 != Vector3.zero && (ray1-v1).magnitude <= (v2-v1).magnitude)
    //         {
    //             rayCheck = false; // tri is no longer valid for preservation
    //             List<mesh_triangle> hit = medit_mesh_intersection.MeshIntersectTriangle(piece, v1, (v2-v1));

    //             newVertices.Add(ray1);
    //             newNormals.Add(Vector3.up); // really don't care what the normal is, so lo and behold it's up

    //             hitVertexIndices.Add(newVertices.Count - 1);
    //             hitSourceVertexIndices.Add(cutter_triangles[i]);
    //             hitVertices.Add(ray1);

    //             hitTriangles.Add(hit.ToArray());

    //             // we don't add them to the debug view HERE, we do that later
    //         }

    //         if (!medit_mesh_intersection.IsPointInsideMesh(piece, v2) && ray2 != Vector3.zero && (ray2-v2).magnitude <= (v3-v2).magnitude)
    //         {
    //             rayCheck = false; // tri is no longer valid for preservation
    //             List<mesh_triangle> hit = medit_mesh_intersection.MeshIntersectTriangle(piece, v2, (v3-v2));

    //             newVertices.Add(ray2);
    //             newNormals.Add(Vector3.up); // really don't care what the normal is, so lo and behold it's up

    //             hitVertexIndices.Add(newVertices.Count - 1);
    //             hitSourceVertexIndices.Add(cutter_triangles[i+1]);
    //             hitVertices.Add(ray2);

    //             hitTriangles.Add(hit.ToArray());
                
    //             // we don't add them to the debug view HERE, we do that later
    //         }

    //         if (!medit_mesh_intersection.IsPointInsideMesh(piece, v3) && ray3 != Vector3.zero && (ray3-v3).magnitude <= (v1-v3).magnitude)
    //         {
    //             rayCheck = false; // tri is no longer valid for preservation
    //             List<mesh_triangle> hit = medit_mesh_intersection.MeshIntersectTriangle(piece, v3, (v1-v3));

    //             newVertices.Add(ray3);
    //             newNormals.Add(Vector3.up); // really don't care what the normal is, so lo and behold it's up

    //             hitVertexIndices.Add(newVertices.Count - 1);
    //             hitSourceVertexIndices.Add(cutter_triangles[i+2]);
    //             hitVertices.Add(ray3);

    //             hitTriangles.Add(hit.ToArray());

    //             // we don't add them to the debug view HERE, we do that later
    //         }

    //         if (inOutCheck && rayCheck) // I really don't have to check both, if the whole mesh is inside then of course the rays will miss
    //         {
    //             // cutter triangles have their order reversed
    //             // think about it, the cutter faces are actually inside faces
    //             newTris.Add(cNewVerts[cutter_triangles[i]]);
    //             newTris.Add(cNewVerts[cutter_triangles[i+2]]);
    //             newTris.Add(cNewVerts[cutter_triangles[i+1]]);
    //         }
    //     }

    //     // NOW what we're gonna do is look through all of the hit vertices and remove the collinear ones
    //     int[] badIndices = GetCollinears(hitVertices);
    //     System.Array.Sort(badIndices);

    //     for (int i = badIndices.Length - 1; i >= 0; i--) {
            
    //         newVertices.RemoveAt(hitVertexIndices[badIndices[i]]);
    //         newNormals.RemoveAt(hitVertexIndices[badIndices[i]]);

    //         hitVertices.RemoveAt(badIndices[i]);
    //         hitSourceVertexIndices.RemoveAt(badIndices[i]);

    //         for (int j = 0; j < hitVertexIndices.Count; j++)
    //         {
    //             if (hitVertexIndices[j] > hitVertexIndices[badIndices[i]])
    //             {
    //                 hitVertexIndices[j]--;
    //             }
    //         }
    //         hitVertexIndices.RemoveAt(badIndices[i]);
    //     }

    //     // one other thing I have to do here is connect the new vertices with lines
    //     line_segment[] intersectingConnections = FindConnectingVertices(hitVertices.ToArray(), hitSourceVertexIndices.ToArray(), cutter, cutter_reduced);
        
    //     // copying over the connections
    //     List<line_segment> copiedConnections = new List<line_segment>();
    //     for (int i = 0; i < intersectingConnections.Length; i++)
    //     {
    //         int iA = System.Array.IndexOf(hitSourceVertexIndices.ToArray(), intersectingConnections[i].a);
    //         int iB = System.Array.IndexOf(hitSourceVertexIndices.ToArray(), intersectingConnections[i].b);
            
    //         if (iA == -1 || iB == -1)
    //         {
    //             continue;
    //         }
    //         int newA = hitVertexIndices[iA];
    //         int newB = hitVertexIndices[iB];

    //         copiedConnections.Add(new line_segment(newA, newB));
    //     }

    //     // then the piece, verts/normals
    //     for (int i = 0; i < piece.vertices.Length; i++)
    //     {
    //         if (!medit_mesh_intersection.IsPointInsideMesh(cutter, piece.vertices[i]))
    //         {
    //             newVertices.Add(piece.vertices[i]);
    //             newNormals.Add(piece.normals[i]);
    //             pNewVerts[i] = newVertices.Count - 1;
    //         }
    //     }
    //     // then triangles
    //     for (int i = 0; i < piece_triangles.Length; i+=3)
    //     {
    //         // the three verts in question
    //         // all three of these have to be outside the mesh and their line segments CANNOT CROSS the cutter
    //         Vector3 v1 = piece.vertices[piece_triangles[i]];
    //         Vector3 v2 = piece.vertices[piece_triangles[i+1]];
    //         Vector3 v3 = piece.vertices[piece_triangles[i+2]];

    //         // for a triangle to be considered "preserved" it needs to meet 2 criteria:
    //         // * can't have any vertices inside the cutter
    //         // * can't have any lines intersecting the cutter

    //         // the inside/outside check
    //         bool inOutCheck = !medit_mesh_intersection.IsPointInsideMesh(cutter, v1) && !medit_mesh_intersection.IsPointInsideMesh(cutter, v2) && !medit_mesh_intersection.IsPointInsideMesh(cutter, v3);

    //         if (inOutCheck)
    //         {
    //             // the three line checks
    //             Vector3 ray1 = medit_mesh_intersection.MeshIntersectPoint(cutter, v1, (v2-v1));
    //             Vector3 ray2 = medit_mesh_intersection.MeshIntersectPoint(cutter, v2, (v3-v2));
    //             Vector3 ray3 = medit_mesh_intersection.MeshIntersectPoint(cutter, v3, (v1-v3));
    //             // this function returns (0,0,0) if the ray misses
    //             bool valid = ray1 == Vector3.zero && ray2 == Vector3.zero && ray3 == Vector3.zero;

    //             if (valid)
    //             {
    //                 newTris.Add(pNewVerts[piece_triangles[i]]);
    //                 newTris.Add(pNewVerts[piece_triangles[i+1]]);
    //                 newTris.Add(pNewVerts[piece_triangles[i+2]]);
    //             }
    //         }
    //     }

    //     // we need existing mesh structure to check line validity
    //     // this is like 'preparing' for the last step
    //     result.SetVertices(newVertices);
    //     result.SetNormals(newNormals);
    //     result.SetTriangles(newTris, 0);

    //     // actually adding the primary triangles to the mesh

    //     // (keep in mind the hitTriangles array contains vertex indices for the OLD piece mesh)
    //     // (but the hitVertex is a NEW vertex index because it didn't exist before)
    //     for (int i = 0; i < hitVertices.Count; i++)
    //     {
    //         for (int j=0; j < hitTriangles[i].Length; j++)
    //         {
    //             // what we WANT to do is a 012 , 023 triangulation using the vertex (0) and the three triangle vertices (1,2,3)

    //             int valid01 = medit_mesh_intersection.LineIntersectForMesh(result, hitVertices[i], hitTriangles[i][j].v1);
    //             int valid02 = medit_mesh_intersection.LineIntersectForMesh(result, hitVertices[i], hitTriangles[i][j].v2);
    //             int valid03 = medit_mesh_intersection.LineIntersectForMesh(result, hitVertices[i], hitTriangles[i][j].v3);

    //             int valid12 = medit_mesh_intersection.LineIntersectForMesh(result, hitTriangles[i][j].v1, hitTriangles[i][j].v2);

    //             int valid23 = medit_mesh_intersection.LineIntersectForMesh(result, hitTriangles[i][j].v3, hitTriangles[i][j].v2);

    //             int valid13 = medit_mesh_intersection.LineIntersectForMesh(result, hitTriangles[i][j].v3, hitTriangles[i][j].v1);

    //             bool cutter01 = medit_mesh_intersection.MeshIntersect(cutter, hitVertices[i] + (hitTriangles[i][j].v1-hitVertices[i]).normalized * epsilon, (hitTriangles[i][j].v1-hitVertices[i])) || medit_mesh_intersection.IsPointInsideMesh(cutter, hitVertices[i] + (hitTriangles[i][j].v1-hitVertices[i]).normalized * 0.5f);
    //             bool cutter02 = medit_mesh_intersection.MeshIntersect(cutter, hitVertices[i] + (hitTriangles[i][j].v2-hitVertices[i]).normalized * epsilon, (hitTriangles[i][j].v2-hitVertices[i])) || medit_mesh_intersection.IsPointInsideMesh(cutter, hitVertices[i] + (hitTriangles[i][j].v2-hitVertices[i]).normalized * 0.5f);
    //             bool cutter03 = medit_mesh_intersection.MeshIntersect(cutter, hitVertices[i] + (hitTriangles[i][j].v3-hitVertices[i]).normalized * epsilon, (hitTriangles[i][j].v3-hitVertices[i])) || medit_mesh_intersection.IsPointInsideMesh(cutter, hitVertices[i] + (hitTriangles[i][j].v3-hitVertices[i]).normalized * 0.5f);

    //             bool cutter12 = medit_mesh_intersection.MeshIntersect(cutter, hitTriangles[i][j].v1 + (hitTriangles[i][j].v2-hitTriangles[i][j].v1).normalized * epsilon, (hitTriangles[i][j].v2-hitTriangles[i][j].v1)) || medit_mesh_intersection.IsPointInsideMesh(cutter, hitTriangles[i][j].v1 + (hitTriangles[i][j].v2-hitTriangles[i][j].v1).normalized * 0.5f);

    //             bool cutter23 = medit_mesh_intersection.MeshIntersect(cutter, hitTriangles[i][j].v2 + (hitTriangles[i][j].v3-hitTriangles[i][j].v2).normalized * epsilon, (hitTriangles[i][j].v3-hitTriangles[i][j].v2)) || medit_mesh_intersection.IsPointInsideMesh(cutter, hitTriangles[i][j].v2 + (hitTriangles[i][j].v3-hitTriangles[i][j].v2).normalized * 0.5f);

    //             bool cutter13 = medit_mesh_intersection.MeshIntersect(cutter, hitTriangles[i][j].v1 + (hitTriangles[i][j].v3-hitTriangles[i][j].v1).normalized * epsilon, (hitTriangles[i][j].v3-hitTriangles[i][j].v1)) || medit_mesh_intersection.IsPointInsideMesh(cutter, hitTriangles[i][j].v1 + (hitTriangles[i][j].v3-hitTriangles[i][j].v1).normalized * 0.5f);

    //             // just so we're aware, this code does create duplicate triangles
    //             // yes, this is a bug
    //             // TODO: fix it


    //             // primary triangles
    //             if (valid01 == -1 && valid02 == -1 && valid12 == -1 && !cutter01 && !cutter02 && !cutter12) // the 012 triangle
    //             {
    //                 if (IsTriangleClockwise(new Vector3[]
    //                 {
    //                     hitVertices[i],
    //                     hitTriangles[i][j].v2,
    //                     hitTriangles[i][j].v1
    //                 }, Vector3.up))
    //                 {
    //                     newTris.Add(hitVertexIndices[i]); //0
                       
    //                     newTris.Add(pNewVerts[hitTriangles[i][j].n2]); // 2
    //                     newTris.Add(pNewVerts[hitTriangles[i][j].n1]); // 1
    //                 }
    //                 else
    //                 {
    //                     newTris.Add(hitVertexIndices[i]); //0
    //                     newTris.Add(pNewVerts[hitTriangles[i][j].n1]); // 1
    //                     newTris.Add(pNewVerts[hitTriangles[i][j].n2]); // 2
    //                 }

                    
    //             }
    //             // if (valid02 == -1 && valid03 == -1 && !cutter02 && !cutter03 && valid23 == -1 && !cutter23) // the 023 triangle
    //             // {
    //             //     if (IsTriangleClockwise(new Vector3[]
    //             //     {
    //             //         hitVertices[i],
    //             //         hitTriangles[i][j].v2,
    //             //         hitTriangles[i][j].v3
    //             //     }, Vector3.up))
    //             //     {
    //             //         newTris.Add(hitVertexIndices[i]); //0
                    
    //             //     newTris.Add(pNewVerts[hitTriangles[i][j].n2]); // 2
    //             //     newTris.Add(pNewVerts[hitTriangles[i][j].n3]); // 3
    //             //     }
    //             //     else
    //             //     {
    //             //         newTris.Add(hitVertexIndices[i]); //0
                    
    //             //     newTris.Add(pNewVerts[hitTriangles[i][j].n3]); // 3
    //             //     newTris.Add(pNewVerts[hitTriangles[i][j].n2]); // 2
    //             //     }

                    
    //             // }

    //             // if (valid03 == -1 && valid01 == -1 && !cutter03 && !cutter01 && !cutter13 && valid13 == -1) // the 031 triangle
    //             // {
    //             //     if (IsTriangleClockwise(new Vector3[]
    //             //     {
    //             //         hitVertices[i],
    //             //         hitTriangles[i][j].v3,
    //             //         hitTriangles[i][j].v1
    //             //     }, Vector3.up))
    //             //     {
    //             //         newTris.Add(hitVertexIndices[i]); //0
                    
                    
    //             //     newTris.Add(pNewVerts[hitTriangles[i][j].n3]); // 2
    //             //     newTris.Add(pNewVerts[hitTriangles[i][j].n1]); // 3
    //             //     }
    //             //     else
    //             //     {
    //             //        newTris.Add(hitVertexIndices[i]); //0
                    
    //             //     newTris.Add(pNewVerts[hitTriangles[i][j].n1]); // 3
    //             //     newTris.Add(pNewVerts[hitTriangles[i][j].n3]); // 2
    //             //     }

                    
    //             // }

    //             result.SetVertices(newVertices);
    //             result.SetNormals(newNormals);
    //             result.SetTriangles(newTris, 0);
    //         }
    //     }

    //     for (int i = 0; i < hitVertices.Count; i++)
    //     {
    //         for (int j = 0; j < hitTriangles[i].Length; j++)
    //         {
    //             int valid01 = medit_mesh_intersection.LineIntersectForMesh(result, hitVertices[i], hitTriangles[i][j].v1);
    //             int valid02 = medit_mesh_intersection.LineIntersectForMesh(result, hitVertices[i], hitTriangles[i][j].v2);
    //             int valid03 = medit_mesh_intersection.LineIntersectForMesh(result, hitVertices[i], hitTriangles[i][j].v3);

    //             bool cutter01 = medit_mesh_intersection.MeshIntersect(cutter, hitVertices[i] + (hitTriangles[i][j].v1-hitVertices[i]).normalized * epsilon, (hitTriangles[i][j].v1-hitVertices[i])) || medit_mesh_intersection.IsPointInsideMesh(cutter, hitVertices[i] + (hitTriangles[i][j].v1-hitVertices[i]).normalized * 0.5f);
    //             bool cutter02 = medit_mesh_intersection.MeshIntersect(cutter, hitVertices[i] + (hitTriangles[i][j].v2-hitVertices[i]).normalized * epsilon, (hitTriangles[i][j].v2-hitVertices[i])) || medit_mesh_intersection.IsPointInsideMesh(cutter, hitVertices[i] + (hitTriangles[i][j].v2-hitVertices[i]).normalized * 0.5f);
    //             bool cutter03 = medit_mesh_intersection.MeshIntersect(cutter, hitVertices[i] + (hitTriangles[i][j].v3-hitVertices[i]).normalized * epsilon, (hitTriangles[i][j].v3-hitVertices[i])) || medit_mesh_intersection.IsPointInsideMesh(cutter, hitVertices[i] + (hitTriangles[i][j].v3-hitVertices[i]).normalized * 0.5f);
                
    //             // secondary triangles

    //             List<int> secondaryVertices = new List<int>();
    //             for (int k = 0; k < hitVertexIndices.Count; k++)
    //             {
    //                 if (k == i) continue;

    //                 for (int l = 0; l < copiedConnections.Count; l++)
    //                 {
    //                     int a = hitVertexIndices[i];
    //                     int b = hitVertexIndices[k];

    //                     if (copiedConnections[l].a == a && copiedConnections[l].b == b)
    //                     {
    //                         secondaryVertices.Add(k);
    //                     }
    //                     if (copiedConnections[l].b == a && copiedConnections[l].a == b)
    //                     {
    //                         secondaryVertices.Add(k);
    //                     }
    //                 }
    //             }

    //             for (int k = 0; k < secondaryVertices.Count; k++)
    //             {
                    
    //                 int valid14 = medit_mesh_intersection.LineIntersectForMesh(result, hitVertices[secondaryVertices[k]], hitTriangles[i][j].v1);
    //                 int valid24 = medit_mesh_intersection.LineIntersectForMesh(result, hitVertices[secondaryVertices[k]], hitTriangles[i][j].v2);
    //                 int valid34 = medit_mesh_intersection.LineIntersectForMesh(result, hitVertices[secondaryVertices[k]], hitTriangles[i][j].v3);
                    
    //                 bool cutter14 = medit_mesh_intersection.MeshIntersect(cutter, hitVertices[secondaryVertices[k]] + (hitTriangles[i][j].v1-hitVertices[secondaryVertices[k]]).normalized * epsilon, (hitTriangles[i][j].v1-hitVertices[secondaryVertices[k]])) || medit_mesh_intersection.IsPointInsideMesh(cutter, hitVertices[secondaryVertices[k]] + (hitTriangles[i][j].v1-hitVertices[secondaryVertices[k]]).normalized * epsilon);
    //                 bool cutter24 = medit_mesh_intersection.MeshIntersect(cutter, hitVertices[secondaryVertices[k]] + (hitTriangles[i][j].v2-hitVertices[secondaryVertices[k]]).normalized * epsilon, (hitTriangles[i][j].v2-hitVertices[secondaryVertices[k]])) || medit_mesh_intersection.IsPointInsideMesh(cutter, hitVertices[secondaryVertices[k]] + (hitTriangles[i][j].v2-hitVertices[secondaryVertices[k]]).normalized * epsilon);
    //                 bool cutter34 = medit_mesh_intersection.MeshIntersect(cutter, hitVertices[secondaryVertices[k]] + (hitTriangles[i][j].v3-hitVertices[secondaryVertices[k]]).normalized * epsilon, (hitTriangles[i][j].v3-hitVertices[secondaryVertices[k]])) || medit_mesh_intersection.IsPointInsideMesh(cutter, hitVertices[secondaryVertices[k]] + (hitTriangles[i][j].v3-hitVertices[secondaryVertices[k]]).normalized * epsilon);

    //                 // primary, secondary, and v1
    //                 // (041)
    //                 //Debug.Log("1  " + newTris.Count);
    //                 // if (valid14 == -1 && valid01 == -1 && !cutter14 && !cutter01)
    //                 // {
    //                 //     if (IsTriangleClockwise(new Vector3[]
    //                 //     {
    //                 //         hitVertices[i],
    //                 //         hitTriangles[i][j].v1,
    //                 //         hitVertices[secondaryVertices[k]]
    //                 //     }, Vector3.up))
    //                 //     {
    //                 //         newTris.Add(hitVertexIndices[i]);
                            
    //                 //         newTris.Add(pNewVerts[hitTriangles[i][j].n1]);
    //                 //         newTris.Add(hitVertexIndices[secondaryVertices[k]]);
    //                 //     }
    //                 //     else
    //                 //     {
    //                 //     newTris.Add(hitVertexIndices[i]);
    //                 //         newTris.Add(hitVertexIndices[secondaryVertices[k]]);
    //                 //         newTris.Add(pNewVerts[hitTriangles[i][j].n1]);
    //                 //     }
                        
    //                 // }
    //                 // // // (042)
    //                 // // Debug.Log("2 " + newTris.Count);
    //                 // if (valid24 == -1 && valid02 == -1 && !cutter24 && !cutter02)
    //                 // {
    //                 //     if (IsTriangleClockwise(new Vector3[]
    //                 //     {
    //                 //         hitVertices[i],
    //                 //         hitVertices[secondaryVertices[k]],
    //                 //         hitTriangles[i][j].v2
    //                 //     }, Vector3.up))
    //                 //     {
    //                 //         newTris.Add(hitVertexIndices[i]);
    //                 //         newTris.Add(hitVertexIndices[secondaryVertices[k]]);
    //                 //         newTris.Add(pNewVerts[hitTriangles[i][j].n2]);
    //                 //     }
    //                 //     else
    //                 //     {
    //                 //         newTris.Add(hitVertexIndices[i]);
    //                 //         newTris.Add(pNewVerts[hitTriangles[i][j].n2]);
    //                 //         newTris.Add(hitVertexIndices[secondaryVertices[k]]);
    //                 //     }
                        
    //                 // }
    //                 // // Debug.Log("3  " + newTris.Count);
    //                 // // // (043)
    //                 // if (valid34 == -1 && valid03 == -1 && !cutter34 && !cutter03)
    //                 // {
    //                 //     if (IsTriangleClockwise(new Vector3[]
    //                 //     {
    //                 //         hitVertices[i],
    //                 //         hitVertices[secondaryVertices[k]],
    //                 //         hitTriangles[i][j].v3
    //                 //     }, Vector3.up))
    //                 //     {
    //                 //         newTris.Add(hitVertexIndices[i]);
                        
    //                 //     newTris.Add(pNewVerts[hitTriangles[i][j].n3]);
    //                 //     newTris.Add(hitVertexIndices[secondaryVertices[k]]);
    //                 //     }
    //                 //     else
    //                 //     {
    //                 //         newTris.Add(hitVertexIndices[i]);
    //                 //     newTris.Add(hitVertexIndices[secondaryVertices[k]]);
    //                 //     newTris.Add(pNewVerts[hitTriangles[i][j].n3]);
    //                 //     }

                        
    //                 // }
    //             }
    //         }
    //     }

    //     result.SetVertices(newVertices);
    //     result.SetNormals(newNormals);
    //     result.SetTriangles(newTris, 0);

    //     return result;
    // }
}