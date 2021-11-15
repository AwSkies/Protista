using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>Class <c>Piece</c> contains information about pieces and methods for interacting with and modifying pieces' states</summary> 
public class Piece : MonoBehaviour
{
    #region Prefabs and scene references
    public GameManager gameManager;
    public Board board;
    #endregion

    // Variables for moving animation
    public float speed;
    // Whether the piece is moving
    public bool moving;
    // Whether the piece can damage other pieces
    private bool canHit;
    // The height of a piece, how high each piece should stack
    public Vector3 stackingHeight;
    // Whether the piece is going to update its stack count once it stops moving
    public bool goingToUpdateStack;
    // The last position of the piece
    public Vector3 lastPosition;
    // The position that the piece needs to move to
    private List<Vector3> targets = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        board = GameObject.FindObjectOfType<Board>();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            // Move our position a step closer to the target
            // calculate distance to move
            float step = speed * Time.deltaTime; 
            transform.position = Vector3.MoveTowards(transform.position, targets[0], step);
            
            // Check if the position is about where it should be
            if (Vector3.Distance(transform.position, targets[0]) < 0.001f)
            {
                targets.RemoveAt(0);
                if (targets.Count == 0)
                {
                    // Stop piece
                    moving = false;
                }
            }
        }
        
        if (goingToUpdateStack)
        {
            UpdateStackCount();
        }
    }

    /// <summary>Moves a piece to a new location</summary>
    /// <param name = "targets">list of hexes to travel by, final location should be last element in the list</param>
    /// <param name = "movementType">the type of movement</param>
    /// <param name = "canStack">whether or not the piece can stack during this movement</param>
    /// <param name = "movementDirection">the direction the piece is moving if the piece is moving multiple hexes at a time in a straight line</param>
    public void Move(
        // List of targets to move to
        List<BoardPos> targets,
        // Movement type
        MovementType movementType,
        // Whether or not the piece is allowed to stack during this movement
        bool canStack = false,
        // The direction that the multiple hex move (if there is one) is going in
        Direction movementDirection = 0,
        // Whether this piece is in a stack that is being added onto another piece or stack
        bool stackingAStack = false, 
        // Whether this piece is the only, or bottom piece in a stack
        bool bottomPiece = true, 
        // The original (before moving pieces on this stack into it) amount of pieces in the stack that this piece is moving onto
        int origStackCount = 0
    )
    {
        // The new position should be at the final target position
        BoardPos newPos = targets[targets.Count - 1];
        // Initialize stacking variable
        bool stacking = false;

        // Reassign the pieces on the hexes if the piece is not stacking
        // Stacking case
        if (canStack && board.hexDex[newPos.z, newPos.x].GetComponent<Hex>().piece != null && board.hexDex[newPos.z, newPos.x].GetComponent<Hex>().piece.tag == tag)
        {
            stacking = true;
            // Make old hex have no pieces
            board.hexDex[GetComponent<BoardPos>().z, GetComponent<BoardPos>().x].GetComponent<Hex>().piece = null;
        }
        // Normal movement or attacking a single piece (not stacking or attacking a stack) case
        else if (board.hexDex[newPos.z, newPos.x].GetComponent<Hex>().piece == null || board.hexDex[newPos.z, newPos.x].GetComponent<Hex>().piece.transform.childCount == 0)
        {
            stacking = false;
            // Assign this piece to new hex
            board.hexDex[newPos.z, newPos.x].GetComponent<Hex>().piece = gameObject;
            // Make old hex have no pieces
            board.hexDex[GetComponent<BoardPos>().z, GetComponent<BoardPos>().x].GetComponent<Hex>().piece = null;
        }
        // Attacking a stack and multiple hex moving
        else if (movementType == MovementType.Cannon || movementType == MovementType.V)
        {
            // Assign this piece to new hex
            board.hexDex[newPos.z, newPos.x].GetComponent<Hex>().neighbors[board.GetOppositeDirection((int) movementDirection)].GetComponent<Hex>().piece = gameObject;
            // Make old hex have no pieces
            board.hexDex[GetComponent<BoardPos>().z, GetComponent<BoardPos>().x].GetComponent<Hex>().piece = null;
            stacking = false;
        }

        // Reassign board position if this piece is not attacking a stack or doing a multiple hex movement and bouncing off
        if (board.hexDex[newPos.z, newPos.x].GetComponent<Hex>().piece.transform.childCount == 0 
            || stacking 
            || transform.IsChildOf(board.hexDex[newPos.z, newPos.x].GetComponent<Hex>().piece.transform)
            || board.hexDex[newPos.z, newPos.x].GetComponent<Hex>().piece == gameObject)
        {
            // Reassign the piece's x and z values
            GetComponent<BoardPos>().z = newPos.z;
            GetComponent<BoardPos>().x = newPos.x;
        }
        // If the piece is attacking a stack, is it doing a multiple hex move?
        else if (movementType == MovementType.Cannon || movementType == MovementType.V)
        {
            // Reassign position to one hex short of the target
            BoardPos bouncingOnto;
            bouncingOnto = board.hexDex[newPos.z, newPos.x].GetComponent<Hex>().neighbors[board.GetOppositeDirection((int) movementDirection)].GetComponent<BoardPos>();
            GetComponent<BoardPos>().x = bouncingOnto.x;
            GetComponent<BoardPos>().z = bouncingOnto.z;
        }

        foreach (BoardPos target in targets)
        {
            this.targets.Add(board.hexDex[target.z, target.x].transform.position + new Vector3(0f, transform.position.y, 0f));
        }

        // Set last position
        if (!(movementType == MovementType.Cannon || movementType == MovementType.V))
        {
            lastPosition = transform.position;
        }
        else
        {
            lastPosition = gameManager.board.hexDex[newPos.z, newPos.x].GetComponent<Hex>().neighbors[board.GetOppositeDirection((int) movementDirection)].transform.position + new Vector3(0f, transform.position.y, 0f);
        }

        // Whether this is the bottom piece of a stack that is being moved onto another piece
        bool startingStackingAStack;
        // How high offset the bottom stacking piece needs to be in the piece its moving onto is stacked
        int stackCount;
        // The piece this piece is stacking onto (if applicable)
        GameObject stackingOnto = stacking ? board.hexDex[newPos.z, newPos.x].GetComponent<Hex>().piece : null;

        // Stacking 
        if (stacking)
        {
            // Checks if the piece this piece is stacking onto has pieces stacked onto it
            if (stackingOnto.transform.childCount != 0)
            {
                // Sets the original stack count if this piece is in a stack or the first piece in a stack
                if (stackingAStack)
                {
                    stackCount = origStackCount;
                }
                else
                {
                    stackCount = stackingOnto.transform.childCount;
                }
            }
            else
            {
                stackCount = 0;
            }

            // Make stack offset for stacking on a stack
            Vector3 stackOffset = stackingHeight * stackCount;
            for (int i = 0; i < this.targets.Count; i++)
            {
                this.targets[i] += stackOffset;
            }

            // Checks if this piece has stacked pieces and is not a piece in the middle of a stack
            // Begins a stack if so, doesn't if not
            if (transform.childCount != 0 && !stackingAStack)
            {
                startingStackingAStack = true;
            }
            else
            {
                startingStackingAStack = false;
            }

            // Parent piece to the piece it's being stacked on
            transform.SetParent(stackingOnto, true);

            // Assign target position based on the height of the stack the piece is moving onto
            // Since this method is recursive, every time it goes through each stacked piece, target gets added to each time 
            // It gets offset by stackingHeight each time
            for (int i = 0; i < this.targets.Count; i++)
            {
                this.targets[i] += stackingHeight;
            }
        }
        else
        {
            // Make sure startingStackingAStack doesn't go unassigned
            startingStackingAStack = false;
            stackCount = 0;
        }

        // Piece is now moving to its next position
        moving = true;
        // Piece can damage other pieces
        canHit = true;

        if (bottomPiece && stacking)
        {
            stackingOnto.GetComponent<Piece>().goingToUpdateStack = true;
        }
    }

    void OnCollisionEnter(Collision otherObj)
    {
        Piece otherPiece = otherObj.gameObject.GetComponent<Piece>();
        if (
            // If a piece collides with another piece of the opposite color 
            (otherObj.gameObject.tag == "black" || otherObj.gameObject.tag == "white") && otherObj.gameObject.tag != tag 
            // and that piece is not moving (to prevent both pieces calling this function at the same and destroying each other at the same time)
            && !otherPiece.moving
            // and this piece the bottom of a stack or has no pieces on top of it
            && (transform.childCount != 0 || transform.position.y == gameManager.pieceVertical.y)
            // and this is the final position the piece is going to go in
            && targets.Count == 1
            // and the piece can damage other pieces
            && canHit
        )
        {
            GameObject pieceToDestroy;
            // If attacking a stack
            if (otherPiece.transform.childCount != 0)
            {
                // Set pieceToDestroy
                pieceToDestroy = otherPiece.transform.GetChild(otherPiece.transform.childCount - 1);
                // Updates stack count for one less piece
                otherPiece.UpdateStackCount();
                // Update target to last position
                targets[0] = lastPosition; 
                // Piece cannot damage other pieces while moving back to last position
                canHit = false;
                targets[0] = lastPosition;
            }
            // If attacking a single piece
            else
            {
                // Set pieceToDestroy
                pieceToDestroy = otherObj.gameObject;
            }
            // Destroy piece
            Destroy(pieceToDestroy);
        }
    }

    /// <summary>Updates the stack number (stack count) displayed above a piece</summary>
    public void UpdateStackCount()
    {
        List<bool> stackMoving = new List<bool>();
        foreach (Tranform piece in transform)
        {
            bool otherPieceMoving = piece.GetComponent<Piece>().moving;
            if (otherPieceMoving)
            {
                stackMoving.Add(otherPieceMoving);
            }
        }
        if (!stackMoving.Contains(true))
        {
            // Get canvas
            GameObject canvas;
            if (transform.childCount != 0) 
            {
                canvas = transform.GetChild(transform.childCount - 1).transform.GetChild(0).gameObject;
            }
            else
            {
                canvas = transform.GetChild(0).gameObject;
            }

            // Get text
            TextMeshProUGUI text = canvas.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();

            // Hide all the number for pieces that are stacked 
            foreach (Tranform piece in transform)
            {
                // Hide canvas
                piece.transform.GetChild(0).gameObject.SetActive(false);
            }

            if (transform.childCount != 0)
            {
                canvas.SetActive(true);
                text.text = (transform.childCount + 1).ToString();
            }
            else
            {
                canvas.SetActive(false);
            }

            goingToUpdateStack = false;
        }
    }
}
