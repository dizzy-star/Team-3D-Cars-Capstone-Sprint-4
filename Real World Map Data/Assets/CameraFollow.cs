using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform targetObject;
    private Vector3 initialOffset;
    private Vector3 cameraPosition;
    // Start is called before the first frame update
    void Start()
    {
        initialOffset = transform.position - targetObject.position; 
    }

    // Update is called once per frame
    void Update()
    {
        cameraPosition = targetObject.position + initialOffset;
        transform.position = cameraPosition;
    }
}
