using System;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayMenuViewModel : MonoBehaviour, IViewFrame
{
    public Image Sea;
    public Material SeaMaterial;
    public GameObject content;
    public List<GameObject> SaveDisplay;
    public GameObject confirmScreen;
    public GameObject actionScreen;
    public TMP_InputField NewName;
    public TextMeshProUGUI StartButtonText;
    private int selectIndex = 0;
    private bool overwrite = false;
    public bool haveSaves = false;
    private bool loadButtonPressed = false;

    public GameObject loadButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Start()
    {
        SeaMaterial =Instantiate(Sea.material);
        Sea.material = SeaMaterial;
    }

    public void OnButtonClick(int index)
    {
        loadButtonPressed = false;
        selectIndex = index;
        if (DataPersistenceManager.Instance.saves.Count-1 < index)
        {
            NewName.text = "";
            overwrite = false;
            confirmScreen.SetActive(true);
            EventSystem.current.SetSelectedGameObject(NewName.gameObject);
            return;
        }
        else
        {
            actionScreen.SetActive(true);
            EventSystem.current.SetSelectedGameObject(loadButton);
        }
    }

    public void Overwrite()
    {
        overwrite = true;
        actionScreen.SetActive(false);
        NewName.text = "";
        confirmScreen.SetActive(true);
    }
    public void ConfirmLoad()
    {
        if (loadButtonPressed) return;
        loadButtonPressed = true;
        Debug.Log("Loading Save Slot " + selectIndex);
        DataPersistenceManager.Instance.LoadGame(selectIndex);
    }
    public void ConfirmCreateNewGame()
    {
        if (loadButtonPressed) return;
        loadButtonPressed = true;
        if (overwrite)
        {
            var direc = DataPersistenceManager.Instance.saves[selectIndex].directory;
            if (Directory.Exists(direc))
            {
                Directory.Delete(direc, true);
            } 
            DataPersistenceManager.Instance.saves.RemoveAt(selectIndex);
        }
        if (NewName.text == "") return;
        DataPersistenceManager.Instance.CreateNewSave(NewName.text);
    }

    public void checkSaves()
    {
        for (int i = 0; i < 6; i++)
        {
            var display = SaveDisplay[i];
            var slot = (i+1 <= DataPersistenceManager.Instance.saves.Count)? DataPersistenceManager.Instance.saves[i]:null;
            if (slot == null)
            {
                display.transform.Find("ScreenShot").gameObject.SetActive(false);
                display.transform.Find("Time").GetComponent<TextMeshProUGUI>().text = "";
                continue;
            };
            haveSaves = true;
            Texture2D tex = new Texture2D(640, 360, TextureFormat.RGBA32, false);
            display.transform.Find("Time").GetComponent<TextMeshProUGUI>().text = File.GetLastWriteTime(slot.directory + "/tide.PlayerData").ToString("yyyy/MM/dd HH:mm:ss");
            if (File.Exists(slot.directory + "/SaveThumbnail.png"))
            {
                tex.LoadImage(File.ReadAllBytes(slot.directory + "/SaveThumbnail.png"));
            }
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
            display.transform.Find("ScreenShot").GetComponent<Image>().sprite = sprite;
            display.transform.Find("SaveName").GetComponent<TextMeshProUGUI>().text = slot.Name;
        }
    }
    public void Begin()
    {
        Sea.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        Sea.material.DOFloat(1f,"_Height",0.5f).SetEase(Ease.OutQuad).OnComplete(()=>
        {
            EventSystem.current.SetSelectedGameObject(SaveDisplay[0]);
            content.SetActive(true);
        });
        checkSaves();
    }

    public void End()
    {
            content.SetActive(false);
        Sea.material.DOFloat(0f,"_Height",0.5f).SetEase(Ease.OutQuad).OnComplete(()=>
        {
            Sea.gameObject.SetActive(false);
        });
    }
}