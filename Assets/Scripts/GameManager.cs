using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    public Board board;

    [Header("Game Pieces")]
    [SerializeField]
    private GameObject whiteHexPrefab;
    [SerializeField]
    private GameObject blackHexPrefab;
    [SerializeField]
    private GameObject whiteObjectiveHexPrefab;
    [SerializeField]
    private GameObject blackObjectiveHexPrefab;
    [SerializeField]
    private GameObject neutralHexPrefab;
    // First dimension is color, second is hex type (normal/objective)
    // 0 = white, 1 = black, 2 = neutral
    // 0 = normal, 1 = objective
    private GameObject[,] hexPrefabs = new GameObject[3, 2];
    [SerializeField]
    private GameObject whitePiecePrefab;
    [SerializeField]
    private GameObject blackPiecePrefab;

    [Header("Icons")]
    [SerializeField]
    private GameObject curledMovementArrow;
    [SerializeField]
    private GameObject attackIcon;
    [SerializeField]
    private GameObject stackIcon;
    [SerializeField]
    private GameObject hoveringPrism;

    [Header("UI")]
    [SerializeField]
    private TextMeshProUGUI invalidMovementOptionText;
    [SerializeField]
    private GameObject[] buttons;

    [Header("Game behavior variables for tweaking")]
    [SerializeField]
    // Number of objective hexes for each player
    private int objHexNum;
    [SerializeField]
    // Hexes between each player's side
    private int rows;
    [SerializeField]
    // The way/distance hexes are tiled from left to right
    private Vector3 rowSpace;
    [SerializeField]
    // Hexes to the right and left of player
    private int columns;
    [SerializeField]
    // The horizontal/z offset when hexes are tiled from top to bottom
    private Vector3 columnSpace;
    [SerializeField]
    // The offset of every other row
    private Vector3 rowOffset;
    [SerializeField]
    // Number of pieces for each player
    private int pieceNum;
    [SerializeField]
    // Vertical offset of each piece
    public Vector3 pieceVertical;
    [SerializeField]
    // Vertical offset of the movement arrows
    private Vector3 movementIconVertical;
    [SerializeField]
    // The amount of time it takes to rescind the invalid movement option text
    // Since the project's fixed timeskip is probably set to 0.02 or 1/50th it should be 100
    private int textRescindTime;

    #region Variables for use during generation and gameplay
    // Selected hexes
    private List<GameObject> selected = new List<GameObject>();
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

    // The hexes that a move would take a piece along if it were to move to a certain hex
    private Dictionary<GameObject, List<BoardPosition>> stepsTo = new Dictionary<GameObject, List<BoardPosition>>();
    // The amount of time left to rescind the invalid movement option text
    private int textRescindCountdown;

    #region Variables for contiguous movement
    // The valid hexes and the directions taken to them
    // For each hex there is a list of the lists of directions that it took to get there
    private Dictionary<GameObject, List<List<int>>> contiguousHexes = new Dictionary<GameObject, List<List<int>>>();
    // The hexes with pieces visited
    private List<GameObject> contiguousVisits;
    // The directions it has taken to get to a hex
    private List<int> directionsList = new List<int>();
    #endregion
    
    #region Movement option chosen
    // Whether a movement option is chosen at all
    private bool selectedMoving = false;
    private MovementType movementType;
    #endregion
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Initialize hexDex as 2D array with size of rows and columns specified
        board.hexDex = new GameObject[rows, columns];

        // Set up hex prefab array
        hexPrefabs[0, 0] = whiteHexPrefab;
        hexPrefabs[0, 1] = blackObjectiveHexPrefab;
        hexPrefabs[1, 0] = blackHexPrefab;
        hexPrefabs[1, 1] = whiteObjectiveHexPrefab;
        hexPrefabs[2, 0] = neutralHexPrefab;

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
        for (int z = 0; z < rows; z++)
        {
            // Dimension 2
            for (int x = 0; x < columns; x++)
            {
                // Hex and piece that are going to be placed
                GameObject hexToPlace;
                GameObject pieceToPlace;

                #region Choose colors
                // Color for top of hex
                int color;
                // Color on base of hex
                int type;
                
                // If on the first half
                // Color is white
                if (z < halfBoard)
                {
                    color = 0;
                    pieceToPlace = whitePiecePrefab;
                }
                // If z is one more than half the board and the number of rows is odd
                // Color is neutral
                else if (z == halfBoard && rows % 2 == 1)
                {
                    color = 2;
                    pieceToPlace = null;
                }
                // If on the second half
                // Color is black
                else
                {
                    color = 1;
                    pieceToPlace = blackPiecePrefab;
                }
                
                // Choose to place normal or objective hex based on earlier generation
                if (objHexes[z, x])
                {
                    type = 1;
                }
                else
                {
                    type = 0;
                }
                // Choose hex to place from color and type
                hexToPlace = hexPrefabs[color, type];
                #endregion

                #region Spawn hexes
                // Spawn hex, add correct board position, and add to Hex object
                GameObject hexSpawned = Instantiate(hexToPlace, lastPosition, Quaternion.Euler(0f, 30f, 0f));
                // Cache board position
                BoardPosition hexPosition = hexSpawned.GetComponent<BoardPosition>();
                hexPosition.x = x;
                hexPosition.z = z;
                board.hexDex[z, x] = hexSpawned;
                #endregion


                // Choose whether or not to spawn pieces based on earlier generation
                if (pieces[z, x])
                {
                    // Spawn piece above hex, add correct board position, and place in hexDex
                    GameObject pieceSpawned = Instantiate(pieceToPlace, lastPosition + pieceVertical, Quaternion.identity);
                    // Cache board position
                    BoardPosition piecePosition = pieceSpawned.GetComponent<BoardPosition>();
                    piecePosition.x = x;
                    piecePosition.z = z;
                    board.hexDex[z, x].GetComponent<Hex>().piece = pieceSpawned;
                }

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
        for (int z = 0; z < board.hexDex.GetLength(0); z++)
        {
            // Determine what top left and top right are based on whether it's an odd or even row
            vertLeft = -1 + z % 2;
            vertRight = 0 + z % 2;

            // Loop through each hex in each row
            for (int x = 0; x < board.hexDex.GetLength(1); x++)
            {
                GameObject[] neighbors = new GameObject[6];
                board.hexDex[z, x].GetComponent<Hex>().neighbors = neighbors;
                // Assign for each of the six surrounding hexes
                for (int direction = 0; direction < 6; direction++)
                {
                    // Assign a vertical translation based on top or bottom position
                    if (board.DirectionIsBottom(direction))
                    {
                        transVert = -1;
                    }
                    else if (board.DirectionIsMiddle(direction))
                    {
                        transVert = 0;
                    }
                    else if (board.DirectionIsTop(direction))
                    {
                        transVert = 1;
                    }

                    // Assign a horizontal translation based on if we're moving up or not and whether we're moving left or right
                    if (board.DirectionIsBottom(direction) || board.DirectionIsTop(direction))
                    {
                        if (board.DirectionIsRight(direction))
                        {
                            transHoriz = vertRight;
                        }
                        else
                        {
                            transHoriz = vertLeft;
                        }
                    }
                    else
                    {
                        if (board.DirectionIsRight(direction))
                        {
                            transHoriz = 1;
                        }
                        else
                        {
                            transHoriz = -1;
                        }
                    }
                    GameObject neighborHex;
                    // Makes sure that hexes on the edge get defined as null
                    if (!((x + transHoriz < 0 || x + transHoriz >= columns) || (z + transVert < 0 || z + transVert >= rows)))
                    {
                        neighborHex = board.hexDex[z + transVert, x + transHoriz];
                    }
                    else
                    {
                        neighborHex = null;
                    }
                    // Assigns the neighbor hex if there is one
                    board.hexDex[z, x].GetComponent<Hex>().neighbors[direction] = neighborHex;
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
            BoardPosition hexPos = hit.transform.gameObject.GetComponent<BoardPosition>();
            // Get hex hit
            GameObject hexHit = board.hexDex[hexPos.z, hexPos.x];
            // Cache color
            int color = hexHit.GetComponent<cakeslice.Outline>().color;

            // Movement icons
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

                    if (movementType == MovementType.Single)
                    {
                        // If there's no piece on moused over hex 
                        if (hexHit.GetComponent<Hex>().piece == null
                            // Or if the piece on the moused over hex is the opposite color and not stacked
                            || (hexHit.GetComponent<Hex>().piece.tag != selected[0].GetComponent<Hex>().piece.tag
                                && hexHit.GetComponent<Hex>().piece.transform.childCount <= 1)
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
                    else if (movementType == MovementType.Cannon || movementType == MovementType.V)
                    {
                        List<BoardPosition> steps = stepsTo[hexHit];
                        // Place arrows up to hit hex
                        for (int i = 0; i < steps.Count - 1; i++)
                        {
                            // Get hex
                            GameObject hex = board.hexDex[steps[i + 1].z, steps[i + 1].x];
                            // Cache hex component
                            Hex hexComponent = hex.GetComponent<Hex>();
                            // If there are pieces on this hex and it's not the selected hex
                            if (hexComponent.piece != null)
                            {
                                // Add attack icon
                                PlaceIcon(hex);
                                // If the pieces on this hex are stacked then don't place arrow
                                if (hexComponent.piece.transform.childCount > 1)
                                {
                                    break;
                                }
                            }
                            // Place arrow between current and next hex
                            PlaceArrow(board.hexDex[steps[i].z, steps[i].x], hex);
                        }
                    }
                    else if (movementType == MovementType.Wave)
                    {
                        
                    }
                    else if (movementType == MovementType.Contiguous)
                    {
                        // Get shortest list of directions
                        List<int> directionList = GetDirectionsTo(hexHit);
                        // Go through list of directions and place arrows
                        // Start with selected hex
                        GameObject hex = selected[0];
                        // Do not place arrows if hitHex has a stack on it
                        // Should only trigger for last hex in the list
                        if (!(hexHit.GetComponent<Hex>().piece != null
                            && hexHit.GetComponent<Hex>().piece.tag != selected[0].GetComponent<Hex>().piece.tag
                            && hexHit.GetComponent<Hex>().piece.transform.childCount > 1))
                        {
                            foreach (int direction in directionList)
                            {
                                // Place arrow
                                PlaceArrow(hex, hex.GetComponent<Hex>().neighbors[direction]);
                                // Update hex for next time through the loop
                                hex = hex.GetComponent<Hex>().neighbors[direction];
                            }
                        }
                        // Place attack icons
                        // Because of our highlighting algorithm, hexes with pieces of the same color should not be highlighted so we don't need to worry
                        if (hexHit.GetComponent<Hex>().piece != null)
                        {
                            PlaceIcon(hexHit);
                        }
                    }
                }
            }
            else if (movementIcons != null)
            {
                KillAllMovementIcons();
            }

            // Place hovering prism if a different hex was hit than last frame or there is no current one
            if (!hoveringPrism.activeSelf || hexHit != previousHexHit)
            {
                hoveringPrism.GetComponent<HoveringPrism>().SetPosition(hexPos);
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
                            if (movementType == MovementType.Single || movementType == MovementType.Cannon || movementType == MovementType.V) 
                            {
                                // Move piece
                                selected[0].GetComponent<Hex>().piece.GetComponent<Piece>().Move(stepsTo[hexHit], movementType);
                            }
                            else if (movementType == MovementType.Wave) 
                            {
                                // Future movement code
                            }
                            else if (movementType == MovementType.Contiguous) 
                            {
                                // Get smallest directions list
                                List<int> directionList = GetDirectionsTo(hexHit);
                                // Start with source hex
                                GameObject hex = selected[0];
                                // Initalize targets
                                List<BoardPosition> targets = new List<BoardPosition>();
                                foreach (int direction in directionList)
                                {
                                    hex = hex.GetComponent<Hex>().neighbors[direction];
                                    targets.Add(hex.GetComponent<BoardPosition>());
                                }
                                // Move piece
                                selected[0].GetComponent<Hex>().piece.GetComponent<Piece>().Move(targets, MovementType.Contiguous);
                                // Reset variables
                                contiguousHexes  = new Dictionary<GameObject, List<List<int>>>();
                                contiguousVisits = new List<GameObject>();
                                directionsList   = new List<int>();
                            }
                            EndSelection();
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
        // Get rid of icons if nothing is hit on this frame
        else
        {
            KillAllMovementIcons();
            hoveringPrism.SetActive(false);
        }

        // Deselect all hexes or movement option with right click
        if (Input.GetMouseButton(1))
        {
            EndSelection();
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
    /// <summary>Destroys all movement icon <c>GameObject</c>s currently in scene</summary>
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

    /// <summary>Deselects all hexes currently selected</summary>
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

    /// <summary>Place movement arrow from <paramref>hex1</paramref> to <paramref>hex2</paramref></summary>
    /// <param name = "hex1">the hex the arrow will point from</param>
    /// <param name = "hex2">the hex the arrow will point to</param>
    private void PlaceArrow(GameObject hex1, GameObject hex2)
    {
        // Get the position the arrow will be in
        // Average position of the two hexes plus the vertical offset
        Vector3 iconPosition = ((hex1.transform.position + hex2.transform.position) / 2) + movementIconVertical;
        // Calculate relative position
        Vector3 relativePosition = hex2.transform.position - hex1.transform.position;
        // Calculate relative angle
        float angle = - (float) (Math.Atan2(relativePosition.z, relativePosition.x) * (180/Math.PI));
        // Spawn movement arrow
        movementIcons["arrows"].Add(Instantiate(curledMovementArrow, iconPosition, Quaternion.Euler(-90f, 0f, angle)));
    }

    /// <summary>Places a movement icon above a hex</summary>
    /// <param name = "hex">the hex to place the movement icon above</param>
    /// <param name = "type">the type of movement icon to place; can be either <c>"attack"</c> (default value) for the attacking icon or 
    /// <c>"stack"</c> for the stacking icon</param>
    private void PlaceIcon(GameObject hex, string type = "attack")
    {
        GameObject icon = null;
        if (type == "attack")
        {
            icon = attackIcon;
        }
        else if (type == "stack")
        {
            icon = stackIcon;
        }
        movementIcons[type].Add(
            Instantiate(icon, movementIconVertical + hex.GetComponent<Hex>().piece.transform.position, Quaternion.identity)
        );
    }

    /// <summary>Starts a move of the specified type by setting buttons and variables</summary>
    private void StartSelection(MovementType movementT)
    {
        // Toggles moving and sets movement type
        selectedMoving = true;
        movementType = movementT;
        ChangeButtons(false, movementT);
        // Resets list of damageable from previous move
        board.damageable = new List<GameObject>();
    }

    /// <summary>Ends a move and resets selections, highlights, and variables.</summary>
    private void EndSelection()
    {
        // Turns off moving
        selectedMoving = false;
        // Copies highlighted list to damageable list
        board.damageable = board.highlighted;
        // Unoutlines and deselects all hexes
        board.DehighlightAllHexes();
        DeselectAllHexes();
        stepsTo = new Dictionary<GameObject, List<BoardPosition>>();
        // Resets all buttons to interactable
        ChangeButtons(true);
    }
    #endregion

    #region Functions for moving
    /// <summary>Displays text when a movement option is invalid</summary>
    /// <param name = "error">the text (which should be a description of the error) to display; default value is <c>"Invalid Movement Option"</c></param>
    private void InvalidMovementOptionDisplay(string error = "Invalid Movement Option")
    {
        invalidMovementOptionText.SetText(error);
        invalidMovementOptionText.enabled = true;
        textRescindCountdown = textRescindTime;
    }

    /// <summary>Changes clickable status of every movement button except for the one specified</summary>
    /// <param name = "button">movement button to not change the status of</param>
    /// <param name = "on">whether to turn the buttons on or off; <c>true</c> corresponds to on and <c>false</c> corresponds to off</param>
    private void ChangeButtons(bool on, MovementType button = (MovementType)(-1))
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i != (int)button)
            {
                buttons[i].GetComponent<Button>().interactable = on;
            }
        }
    }

    #region Functions to check at the beginning of movement buttons
    #region Functions to check number of pieces selected and display errors
    /// <summary>Checks if no pieces are selected and displays <c>"No pieces selected"</c> if true</summary>
    /// <returns><c>true</c> if no pieces are selected, <c>false</c> if any pieces are selected</returns>
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

    /// <summary>Checks if only one piece is selected and displays <c>"Select only one piece"</c> if false</summary>
    /// <returns><c>true</c> if only one piece is selected, <c>false</c> if any other number is</returns>
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

    /// <summary>Determines if a move is being selected and stops the piece being selected if true; 
    /// should be used at the beginning of a movement button method to stop movement selection if button is pressed again</summary>
    /// <returns><c>true</c> if a move is being selected, <c>false</c> if not</returns>
    private bool NotMoving()
    {
        if (selectedMoving)
        {
            EndSelection();
            return false;
        }
        else
        {
            return true;
        }
    }
    #endregion

    public void SingleMovement()
    {
        // Makes sure only one piece is selected and we aren't already trying to move
        if (!NothingSelected() && OnlyOneSelected() && NotMoving())
        {
            StartSelection(MovementType.Single);
            // Loops through all neighbors and outlines them as valid moves
            foreach (GameObject hex in selected[0].GetComponent<Hex>().neighbors)
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
                    board.OutlineHex(hex, color);
                    stepsTo[hex] = new List<BoardPosition> {hex.GetComponent<BoardPosition>()};
                }
            }
        }
    }

    public void WaveMovement()
    {
        // Makes sure pieces are selected and we aren't already trying to move
        if (!NothingSelected() && NotMoving())
        {
            if (selected.Count >= 3)
            {
                // List of hexes that we have seen and validated so far
                // Will be compared to the list of selected to see if they match
                // If the lists match, then all pieces were seen and validated and we can move on
                List<GameObject> wave = new List<GameObject>();

                // Find piece at the end of the wave
                GameObject end = null;
                // The direction that the wave starts in from the end
                int direction = -1;
                // The piece at the end fo the wave should only have 1 selected neighbor while all others should have 2
                // Loop through each selected hex
                foreach (GameObject hex in selected)
                {
                    // Start count
                    int count = 0;
                    // Cache neighbors
                    GameObject[] neighbors = hex.GetComponent<Hex>().neighbors;
                    // Count how many neighbors are selected
                    for (int i = 0; i < 6; i++)
                    {
                        // If hex exists and is selected
                        if (neighbors[i] != null && selected.Contains(neighbors[i]))
                        {
                            // Increment count
                            count++;
                            // Set direction to current direction (if this is the hex we're looking for, then this if statement should only trigger once
                            // since there's only one neighbor that is selected, so the direction should get saved and not overwritten)
                            direction = i;
                        }
                    }
                    // If count is 1, then it is on the end
                    if (count == 1)
                    {
                        // Set this hex as end
                        end = hex;
                        // Say that we've seen this hex
                        wave.Add(end);
                        break;
                    }
                }

                // If an end hex was found
                if (end != null && direction != -1)
                {
                    // Find direction(s) to find the wave in
                    // Cache end hex component
                    Hex endHexComponent = end.GetComponent<Hex>();
                    // Set neighbors to be equal to the neighbors of the hex adjacent to the end hex in the found direction
                    GameObject[] neighbors = endHexComponent.neighbors[direction].GetComponent<Hex>().neighbors;
                    // Get directions cycled clockwise and counterclockwise
                    int directionClockwise = board.CycleDirection(direction, 1);
                    int directionCounterclockwise = board.CycleDirection(direction, -1);
                    // Offset to start cycling in
                    int cycle = 0;
                    // If the hex in the cycled direction exists and is selected
                    if (neighbors[directionClockwise] != null && selected.Contains(neighbors[directionClockwise]))
                    {
                        cycle = 1;
                    }
                    else if (neighbors[directionCounterclockwise] != null && selected.Contains(neighbors[directionCounterclockwise]))
                    {
                        cycle = -1;
                    }
                    else
                    {
                        InvalidMovementOptionDisplay("Select a wave");
                        return;
                    }

                    // Store directions perpendicular to wave
                    int otherDirection = board.CycleDirection(direction, cycle);
                    int[] perpendicularDirections = {
                        board.CycleDirection(direction, -cycle),
                        board.CycleDirection(otherDirection, cycle)
                    };

                    // Set initial hex to hex one away from the end of the wave
                    GameObject hex = endHexComponent.neighbors[direction];
                    // Go up the wave, starting in direction and cycling by cycle and -cycle each time
                    // Neighbors was already set as the neighbors of the second hex in the wave so we can just continue from there
                    while (selected.Contains(hex))
                    {
                        // Say that we have seen this hex
                        wave.Add(hex);

                        // Point to next hex
                        // Cache hex component
                        Hex hexComponent = hex.GetComponent<Hex>();
                        // Get next direction
                        int cycledDirection = board.CycleDirection(direction, cycle);
                        // Make sure hex exists
                        if (hexComponent.neighbors[cycledDirection] != null)
                        {
                            hex = hexComponent.neighbors[cycledDirection];
                        }
                        else
                        {
                            break;
                        }
                        // Invert cycle for next time through the loop
                        cycle = -cycle;
                        direction = cycledDirection;
                    }

                    #region Make sure all selected hexes are part of the wave
                    // Dictionary of all hexes that are selected and whether or not they are in the wave
                    bool[] selectedInWave = new bool[selected.Count];
                    for (int i = 0; i < selectedInWave.Length; i++)
                    {
                        if (wave.Contains(selected[i]))
                        {
                            selectedInWave[i] = true;
                        }
                    }
                    // Whether or not every hex in the selected list is in the wave
                    bool allSelectedInWave = true;
                    foreach (bool inWave in selectedInWave)
                    {
                        if (!inWave)
                        {
                            allSelectedInWave = false;
                        }
                    }
                    #endregion

                    // Check if every selected hex was seen and validated
                    if (allSelectedInWave)
                    {
                        StartSelection(MovementType.Wave);

                        // Loop through both perpendicular direction
                        foreach (int perpendicularDirection in perpendicularDirections)
                        {
                            // The worst status (not being able to move is worse than being able to bounce off is worse than being able to move)
                            //  that all pieces in this direction will obey
                            int worstStatus = 2;
                            // Find worst status
                            foreach (GameObject waveHex in wave)
                            {
                                // Get adjacent hex
                                GameObject nextHex = waveHex.GetComponent<Hex>().neighbors[perpendicularDirection];
                                if (nextHex != null)
                                {
                                    int positionStatus = board.PositionStatus(nextHex.GetComponent<BoardPosition>(), waveHex.GetComponent<Hex>().piece);
                                    if (positionStatus < worstStatus)
                                    {
                                        worstStatus = positionStatus;
                                    }
                                }
                            }

                            if (worstStatus != 0)
                            {
                                // Loop through each hex in the wave
                                foreach (GameObject waveHex in wave)
                                {
                                    // Get hex adjacent to waveHex perpedicular to the wave
                                    GameObject perpendicularHex = waveHex.GetComponent<Hex>().neighbors[perpendicularDirection];
                                    if (perpendicularHex != null)
                                    {
                                        // If there is a piece on the hex
                                        if (perpendicularHex.GetComponent<Hex>().piece != null)
                                        {
                                            board.OutlineHex(perpendicularHex, 2);
                                        }
                                        else if (worstStatus != 1)
                                        {
                                            board.OutlineHex(perpendicularHex, 1);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        InvalidMovementOptionDisplay("Select only a wave");
                    }
                }
                else
                {
                    InvalidMovementOptionDisplay("Select a wave");
                }
            }
            else
            {
                InvalidMovementOptionDisplay("Select at least three pieces");
            }
        }
    }

    public void CannonMovement()
    {
        // Make sure that only one hex is selected and we aren't already trying to move
        if (!NothingSelected() && OnlyOneSelected() && NotMoving())
        {
            #region Initialize/Cache variables
            // Hex to perform operations on
            GameObject hex = selected[0];
            // Gets lines from selected
            List<GameObject>[] lines = board.FindLines(hex.GetComponent<BoardPosition>());
            // Get directions line go in
            List<int> directions = board.GetLineDirections(lines);
            // Cache neighbors
            GameObject[] neighbors = hex.GetComponent<Hex>().neighbors;
            #endregion

            // Make sure that the piece has at least one line and is not only in the middle of line(s)
            if (directions.Count != 0)
            {
                StartSelection(MovementType.Cannon);
                // Loop through all directions piece can move
                // This if for if piece is the end of multiple lines
                foreach (int direction in directions)
                {
                    // Set source hex as first hex
                    hex = selected[0];
                    // Initialize step list for this direction
                    List<BoardPosition> steps = new List<BoardPosition>();
                    // Set source hex as first step
                    steps.Add(hex.GetComponent<BoardPosition>());

                    // Highlight possible moves
                    for (int i = 0; i < lines[board.GetOppositeDirection(direction)].Count; i++)
                    {
                        // Makes sure the hexes down the board exist
                        try
                        {
                            // Cache hex component
                            Hex hexComponent = hex.GetComponent<Hex>();
                            // Checks and highlights valid moves
                            // Make sure the hex down the board exists
                            if (hexComponent.neighbors[direction] != null)
                            {
                                int positionStatus = board.PositionStatus(
                                    hexComponent.neighbors[direction].GetComponent<BoardPosition>(),
                                    selected[0].GetComponent<Hex>().piece
                                );
                                // Checks if the hex can move through the position
                                if (positionStatus != 0)
                                {
                                    // Sets current hex to hex to the direction of the past hex
                                    hex = hexComponent.neighbors[direction];
                                    hexComponent = hex.GetComponent<Hex>();
                                    // Adds current hex to steps
                                    steps.Add(hex.GetComponent<BoardPosition>());
                                    // Set steps to current hex
                                    stepsTo[hex] = new List<BoardPosition>(steps);
                                    // Outline hex
                                    board.OutlineHex(hex, 1);
                                    // If there's a piece on the current hex and it is stacked
                                    if (positionStatus == 1)
                                    {
                                        hex.GetComponent<cakeslice.Outline>().color = 2;
                                        break;
                                    }
                                }
                                else
                                {
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
        if (!NothingSelected() && OnlyOneSelected() && NotMoving())
        {
            // Lines and directions
            List<GameObject>[] lines = board.FindLines(selected[0].GetComponent<BoardPosition>());
            // Find all directions where there is a hex/line
            List<int> directions = new List<int>();
            foreach (int direction in Enum.GetValues(typeof(Direction)))
            {
                if (lines[direction].Count > 1)
                {
                    directions.Add(direction);
                }
            }

            if (directions.Count >= 2)
            {
                // Find valid directions
                // List of pairs of valid directions
                List<int[]> Vs = new List<int[]>();
                // Loop through all directions of lines found
                foreach (int direction in directions)
                {
                    // The two directions off the selected hex that the V goes in
                    int[] VDirections = new int[2];
                    // Next consecutive direction
                    int direction2 = board.CycleDirection(direction, 1);
                    // If direction and the next direction clockwise have lines going in those directions, add the V going in those two directions to the list of Vs
                    if (directions.Contains(direction2))
                    {
                        VDirections[0] = direction;
                        VDirections[1] = direction2;
                        Vs.Add(VDirections);
                    }
                }
                // If any valid Vs are found
                if (Vs.Count != 0)
                {
                    StartSelection(MovementType.V);
                    // Loops through each V and reverses its direction (since Vs point away from direction they should be firing)
                    for (int i = 0; i < Vs.Count; i++)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            Vs[i][j] = board.GetOppositeDirection(Vs[i][j]);
                        }
                    }
                    
                    // Loops through each V
                    foreach (int[] V in Vs)
                    {
                        // Get starting BoardPosition
                        BoardPosition position = selected[0].GetComponent<BoardPosition>();
                        int z = position.z;
                        int x = position.x;

                        // List of steps taken
                        List<BoardPosition> steps = new List<BoardPosition>();
                        // Set source hex as first step
                        steps.Add(position);

                        // Loops as many times as the smallest amount of pieces in either part of the V
                        // Since we revered the direction earlier we need to re-reverse it
                        for (int i = 0; i < Math.Min(lines[board.GetOppositeDirection(V[0])].Count, lines[board.GetOppositeDirection(V[1])].Count); i++)
                        {
                            // If V is pointing straight up
                            if (board.DirectionIsTop(V[0]) && board.DirectionIsTop(V[1]))
                            {
                                z += 2;
                            }
                            // If V is pointing straight down
                            else if (board.DirectionIsBottom(V[0]) && board.DirectionIsBottom(V[1]))
                            {
                                z -= 2;
                            }
                            else
                            {
                                // If either direction of the V is left, then it will be pointing left
                                if (board.DirectionIsLeft(V[0]))
                                {
                                    x -= 2 - z % 2;
                                }
                                else
                                {
                                    x += 1 + z % 2;
                                }
                                // If either direction of the V is up, then it will be pointing up
                                if (board.DirectionIsTop(V[0]) || board.DirectionIsTop(V[1]))
                                {
                                    z++;
                                }
                                else
                                {
                                    z--;
                                }
                            }
                            // Make sure we don't go over the edge of the board
                            try
                            {
                                // Cache hex component
                                Hex hex = board.hexDex[z, x].GetComponent<Hex>();
                                // Find position status
                                int positionStatus = board.PositionStatus(z, x, selected[0].GetComponent<Hex>().piece);
                                // If piece can move through position
                                if (positionStatus != 0)
                                {
                                    // Set steps and outline hex
                                    steps.Add(board.hexDex[z, x].GetComponent<BoardPosition>());
                                    stepsTo[board.hexDex[z, x]] = new List<BoardPosition>(steps);
                                    board.OutlineHex(board.hexDex[z, x], 1);
                                    // If there's a piece on the current hex and it is stacked
                                    if (positionStatus == 1)
                                    {
                                        hex.GetComponent<cakeslice.Outline>().color = 2;
                                        break;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                            catch (IndexOutOfRangeException)
                            {
                                break;
                            }
                        }
                    }
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
        if (!NothingSelected() && OnlyOneSelected() && NotMoving())
        {
            // Reset variables
            contiguousHexes  = new Dictionary<GameObject, List<List<int>>>();
            contiguousVisits = new List<GameObject>();
            directionsList   = new List<int>();
            // Finds contiguous pieces
            FindContiguous(selected[0]);
            // Makes sure there are contiguous pieces
            if (contiguousHexes.Count != 0)
            {
                StartSelection(MovementType.Contiguous);
                // Go through every found piece
                foreach (GameObject hex in contiguousHexes.Keys)
                {
                    // Choose color
                    int color = 0;
                    // If there's no piece, green
                    if (hex.GetComponent<Hex>().piece == null)
                    {
                        color = 1;
                    }
                    // If there is a piece of opposite color, hit color
                    else
                    {
                        color = 2;
                    }
                    board.OutlineHex(hex, color);
                }
            }
            else
            {
                InvalidMovementOptionDisplay("Piece has no contiguous pieces");
            }
        }
    }

    #region Contiguous movement utility functions 
    /// <summary>Finds shortest path from the selected hex to the given hex</summary>
    /// <returns>smallest list of directions to get from the selected hex to the given hex</returns>
    private List<int> GetDirectionsTo(GameObject hex)
    {
        // Initialize list to store shortest list of directions
        List<int> directionList = null;
        // Go through each directions list and find the shortest one
        foreach (List<int> directions in contiguousHexes[hex])
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
        return directionList;
    }

    /// <summary>Find hexes contiguous to the given hex and creates a list of directions it took to get there from the given hex</summary>
    /// <param name = "sourceHex">the hex to find hexes contiguous to</param>
    private void FindContiguous(GameObject sourceHex)
    {
        // Get pieces adjacent to current hex
        List<GameObject> adjacent = board.GetAdjacentPieces(sourceHex);
        // Loops through all found adjacent hexes with pieces
        foreach (GameObject hex in adjacent)
        {
            // If hex is not the source hex or has been visited in this string of visits
            if (hex != selected[0] && !contiguousVisits.Contains(hex))
            {
                // Add hex to list of visits
                contiguousVisits.Add(hex);
                // Add direction to directions list
                directionsList.Add(board.GetDirection(sourceHex, hex));
                
                // Go through every neighbor of each found contiguous piece
                foreach (GameObject hexNeighbor in hex.GetComponent<Hex>().neighbors)
                {
                    // Don't deal with it if it has a piece of the same color on it
                    if (hexNeighbor != null
                        && !(hexNeighbor.GetComponent<Hex>().piece != null && hexNeighbor.GetComponent<Hex>().piece.tag == selected[0].GetComponent<Hex>().piece.tag))
                    {
                        // Add direction to directions list
                        directionsList.Add(board.GetDirection(hex, hexNeighbor));
                        // Initialize the list of lists at the key of the hex if not already done
                        if (!contiguousHexes.ContainsKey(hexNeighbor))
                        {
                            contiguousHexes[hexNeighbor] = new List<List<int>>();
                        }
                        // Add list of directions to big dictionary of directions for each hex
                        contiguousHexes[hexNeighbor].Add(new List<int>(directionsList));

                        // Clear this direction from the end of the list
                        if (directionsList.Count != 0)
                        {
                            directionsList.RemoveAt(directionsList.Count - 1);
                        }
                    }
                }
                // Find contiguous from current hex
                FindContiguous(hex);
                // We're done with this hex, so remove it from the string of visits
                contiguousVisits.RemoveAt(contiguousVisits.Count - 1);
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
