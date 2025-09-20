using System.Collections.Generic;
using System.Linq;
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
    public RectTransform craftElementContainer;
    public RectTransform craftRequirementCellContainer;
    public Image craftElementImage;
    public TextMeshProUGUI craftElementName;
    public TextMeshProUGUI craftElementDescription;

    private Dictionary<GameObject,CraftElement> craftElementDisplays = new Dictionary<GameObject,CraftElement>();
    
    private CraftElement currentCraftElement;
    private List<GameObject> requirements = new List<GameObject>();
    
    private void SelectElement(GameObject element)
    {
        if (currentCraftElement == craftElementDisplays[element]) return;
        currentCraftElement = craftElementDisplays[element];
        PrepareCraftElementDetail(currentCraftElement);
    }

    private void PrepareCraftElementDetail(CraftElement craftElement)
    {
        foreach (var go in requirements)
        {
            Destroy(go);
        }
        requirements.Clear();
        craftElementImage.sprite = craftElement.craftItem.icon;
        craftElementDescription.text = craftElement.craftItem.description;
        craftElementName.text = craftElement.craftItem.name;
        foreach (var cr in craftElement.requirements)
        {
            var amount = (cr.type == CraftRequirementType.Item)
                ? InventoryManager.Instance.items.Find(x => x.item == cr.item).amount
                : InventoryManager.Instance.fishes.FindAll(fish => fish.fishType == cr.fish).Count;
            var rc = Instantiate(craftRequirementCellPrefab, craftRequirementCellContainer);
            rc.transform.Find("Icon").GetComponent<Image>().sprite = (cr.type == CraftRequirementType.Item) ? cr.item.icon : cr.fish.Art;
            rc.transform.Find("RequirementName").GetComponent<TextMeshProUGUI>().text =(cr.type == CraftRequirementType.Item) ? cr.item.name : cr.fish.name;
            rc.transform.Find("Amount").GetComponent<TextMeshProUGUI>().text = $"{amount}/{cr.amount}";
            rc.transform.Find("Percentage").GetComponent<RectTransform>().DOScaleX((float)amount / (float)cr.amount, 0.5f)
                .SetEase(Ease.InOutSine);
            requirements.Add(rc);
        }
    }
    private void SetupCraftElementDisplay(CraftElement craftElement)
    {
        var obj = Instantiate(craftElementPrefab, craftElementContainer, false);
        craftElementDisplays.Add(obj,craftElement);
        obj.transform.Find("Icon").GetComponent<Image>().sprite = craftElement.craftItem.icon;
        obj.transform.Find("ElementName").GetComponent<TextMeshProUGUI>().text = craftElement.craftItem.name;
        var totalAmount = 0;
        var totalHave = 0;
        foreach (var cr in craftElement.requirements)
        {
            totalAmount += cr.amount;
            
            totalHave += InventoryManager.Instance.items.FindAll(item => item.item == cr.item).Count;
        }
        obj.transform.Find("ProgressText").GetComponent<TextMeshProUGUI>().text = $"{totalHave}/{totalAmount}";
        obj.transform.Find("Bar").transform.DOScaleX(totalHave/totalAmount, 0.5f).SetEase(Ease.InOutSine);
        obj.GetComponent<Button>().onClick.AddListener(()=> SelectElement(obj));
        //2025/09/16/22:19 : stopped to sleep, health is more important than anything -- halfmoon
    }
    private void CleanElements()
    {
        foreach (var go in craftElementDisplays)
        {
            Destroy(go.Key);
        }
        craftElementDisplays.Clear();
    }
    private void SetupCraftView()
    {
        CleanElements();
        var elements = DataPersistenceManager.Instance.craftElements;
        foreach (var ce in elements)
        {
            SetupCraftElementDisplay(ce);
        }
        currentCraftElement = null;
        SelectElement(craftElementDisplays.First().Key);
    }
    public void Begin()
    {
        GameManager.Instance.player.isActive = false;
        ViewManager.instance.frameLock = true;
        SetupCraftView();
        MainCanvasGroup.interactable = true;
        MainCanvasGroup.blocksRaycasts = true;
        MainCanvasGroup.DOFade(1, 0.3f);
    }

    public void End()
    {
        GameManager.Instance.player.isActive = true;
        ViewManager.instance.frameLock = false;
        MainCanvasGroup.DOFade(0, 0.3f).onComplete += () =>
        {
            MainCanvasGroup.interactable = false;
            MainCanvasGroup.blocksRaycasts = false;
        };
    }
}