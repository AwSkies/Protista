using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultsMenu : MonoBehaviour
{
    public void Rematch()
    {
        SceneManager.LoadScene("GameBoard");
    }

    public void ReturnToMain()
    {
        MainMenu.layout = null;
        SceneManager.LoadScene("MainMenu");
    }

    public void Quit()
    {
        Debug.Log("Quitting application...");
        Application.Quit();
    }
}
