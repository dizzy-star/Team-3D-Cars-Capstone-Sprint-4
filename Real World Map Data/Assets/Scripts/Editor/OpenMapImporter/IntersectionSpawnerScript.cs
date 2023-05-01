using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntersectionSpawnerScript : MonoBehaviour
{

    public GameObject block;
    Vector3 pos;

    // Start is called before the first frame update
    private void Start()
    {

    }


    public void spawnIntersectMarker(float nodeLat, float nodeLon)
    {
        //pos = new Vector3(nodeLat, (float)0.5, nodeLon);
        Debug.Log("5. spawn complete");
        Instantiate(block, new Vector3(nodeLat, (float)0.5, nodeLon), transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
