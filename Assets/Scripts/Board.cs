using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    // Reference to Game Manager
    public GameManager gameManager;

    // Index of hexes ordered by z, x position
    public GameObject[,] hexDex;

    // Start is called before the first frame update
    void Start()
    {
        hexDex = new GameObject[gameManager.rows, gameManager.columns];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
