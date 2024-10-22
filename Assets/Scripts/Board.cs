﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>Class <c>Board</c> contains information about the current state of the board and utility methods for interacting with the board</summary>
public class Board : MonoBehaviour
{
    // The layout of the board
    public Layout layout;
    // Index of hexes ordered by z, x position
    public GameObject[,] hexDex;
    // List of highlighted hexes
    public List<GameObject> highlighted = new List<GameObject>();
    // List of damageable hexes when moving
    public List<GameObject> damageable = new List<GameObject>();

    #region Utility functions for directions and direction manipulation
    /// <summary>Returns direction that is offset from the current direction. Giving a positive offset goes clockwise and a negative offset goes counterclockwise.</summary>
    public static int CycleDirection(int direction, int offset)
    {
        // Get new direction from going offset steps in one direction       
        int newDirection = direction + offset;
        // If direction has overflowed over 6 or past 0, bring it back
        if (newDirection >= 6) { newDirection -= 6; }
        else if (newDirection < 0) {newDirection += 6; }
        // Return the new cycled direction
        return newDirection;
    }

    /// <summary>Returns direction that is offset from the current direction. Giving a positive offset goes clockwise and a negative offset goes counterclockwise.</summary>
    public static Direction CycleDirection(Direction direction, int offset)
    {
        return (Direction)CycleDirection((int)direction, offset);
    }
    
    /// <summary>Returns the opposite direction</summary>
    public static int GetOppositeDirection(int direction) 
    {
        return CycleDirection(direction, 3);
    }

    /// <summary>Returns the opposite direction</summary>
    public static Direction GetOppositeDirection(Direction direction) 
    {
        return CycleDirection(direction, 3);
    }

