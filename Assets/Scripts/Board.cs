using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Board : MonoBehaviour
{
    #region Prefabs & scene objects
    #region Game pieces
    public GameObject hexPrefab;
    public GameObject objHexPrefab;
    public GameObject whitePiecePrefab;
    public GameObject blackPiecePrefab;
    #endregion

    #region Movement icons
    public GameObject curledMovementArrow;
    public GameObject attackIcon;
    public GameObject stackIcon;
    #endregion

    public TextMeshProUGUI invalidMovementOptionText;
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
    // Vertical offset of the movement arrows
    public Vector3 movementIconVertical;

    // The amount of time it takes to rescind the invalid movement option text
    // Since the project's fixed timeskip is probably set to 0.02 or 1/50th it should be 100
    public int textRescindTime;
    #endregion

    #region Variables for use during generation and gameplay
    // Index of hexes ordered by z, x position
    public GameObject[,] hexDex;
    // Selected hexes
    private List<GameObject> selected = new List<GameObject>();
    // Highlighted hexes
    private List<GameObject> highlighted = new List<GameObject>();
    // Whether the player clicked the previous frame
    private bool clickedLastFrame = false;

    // Movement arrow object currently in use
    private Dictionary<string, List<GameObject>> movementIcons;
    // Template for empty movement icons variable
    private Dictionary<string, List<GameObject>> emptyMovementIcons = new Dictionary<string, List<GameObject>> {
                                                                                                                    {"arrows", new List<GameObject>()},
                                                                                                                    {"attack", new List<GameObject>()},
                                                                                                                    {"stack",  new List<GameObject>()}
                                                                                                                };
    // The hex hit with a raycast on the previous frame
    private GameObject previousHexHit;

    // List of all the string directions
    private string[] possibleDirections = {"right", "bottomRight", "bottomLeft", "left", "topLeft", "topRight"};
    // The direction the piece is moving for multiple piece moving
    private List<string> movementDirections = new List<string>();
    // The amount of time left to rescind the invalid movement option text
    private int textRescindCountdown;

    #region Variables for contiguous movement
    // The contiguous pieces
    private List<GameObject> contiguousPieces = new List<GameObject>();
    // The valid hexes and the directions taken to them
    // For each hex there is a list of the lists of directions that it took to get there
    private Dictionary<GameObject, List<List<string>>> contiguousHexes = new Dictionary<GameObject, List<List<string>>>();
    // The directions it has taken to get to a hex
    private List<string> directionsList = new List<string>();
    #endregion
    
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
        // Casts the ray and get the first game object hit
        // This required colliders since it's a physics action
        // Since everything was made with Maya they won't have colliders already
        // So make sure that everything we need to click on is set to have a collider
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Physics.Raycast(ray, out hit);

        // If something was hit
        if (hit.collider != null)
        {
            // Cache board position
            BoardPos hexHitBoardPos = hit.transform.gameObject.GetComponent<BoardPos>();
            // Gets coordinates of hit piece
            int hitX = hexHitBoardPos.x;
            int hitZ = hexHitBoardPos.z;
            // Get hex hit
            GameObject hexHit = hexDex[hitZ, hitX];

            // Cache color
            int color = hexHit.GetComponent<cakeslice.Outline>().color;
            // Movement icons case
            // If we're selecting a move and the hex hit is highlighted a valid color or there is already a movement icon
            if (selectedMoving && (hexHit.GetComponent<cakeslice.Outline>().enabled && (color == 1 || color == 2)))
            {
                // Only generate icons if there isn't one already or if the one hit this frame doesn't match the one from last frame
                if (movementIcons == null || hexHit != previousHexHit)
                {
                    // Destroy old icons if hex hit this frame doesn't match last frame's
                    if (hexHit != previousHexHit)
                    {
                        KillAllMovementIcons();
                    }
                    // Initialize movementIcons and keys
                    movementIcons = emptyMovementIcons;

                    if (singleMoving)
                    {
                        // If there's no piece on moused over hex 
                        if (hexHit.GetComponent<Hex>().piece == null
                            // Or if the piece on the moused over hex is the opposite color and not stacked
                            || (hexHit.GetComponent<Hex>().piece.tag != selected[0].GetComponent<Hex>().piece.tag
                                && hexHit.GetComponent<Hex>().piece.GetComponent<Piece>().stackedPieces.Count == 0)
                            // Or if the piece on the moused over hex is the same color as the selected piece
                            || hexHit.GetComponent<Hex>().piece.tag == selected[0].GetComponent<Hex>().piece.tag)
                        // Otherwise display only movement icon
                        {
                            // Place arrow
                            PlaceArrow(selected[0], hexHit);

                            // Spawn aditional icons
                            // If there is a piece on the hex
                            if (hexHit.GetComponent<Hex>().piece != null)
                            {
                                // Cache piece
                                GameObject piece = hexHit.GetComponent<Hex>().piece;
                                // Type of icon to choose
                                string key;
                                // If the piece moused over is the opposite color
                                // (We shouldn't have to check for it being stacked since we did that already)
                                if (piece.tag != selected[0].GetComponent<Hex>().piece.tag)
                                {
                                    key = "attack";
                                }
                                // If the piece moused over is the same color
                                else
                                {
                                    key = "stack";
                                }
                                PlaceIcon(hexHit, key);
                            }
                        }
                        else
                        {
                            PlaceIcon(hexHit);
                        }
                    }
                    else if (cannonMoving)
                    {
                        // Cache hex
                        GameObject hex = selected[0];
                        // Get direction to put arrows in
                        string direction = GetDirection(hex, hexHit, movementDirections);
                        // Place arrows up to hit hex
                        do 
                        {
                            // Set hex to be next
                            hex = hex.GetComponent<Hex>().neighbors[direction];
                            // Cache hex component
                            Hex hexComponent = hex.GetComponent<Hex>();
                            // If there are pieces on this hex add attack icon
                            if (hexComponent.piece != null)
                            {
                                // Add attack icon
                                PlaceIcon(hex);
                                // If the pieces on this hex are stacked then don't place arrow
                                if (hexComponent.piece.GetComponent<Piece>().stackedPieces.Count != 0)
                                {
                                    break;
                                }
                            }
                            // Place arrow between current and previous hex
                            PlaceArrow(hex.GetComponent<Hex>().neighbors[GetOppositeDirection(direction)], hex);
                        }
                        while (hex != hexHit);
                    }
                    else if (waveMoving)
                    {
                        
                    }
                    else if (contiguousMoving)
                    {
                        // Initialize list to store shortest list of directions
                        List<string> directionList = null;
                        // Go through each directions list and find the shortest one
                        foreach (List<string> directions in contiguousHexes[hexHit])
                        {
                            if (directionList == null)
                            {
                                directionList = directions;
                            }
                            else if (directions.Count < directionList.Count)
                            {
                                directionList = directions;
                            }
                        }
                        // Go through list of directions and place arrows
                        GameObject hex = selected[0];
                        foreach (string direction in directionList)
                        {
                            PlaceArrow(hex, hex.GetComponent<Hex>().neighbors[direction]);
                            hex = hex.GetComponent<Hex>().neighbors[direction];
                        }
                        // Place attack icons
                        // Because of our highlighting algorithm, hexes with pieces of the same color should not be highlighted so we don't need to worry
                        if (hexHit.GetComponent<Hex>().piece != null)
                        {
                            PlaceIcon(hexHit);
                        }
                    }
                    else if (vMoving)
                    {
                        
                    }
                }
            }
            else if (movementIcons != null)
            {
                KillAllMovementIcons();
            }

            // If clicked
            if (Input.GetMouseButton(0))
            {
                // If not holding down mouse button and hit something
                if (!clickedLastFrame && hit.collider != null)
                {
                    // Checks if player has already selected a movement option
                    // If they haven't, go on with selecting, if they have, go on with checking movement
                    if (!selectedMoving)
                    {
                        // Only select if there's a piece on the hex
                        if (hexHit.GetComponent<Hex>().piece != null)
                        {
                            // Makes sure outline color is selection color
                            hexHit.GetComponent<cakeslice.Outline>().color = 0;
                            // Adds to list of selected if it's not selected, remove if it is
                            if (!selected.Contains(hexHit))
                            {
                                selected.Add(hexHit);
                            }
                            else
                            {
                                selected.Remove(hexHit);
                            }
                            // Toggles outline
                            hexHit.GetComponent<cakeslice.Outline>().enabled = selected.Contains(hexHit);
                        }
                    }
                    else
                    {
                        // When you click the movement option button, the correct options are highlighted green
                        // Checks if hex clicked is highlighted green which would mean that you can move there
                        if (hexHit.GetComponent<cakeslice.Outline>().enabled && (color == 1 || color == 2))
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
                            else if (waveMoving) 
                            {
                                // Future movement code
                                // Resests moving variable and buttons
                                waveMoving = false;
                                ChangeButtons(1, true);
                            }
                            else if (cannonMoving) 
                            {
                                // Move piece via move piece function
                                MovePiece(selected[0].GetComponent<Hex>().piece, hitX, hitZ, movementDirection: GetDirection(selected[0], hexHit, movementDirections));
                                cannonMoving = false;
                                ChangeButtons(2, true);
                                // Reset movement directions
                                movementDirections = new List<string>();
                            }
                            else if (vMoving) 
                            {
                                // Future movement code
                                // Resests moving variable and buttons
                                vMoving = false;
                                ChangeButtons(3, true);
                            }
                            else if (contiguousMoving) 
                            {
                                MovePiece(selected[0].GetComponent<Hex>().piece, hitX, hitZ);
                                // Resests moving variable and buttons
                                contiguousMoving = false;
                                ChangeButtons(4, true);
                                // Reset variables
                                contiguousHexes  = new Dictionary<GameObject, List<List<string>>>();
                                contiguousPieces = new List<GameObject>();
                                directionsList   = new List<string>();
                            }
                            // Turns off moving
                            selectedMoving = false;
                            // Unoutlines and deselects all
                            DehighlightAllHexes();
                            DeselectAllHexes();
                        }
                    }
                }
                clickedLastFrame = true;
            } 
            else 
            {
                clickedLastFrame = false;
            }
            // Set hex hit this frame to hex hit previous frame
            previousHexHit = hexHit;
        }
        // Deselect all hexes with right click
        if (Input.GetMouseButton(1) && !selectedMoving)
        {
            DeselectAllHexes();
        }
    }

    // FixedUpdate is called at a fixed interval
    private void FixedUpdate() 
    {
        if (invalidMovementOptionText.enabled)
        {
            textRescindCountdown--;
            if (textRescindCountdown <= 0 )
            {
                invalidMovementOptionText.enabled = false;
            }
        }
    }

    #region Functions for utility
    private void KillAllMovementIcons()
    {
        if (movementIcons != null)
        {
            foreach (string type in movementIcons.Keys)
            {
                foreach (GameObject icon in movementIcons[type])
                {
                    GameObject.Destroy(icon);
                }
            }
            movementIcons = null;
        }
    }  

    #region Deselect and dehighlight selected or highlighted hexes
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

    private void DehighlightAllHexes(bool excludeSelected = false)
    {
        // Iterate through each highlighted hex
        foreach (GameObject hex in highlighted)
        {
            // If not exclude selected or exclude selected and this hex isn't selected
            if (!excludeSelected || (excludeSelected && !selected.Contains(hex)))
            {
                // Turn off outline
                hex.GetComponent<cakeslice.Outline>().enabled = false;
            }
        }
        // Resets highlighted back to empty list
        highlighted = new List<GameObject>();
    }
    #endregion

    private void OutlineHex(GameObject hex, int color)
    {
        // Cache outline component
        cakeslice.Outline outline = hex.GetComponent<cakeslice.Outline>();
        // Changes outline color
        outline.color = color;
        // Setting the value to singleMoving makes it so if we're selecting the single movement movement option, it turns on, but turns off if deselecting
        outline.enabled = true;
        // Adds hex to the list of hilighted
        if (!highlighted.Contains(hex))
        {
            highlighted.Add(hex);
        }
    }

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

    // Gets directions that lines are in
    List<string> GetLineDirections(Dictionary<string, List<GameObject>> lines)
    {
        // List of directions to return
        List<string> directions = new List<string>();
        foreach (string direction in lines.Keys)
        {
            // Make sure not to add directions where a piece is in the middle of a line
            // If the line is just one (just the source hex) in one direction and the opposite direction it's more than one
            if (lines[direction].Count == 1 && lines.ContainsKey(GetOppositeDirection(direction)) && lines[GetOppositeDirection(direction)].Count > 1)
            {
                directions.Add(direction);
            }
        }

        return directions;
    }

    // Gets direction target hex (not adjacent to source) is in
    string GetDirection(GameObject source, GameObject target, List<string> directions)
    {
        // Initialize movement direction
        string movementDirection = "";
        // Loop out in the possible directions from the selected hex until we hit the hex we want to move to or run out
        // Then move on or choose that direction
        foreach (string direction in directions)
        {
            // Choose selected hex to start from
            GameObject hex = source;
            // Check if current hex is hex we want to move to
            while (hex.GetComponent<Hex>().neighbors.ContainsKey(direction) && hex.GetComponent<Hex>().neighbors[direction] != target)
            {
                // Set hex to be next
                hex = hex.GetComponent<Hex>().neighbors[direction];
            }
            // If the next hex existed, then we must have exited the loop because it was the right direction
            if (hex.GetComponent<Hex>().neighbors.ContainsKey(direction))
            {
                // Save movement direction
                movementDirection = direction;
            }
        }
        return movementDirection;
    }

    // Place movement arrows from hex1 to hex2
    private void PlaceArrow(GameObject hex1, GameObject hex2)
    {
        // Get the position the arrow will be in
        // Average position of the two hexes plus the vertical offset
        Vector3 iconPosition = ((hex1.transform.position + hex2.transform.position) / 2) + movementIconVertical;
        // Find multiple of rotation by iterating through and finding which direcion it faces
        int rotation = 0;
        for (int i = 0; i < possibleDirections.Length; i++)
        {
            // If the hex in that direction exists and in that direction is the hex we hit
            if (hex1.GetComponent<Hex>().neighbors.ContainsKey(possibleDirections[i]) 
                && hex1.GetComponent<Hex>().neighbors[possibleDirections[i]] == hex2)
            {
                // Save rotation and end search
                rotation = i;
                break;
            }
        }
        // Spawn movement arrow
        movementIcons["arrows"].Add(Instantiate(curledMovementArrow, iconPosition, Quaternion.Euler(-90f, 0f, (float)(rotation * 60))));
    }

    private void PlaceIcon(GameObject hex, string key = "attack")
    {
        GameObject icon = null;
        if (key == "attack")
        {
            icon = attackIcon;
        }
        else if (key == "stack")
        {
            icon = stackIcon;
        }
        movementIcons[key].Add(
            Instantiate(icon, movementIconVertical + hex.GetComponent<Hex>().piece.transform.position, Quaternion.identity)
        );
    }

    public string GetOppositeDirection(string direction) 
    {
        // Get index of direction from possile directions
        int directionIndex = Array.IndexOf(possibleDirections, direction);
        // Add 3 to get direction halfway through list from current index
        directionIndex += 3;
        // If direction has overflowed past 6, bring it back
        if (directionIndex >= 6) { directionIndex -= 6; }
        // Return direction in new index
        return possibleDirections[directionIndex];
    }

    // Returns adjacent hexes with pieces on them
    private List<GameObject> GetAdjacentPieces(GameObject hex)
    {
        // Initialize list of hexes with pieces on them
        List<GameObject> adjacentPieces = new List<GameObject>();
        // Cache neighbors
        Dictionary<string, GameObject> neighbors = hex.GetComponent<Hex>().neighbors;
        // Loop through directions
        foreach (string direction in neighbors.Keys)
        {
            // If hex in that direction has a piece on it and that piece is the same color add that hex to the list
            if (neighbors[direction].GetComponent<Hex>().piece != null && neighbors[direction].GetComponent<Hex>().piece.tag == hex.GetComponent<Hex>().piece.tag)
            {
                adjacentPieces.Add(neighbors[direction]);
            }
        }
        return adjacentPieces;
    }
    #endregion

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
            // Make old hex have no pieces
            hexDex[piece.GetComponent<BoardPos>().z, piece.GetComponent<BoardPos>().x].GetComponent<Hex>().piece = null;
            // Assign piece to new hex
            hexDex[newZ, newX].GetComponent<Hex>().neighbors[GetOppositeDirection(movementDirection)].GetComponent<Hex>().piece = piece;
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
        invalidMovementOptionText.SetText(error);
        invalidMovementOptionText.enabled = true;
        textRescindCountdown = textRescindTime;
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

    #region Functions to check at the beginning of movement buttons
    #region Functions to check number of pieces selected and display errors
    private bool NothingSelected()
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

    private bool OnlyOneSelected()
    {
        if (selected.Count == 1)
        {
            return true;
        }
        else
        {
            InvalidMovementOptionDisplay("Select only one piece");
            return false;
        }
    }
    #endregion

    // Saves time looking for and toggling highlights for hexes
    private bool NotMoving(int buttonNum)
    {
        if (selectedMoving)
        {
            selectedMoving   = false;
            singleMoving     = false;
            cannonMoving     = false;
            waveMoving       = false;
            contiguousMoving = false;
            vMoving          = false;
            ChangeButtons(buttonNum, true);
            DehighlightAllHexes(true);
            return false;
        }
        else
        {
            return true;
        }
    }
    #endregion

    // When pressed, they enable moving and update hilighting
    public void SingleMovement()
    {
        // Makes sure only one piece is selected and we aren't already trying to move
        if (!NothingSelected() && OnlyOneSelected() && NotMoving(0))
        {
            // Toggles moving and singleMoving
            selectedMoving = true;
            singleMoving = true;
            ChangeButtons(0, false);
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
                    OutlineHex(hex, color);
                }
            }
        }
    }

    public void WaveMovement()
    {
        // Makes sure pieces are selected and we aren't already trying to move
        if (!NothingSelected() && NotMoving(1))
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
        // Make sure that only one hex is selected and we aren't already trying to move
        if (!NothingSelected() && OnlyOneSelected() && NotMoving(2))
        {
            #region Initialize/Cache variables
            // Hex to perform operations on
            GameObject hex = selected[0];
            // Gets lines from selected
            Dictionary<string, List<GameObject>> lines = FindLines(hex.GetComponent<BoardPos>());
            // Get directions line go in
            List<string> directions = GetLineDirections(lines);
            // Cache neighbors
            Dictionary<string, GameObject> neighbors = hex.GetComponent<Hex>().neighbors;
            #endregion

            // Make sure that the piece has at least one line and is not only in the middle of line(s)
            if (directions.Count != 0)
            {
                // Toggles moving and cannonMoving
                selectedMoving = true;
                cannonMoving = true;
                ChangeButtons(2, false);

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
                                // Outline hex
                                OutlineHex(hex, 1);
                                // If there's a piece on the current hex and it is stacked
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
    }

    public void VMovement()
    {
        // Make sure that only one hex is selected and we aren't already trying to move
        if (!NothingSelected() && OnlyOneSelected() && NotMoving(3))
        {
            // Lines and directions
            Dictionary<string, List<GameObject>> lines = FindLines(selected[0].GetComponent<BoardPos>());
            List<string> directions = GetLineDirections(lines);

            if (directions.Count >= 2)
            {
                // Find valid directions
                // List of pairs of valid directions
                List<string[]> Vs = new List<string[]>();
                foreach (string direction in directions)
                {
                    // String array to store two V pieces
                    string[] VPieces = new string[2];
                    // Index of current direction
                    int index1 = Array.IndexOf(possibleDirections, direction);
                    // Index of current direction +1, or the next consecutive direction
                    int index2 = index1 + 1;
                    // Roll back to zero if out of index range
                    if (index2 >= 6) { index2 -= 6; }
                    if (directions.Contains(possibleDirections[index2]))
                    {
                        VPieces[0] = direction;
                        VPieces[1] = possibleDirections[index2];
                        Vs.Add(VPieces);
                    }
                }
                // If any valid Vs are found
                if (Vs.Count != 0)
                {
                    // Toggles moving and vMoving
                    selectedMoving = true;
                    vMoving = true;
                    ChangeButtons(3, false);
                }
                else
                {
                    InvalidMovementOptionDisplay("Directions of a V must be consecutive");
                }
            }
            else
            {
                InvalidMovementOptionDisplay("Select a piece at the end of a V");
            }
        }
    }

    public void ContiguousMovement()
    {
        // Make sure that only one hex is selected and we aren't already trying to move
        if (!NothingSelected() && OnlyOneSelected() && NotMoving(4))
        {
            // Reset variables
            contiguousHexes  = new Dictionary<GameObject, List<List<string>>>();
            contiguousPieces = new List<GameObject>();
            directionsList   = new List<string>();
            // Finds contiguous pieces
            FindContiguous(selected[0]);
            // Makes sure there are contiguous pieces
            if (contiguousHexes.Count != 0)
            {
                // Toggles moving and contiguousMoving
                selectedMoving = true;
                contiguousMoving = true;
                ChangeButtons(4, false);
                // Go through every found piece
                foreach (GameObject hex in contiguousHexes.Keys)
                {
                    // If the neighbor doesn't have a piece on it, or if it has a piece of the opposite color, it is valid
                    if (hex.GetComponent<Hex>().piece == null || hex.GetComponent<Hex>().piece.tag != selected[0].GetComponent<Hex>().piece.tag)
                    {
                        // Choose color
                        int color = 0;
                        // If there's no piece, green
                        if (hex.GetComponent<Hex>().piece == null)
                        {
                            color = 1;
                        }
                        // If there is a piece of opposite color, hit color
                        else if (hex.GetComponent<Hex>().piece != null && hex.GetComponent<Hex>().piece.tag != selected[0].GetComponent<Hex>().piece.tag)
                        {
                            color = 2;
                        }
                        OutlineHex(hex, color);
                    }
                }
            }
            else
            {
                InvalidMovementOptionDisplay("Piece has no contiguous pieces");
            }
        }
    }

    private void FindContiguous(GameObject sourceHex, GameObject previousHex = null)
    {
        // Get pieces adjacent to current hex
        List<GameObject> adjacent = GetAdjacentPieces(sourceHex);
        // Loops through all found adjacent hexes with pieces
        foreach (GameObject hex in adjacent)
        {
            // If hex is not already found in contiguous pieces or is not the source hex
            if (hex != previousHex && hex != selected[0])
            {
                // Add hex to contiguous pieces
                contiguousPieces.Add(hex);
                // Add direction to directions list
                directionsList.Add(GetDirection(sourceHex, hex, new List<string>(possibleDirections)));
                
                // Go through every neighbor of each found contiguous piece
                foreach (GameObject hexNeighbor in hex.GetComponent<Hex>().neighbors.Values)
                {
                    // Add direction to directions list
                    directionsList.Add(GetDirection(hex, hexNeighbor, new List<string>(possibleDirections)));
                    // Initialize the list of lists at the key of the hex if not already done
                    if (!contiguousHexes.ContainsKey(hexNeighbor))
                    {
                        contiguousHexes[hexNeighbor] = new List<List<string>>();
                    }
                    // Add list of directions to big dictionary of directions for each hex
                    contiguousHexes[hexNeighbor].Add(new List<string>(directionsList));

                    // Clear this direction from the end of the list
                    if (directionsList.Count != 0)
                    {
                        directionsList.RemoveAt(directionsList.Count - 1);
                    }
                }
                FindContiguous(hex, sourceHex);
            }
        }
        if (directionsList.Count != 0)
        {
            directionsList.RemoveAt(directionsList.Count - 1);
        }
    }
    #endregion
    #endregion
}
