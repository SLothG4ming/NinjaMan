using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{

    public AudioMixer audioMixer;
    // Start is called before the first frame update
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Volume", volume);
    }


    public void SetQuality(int qualityindex)
    {
        QualitySettings.SetQualityLevel(qualityindex);

}

    public void SetFullScreen (bool isFullscreen) 
    {
        Screen.fullScreen = isFullscreen;
    }
}