    /// <summary>Gets direction target hex is in</summary>
    /// <param name = "source">source hex for determining direction</param>
    /// <param name = "target">target hex for determining direction</param>
    /// <param name = "possibleDirections">possible directions to look in, defaults to all possible directions</param>
    /// <returns>Direction target hex is in, in relation to the source hex</returns>
    public int GetDirection(GameObject source, GameObject target, int[] possibleDirections = null)
    {
        // Set directions to numbers 0-5 if no other list of possible directions is given
        int[] directions = possibleDirections == null ? Enumerable.Range(0, 6).ToArray<int>() : possibleDirections;
        // Initialize movement direction
        int movementDirection = 0;
        // Loop out in the possible directions from the selected hex until we hit the hex we want to move to or run out
        // Then move on or choose that direction
        foreach (int direction in directions)
        {
            // Choose selected hex to start from
            GameObject hex = source;
            // Check if current hex is hex we want to move to
            while (hex.GetComponent<Hex>().neighbors[direction] != null && hex.GetComponent<Hex>().neighbors[direction] != target)
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
        return movementDirection;
    }

    #region Methods about determining which ways directions go
    #region Left or right semicircle
    /// <summary>Determines if a direction is in the left semicircle</summary>
    public static bool DirectionIsLeft(int direction)
    {
        return direction >= 2 && direction <= 4;
    }

    /// <summary>Determines if a direction is in the left semicircle</summary>
    public static bool DirectionIsLeft(Direction direction)
    {
        return DirectionIsLeft((int)direction);
    }

    /// <summary>Determines if a direction is in the right semicircle</summary>
    public static bool DirectionIsRight(int direction)
    {
        return !DirectionIsLeft(direction);
    }

    public static bool DirectionIsRight(Direction direction)
    {
        return !DirectionIsLeft(direction);
    }
    #endregion
    #region Top, bottom, or middle
    /// <summary>Determines if a direction is at the top</summary>
    public static bool DirectionIsTop(int direction)
    {
        return direction == 4 || direction == 5;
    }

    /// <summary>Determines if a direction is at the top</summary>
    public static bool DirectionIsTop(Direction direction)
    {
        return DirectionIsTop((int)direction);
    }

    /// <summary>Determines if a direction is in the middle</summary>
    public static bool DirectionIsMiddle(int direction)
    {
        return direction == 0 || direction == 3;
    }

    /// <summary>Determines if a direction is in the middle</summary>
    public static bool DirectionIsMiddle(Direction direction)
    {
        return DirectionIsMiddle((int)direction);
    }

    /// <summary>Determines if a direction is at the bottom</summary>
    public static bool DirectionIsBottom(int direction)
    {
        return direction == 1 || direction == 2;
    }
    
    /// <summary>Determines if a direction is at the bottom</summary>
    public static bool DirectionIsBottom(Direction direction)
    {
        return DirectionIsBottom((int)direction);
    }
    #endregion
    #endregion
    #endregion

    #region Utility functions for lines
    /// <summary>Finds lines of pieces of the same color</summary>
    /// <param name = "position">position to look for lines going through or starting/ending at</param>
    /// <returns>An array of lists of hexes, each index of the array corresponding to a certain direction 
    /// and each list containing the hexes that have pieces in an unbroken line in that direction (the first 
    /// index of each list in each direction is always the source gex)</returns>
    public List<GameObject>[] FindLines(BoardPosition position)
    {
        // Initialize variables
        // Lines to return
        // List of GameObjects is the list of hexes in the line
        List<GameObject>[] lines = new List<GameObject>[6];
        // Hex that is the source of the line
        Hex sourceHex = hexDex[position.z, position.x].GetComponent<Hex>();
        // Color of the piece which line we want to get
        string color = sourceHex.piece.tag;

        // Loop through each neighbor of the original hex
        for (int direction = 0; direction < sourceHex.neighbors.Length; direction++)
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

    /// <summary>Gets directions that lines are in</summary>
    public static List<int> GetLineDirections(List<GameObject>[] lines)
    {
        // List of directions to return
        List<int> directions = new List<int>();
        for (int direction = 0; direction < lines.Length; direction ++)
        {
            // Make sure not to add directions where a piece is in the middle of a line
            // If the line is just one (just the source hex) in one direction and the opposite direction it's more than one
            if (lines[direction].Count == 1 && lines[GetOppositeDirection(direction)] != null && lines[GetOppositeDirection(direction)].Count > 1)
            {
                directions.Add(direction);
            }
        }

        return directions;
    }
    #endregion
    
    /// <summary>Returns adjacent hexes with pieces on them</summary>
    public List<GameObject> GetAdjacentPieces(GameObject hex)
    {
        // Initialize list of hexes with pieces on them
        List<GameObject> adjacentPieces = new List<GameObject>();
        // Cache neighbors
        GameObject[] neighbors = hex.GetComponent<Hex>().neighbors;
        // Loop through directions
        for (int direction = 0; direction < neighbors.Length; direction++)
        {
            // If hex in that direction exists, has a piece on it, and that piece is the same color, add that hex to the list
            if (neighbors[direction] != null &&
                neighbors[direction].GetComponent<Hex>().piece != null &&
                neighbors[direction].GetComponent<Hex>().piece.tag == hex.GetComponent<Hex>().piece.tag)
            {
                adjacentPieces.Add(neighbors[direction]);
            }
        }
        return adjacentPieces;
    }

    /// <summary>Outline a hex</summary>
    public void OutlineHex(GameObject hex, int color)
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

    /// <summary>Dehighlights all hexes</summary>
    public void DehighlightAllHexes(List<GameObject> excluded = null)
    {
        // Iterate through each highlighted hex
        foreach (GameObject hex in highlighted)
        {
            // If no list of excluded hexes is provided or this hex is not within the list provided
            if (excluded == null || !excluded.Contains(hex))
            {
                // Turn off outline
                hex.GetComponent<cakeslice.Outline>().enabled = false;
            }
        }
        // Resets highlighted back to empty list
        highlighted = new List<GameObject>();
    }

    /// <summary>Checks if the given piece can move through a position.</summary>
    /// <param name = "tag">The tag of the piece in question.</param>
    /// <returns><c>0</c> if the piece cannot move through the hex, <c>1</c> if the piece would bounce off a stack, 
    /// and <c>2</c> if the piece can move through the hex.</returns>
    /// <exception cref = "System.IndexOutOfRangeException">Thrown when <c>z</c> or <c>x</c> 
    /// are outside of the board.</exception>
    public int PositionStatus(int z, int x, string tag)
    {
        // Cache hex component
        Hex hex = hexDex[z, x].GetComponent<Hex>();
        // If the position has a piece on it
        if (hex.piece != null)
        {
            // Cache piece component
            Piece piece = hex.piece.GetComponent<Piece>();
            // If the position has a piece of the same color on it
            if (hex.piece.tag == tag)
            {
                return 0;
            }
            // If the position has a piece of the opposite color and is stacked
            else if (piece.transform.childCount > 1)
            {
                return 1;
            }
            // If the position has a piece of the opposite color and isn't stacked
            else
            {
                return 2;
            }
        }
        else
        {
            return 2;
        }
    }

    /// <summary>Checks if this piece can move through a position.</summary>
    /// <param name = "tag">The tag of the piece in question.</param>
    /// <returns><c>0</c> if the piece cannot move through the hex, <c>1</c> if the piece would bounce off a stack, 
    /// and <c>2</c> if the piece can move through the hex.</returns>
    public int PositionStatus(BoardPosition position, string tag)
    {
        return PositionStatus(position.z, position.x, tag);
    }

    /// <summary>Checks if this piece can move through a position.</summary>
    /// <param name = "piece">The piece in question.</param>
    /// <returns><c>0</c> if the piece cannot move through the hex, <c>1</c> if the piece would bounce off a stack, 
    /// and <c>2</c> if the piece can move through the hex.</returns>
    public int PositionStatus(int z, int x, GameObject piece)
    {
        return PositionStatus(z, x, piece.tag);
    }

    /// <summary>Checks if this piece can move through a position.</summary>
    /// <param name = "piece">The piece in question.</param>
    /// <returns><c>0</c> if the piece cannot move through the hex, <c>1</c> if the piece would bounce off a stack, 
    /// and <c>2</c> if the piece can move through the hex.</returns>
    public int PositionStatus(BoardPosition position, GameObject piece)
    {
        return PositionStatus(position, piece.tag);
    }
}
