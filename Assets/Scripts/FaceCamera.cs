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
        // Sets rotation to equal camera rotation
        // Since camera rotation is always looking downwards it will remain flat
        transform.rotation = cam.transform.rotation;
    }
}
