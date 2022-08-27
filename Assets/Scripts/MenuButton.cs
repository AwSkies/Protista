using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton : MonoBehaviour
{
    public GameObject description;

    public void OnDisable()
    {
        description.SetActive(false);
    }
}
