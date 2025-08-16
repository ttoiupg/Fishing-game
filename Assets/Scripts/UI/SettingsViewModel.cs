using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public struct SettingTabBinding
{
    public GameObject Button;
    public GameObject Frame;
}

public class SettingsViewModel : MonoBehaviour,IViewFrame
{
    public Color DarkColor;
    public Color LightColor;
    public Slider volumeSlider;
    public RectTransform mainFrame;
    public List<SettingTabBinding> TabButtons;
    public  DofController dofController;
    private Player _player;
    public void Start()
    {
        _player = GameManager.Instance.player;
        volumeSlider.value = PlayerPrefs.GetFloat("Volume");
        dofController = FindAnyObjectByType<DofController>();
       
        for(int i = 0; i < TabButtons.Count; i++)
        {
            var button = TabButtons[i].Button.GetComponent<Button>();
            var navigation = new Navigation();
            navigation.mode = Navigation.Mode.Explicit;
            if (i - 1 < 0)
            {
                navigation.selectOnRight = TabButtons[i +1].Button.GetComponent<Button>();
                navigation.selectOnLeft = TabButtons[^1].Button.GetComponent<Button>();
            }
            else
            {
                navigation.selectOnRight = TabButtons[0].Button.GetComponent<Button>();
                navigation.selectOnLeft = TabButtons[i - 1].Button.GetComponent<Button>();
            }
            button.navigation = navigation;
        }


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
        dofController.SetFocusDistance(0f);
    }
    public void SelectTab(int index)
    {
        for (int i = 0; i < TabButtons.Count; i++)
        {
            TabButtons[i].Button.GetComponent<Image>().color = LightColor;
            TabButtons[i].Button.GetComponent<Image>().GetComponentInChildren<TextMeshProUGUI>().color = DarkColor;
            TabButtons[i].Frame.SetActive(false);
        }
        TabButtons[index].Frame.SetActive(true);
        TabButtons[index].Button.GetComponent<Image>().color = DarkColor;
        TabButtons[index].Button.GetComponent<Image>().GetComponentInChildren<TextMeshProUGUI>().color = LightColor;
    }
    public void CloseUI()
    {
        SoundFXManger.Instance.PlaySoundFXClip(ViewManager.instance.defaultCloseSound, _player.characterTransform, 1f);
        mainFrame.gameObject.SetActive(false);
        dofController.SetFocusDistance(100f);
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
