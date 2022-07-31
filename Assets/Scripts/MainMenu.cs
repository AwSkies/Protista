using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>Stores information about the type and parameters of the game and contains button functions for configuring them.</summary>
public class MainMenu : MonoBehaviour
{
    #region Static data recorded for passing to GameBoard scene
    public static Layout layout;
    public static GameType gameType;
    #endregion

    #region Main menu scene objects
    #region Layout list
    [SerializeField]
    private GameObject noLayoutsText;
    [SerializeField]
    private GameObject openFolderButton;
    [SerializeField]
    private GameObject layoutButton;
    [SerializeField]
    private Transform contentField;
    #endregion

    #region Layout information window
    #region Basic information
    [SerializeField]
    private TMP_Text title;
    [SerializeField]
    private TMP_Text author;
    [SerializeField]
    private TMP_Text description;
    #endregion

    #region Size information
    [SerializeField]
    private GameObject sizeGroup;
    [SerializeField]
    private TMP_Text rowCount;
    [SerializeField]
    private TMP_Text columnCount;
    #endregion

    #region Piece information
    [SerializeField]
    private GameObject pieceGroup;
    [SerializeField]
    private GameObject pieceEachGroup;
    [SerializeField]
    private TMP_Text pieceEachNum;
    [SerializeField]
    private GameObject pieceWhiteGroup;
    [SerializeField]
    private TMP_Text pieceWhiteNum;
    [SerializeField]
    private GameObject pieceBlackGroup;
    [SerializeField]
    private TMP_Text pieceBlackNum;
    #endregion

    #region Objective hex information
    [SerializeField]
    private GameObject objectiveHexGroup;
    [SerializeField]
    private GameObject objectiveHexEachGroup;
    [SerializeField]
    private TMP_Text objectiveHexEachNum;
    [SerializeField]
    private GameObject objectiveHexWhiteGroup;
    [SerializeField]
    private TMP_Text objectiveHexWhiteNum;
    [SerializeField]
    private GameObject objectiveHexBlackGroup;
    [SerializeField]
    private TMP_Text objectiveHexBlackNum;
    #endregion

    [SerializeField]
    private GameObject playButton;
    #endregion
    #endregion

    private List<GameObject> layoutButtons = new List<GameObject>();

    /// <summary>Loads the GameBoard scene containing the actual game board.
    /// The static fields layout and gameType will be processed by the board GameManager script and be handled accordingly.</summary>
    public void LoadGame()
    {
        SceneManager.LoadScene("GameBoard");
    }

    /// <summary>Displays in the UI the json layouts appearing in the player's data directory. Creates a button for each one.</summary>
    public void DisplayJsonLayouts()
    {
        // Selects json files in the persistent data path
        var files = from file in Directory.GetFiles(Application.persistentDataPath) where file.EndsWith(".json") select file;
        // If there are any json files in the directory
        if (files.Count() > 0)
        {
            // Set the no layouts text and button inactive
            noLayoutsText.SetActive(false);
            openFolderButton.SetActive(false);
            
            // Create a button for each file
            foreach (string path in files)
            {
                // Make a button to click on for each of the names
                // Instantiates a button at the correct position with no rotation and parented to the content field
                GameObject button = Instantiate(layoutButton, contentField);
                button.SetActive(true);
                // Sets the text of the button to be the name of the layout
                button.transform.GetChild(0).GetComponent<TMP_Text>().SetText(Path.GetFileNameWithoutExtension(path));
                // Stores path in button
                button.GetComponent<LayoutPath>().path = path;
                // Add button to list
                layoutButtons.Add(button);
            }
        }
        else
        {
            // Make sure the no layouts text and open folder button are enabled
            noLayoutsText.SetActive(true);
            openFolderButton.SetActive(true);
        }
    }

    /// <summary>Deletes the buttons for the json layouts</summary>
    public void DeleteJsonLayouts()
    {
        // Destroy each layout in the list
        foreach (GameObject button in layoutButtons)
        {
            Destroy(button);
        }
        // Reset button list
        layoutButtons = new List<GameObject>();
    }

