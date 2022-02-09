using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Contains data about a hex, like the its neighbors stored in an array and the piece on this hex.</summary>
public class Hex : MonoBehaviour
{
    public GameObject[] neighbors = new GameObject[6];
    public GameObject piece;
}
