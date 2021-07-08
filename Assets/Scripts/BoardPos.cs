using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardPos : MonoBehaviour
{
    public int x;
    public int z;

    public override string ToString()
    {
        return $"({x}, {z})";
    }
}
