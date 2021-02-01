using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    #region Prefabs & scene objects
    public GameObject hexPrefab;
    public GameObject objHexPrefab;
    public GameObject whitePiecePrefab;
    public GameObject blackPiecePrefab;
    public GameObject cam;
    #endregion

    #region Game behavior variables for tweaking
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
    #endregion

    #region Variables for use during generation and gameplay
    private GameObject[,] hexDex;
    private bool[,] selected;
    private bool clickedLastFrame = false;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        #region Initialize varibles for use during board generation
        // Initialize hexDex as 2D array with size of rows and columns specified
        hexDex = new GameObject[rows, columns];
        // Initialize the "selected" array as a 2D array referring to the hexes and pieces that are selected
        selected = new bool[rows, columns];
        // Set all values of selected to false
        for (int i = 0; i < rows * columns; i++) { selected[i % rows, i / rows] = false; }

        // halfBoard = the number of rows that make up half the board, minus the middle row 
        int halfBoard = rows / 2;
        // Initialize random
        System.Random random = new System.Random();
        #endregion

        #region Generate objective hex arrangement
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
        #endregion

        #region Generate piece arrangement
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
        #endregion

        #region Generate gameboard
        // lastPosition = the last place we spawned in a hex, we'll then add some vectors to it to get our new position and spawn a new hex there
        Vector3 lastPosition = new Vector3(0f, 0f, 0f);
        // Whether or not the first hex in the last row generated was offsetted to the right
        bool lastWentRight = true;
        // Loop through each row and hex in each row
        for (int i = 0; i < hexDex.GetLength(0); i++)
        {
            for (int hexX = 0; hexX < hexDex.GetLength(1); hexX++)
            {
                #region Spawn hexes
                // Choose to place normal or objective hex based on earlier generation
                GameObject hexToPlace;
                if (objHexes[i, hexX]) { hexToPlace = objHexPrefab; } else { hexToPlace = hexPrefab; }
                // Spawn hex, add correct board position, and add to Hex object
                GameObject hexSpawned = Instantiate(hexToPlace, lastPosition, Quaternion.Euler(0f, 30f, 0f));
                hexSpawned.GetComponent<BoardPos>().x = hexX;
                hexSpawned.GetComponent<BoardPos>().z = i;
                hexDex[i, hexX] = hexSpawned;
                #endregion

                #region Spawn pieces
                // Choose whether or not to spawn pieces based on earlier generation
                if (pieces[i, hexX])
                {
                    // Choose to place black or white piece depending on position in the board
                    GameObject pieceToPlace;
                    if (i < halfBoard) { pieceToPlace = whitePiecePrefab; } else { pieceToPlace = blackPiecePrefab; }
                    // Spawn piece above hex, add correct board position, and place in hexDex
                    GameObject pieceSpawned = Instantiate(pieceToPlace, lastPosition + pieceVertical, Quaternion.identity);
                    pieceSpawned.GetComponent<BoardPos>().x = hexX;
                    pieceSpawned.GetComponent<BoardPos>().z = i;
                    hexDex[i, hexX].GetComponent<Hex>().piece = pieceSpawned;
                }
                #endregion

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
        #endregion

        #region Let hexes know their neighboring hexes
        int vertLeft;
        int vertRight;
        // These two variables determine what the top left and top right are, since they change depending on which row and the offset is
        int transVert = 0;
        int transHoriz = 0;
        // Loop through each row
        for (int i = 0; i < hexDex.GetLength(0); i++)
        {
            // Determine what top left and top right are based on whether it's an odd or even row
            if (i % 2 == 0)
            {
                vertLeft = -1;
                vertRight = 0;
            }
            else
            {
                vertLeft = 0;
                vertRight = 1;
            }

            // Loop through each hex in each row
            for (int hexX = 0; hexX < hexDex.GetLength(1); hexX++)
            {
                GameObject[] allNeighbors = new GameObject[6];
                // Assign for each of the six surrounding hexes
                for (int iter = 0; iter < 6; iter++)
                {
                    // Say where each hex is in relation to the current hex
                    switch (iter)
                    {
                        #region Left
                        case 0:
                            transHoriz = -1;
                            transVert = 0;
                            break;
                        #endregion
                        #region Right
                        case 1:
                            transHoriz = 1;
                            transVert = 0;
                            break;
                        #endregion
                        #region Top left
                        case 2:
                            transHoriz = vertLeft;
                            transVert = 1;
                            break;
                        #endregion
                        #region Top right
                        case 3:
                            transHoriz = vertRight;
                            transVert = 1;
                            break;
                        #endregion
                        #region Bottom left
                        case 4:
                            transHoriz = vertLeft;
                            transVert = -1;
                            break;
                        #endregion
                        #region Bottom right
                        case 5:
                            transHoriz = vertRight;
                            transVert = -1;
                            break;
                        #endregion
                    }
                    GameObject neighborHex;
                    // Makes sure that hexes on the edge get defined as null
                    if (!((hexX + transHoriz < 0 || hexX + transHoriz >= columns) || (i + transVert < 0 || i + transVert >= rows)))
                    {
                        neighborHex = hexDex[i + transVert, hexX + transHoriz];
                    }
                    else
                    {
                        neighborHex = null;
                    }
                    // Assigns the neighbor hex if there is one
                    switch (iter)
                    {
                        #region Left
                        case 0:
                            if (neighborHex != null)
                            {
                                hexDex[i, hexX].GetComponent<Hex>().left = neighborHex;
                            }
                            break;
                        #endregion
                        #region Right
                        case 1:
                            if (neighborHex != null)
                            {
                                hexDex[i, hexX].GetComponent<Hex>().right = neighborHex;
                            }
                            break;
                        #endregion
                        #region Top left
                        case 2:
                            if (neighborHex != null)
                            {
                                hexDex[i, hexX].GetComponent<Hex>().topLeft = neighborHex;
                            }
                            break;
                        #endregion
                        #region Top right
                        case 3:
                            if (neighborHex != null)
                            {
                                hexDex[i, hexX].GetComponent<Hex>().topRight = neighborHex;
                            }
                            break;
                        #endregion
                        #region Bottom left
                        case 4:
                            if (neighborHex != null)
                            {
                                hexDex[i, hexX].GetComponent<Hex>().bottomLeft = neighborHex;
                            }
                            break;
                        #endregion
                        #region Bottom right
                        case 5:
                            if (neighborHex != null)
                            {
                                hexDex[i, hexX].GetComponent<Hex>().bottomRight = neighborHex;
                            }
                            break;
                        #endregion
                    }

                    // Put this hex into the all neighbor hexes array
                    if (neighborHex != null)
                    {
                        allNeighbors[iter] = neighborHex;
                    }
                }
                // Set all neighbors array
                hexDex[i, hexX].GetComponent<Hex>().all = allNeighbors;
            }
            
        }
        #endregion

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
                    // Gets coordinates of hit piece
                    int hitX = hit.transform.gameObject.GetComponent<BoardPos>().x;
                    int hitZ = hit.transform.gameObject.GetComponent<BoardPos>().z;
                    // Only select if there's a piece on the hex
                    if (hexDex[hitZ, hitX].GetComponent<Hex>().piece != null)
                    {
                        // Toggles selected at coordinate
                        selected[hitZ, hitX] = !selected[hitZ, hitX];
                        // Toggles outline
                        hexDex[hitZ, hitX].GetComponent<cakeslice.Outline>().enabled = !hexDex[hitZ, hitX].GetComponent<cakeslice.Outline>().enabled;
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
