using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Camera cam;

    // Start is called before the first frame update
    private void Start() 
    {
        // Cache main camera
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        // Transforms rotation to look at the camera
        transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.GetChild(0).position);
    }
}
