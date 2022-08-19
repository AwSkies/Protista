using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>Sets the audio preferences on startup.</summary>
public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioMixer mixer;
    [SerializeField]
    private string[] tracks;

    // Start is called before the first frame update
    void Start()
    {
        foreach (string track in tracks)
        {
            if (PlayerPrefs.HasKey(track))
            {
                mixer.SetFloat(track, PlayerPrefs.GetFloat(track));
            }
        }
    }
}
