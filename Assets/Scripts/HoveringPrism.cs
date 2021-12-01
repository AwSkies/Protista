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
    
    private Vector3 target;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetPosition(BoardPosition boardPosition)
    {
        // Determine number of pieces stacked on hex
        int pieceNum = 0;
        if (board.hexDex[boardPosition.z, boardPosition.x].GetComponent<Hex>().piece != null)
        {
            pieceNum = board.hexDex[boardPosition.z, boardPosition.x].GetComponent<Hex>().piece.transform.childCount;
        }
        // Set position
        transform.position = board.hexDex[boardPosition.z, boardPosition.x].transform.position + verticalOffset + canvasOffset + (pieceOffset * pieceNum);
        // Set visible
        gameObject.SetActive(true);
    }
}
