using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>Class <c>Piece</c> contains information about pieces and methods for interacting with and modifying pieces' states</summary> 
public class Piece : MonoBehaviour
{
    #region Prefabs and scene references
    private GameManager gameManager;
    private Board board;
    [SerializeField]
    private GameObject canvas;
    #endregion

    // Variables for moving animation
    public float speed;
    // Whether the piece is moving
    public bool moving;
    // Whether the piece can damage other pieces
    private bool canHit;
    // The height of a piece, how high each piece should stack
    public Vector3 stackingHeight;
    // The position that the piece needs to move to
    private List<Vector3> targets = new List<Vector3>();
    // The piece this piece is stacking onto (if applicable)
    private GameObject stackingOnto;

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

                    #region Stacking stuff
                    // The pieces currently stacked on this piece
                    List<Transform> stackedPieces = new List<Transform>();
                    // Get all pieces currently stacked
                    foreach (Transform piece in transform)
                    {
                        stackedPieces.Add(piece);
                    }
                    // Remove the first element of the list (should always be canvas)
                    stackedPieces.RemoveAt(0);

                    // Reassign board position for all stacked pieces
                    foreach (Transform piece in stackedPieces)
                    {
                        BoardPosition piecePos = piece.gameObject.GetComponent<BoardPosition>();
                        piecePos.x = GetComponent<BoardPosition>().x;
                        piecePos.z = GetComponent<BoardPosition>().z;
                    }

