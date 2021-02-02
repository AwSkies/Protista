using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public List<GameObject> stackedPieces;
    // Variables for moving animation
    public float speed;
    public bool moving;
    private Vector3 target;
    public Vector3 stackingHeight;

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

    public void Move(float newXf, float newZf, int newX, int newZ, bool stacking = false, GameObject stackingOnto = null)
    {
        // Reassign the piece's x and z values
        GetComponent<BoardPos>().z = newZ;
        GetComponent<BoardPos>().x = newX;
        // Set target and make moving true
        target = new Vector3(newXf, transform.position.y, newZf);
        if (stacking)
        {
            target += (stackingHeight * stackingOnto.GetComponent<Piece>().stackedPieces.Count);
        }
        moving = true;
        foreach (GameObject piece in stackedPieces)
        {
            piece.GetComponent<Piece>().Move(newXf, newZf, newX, newZ);
        }
    }    

    void OnCollisionEnter(Collision otherObj)
    {
        // If a piece collides with another piece of the opposite color 
        // and that piece is not moving (to prevent both pieces calling this function at the same and destroying each other at the same time)
        // the piece will destroy the other piece
        if ((otherObj.gameObject.tag == "black" || otherObj.gameObject.tag == "white") && otherObj.gameObject.tag != tag && !otherObj.gameObject.GetComponent<Piece>().moving)
        {
            Destroy(otherObj.gameObject);
        }
    }
}
