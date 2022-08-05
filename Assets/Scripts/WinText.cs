using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinText : MonoBehaviour
{
    [SerializeField]
    private TMP_Text winner;
    [SerializeField]
    private TMP_Text[] texts;

    // Start is called before the first frame update
    void Start()
    {
        // Set winner text to black or white depending on who won
        winner.SetText(Results.whiteWinner ? "White" : "Black");
        // Decide winning color based on results
        Color color = Results.whiteWinner ? Color.white : Color.black;
        // Set text to colors
        foreach (TMP_Text text in texts)
        {
            text.color = color;
        }
    }
}