    /// <summary>Process the information from the selected layout and displays it in the window</summary>
    public void DisplayInformation(LayoutPath path)
    {
        // Read json from file
        string json = File.ReadAllText(path.path);
        // Whether or not the file is valid
        bool valid = true;
        // Record if there is an error parsing
        try
        {
            layout = JsonConvert.DeserializeObject<Layout>(json);
        }
        catch (JsonException)
        {
            valid = false;
        }

        // Set title to file name
        title.SetText(Path.GetFileNameWithoutExtension(path.path));
        // If the layout is valid
        if (valid)
        {
            // Set author, description
            author.SetText("By " + layout.Author);
            description.SetText(layout.Description);

            // Set default row and columns if not set
            if (layout.Rows == null)
            {
                layout.Rows = Layout.standard.Rows;
            }
            if (layout.Columns == null)
            {
                layout.Columns = Layout.standard.Columns;
            }
            // Set board sizes
            rowCount.SetText(layout.Rows.ToString());
            columnCount.SetText(layout.Columns.ToString());

            #region Piece information
            // If there are prespecified pieces
            bool piecesSpecified = layout.Pieces != null;
            if (piecesSpecified)
            {
                // Count number of black and white pieces
                int black = 0, white = 0;
                foreach (PieceInfo piece in layout.Pieces)
                {
                    // The actual number of pieces that this piece has, accounting for stacks
                    int number = piece.Stacked + 1;
                    if (piece.White)
                    {
                        white += number;
                    }
                    else
                    {
                        black += number;
                    }
                }

                // Set white and black numbers to the number of pieces
                pieceWhiteNum.SetText(white.ToString());
                pieceBlackNum.SetText(black.ToString());
            }
            // If a not default piece num is given
            else if (layout.PieceNum != null)
            {
                pieceEachNum.SetText(layout.PieceNum.ToString());
            }
            // If the default amount of pieces is wanted
            else
            {
                pieceEachNum.SetText(Layout.standard.PieceNum.ToString());
            }
            // Set correct set of information groups active
            pieceEachGroup.SetActive(!piecesSpecified);
            pieceWhiteGroup.SetActive(piecesSpecified);
            pieceBlackGroup.SetActive(piecesSpecified);
            #endregion

            #region Objective hex information
            // If there are prespecified pieces
            bool objectiveHexesSpecified = layout.ObjectiveHexes != null;
            if (objectiveHexesSpecified)
            {
                // Count number of black and white objective hexes
                int black = 0, white = 0;
                foreach (int[] objectiveHex in layout.ObjectiveHexes)
                {
                    // Counts as black or white depending on which side of the board it's on
                    if (objectiveHex[0] < layout.Rows / 2)
                    {
                        white++;
                    }
                    else
                    {
                        black++;
                    }
                }

                // Set white and black numbers to the number of objective hexes
                objectiveHexWhiteNum.SetText(white.ToString());
                objectiveHexBlackNum.SetText(black.ToString());
            }
            // If a not default objectiveHex num is given
            else if (layout.ObjectiveHexNum != null)
            {
                objectiveHexEachNum.SetText(layout.ObjectiveHexNum.ToString());
            }
            // If the default amount of objectiveHexes is wanted
            else
            {
                objectiveHexEachNum.SetText(Layout.standard.ObjectiveHexNum.ToString());
            }
            // Set correct set of information groups active
            objectiveHexEachGroup.SetActive(!objectiveHexesSpecified);
            objectiveHexWhiteGroup.SetActive(objectiveHexesSpecified);
            objectiveHexBlackGroup.SetActive(objectiveHexesSpecified);
            #endregion
        }
        else
        {
            description.SetText("There is an error with this layout file. Check the formatting and try again.");
        }

        // Set everything active or inactive based on validity
        author.gameObject.SetActive(valid);
        playButton.gameObject.SetActive(valid);
        sizeGroup.SetActive(valid);
        pieceGroup.SetActive(valid);
        objectiveHexGroup.SetActive(valid);
    }

    /// <summary>Sets game type to corresponding enum based on a string</summary>
    public void SetGameType(string gt)
    {
        switch (gt)
        {
            case "PassAndPlay":
                gameType = GameType.PassAndPlay;
                break;
            case "Local":
                gameType = GameType.Local;
                break;
            case "Online":
                gameType = GameType.Online;
                break;
            default:
                gameType = GameType.PassAndPlay;
                break;
        }
    }

    public void OpenFolder()
    {
        Process process = new Process();
        process.StartInfo.FileName = Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor ? "explorer.exe" : "open";
        process.StartInfo.Arguments = "file://" + Application.persistentDataPath;
        process.Start();
    }

    public void Quit()
    {
        Application.Quit();
        UnityEngine.Debug.Log("Qutting application...");
    }
}
