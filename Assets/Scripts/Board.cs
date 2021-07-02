using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Board : MonoBehaviour
{
    #region Prefabs & scene objects
    public GameObject hexPrefab;
    public GameObject objHexPrefab;
    public GameObject whitePiecePrefab;
    public GameObject blackPiecePrefab;
    public GameObject invalidMovementOptionText;
    public GameObject[] buttons;
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
    // The horizontal/z offset when hexes are tiled from top to bottom
    public Vector3 columnSpace;
    // The offset of every other row
    public Vector3 rowOffset;
    // Number of pieces for each player
    public int pieceNum;
    // Vertical offset of each piece
    public Vector3 pieceVertical;
    #endregion

    #region Variables for use during generation and gameplay
    public GameObject[,] hexDex;
    private List<GameObject> selected = new List<GameObject>();
    private List<GameObject> highlighted = new List<GameObject>();
    private bool clickedLastFrame = false;
    // The direction the piece is moving for multiple piece moving
    private List<string> movementDirections = new List<string>();
    
    #region Movement option chosen
    // Whether a movement option is chosen at all
    private bool selectedMoving = false;
    private bool singleMoving = false;
    private bool cannonMoving = false;
    private bool waveMoving = false;
    private bool contiguousMoving = false;
    private bool vMoving = false;
    #endregion
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Initialize hexDex as 2D array with size of rows and columns specified
        hexDex = new GameObject[rows, columns];

        // halfBoard = the number of rows that make up half the board, minus the middle row 
        int halfBoard = rows / 2;
        // Initialize random
        System.Random random = new System.Random();

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
        // Dimension 1
        for (int i = 0; i < hexDex.GetLength(0); i++)
        {
            // Dimension 2
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
            lastPosition += columnSpace;
            // Offsets first hex (for next time through loop)
            if (lastWentRight) 
            {
                lastPosition += rowOffset;
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
                Dictionary<string, GameObject> neighbors = new Dictionary<string, GameObject>();
                hexDex[i, hexX].GetComponent<Hex>().neighbors = neighbors;
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
                                hexDex[i, hexX].GetComponent<Hex>().neighbors["left"] = neighborHex;
                            }
                            break;
                        #endregion
                        #region Right
                        case 1:
                            if (neighborHex != null)
                            {
                                hexDex[i, hexX].GetComponent<Hex>().neighbors["right"] = neighborHex;
                            }
                            break;
                        #endregion
                        #region Top left
                        case 2:
                            if (neighborHex != null)
                            {
                                hexDex[i, hexX].GetComponent<Hex>().neighbors["topLeft"] = neighborHex;
                            }
                            break;
                        #endregion
                        #region Top right
                        case 3:
                            if (neighborHex != null)
                            {
                                hexDex[i, hexX].GetComponent<Hex>().neighbors["topRight"] = neighborHex;
                            }
                            break;
                        #endregion
                        #region Bottom left
                        case 4:
                            if (neighborHex != null)
                            {
                                hexDex[i, hexX].GetComponent<Hex>().neighbors["bottomLeft"] = neighborHex;
                            }
                            break;
                        #endregion
                        #region Bottom right
                        case 5:
                            if (neighborHex != null)
                            {
                                hexDex[i, hexX].GetComponent<Hex>().neighbors["bottomRight"] = neighborHex;
                            }
                            break;
                        #endregion
                    }
                }
            }
            
        }
        #endregion

        // Centers camera with generated board by setting transform x position to be half the distance of the number of columns * row space offset
        Camera.main.transform.position = new Vector3((columns * rowSpace.x) / 2, Camera.main.transform.position.y, Camera.main.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (!clickedLastFrame)
            {
                // Casts the ray and get the first game object hit
                // This required colliders since it's a physics action
                // Since everything was made with Maya they won't have colliders already
                // So make sure that everything we need to click on is set to have a mesh collider
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                Physics.Raycast(ray, out hit);
                
                // Makes sure it hit something
                if (hit.collider != null)
                {
                    // Gets coordinates of hit piece
                    int hitX = hit.transform.gameObject.GetComponent<BoardPos>().x;
                    int hitZ = hit.transform.gameObject.GetComponent<BoardPos>().z;
                    // Checks if player has already selected a movement option
                    // If they haven't, go on with selecting, if they have, go on with checking movement
                    if (!selectedMoving)
                    {
                        // Only select if there's a piece on the hex
                        if (hexDex[hitZ, hitX].GetComponent<Hex>().piece != null)
                        {
                            // Makes sure outline color is selection color
                            hexDex[hitZ, hitX].GetComponent<cakeslice.Outline>().color = 0;
                            // Adds to list of selected if it's not selected, remove if it is
                            if (!selected.Contains(hexDex[hitZ, hitX]))
                            {
                                selected.Add(hexDex[hitZ, hitX]);
                            }
                            else
                            {
                                selected.Remove(hexDex[hitZ, hitX]);
                            }
                            // Toggles outline
                            hexDex[hitZ, hitX].GetComponent<cakeslice.Outline>().enabled = selected.Contains(hexDex[hitZ, hitX]);
                        }
                    }
                    else
                    {
                        // When you click the movement option button, the correct options are highlighted green
                        // Checks if hex clicked is highlighted green which would mean that you can move there
                        int color = hexDex[hitZ, hitX].GetComponent<cakeslice.Outline>().color;
                        if (hexDex[hitZ, hitX].GetComponent<cakeslice.Outline>().enabled && (color == 1 || color == 2))
                        {
                            // Checks movement option and executes proper move when clicked
                            if (singleMoving) 
                            {
                                // Moves piece via movepiece function
                                MovePiece(selected[0].GetComponent<Hex>().piece, hitX, hitZ, canStack: true);
                                // Resets moving variable and buttons
                                singleMoving = false;
                                ChangeButtons(0, true);
                            }
                            else if (cannonMoving) 
                            {
                                // Initialize movement direction
                                string movementDirection = "";
                                // Cache selected hex to start from
                                GameObject hex = selected[0];
                                // Loop out in the possible directions from the selected hex until we hit the hex we want to move to or run out
                                // Then move on or choose that direction
                                foreach (string direction in movementDirections)
                                {
                                    // Check if current hex is hex we want to move to
                                    while (hex.GetComponent<Hex>().neighbors[direction] != null && hex.GetComponent<Hex>().neighbors[direction] != hexDex[hitZ, hitX])
                                    {
                                        // Set hex to be next
                                        hex = hex.GetComponent<Hex>().neighbors[direction];
                                    }
                                    // If the next hex existed, then we must have exited the loop because it was the right direction
                                    if (hex.GetComponent<Hex>().neighbors[direction] != null)
                                    {
                                        // Save movement direction
                                        movementDirection = direction;
                                    }
                                }
                                // Move piece via move piece function
                                MovePiece(selected[0].GetComponent<Hex>().piece, hitX, hitZ, movementDirection: movementDirection);
                                cannonMoving = false;
                                ChangeButtons(2, true);
                                // Reset movement directions
                                movementDirections = new List<string>();
                            }
                            else if (waveMoving) 
                            {
                                // Future movement code
                                // Resests moving variable and buttons
                                waveMoving = false;
                                ChangeButtons(1, true);
                            }
                            else if (contiguousMoving) 
                            {
                                // Future movement code
                                // Resests moving variable and buttons
                                contiguousMoving = false;
                                ChangeButtons(4, true);
                            }
                            else if (vMoving) 
                            {
                                // Future movement code
                                // Resests moving variable and buttons
                                vMoving = false;
                                ChangeButtons(3, true);
                            }
                            // Turns off moving
                            selectedMoving = false;
                            // Loops through all neighbors and unoutlines them
                            DehighlightAllHexes();
                            DeselectAllHexes();
                        }
                    }
                }
            }
            clickedLastFrame = true;
        } 
        else 
        {
            clickedLastFrame = false;
        }

        // Deselect all hexes with right click
        if (Input.GetMouseButton(1) && !selectedMoving)
        {
            DeselectAllHexes();
        }
    }

    #region Deselect and dehighlight selected or hghilighted hexes
    private void DeselectAllHexes()
    {
        // Iterate through each selected hex
        foreach (GameObject hex in selected)
        {
            // Turn off outline
            hex.GetComponent<cakeslice.Outline>().enabled = false;
        }
        // Resets selected back to empty list
        selected = new List<GameObject>();
    }

    private void DehighlightAllHexes()
    {
        // Iterate through each highlighted hex
        foreach (GameObject hex in highlighted)
        {
            // Turn off outline
            hex.GetComponent<cakeslice.Outline>().enabled = false;
        }
        // Resets highlighted back to empty list
        highlighted = new List<GameObject>();
    }
    #endregion

    // Finds lines of pieces of the same color
    private Dictionary<string, List<GameObject>> FindLines(BoardPos position)
    {
        // Initialize variables
        // Lines to return
        // String is the direction the line is in
        // List of GameObjects is the list of hexes in the line
        Dictionary<string, List<GameObject>> lines = new Dictionary<string, List<GameObject>>();
        // Hex that is the source of the line
        Hex sourceHex = hexDex[position.z, position.x].GetComponent<Hex>();
        // Color of the piece which line we want to get
        string color = sourceHex.piece.tag;

        // Loop through each neighbor of the original hex
        foreach (string direction in sourceHex.neighbors.Keys)
        {
            // Set hex to original hex
            GameObject hex = hexDex[position.z, position.x];
            // Initialize list in this direction as empty list
            lines[direction] = new List<GameObject>();
            // Add source hex (for seeing if line is selected)
            lines[direction].Add(hex);

            // Loop infinitely in the same direction
            while (true)
            {
                // Make sure key is assigned
                GameObject nextHex;
                try
                {
                    // Get next hex in the line
                    nextHex = hex.GetComponent<Hex>().neighbors[direction];
                }
                catch (KeyNotFoundException)
                {
                    break;
                }

                // Add piece to line if there's a piece of the same color in the same direction
                if (nextHex != null && nextHex.GetComponent<Hex>().piece != null && nextHex.GetComponent<Hex>().piece.tag == color)
                {
                    lines[direction].Add(nextHex);
                }
                // Break the loop if the line ends
                else
                {
                    break;
                }

                // Set up the next hex for the next time through the loop
                hex = nextHex;
            }
        }

        return lines;
    }

    #region Functions for moving
    private void MovePiece(GameObject piece, int newX, int newZ, bool canStack = false, string movementDirection = null)
    {
        // Initialize stacking variable
        bool stacking;

        // Reassign the pieces on the hexes if the piece is not stacking
        // Stacking case
        if (canStack && hexDex[newZ, newX].GetComponent<Hex>().piece != null && hexDex[newZ, newX].GetComponent<Hex>().piece.tag == piece.tag)
        {
            stacking = true;
            // Make old hex have no pieces
            hexDex[piece.GetComponent<BoardPos>().z, piece.GetComponent<BoardPos>().x].GetComponent<Hex>().piece = null;
        }
        // Not stacking or attacking a stack case
        else if (hexDex[newZ, newX].GetComponent<Hex>().piece == null || hexDex[newZ, newX].GetComponent<Hex>().piece.GetComponent<Piece>().stackedPieces.Count == 0)
        {
            stacking = false;
            hexDex[newZ, newX].GetComponent<Hex>().piece = piece;
            // Make old hex have no pieces
            hexDex[piece.GetComponent<BoardPos>().z, piece.GetComponent<BoardPos>().x].GetComponent<Hex>().piece = null;
        }
        // Attacking a stack and multiple hex moving
        else if (cannonMoving || vMoving)
        {
            hexDex[newZ, newX].GetComponent<Hex>().neighbors[movementDirection].GetComponent<Hex>().piece = piece;
            // Make old hex have no pieces
            hexDex[piece.GetComponent<BoardPos>().z, piece.GetComponent<BoardPos>().x].GetComponent<Hex>().piece = null;
            stacking = false;
        }
        // Attacking a stack case
        else
        {
            stacking = false;
        }

        // Move piece
        piece.GetComponent<Piece>().Move(
            hexDex[newZ, newX].transform.position,
            newX, 
            newZ, 
            stacking: stacking,  
            stackingOnto: hexDex[newZ, newX].GetComponent<Hex>().piece,
            bottomPiece: true,
            multipleHexMove: cannonMoving,
            multipleHexDirection: movementDirection
        );
    }

    #region Moving buttons
    #region Invalid movement option display and rescind
    private void InvalidMovementOptionDisplay(string error = "Invalid Movement Option")
    {
        invalidMovementOptionText.GetComponent<TextMeshProUGUI>().SetText(error);
        invalidMovementOptionText.GetComponent<TextMeshProUGUI>().enabled = true;
        Invoke(nameof(InvalidMovementOptionRescind), 2f);
    }

    private void InvalidMovementOptionRescind()
    {
        invalidMovementOptionText.GetComponent<TextMeshProUGUI>().enabled = false;
    }
    #endregion

    private void ChangeButtons(int buttonNum, bool on)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i != buttonNum)
            {
                buttons[i].GetComponent<Button>().interactable = on;
            }
        }
    }

    public string GetOppositeDirection(string direction) 
    {
        switch (direction)
        {
            case "left":
                return "right";
            case "right":
                return "left";
            case "topLeft":
                return "bottomRight";
            case "topRight":
                return "bottomLeft";
            case "bottomLeft":
                return "topRight";
            case "bottomRight":
                return "topLeft";
            default:
                return null;
        }
    }

    public bool NothingSelected()
    {
        if (selected.Count == 0)
        {
            InvalidMovementOptionDisplay("No pieces selected");
            return true;
        }
        else
        {
            return false;
        }
    }

    // When pressed, they enable moving and update hilighting
    public void SingleMovement()
    {
        // Makes sure a piece is selected
        if (!NothingSelected())
        {
            // Makes sure there's only one piece selected for single movement
            // If the button was pressed again to deselect single movement, there should still only be one hex selected since movement option select should disable selection
            if (selected.Count == 1)
            {
                // Toggles moving and singleMoving
                selectedMoving = !selectedMoving;
                singleMoving = !singleMoving;
                ChangeButtons(0, !selectedMoving);
                if (selectedMoving)
                {
                    // Loops through all neighbors and outlines them as valid moves
                    foreach (GameObject hex in selected[0].GetComponent<Hex>().neighbors.Values)
                    {
                        // Makes sure there is a hex in the neighbor position
                        if (hex != null)
                        {
                            int color;
                            if (hex.GetComponent<Hex>().piece == null)
                            {
                                color = 1;
                            }
                            else
                            {
                                color = 2;
                            }
                            // Changes outline color to one/green and turns on or off the outline
                            hex.GetComponent<cakeslice.Outline>().color = color;
                            // Setting the value to singleMoving makes it so if we're selecting the single movement movement option, it turns on, but turns off if deselecting
                            hex.GetComponent<cakeslice.Outline>().enabled = singleMoving;
                            // Adds hex to the list of hilighted
                            if (!highlighted.Contains(hex))
                            {
                                highlighted.Add(hex);
                            }
                        }
                    }
                }
                else
                {
                    DehighlightAllHexes();
                }
            }
            else
            {
                InvalidMovementOptionDisplay("Select only one piece for Single Movement");
            }
        }
    }

    public void WaveMovement()
    {
        if (!NothingSelected())
        {
            // Toggles moving and waveMoving
            selectedMoving = !selectedMoving;
            waveMoving = !waveMoving;
            ChangeButtons(1, !selectedMoving);
            // Future code that checks and highlights possible moves
        }
    }

    public void CannonMovement()
    {
        if (!NothingSelected())
        {
            // Makes sure that only one hex is selected
            if (selected.Count == 1)
            {
                #region Initialize/Cache variables
                // Hex to perform operations on
                GameObject hex = selected[0];
                // Gets lines from selected
                Dictionary<string, List<GameObject>> lines = FindLines(hex.GetComponent<BoardPos>());
                // Initialize directions list
                List<string> directions = new List<string>();
                // Cache neighbors
                Dictionary<string, GameObject> neighbors = hex.GetComponent<Hex>().neighbors;
                #endregion

                // Gets directions to highlight
                foreach (string direction in lines.Keys)
                {
                    // Make sure not to add directions where a piece is in the middle of a line
                    // If there is a piece in one direction and not in the opposite direction then the piece is at the end of a line in that direction
                    if (neighbors[direction].GetComponent<Hex>().piece == null && neighbors[GetOppositeDirection(direction)].GetComponent<Hex>().piece != null)
                    {
                        directions.Add(direction);
                    }
                }
                // Make sure that the piece has at least one line and is not only in the middle of line(s)
                if (directions.Count != 0)
                {
                    // Toggles moving and cannonMoving
                    selectedMoving = !selectedMoving;
                    cannonMoving = !cannonMoving;
                    ChangeButtons(2, !selectedMoving);

                    // Dehilight all hexes if deselecting a movement option
                    if (!selectedMoving)
                    {
                        DehighlightAllHexes();
                        return;
                    }

                    // Loop through all directions piece can move
                    // This if for if piece is the end of multiple lines
                    foreach (string direction in directions)
                    {
                        hex = selected[0];
                        // Store direction for moving
                        movementDirections.Add(direction);

                        // Highlight possible moves
                        for (int i = 0; i < lines[GetOppositeDirection(direction)].Count; i++)
                        {
                            // Makes sure the hexes down the board exist
                            try
                            {
                                // Checks and highlights valid moves
                                // Make sure the hex down the board exists
                                if (hex.GetComponent<Hex>().neighbors[direction] != null
                                    // Makes sure no pieces of the same color are blocking the way
                                    // Checks if there's a piece on the hex
                                    && !(hex.GetComponent<Hex>().neighbors[direction].GetComponent<Hex>().piece != null 
                                        // Checks if the piece on hex is the same color as the first piece in the line
                                        && hex.GetComponent<Hex>().neighbors[direction].GetComponent<Hex>().piece.tag 
                                            == lines[direction][0].GetComponent<Hex>().piece.tag))
                                {
                                    // Sets current hex to hex to the direction of the past hex
                                    hex = hex.GetComponent<Hex>().neighbors[direction];
                                    // Changes color to one/green
                                    hex.GetComponent<cakeslice.Outline>().color = 1;
                                    // Toggles outline
                                    hex.GetComponent<cakeslice.Outline>().enabled = cannonMoving;
                                    // Makes sure hex is not in highlighted
                                    if (!highlighted.Contains(hex))
                                    {
                                        // Add hex to highlighted list
                                        highlighted.Add(hex);
                                    }
                                    // If there's a piece on the hex and it is stacked
                                    if (hex.GetComponent<Hex>().piece != null && hex.GetComponent<Hex>().piece.GetComponent<Piece>().stackedPieces.Count != 0)
                                    {
                                        hex.GetComponent<cakeslice.Outline>().color = 2;
                                        break;
                                    }
                                }
                                // Stop the highlighting, make all moves down the line invalid
                                else
                                {
                                    break;
                                }
                            }
                            catch (KeyNotFoundException)
                            {
                                break;
                            }
                        }
                    }
                }
                else
                {
                    InvalidMovementOptionDisplay("Piece must be at the end of a line");
                }
            }
            else
            {
                InvalidMovementOptionDisplay("Select only one piece for Cannon Movement");
            }
        }
    }

    public void VMovement()
    {
        if (!NothingSelected())
        {
            // Toggles moving and vMoving
            selectedMoving = !selectedMoving;
            vMoving = !vMoving;
            ChangeButtons(3, !selectedMoving);
            // Future code that checks and highlights possible moves
        }
    }

    public void ContiguousMovement()
    {
	// gsdebug: compare with SingleMovement
        if (!NothingSelected())
        {
            // Toggles moving and contiguousMoving
            // Future code that checks and highlights possible moves
            selectedMoving = !selectedMoving;
            contiguousMoving = !contiguousMoving;
            ChangeButtons(4, !selectedMoving);
        }
    }
    #endregion
    #endregion
}
