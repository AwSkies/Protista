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
public class Game : MonoBehaviour
{
    // The deserialized data
    public static Layout layout;
    public static GameType gameType;

    [SerializeField]
    private GameObject noLayoutsText;
    [SerializeField]
    private GameObject openFolderButton;
    [SerializeField]
    private GameObject layoutButton;
    [SerializeField]
    private Transform contentField;

    [SerializeField]
    private TMP_Text title;
    [SerializeField]
    private TMP_Text author;
    [SerializeField]
    private TMP_Text description;
    [SerializeField]
    private TMP_Text rowText;
    [SerializeField]
    private TMP_Text rowCount;
    [SerializeField]
    private TMP_Text columnText;
    [SerializeField]
    private TMP_Text columnCount;
    [SerializeField]
    private GameObject playButton;

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
        foreach (GameObject button in layoutButtons)
        {
            Destroy(button);
        }
        layoutButtons = new List<GameObject>();
    }

    public void DisplayInformation(LayoutPath path)
    {
        string json = File.ReadAllText(path.path);
        try
        {
            layout = JsonConvert.DeserializeObject<Layout>(json);
        }
        catch (JsonReaderException)
        {
            layout = null;
        }
        title.SetText(Path.GetFileNameWithoutExtension(path.path));
        if (layout != null)
        {
            author.SetText("By " + layout.Author);
            description.SetText(layout.Description);
            rowCount.SetText(layout.Rows.ToString());
            columnCount.SetText(layout.Columns.ToString());
            author.gameObject.SetActive(true);
            rowText.gameObject.SetActive(true);
            rowCount.gameObject.SetActive(true);
            columnText.gameObject.SetActive(true);
            columnCount.gameObject.SetActive(true);
            playButton.gameObject.SetActive(true);
        }
        else
        {
            description.SetText("There is an error with this layout file. Check the formatting to make sure it is correct.");
            author.gameObject.SetActive(false);
            rowText.gameObject.SetActive(false);
            rowCount.gameObject.SetActive(false);
            columnText.gameObject.SetActive(false);
            columnCount.gameObject.SetActive(false);
            playButton.gameObject.SetActive(false);
        }
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
}
