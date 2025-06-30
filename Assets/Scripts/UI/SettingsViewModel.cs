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
    public  DofController dofController;
    private Player _player;
    public void Start()
    {
        _player = GameManager.Instance.player;
        volumeSlider.value = PlayerPrefs.GetFloat("Volume");
        dofController = FindAnyObjectByType<DofController>();
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
        dofController.SetFocusDistance(0.1f);
    }

    public void CloseUI()
    {
        SoundFXManger.Instance.PlaySoundFXClip(ViewManager.instance.defaultCloseSound, _player.characterTransform, 1f);
        mainFrame.gameObject.SetActive(false);
        dofController.SetFocusDistance(2f);
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
