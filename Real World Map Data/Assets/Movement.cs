using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour
{
    public GameObject[] position;
    NavMeshAgent navage;
    int pos_value;
    public GameObject car;
    
    // Start is called before the first frame update
    void Start()
    {
       
        navage = GetComponent<NavMeshAgent>();
        pos_value = 0;
        GameObject cube = GameObject.Find("Cube");
        Debug.Log(cube);

    }

    // Update is called once per frame
    void Update()
    {
       // car.transform.Rotate(-87, 66, 14);
        if (navage.remainingDistance<0.2f)
        {
            
            if (pos_value == position.Length - 1)
            {

            }
            else
            {
                pos_value += 1;

                if (pos_value > position.Length - 1)
                {
                    pos_value = 0;
                }

                navage.SetDestination(position[pos_value].transform.position);
            }
        }
    }
}
