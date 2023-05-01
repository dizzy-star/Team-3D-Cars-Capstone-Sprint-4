﻿using System.Collections.Generic;
using UnityEngine;


/*
    Copyright (c) 2017 Sloan Kelly

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/
/// <summary>
/// Road infrastructure maker.
/// </summary>
internal sealed class RoadMaker : BaseInfrastructureMaker
{


    public Material roadMaterial;
    //public IntersectionSpawnerScript spawner;
    public GameObject block;

    public override int NodeCount
    {
        get { return map.ways.FindAll((w) => { return w.IsRoad; }).Count; }
    }

    public RoadMaker(MapReader mapReader, Material roadMat)
        : base(mapReader)
    {
        roadMaterial = roadMat;
    }



    /// <summary>
    /// Create the roads.
    /// </summary>
    /// <returns></returns>

    public override IEnumerable<int> Process()
    {
        //IntersectionSpawnerScript spawner = new IntersectionSpawnerScript();
        //spawner = new IntersectionSpawnerScript();
        int count = 0;
        //GameObject block = new GameObject("Intersection");
        //var meshFilter = block.AddComponent<MeshFilter>();
        //block.AddComponent<MeshRenderer>();

        // Iterate through the roads and build each one
        foreach (var way in map.ways.FindAll((w) => { return w.IsRoad; }))
        {
            CreateObject(way, roadMaterial, way.Name);

            count++;
            yield return count;
        }


        //on top of road interesections, add square
        
        Debug.Log("1. getting intersection list");
        List<ulong> intersectionList = DetectIntersection();
        Debug.Log("3. intersection list has been retrieved from Base Infrastructure");

        foreach (var node in map.nodes) //for each node
        {
            for (int i = 0; i < intersectionList.Count; i++) //for each intersection
            {                
                Debug.Log("(3.2) in the for loop. for each intersection");

                if (node.Value.ID == intersectionList[i]) //if node id == intersection node id
                {
                    Debug.Log("(3.3) in if. check if its in intersection list");
                    float nodeLat = (float)node.Value.Latitude;
                    float nodeLon = (float)node.Value.Longitude;
                    Debug.Log(nodeLat);
                    Debug.Log(nodeLon);

                    Debug.Log("4. spawning");

                    //spawner.spawnIntersectMarker(nodeLat, nodeLon);
                    spawnBlock(nodeLat,nodeLon);
                    
                    
                }
            }
        }
        

    }

    void spawnBlock(float nodeLat, float nodeLon)
    {
        Debug.Log(nodeLat);
        Debug.Log(nodeLon);
       
        //Instantiate(block, new Vector3(nodeLat, height, nodeLon), transform.rotation);
        //
        Debug.Log("5. spawn complete");
    }

    protected override void OnObjectCreated(OsmWay way, Vector3 origin, List<Vector3> vectors, List<Vector3> normals, List<Vector2> uvs, List<int> indices)
    {
        for (int i = 1; i < way.NodeIDs.Count; i++)
        {
            OsmNode p1 = map.nodes[way.NodeIDs[i - 1]];
            OsmNode p2 = map.nodes[way.NodeIDs[i]];

            Vector3 s1 = p1 - origin;
            Vector3 s2 = p2 - origin;

            Vector3 diff = (s2 - s1).normalized;

            // https://en.wikipedia.org/wiki/Lane
            // According to the article, it's 3.7m in Canada
            var cross = Vector3.Cross(diff, Vector3.up) * 3.7f * way.Lanes;

            // Create points that represent the width of the road
            Vector3 v1 = s1 + cross;
            Vector3 v2 = s1 - cross;
            Vector3 v3 = s2 + cross;
            Vector3 v4 = s2 - cross;

            vectors.Add(v1);
            vectors.Add(v2);
            vectors.Add(v3);
            vectors.Add(v4);

            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));

            normals.Add(Vector3.up);
            normals.Add(Vector3.up);
            normals.Add(Vector3.up);
            normals.Add(Vector3.up);

            int idx1, idx2, idx3, idx4;
            idx4 = vectors.Count - 1;
            idx3 = vectors.Count - 2;
            idx2 = vectors.Count - 3;
            idx1 = vectors.Count - 4;

            // first triangle v1, v3, v2
            indices.Add(idx1);
            indices.Add(idx3);
            indices.Add(idx2);

            // second         v3, v4, v2
            indices.Add(idx3);
            indices.Add(idx4);
            indices.Add(idx2);
        }
    }
}

