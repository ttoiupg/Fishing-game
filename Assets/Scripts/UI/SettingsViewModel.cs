using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Screen = UnityEngine.Screen;

[System.Serializable]
public struct SettingTabBinding
{
    public GameObject Button;
    public GameObject Frame;
    public Selectable FirstObject;
    public Selectable LastObject;
}

public class SettingsViewModel : MonoBehaviour,IViewFrame,IDataPersistence
{
    public Color DarkColor;
    public Color LightColor;
    public Slider volumeSlider;
    public RectTransform mainFrame;
    public List<SettingTabBinding> TabButtons;
    public Button closeButton;
    public Camera camera;
    public int CurrentTab;
    public List<Vector2Int> screenResolutions = new List<Vector2Int>()
    {
        new Vector2Int(3840, 2160),
        new Vector2Int(2560, 1440),
        new Vector2Int(1920, 1080),
        new Vector2Int(1600, 900),
        new Vector2Int(1366, 768),
        new Vector2Int(845, 480),
        new Vector2Int(640, 360),
    };
    private Player _player;
    public TMP_Dropdown AntiAliasDropdown;
    public TMP_Dropdown ScreenSizeDropdown;
    public TMP_Dropdown FullscreenDropdown;

    public void CloseGame()
    {
#if UNITY_STANDALONE
        Application.Quit();
        // If running in the Unity Editor
#elif UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        Debug.Log("Application is attempting to quit.");
    }
    public void Start()
    {
        ScreenSizeDropdown.onValueChanged.AddListener(delegate { ScreenSizeChanged(); });
        AntiAliasDropdown.onValueChanged.AddListener(delegate { AntiAliasChanged(); });
        FullscreenDropdown.onValueChanged.AddListener(delegate { FullscreenChanged(); });
        if (GameManager.Instance)
        {
            _player = GameManager.Instance.player;
        }
       
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

            navigation.selectOnLeft = TabButtons[CurrentTab].FirstObject;
            button.navigation = navigation;
        }
        var nav = new Navigation();
        nav.mode = Navigation.Mode.Explicit;
        nav.selectOnUp = TabButtons[CurrentTab].LastObject;
    }

    private void ScreenSizeChanged()
    {
        var resolution = screenResolutions[ScreenSizeDropdown.value];
        var full = (FullScreenMode)FullscreenDropdown.value;
        Screen.SetResolution(resolution.x,resolution.y,full);
    }
    
    private void FullscreenChanged()
    {
        var resolution = screenResolutions[ScreenSizeDropdown.value];
        var full = (FullScreenMode)FullscreenDropdown.value;
        Screen.SetResolution(resolution.x,resolution.y,full);
    }

    private void AntiAliasChanged()
    {
        var antiAlias = AntiAliasDropdown.value;
        UniversalAdditionalCameraData data = camera.GetUniversalAdditionalCameraData();
        data.antialiasing = (AntialiasingMode)antiAlias;
    }
    public void UpdateMainMixerVolume()
    {
        AudioListener.volume = volumeSlider.value;
    }
    public void OpenUI()
    {
        SoundFXManger.Instance.PlaySoundFXClip(ViewManager.instance.defaultOpenSound, camera.transform, 0.5f);
        mainFrame.gameObject.SetActive(true);
    }
    public void SelectTab(int index)
    {
        CurrentTab = index;
        for (int i = 0; i < TabButtons.Count; i++)
        {
            TabButtons[i].Button.GetComponent<Image>().color = LightColor;
            TabButtons[i].Button.GetComponent<Image>().GetComponentInChildren<TextMeshProUGUI>().color = DarkColor;
            TabButtons[i].Frame.SetActive(false);
        }
        TabButtons[index].Frame.SetActive(true);
        TabButtons[index].Button.GetComponent<Image>().color = DarkColor;
        TabButtons[index].Button.GetComponent<Image>().GetComponentInChildren<TextMeshProUGUI>().color = LightColor;
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

            navigation.selectOnLeft = TabButtons[index].FirstObject;
            button.navigation = navigation;
        }
        var nav = new Navigation();
        nav.mode = Navigation.Mode.Explicit;
        nav.selectOnUp = TabButtons[index].LastObject;
    }
    public void CloseUI()
    {
        SoundFXManger.Instance.PlaySoundFXClip(ViewManager.instance.defaultCloseSound, camera.transform, 0.5f);
        mainFrame.gameObject.SetActive(false);
    }
    
    public void Begin()
    {
        if (_player)
        {
            _player.isActive = false; 
        }
        OpenUI();
    }

    public void End()
    {
        if (_player)
        {
            _player.isActive = true; 
        }
        CloseUI();
    }

    public void LoadData(GameData data)
    {
        volumeSlider.value = data.globalSettings.masterVolume;
        AntiAliasDropdown.value = (int)data.globalSettings.antiAlias;
        AntiAliasDropdown.RefreshShownValue();
        ScreenSizeDropdown.value = screenResolutions.IndexOf(data.globalSettings.screenSize);
        ScreenSizeDropdown.RefreshShownValue();
        FullscreenDropdown.value = data.globalSettings.Fullscreen;
        FullscreenDropdown.RefreshShownValue();
        Debug.Log(data.globalSettings.screenSize);
        AntiAliasChanged();
        ScreenSizeChanged();
        UpdateMainMixerVolume();
    }

    public void SaveData(ref GameData data)
    {
        data.globalSettings.masterVolume = volumeSlider.value;
        data.globalSettings.antiAlias = (AntiAlias)AntiAliasDropdown.value;
        data.globalSettings.screenSize = screenResolutions[ScreenSizeDropdown.value];
        data.globalSettings.Fullscreen = FullscreenDropdown.value;
    }
}