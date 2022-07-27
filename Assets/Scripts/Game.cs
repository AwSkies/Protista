using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>Stores information about the type and parameters of the game and contains button functions for configuring them.</summary>
public class Game : MonoBehaviour
{
    // The deserialized data
    public static Layout layout;
    public static GameType gameType;

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
        string[] files = (string[]) from file in Directory.GetFiles(Application.persistentDataPath) where file.EndsWith(".json") select file;
        foreach (string path in files)
        {
            // Make a button to click on for each of the names
            
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
}
