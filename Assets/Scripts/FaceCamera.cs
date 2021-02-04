using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FaceCamera : MonoBehaviour
{
    public TextMeshProUGUI text;

    // Update is called once per frame
    void Update()
    {
        text.transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.GetChild(0).position);
    }
}
