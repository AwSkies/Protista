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
    private GameObject layoutButtonPrefab;
    [SerializeField]
    private Transform contentField;

    private List<GameObject> layoutButtons = new List<GameObject>();

    /// <summary>Loads the GameBoard scene containing the actual game board.
    /// The static fields layout and gameType will be processed by the board GameManager script and be handled accordingly.</summary>
    public void LoadGame()
    {
        SceneManager.LoadScene("GameBoard");
    }

    /// <summary>Displays in the UI the json layouts appearing in the player's data directory</summary>
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
                GameObject button = Instantiate(layoutButtonPrefab, contentField);
                // Sets the text of the button to be the name of the layout
                button.transform.GetChild(0).GetComponent<TMP_Text>().SetText(path);
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