                    // Correctly parent and reassign BoardPos of stacked pieces to piece/coordinates newly stacked on (if applicable)
                    if (stackingOnto != null)
                    {   
                        // Parent piece to the piece it's being stacked on
                        ParentTo(stackingOnto.transform);
                        // Repeat for all pieces currently stacked on this piece
                        foreach (Transform piece in stackedPieces)
                        {
                            // Remove pieces from being parented to current hex and parent to new hex
                            piece.SetParent(null, true);
                            piece.gameObject.GetComponent<Piece>().ParentTo(stackingOnto.transform);
                        }
                        // Update the stack count for the parent piece
                        transform.parent.gameObject.GetComponent<Piece>().UpdateStackCount();
                        // Nullify stackingOnto
                        stackingOnto = null;
                    }
                    #endregion
                }
            }
        }
    }

    /// <summary>Moves a piece to a new location.</summary>
    /// <param name = "steps">List of hexes to travel by. The final location should be last element in the list, and if the piece bounces off of a stack, it will be 
    /// second to last.</param>
    /// <param name = "movementType">The type of movement.</param>
    public void Move(
        // List of targets to move to
        List<BoardPosition> steps,
        // Movement type
        MovementType movementType
    )
    {
        // The new position should be at the final target position
        BoardPosition newPos = steps[steps.Count - 1];
        // Cache current position
        BoardPosition currentPos = GetComponent<BoardPosition>();
        // Initialize stacking variable
        bool stacking = false;

        // Reassign the pieces on the hexes if the piece is not stacking
        // Stacking case
        if (movementType == MovementType.Single && board.hexDex[newPos.z, newPos.x].GetComponent<Hex>().piece != null && board.hexDex[newPos.z, newPos.x].GetComponent<Hex>().piece.tag == tag)
        {
            stacking = true;
            // Make old hex have no pieces
            board.hexDex[currentPos.z, currentPos.x].GetComponent<Hex>().piece = null;
        }
        // Normal movement or attacking a single piece (not stacking or attacking a stack) case
        // If there's no piece or the piece on the hex has no pieces stacked on it
        else if (board.hexDex[newPos.z, newPos.x].GetComponent<Hex>().piece == null || board.hexDex[newPos.z, newPos.x].GetComponent<Hex>().piece.transform.childCount <= 1)
        {
            // Assign this piece to new hex
            board.hexDex[newPos.z, newPos.x].GetComponent<Hex>().piece = gameObject;
            // Make old hex have no pieces
            board.hexDex[currentPos.z, currentPos.x].GetComponent<Hex>().piece = null;
        }
        // Attacking a stack and multiple hex moving
        else if (movementType == MovementType.Cannon || movementType == MovementType.V)
        {
            // If this piece is attacking a stacked piece more than one step away
            if (steps.Count > 1) 
            {
                // newPos should be the second to last position since it is bouncing off
                newPos = steps[steps.Count - 2];
                // Assign this piece to new hex
                board.hexDex[newPos.z, newPos.x].GetComponent<Hex>().piece = gameObject;
                // Make old hex have no pieces
                board.hexDex[currentPos.z, currentPos.x].GetComponent<Hex>().piece = null;
            }
            else
            {
                // The piece is just bouncing back to where it already was
                newPos = currentPos;
            }
        }
        // Attacking a stack and moving normally
        else
        {
            newPos = currentPos;
        }

        // Reassign the piece's x and z values
        currentPos.z = newPos.z;
        currentPos.x = newPos.x;

        foreach (BoardPosition target in steps)
        {
            targets.Add(board.hexDex[target.z, target.x].transform.position + new Vector3(0f, transform.position.y, 0f));
        }

        // Stacking 
        if (stacking)
        {
            // The piece this piece is stacking onto
            stackingOnto = board.hexDex[newPos.z, newPos.x].GetComponent<Hex>().piece;
            // How high offset the bottom stacking piece needs to be in the piece its moving onto is stacked
            int stackCount = stackingOnto.transform.childCount;

            // Make stack offset for stacking on a stack
            Vector3 stackOffset = stackingHeight * stackCount;
            for (int i = 0; i < targets.Count; i++)
            {
                targets[i] += stackOffset;
            }
        }

        // Piece is now moving to its next position
        moving = true;
        // Piece can damage other pieces
        canHit = true;
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
            && (transform.childCount > 1 || transform.position.y == gameManager.pieceVertical.y)
            // and this is the final position the piece is going to go in
            && targets.Count == 1
            // and the piece can damage other pieces
            && canHit
            // and the piece hit can be damaged
            // && otherPiece.damageable
        )
        {
            GameObject pieceToDestroy;
            // If attacking a stack
            if (otherPiece.transform.childCount > 1)
            {
                // Set pieceToDestroy
                pieceToDestroy = otherPiece.transform.GetChild(otherPiece.transform.childCount - 1).gameObject;
                // Unparent piece from stack to prevent a deleted object from being referenced
                pieceToDestroy.transform.SetParent(null);
                // Update target to proper position
                BoardPosition properPos = GetComponent<BoardPosition>();
                targets[0] = board.hexDex[properPos.z, properPos.x].transform.position + new Vector3(0f, transform.position.y, 0f); 
                // Piece cannot damage other pieces while moving back to last position
                canHit = false;
            }
            // If attacking a single piece
            else
            {
                // Set pieceToDestroy
                pieceToDestroy = otherObj.gameObject;
            }
            // Destroy piece
            Destroy(pieceToDestroy);
            // Updates stack count for one less piece (only if the bottom piece still exists since it was a stack)
            if (otherPiece != null)
            {
                otherPiece.UpdateStackCount();
            }
        }
    }

    /// <summary>Updates the stack number (stack count) displayed above a piece</summary>
    public void UpdateStackCount()
    {
        // Declare canvas
        GameObject topCanvas;
        // If there are any stacked pieces
        if (transform.childCount > 1) 
        {
            // Get canvas from highest stacked piece
            topCanvas = transform.GetChild(transform.childCount - 1).GetComponent<Piece>().canvas;
        }
        else
        {
            // Get canvas from current piece
            topCanvas = canvas;
        }

        // Get text
        TextMeshProUGUI text = topCanvas.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();

        // Hide the numbers for all pieces that are stacked 
        foreach (Transform piece in transform)
        {
            // Hide canvas
            piece.transform.GetChild(0).gameObject.SetActive(false);
        }

        // If there are any stacked pieces on this piece
        if (transform.childCount > 1)
        {
            // Show counter on highest stacked piece
            topCanvas.SetActive(true);
            topCanvas.transform.GetChild(0).gameObject.SetActive(true);
            // Do not add one to child count because the first child of every piece should be their canvas
            text.text = transform.childCount.ToString();
        }
        else
        {
            // Hide canvas (there is only one piece so it doesn't need a stack counter)
            topCanvas.SetActive(false);
        }
    }

    /// <summary>Parents piece to another piece</summary>
    private void ParentTo(Transform parent)
    {
        // Parent piece to the piece it's being stacked on
        transform.SetParent(parent, true);
        // Set this piece's canvas to current piece (for some reason if you don't do this the canvas 
        // gets set as a child of the piece you're setting this piece to be a child of)
        canvas.transform.SetParent(transform, true);
    }
}
