using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardPosition : MonoBehaviour
{
    public int x;
    public int z;

    public override string ToString()
    {
        return base.ToString() + $"({x}, {z})";
    }
}
