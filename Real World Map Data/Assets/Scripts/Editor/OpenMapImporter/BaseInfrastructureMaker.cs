using System.Collections;
using System.Collections.Generic;
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
/// Base infrastructure creator.
/// </summary>
internal abstract class BaseInfrastructureMaker : MonoBehaviour
{
    [HideInInspector]
    public Dictionary<ulong, OsmNode> nodes;
    [HideInInspector]
    public List<OsmWay> ways;


    /// <summary>
    /// The map reader object; contains all the data to build procedural geometry.
    /// </summary>
    protected MapReader map;

    /// <summary>
    /// The number of nodes present of this type in a file.
    /// </summary>
    public abstract int NodeCount { get; }

    /// <summary>
    /// Awaken this instance!!!
    /// </summary>
    public BaseInfrastructureMaker(MapReader mapReader)
    {
        map = mapReader;
    }
    
    /// <summary>
    /// Process the nodes to create the geometry.
    /// </summary>
    /// <returns></returns>
    public abstract IEnumerable<int> Process();

    /// <summary>
    /// Get the centre of an object or road.
    /// </summary>
    /// <param name="way">OsmWay object</param>
    /// <returns>The centre point of the object</returns>
    protected Vector3 GetCentre(OsmWay way)
    {
        Vector3 total = Vector3.zero;

        foreach (var id in way.NodeIDs)
        {
            total += map.nodes[id];
        }

        return total / way.NodeIDs.Count;
    }

    /// <summary>
    /// Procedurally generate an object from the data given in the OsmWay instance.
    /// </summary>
    /// <param name="way">OsmWay instance</param>
    /// <param name="mat">Material to apply to the instance</param>
    /// <param name="objectName">The name of the object (building name, road etc.)</param>
    protected void CreateObject(OsmWay way, Material mat, string objectName)
    {
        // Make sure we have some name to display
        objectName = string.IsNullOrEmpty(objectName) ? "OsmWay" : objectName;

        // Create an instance of the object and place it in the centre of its points
        GameObject go = new GameObject(objectName);
        Vector3 localOrigin = GetCentre(way);
        go.transform.position = localOrigin - map.bounds.Centre;

        // Add the mesh filter and renderer components to the object
        MeshFilter mf = go.AddComponent<MeshFilter>();
        MeshRenderer mr = go.AddComponent<MeshRenderer>();

        // Apply the material
        mr.material = mat;

        // Create the collections for the object's vertices, indices, UVs etc.
        List<Vector3> vectors = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> indices = new List<int>();

        // Call the child class' object creation code
        OnObjectCreated(way, localOrigin, vectors, normals, uvs, indices);

        // Apply the data to the mesh
        mf.sharedMesh = new Mesh();
        mf.sharedMesh.vertices = vectors.ToArray();
        mf.sharedMesh.normals = normals.ToArray();
        mf.sharedMesh.triangles = indices.ToArray();
        mf.sharedMesh.uv = uvs.ToArray();
    }

    protected abstract void OnObjectCreated(OsmWay way, Vector3 origin, List<Vector3> vectors, List<Vector3> normals, List<Vector2> uvs, List<int> indices);

    

    
    public int NodeCounter
    {
       get { return map.ways.FindAll((w) => { return w.IsRoad; }).Count; }
    }

    ///the purpose of this is to find the intersections. 
    ///if a node (id) exists in more that one way (nd ref) then the roads join
    public List<ulong> DetectIntersection()
    {
        List<ulong> intersectionList = new List<ulong>(); 
        int nodeCount = NodeCounter;
        //int aCounter = 0;
            
        
        //nodes = new Dictionary<ulong, OsmNode>();
        ways = new List<OsmWay>();

        //Debug.Log(map.nodes.Count);  //14,783
        //Debug.Log(map.ways.Count);   //1,283

        for (int i = 0; i < map.ways.Count; i++)
        {
            var way = map.ways[i];
            List<ulong> wayNodes = way.NodeIDs; //define this list of (node reference)'s in way
            for (int j = 0; j < wayNodes.Count; j++)
            {
                ulong nodeid = wayNodes[j];
                bool isDuplicate = NewMethod(nodeid, i);
                if (isDuplicate == true)
                {
                    intersectionList.Add(nodeid);
                }
            }
        }

        Debug.Log("2. intersection list made. Size:");
        Debug.Log(intersectionList.Count);
        return intersectionList;
    }

    private bool NewMethod(ulong nodeCheck, int count)
    {
        //int bCounter = 0;
        //int cCounter = 0;
        //bool intersect = false;
        int num = count + 1;
        for(int k = num; k < map.ways.Count; k++) //1,283
        {
            //Debug.Log("2.b This is the second loop. It iterates through all of the ways. Below should end with zero");
            //Debug.Log(map.ways.Count-bCounter); //WORKS!
            //bCounter++;
            var way = map.ways[k];
            List<ulong> wayNodes = way.NodeIDs; //define this list of (node reference)'s in way
            for (int l = 0; l < wayNodes.Count; l++)
            {
                if(nodeCheck == wayNodes[l])
                {
                    return true;
                }
            }
        }
        return false;
    }
}
