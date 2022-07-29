using Newtonsoft.Json;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject menu;
    [SerializeField]
    private Board board;

    // Update is called once per frame
    public void Update()
    {
        // Toggle menu on escape
        if (Input.GetKeyDown("escape"))
        {
            menu.SetActive(!menu.activeSelf);
        }
    }

    public void SaveLayout()
    {
        // Reset list of pieces
        board.layout.Pieces = new List<PieceInfo>();
        // Loop through both dimensions of board
        for (int z = 0; z < board.layout.Rows; z++)
        {
            for (int x = 0; x < board.layout.Columns; x++)
            {
                // If the current hex has a piece on it
                if (board.hexDex[z, x].GetComponent<Hex>().piece != null)
                {
                    // Cache piece
                    GameObject piece = board.hexDex[z, x].GetComponent<Hex>().piece;
                    // Add piece coordinates and data to list
                    board.layout.Pieces.Add(new PieceInfo {
                        Position = new int[] {z, x},
                        Stacked = piece.transform.childCount - 1,
                        White = piece.tag == "white"
                    });
                }
            }
        }
        board.layout.PieceNum = board.layout.Pieces.Count;
        // Serialize layout to json
        string json = JsonConvert.SerializeObject(board.layout, Formatting.Indented);
        // Write to file
        File.WriteAllText(Application.persistentDataPath + "/test.json", json);
    }

    public void QuitToTitle()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
