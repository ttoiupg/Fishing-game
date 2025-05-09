using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    public Slider volumeSlider;

    public void Start()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("Volume");
    }

    public void UpdateMainMixerVolume()
    {
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
        AudioListener.volume = volumeSlider.value;
    }
}
