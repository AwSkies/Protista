using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public float speed;
    private bool moving;
    private Vector3 target;

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            // Move our position a step closer to the target
            // calculate distance to move
            float step =  speed * Time.deltaTime; 
            transform.position = Vector3.MoveTowards(transform.position, target, step);

            // Check if the position is about where it should be
            if (Vector3.Distance(transform.position, target) < 0.001f)
            {
                moving = false;
            }
        }
    }

    public void Move(float newX, float newZ)
    {
        // Set target and make moving true
        target = new Vector3(newX, transform.position.y, newZ);
        moving = true;
    }
}
