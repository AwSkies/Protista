using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public abstract class StatsPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject[] statRows;

    // Start is called before the first frame update
    public void Start()
    {
        // Get values and stat texts
        string[] values = GetValues();
        TMP_Text[] stats = (from row in statRows select row.transform.GetChild(1).GetComponent<TMP_Text>()).ToArray();
        // Loop through each row
        for (int i = 0; i < stats.Count(); i++)
        {
            // Set each stat row value to the corresponding stat value
            stats[i].SetText(values[i]);
        }
    }

    public abstract string[] GetValues();
}
