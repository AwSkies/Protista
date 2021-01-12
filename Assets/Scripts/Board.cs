using cakeslice;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    // Prefabs & scene objects -------------------------------
    public GameObject hexPrefab;
    public GameObject objHexPrefab;
    public GameObject whitePiecePrefab;
    public GameObject blackPiecePrefab;
    public GameObject cam;

    // Game variables for tweaking ---------------------------
    // Number of objective hexes for each player
    public int objHexNum;
    // Hexes between each player's side
    public int rows;
    // The way/distance hexes are tiled from left to right
    public Vector3 rowSpace;
    // Hexes to the right and left of player
    public int columns;
    // The horizontal/z offset when hexes are tiled from left to right
    public Vector3 columnZSpace;
    // The way/distance hexes are tiled from top to bottom
    public Vector3 columnXSpace;
    // Number of pieces for each player
    public int pieceNum;
    // Vertical offset of each piece
    public Vector3 pieceVertical;

    private Dictionary<string, GameObject>[,] hexDex;
    private bool[,] selected;
    private bool clickedLastFrame = false;
    // Start is called before the first frame update
    void Start()
    {
        // Set up board --------------------------------------
        // Initialize hexDex as 2D array with size of rows and columns specified
        hexDex = new Dictionary<string, GameObject>[rows, columns];
        // Initialize the "selected" array as a 2D array referring to the hexes and pieces that are selected
        selected = new bool[rows, columns];
        // Set all values of selected to false
        for (int i = 0; i < rows * columns; i++) { selected[i % rows, i / rows] = false; }

        // halfBoard = the number of rows that make up half the board, minus the middle row 
        int halfBoard = rows / 2;
        // Initialize random
        System.Random random = new System.Random();

        // Generate objective hex arrangement ---------------
        // Makes an array that has whether or not an objective hex needs to be generated in a coordinate, then makes all values false
        bool[,] objHexes = new bool[rows, columns];
        for (int i = 0; i < rows * columns; i++) { objHexes[i % rows, i / rows] = false; }
        // Generates the needed number of objective hexes on each side
        for (int i = 0; i < objHexNum; i++)
        {
            // Chooses the rows that belong to each player
            // If the position is already occupied by a previously generated objective hex, it will go through the do while loop again
            int xPos, zPos;
            do
            {
                // Choose position on one half of the board
                xPos = random.Next(0, columns);
                zPos = random.Next(0, halfBoard);
                // Add objective hex at chosen position
                objHexes[zPos, xPos] = true;
                // Mirror objective hexes across board
                zPos = (rows - 1) - zPos;
                // Add objective hex at mirrored position
                objHexes[zPos, xPos] = true;
            } 
            while (!objHexes[zPos, xPos]);
        }

        // Generate piece arrangement ------------------------
        // Makes an array that has whether or not a piece needs to be generated in a coordinate, then makes all values false
        bool[,] pieces = new bool[rows, columns];
        for (int i = 0; i < rows * columns; i++) { pieces[i % rows, i / rows] = false; }
        // Generates the needed number of pieces on each side
        for (int i = 0; i < pieceNum; i++)
        {
            // Chooses the rows that belong to each player
            // If the position is already occupied by a previously generated piece, it will go through the do while loop again
            int xPos, zPos;
            do
            {
                // Choose position on one half of the board
                xPos = random.Next(0, columns);
                zPos = random.Next(0, halfBoard);
                // Add piece at chosen position
                pieces[zPos, xPos] = true;
                // Mirror pieces across board
                zPos = (rows - 1) - zPos;
                // Add piece at mirrored position
                pieces[zPos, xPos] = true;
            } 
            while (!pieces[zPos, xPos]);
        }

        // Generating gameboard ------------------------------
        // lastPosition = the last place we spawned in a hex, we'll then add some vectors to it to get our new position and spawn a new hex there
        Vector3 lastPosition = new Vector3(0f, 0f, 0f);
        // Whether or not the first hex in the last row generated was offsetted to the right
        bool lastWentRight = true;
        // Loop through each row and hex in each row
        for (int i = 0; i < hexDex.GetLength(0); i++)
        {
            for (int hexX = 0; hexX < hexDex.GetLength(1); hexX++)
            {
                // Create empty dictionary in position
                hexDex[i, hexX] = new Dictionary<string, GameObject>();

                // Spawn hexes -------------------------------
                // Choose to place normal or objective hex based on earlier generation
                GameObject hexToPlace;
                if (objHexes[i, hexX]) { hexToPlace = objHexPrefab; } else { hexToPlace = hexPrefab; }
                // Spawn hex and place in hexDex
                hexDex[i, hexX].Add("hex", Instantiate(hexToPlace, lastPosition, Quaternion.Euler(0f, 30f, 0f)));

                // Spawn pieces ------------------------------
                // Choose whether or not to spawn pieces based on earlier generation
                if (pieces[i, hexX])
                {
                    // Choose to place black or white piece depending on position in the board
                    GameObject pieceToPlace;
                    if (i < halfBoard) { pieceToPlace = whitePiecePrefab; } else { pieceToPlace = blackPiecePrefab; }
                    // Spawn piece above hex and place in hexDex
                    hexDex[i, hexX].Add("piece", Instantiate(pieceToPlace, lastPosition + pieceVertical, Quaternion.identity));
                }
                // Make sure every index in the hexDex has a piece field so we can tell which ones don't have pieces
                else
                {
                    hexDex[i, hexX].Add("piece", null);
                }

                // Offets next hex position (for next time through loop)
                lastPosition += rowSpace;
            }
            // Resets the x position of the first hex in the row to 0 and then adds the column space
            lastPosition.Set(0f, lastPosition.y, lastPosition.z);
            lastPosition += columnZSpace;
            // Offsets first hex (for next time through loop)
            if (lastWentRight) 
            {
                lastPosition += columnXSpace;
                lastWentRight = false;
            } 
            else 
            {
                lastWentRight = true; 
            }
        }
        // Centers camera with generated board by setting transform x position to be half the distance of the number of columns * row space offset
        cam.transform.position = new Vector3((columns * rowSpace.x) / 2, cam.transform.position.y, cam.transform.position.z);
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
                // Makes sure it hit something
                if (hit.collider != null)
                {
                    // Gets coordinates to turn into array indexes
                    Vector3 hitPos = hit.transform.position;
                    // Accounts for sideways offset tiling
                    if ((hit.transform.position.z / 4.5f) % 2 == 1) { hitPos -= columnXSpace; }
                    // Turns coordinates into array indexes
                    int hitZ = (int)(hitPos.z / columnZSpace.z);
                    int hitX = (int)(hitPos.x / rowSpace.x);
                    // Only select if there's a piece on the hex
                    if (hexDex[hitZ, hitX]["piece"] != null)
                    {
                        // Toggles selected at coordinate
                        selected[hitZ, hitX] = !selected[hitZ, hitX];
                        // Toggles outline
                        hexDex[hitZ, hitX]["hex"].GetComponent<cakeslice.Outline>().enabled = !hexDex[hitZ, hitX]["hex"].GetComponent<cakeslice.Outline>().enabled;
                    }
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
