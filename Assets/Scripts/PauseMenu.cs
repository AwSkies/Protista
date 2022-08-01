using Newtonsoft.Json;
using TMPro;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private Board board;
    [SerializeField]
    private GameManager gameManager;

    [SerializeField]
    private TMP_InputField titleInput;
    [SerializeField]
    private TMP_InputField authorInput;
    [SerializeField]
    private TMP_InputField descriptionInput;

    [SerializeField]
    private GameObject menu;

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
        // Get title, author, and description from input fields
        string title = titleInput.text;
        string author = authorInput.text;
        string description = descriptionInput.text;
        if (title == "" || author == "" || description == "")
        {
            gameManager.DisplayMessage("Complete all fields");
        }
        else
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
            // Clear piece and objective hex numbers since the numbers cannot be found reliably
            board.layout.PieceNum = null;
            board.layout.ObjectiveHexNum = null;

            // Set author and description
            board.layout.Author = author;
            board.layout.Description = description;

            // Serialize layout to json
            string json = JsonConvert.SerializeObject(board.layout, Formatting.Indented);
            // Write to file
            File.WriteAllText(Application.persistentDataPath + "/" + title + ".json", json);
            // Give feedback
            gameManager.DisplayMessage("Layout saved successfully");
        }
    }

    public void QuitToTitle()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
