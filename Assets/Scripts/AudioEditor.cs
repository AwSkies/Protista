using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class AudioEditor : MonoBehaviour
{
    [SerializeField]
    private AudioMixer mixer;
    [SerializeField]
    private string track;
    [SerializeField]
    private TMP_InputField input;
    [SerializeField]
    private Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        // Get the current value of the volume
        mixer.GetFloat(track, out float value);
        // Set the slider and input field volumes
        slider.value = value;
        input.text = FormatText(value);
    }

    /// <summary>Sets the volume and updates the slider value when the value is edited from the input field.</summary>
    public void UpdateFromInput()
    {
        // If the value can be parsed into an int
        int value;
        if (int.TryParse(input.text, out value))
        {
            value -= 80;
            // Update the slider value and volume
            slider.value = value;
            SetVolume(track, value);
        }
    }

    /// <summary>Sets the volume and updates the input field when the value is edited from the slider.</summary>
    public void UpdateFromSlider()
    {
        float value = slider.value;
        // Update the input field and volume
        input.text = FormatText(value);
        SetVolume(track, value);
    }

    /// <summary>Converts the float value of the volume into a string of an int from 0-100</summary>
    private string FormatText(float value)
    {
        return Mathf.RoundToInt(value + 80).ToString();
    }

    private void SetVolume(string track, float value)
    {
        mixer.SetFloat(track, value);
        PlayerPrefs.SetFloat(track, value);
    }
}
