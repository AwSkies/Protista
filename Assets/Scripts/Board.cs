using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{   
    public GameObject hexPrefab;
    public GameObject objHexPrefab;
    public GameObject whitePiecePrefab;
    public GameObject blackPiecePrefab;
    
    // Hexes between each player's side
    public int rows;
    // The way/distance hexes are tiled from left to right
    public Vector3 rowXSpace;
    // The vertical offset when hexes are tiled from left to right
    public Vector3 rowZSpace;
    // Hexes to the right and left of player
    public int columns;
    // The way/distance hexes are tiled from top to bottom
    public Vector3 columnSpace;

    private Dictionary<string, GameObject>[,] hexDex;
    private bool clickedLastFrame = false;
    // Start is called before the first frame update
    void Start()
    {
        // Set up board --------------------------------------
        // Initialize hexDex as 2D array with size of rows and columns specified
        hexDex = new Dictionary<string, GameObject>[rows, columns];
        // lastPosition = the last place we spawned in a hex, we'll then add some vectors to it to get our new position and spawn a new hex there
        Vector3 lastPosition = new Vector3(0f, 0f, 0f);
        for (int i = 0; i < hexDex.GetLength(0); i++)
        {
            bool lastWentDown = false;
            bool firstHexInRow = true;
            for (int hexX = 0; hexX < hexDex.GetLength(1); hexX++)
            {
                if (!firstHexInRow) 
                {
                    // Vector math including whether to tile up or down
                    lastPosition += rowXSpace;
                    if (lastWentDown) {
                        lastPosition += rowZSpace;
                        lastWentDown = false;
                    } 
                    else 
                    { 
                        lastPosition -= rowZSpace;
                        lastWentDown = true; 
                    }
                }
                // Spawn in hex and put that in the array
                Instantiate(hexPrefab, lastPosition, Quaternion.identity);
                // hexDex[i, hexX].Add("hex", Instantiate(hexPrefab, lastPosition, Quaternion.identity));
                firstHexInRow = false;
            }
            // Resets the x position of the first hex in the row to 0 and then adds the column space
            lastPosition.Set(0f, lastPosition.y, lastPosition.z);
            lastPosition += columnSpace;
        }

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
                    Debug.Log(hit.transform.position);
                }
                catch (System.NullReferenceException) {
                    Debug.Log("Not clicked anything");
                }
            }
            clickedLastFrame = true;
        } 
        else 
        {
            clickedLastFrame = false;
        }
    }
}
