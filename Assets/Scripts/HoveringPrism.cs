using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoveringPrism : MonoBehaviour
{
    public Board board;

    #region Rules and how it behaves
    // How far above zero the hovering icon goes
    public Vector3 verticalOffset;
    // How much higher the hover icon goes for each piece on the hex
    public Vector3 pieceOffset;
    // The size of the number canvas
    public Vector3 canvasOffset;
    #endregion
    
    public GameObject hoveringOver = null;
    private Vector3 target;

    public void HoverOver(BoardPosition boardPosition)
    {
        GameObject hex = board.hexDex[boardPosition.z, boardPosition.x];
        // Determine number of pieces stacked on hex
        int pieceNum = 0;
        Vector3 canvas = new Vector3(0, 0, 0);
        if (hex.GetComponent<Hex>().piece != null)
        {
            pieceNum = hex.GetComponent<Hex>().piece.transform.childCount;
        }
        if (pieceNum > 1)
        {
            canvas = canvasOffset;
        }
        // Set position
        transform.position = hex.transform.position + verticalOffset + canvas + (pieceOffset * pieceNum);
        hoveringOver = hex;
        // Set visible
        gameObject.SetActive(true);
    }
}
