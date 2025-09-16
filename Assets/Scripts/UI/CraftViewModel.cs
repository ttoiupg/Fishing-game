using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftViewModel : MonoBehaviour, IViewFrame
{
    public CanvasGroup MainCanvasGroup;
    public GameObject craftRequirementCellPrefab;
    public GameObject craftElementPrefab;

    private List<GameObject> craftElementDisplays = new List<GameObject>();
    
    private void SelectElement()
    {
        
    }

    private void SetupCraftElementDisplay(CraftElement craftElement)
    {
        var obj = Instantiate(craftElementPrefab);
        obj.transform.Find("Icon").GetComponent<Image>().sprite = craftElement.craftItem.icon;
        obj.transform.Find("ElementName").GetComponent<TextMeshProUGUI>().text = craftElement.craftItem.name;
        var totalAmount = 0;
        var totalHave = 0;
        foreach (var cr in craftElement.requirements)
        {
            totalAmount += cr.amount;
            totalHave = InventoryManager.Instance.items.FindAll(item => item.item == cr.item).Count;
        }
        //2025/09/16/22:19 : stopped to sleep, health is more important than anything -- halfmoon
    }
    private void CleanElements()
    {
        foreach (var go in craftElementDisplays)
        {
            Destroy(go);
        }
        craftElementDisplays.Clear();
    }
    private void SetupCraftView()
    {
        CleanElements();
        var elements = DataPersistenceManager.Instance.craftElements;
        foreach (var ce in elements)
        {
            
        }
    }
    public void Begin()
    {
        GameManager.Instance.player.isActive = false;
        SetupCraftView();
        MainCanvasGroup.interactable = true;
        MainCanvasGroup.blocksRaycasts = true;
        MainCanvasGroup.DOFade(1, 0.3f);
    }

    public void End()
    {
        GameManager.Instance.player.isActive = true;
        MainCanvasGroup.DOFade(0, 0.3f).onComplete += () =>
        {
            MainCanvasGroup.interactable = false;
            MainCanvasGroup.blocksRaycasts = false;
        };
    }
}