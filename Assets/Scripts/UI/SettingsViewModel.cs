using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SettingsViewModel : MonoBehaviour,IViewFrame
{
    public Slider volumeSlider;
    public RectTransform mainFrame;
    private Volume _globalVolume;
    private Player _player;
    public void Start()
    {
        _player = GameManager.Instance.player;
        volumeSlider.value = PlayerPrefs.GetFloat("Volume");
        _globalVolume = FindAnyObjectByType<Volume>();
    }

    public void UpdateMainMixerVolume()
    {
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
        AudioListener.volume = volumeSlider.value;
    }
    public void OpenUI()
    {
        SoundFXManger.Instance.PlaySoundFXClip(ViewManager.instance.defaultOpenSound, _player.characterTransform, 1f);
        mainFrame.gameObject.SetActive(true);
        _globalVolume.weight = 0;
    }

    public void CloseUI()
    {
        FishCardHandler.instance.CloseCard();
        SoundFXManger.Instance.PlaySoundFXClip(ViewManager.instance.defaultCloseSound, _player.characterTransform, 1f);
        mainFrame.gameObject.SetActive(false);
        _globalVolume.weight = 1;
    }
    public void Begin()
    {
        OpenUI();
    }

    public void End()
    {
        CloseUI();
    }
}
