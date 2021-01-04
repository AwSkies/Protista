using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    private bool clickedLastFrame = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (!clickedLastFrame)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                // Casts the ray and get the first game object hit
                // This required colliders since it's a physics action
                // Since everything was made with Maya they won't have colliders already
                // So make sure that everything we need to click on is set to have a mesh collider
                Physics.Raycast(ray, out hit);
                try
                {
                    Debug.Log(hit.transform.name);
                }
                catch (System.NullReferenceException) {
                    Debug.Log("Not clicked anything");
                }
            }
            clickedLastFrame = true;
        } else {
            clickedLastFrame = false;
        }
    }
}
