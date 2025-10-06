using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using Unity.Mathematics;
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
    public Image craftButton;
    public TextMeshProUGUI craftButtonText;
    private Dictionary<GameObject,CraftElement> craftElementDisplays = new Dictionary<GameObject,CraftElement>();
    private Dictionary<GameObject,bool> craftables = new Dictionary<GameObject, bool>();
    private CraftElement currentCraftElement;
    private GameObject currentCraftElementGameObject;
    private List<GameObject> requirements = new List<GameObject>();
    private EventSystem eventSystem;

    private void Start()
    {
        eventSystem = EventSystem.current;
    }

    private void SelectElement(GameObject element)
    {
        if (currentCraftElementGameObject == element) return;
        if (currentCraftElementGameObject != null)
        {
            currentCraftElementGameObject.GetComponent<Image>().color = Color.white;
        }
        currentCraftElementGameObject = element;
        currentCraftElement = craftElementDisplays[element];
        currentCraftElementGameObject.GetComponent<Image>().color = new Color32(28, 255, 239, 255);
        PrepareCraftElementDetail(currentCraftElementGameObject);
    }
    
    private void PrepareCraftElementDetail(GameObject craftElementObject)
    {
        foreach (var go in requirements)
        {
            Destroy(go);
            go.transform.Find("Percentage").GetComponent<RectTransform>().DOKill();
        }
        requirements.Clear();
        var craftElement = craftElementDisplays[craftElementObject];
        craftElementImage.sprite = craftElement.craftItem.icon;
        craftElementDescription.text = craftElement.craftItem.description;
        craftElementName.text = craftElement.craftItem.name;
        foreach (var cr in craftElement.requirements)
        {
            var amount = (cr.type == CraftRequirementType.Item)
                ? InventoryManager.Instance.items.Find(x => x.item == cr.item).amount
                : InventoryManager.Instance.fishes.FindAll(fish => fish.fishType == cr.fish).Count;
            var rc = Instantiate(craftRequirementCellPrefab, craftRequirementCellContainer);
            var percentage = (float)amount / (float)cr.amount;
            rc.transform.Find("Icon").GetComponent<Image>().sprite = (cr.type == CraftRequirementType.Item) ? cr.item.icon : cr.fish.Art;
            rc.transform.Find("RequirementName").GetComponent<TextMeshProUGUI>().text =(cr.type == CraftRequirementType.Item) ? cr.item.name : cr.fish.name;
            rc.transform.Find("Amount").GetComponent<TextMeshProUGUI>().text = $"{amount}/{cr.amount}";
            rc.transform.Find("Percentage").GetComponent<RectTransform>().DOScaleX(math.clamp(percentage,0,1), 0.5f)
                .SetEase(Ease.InOutSine);
            requirements.Add(rc);
        }
        craftButton.color = craftables[craftElementObject] ? new Color32(0,243,177,255) : new Color32(255,52,90,255);
    }
    private void SetupCraftElementDisplay(CraftElement craftElement)
    {
        var obj = Instantiate(craftElementPrefab, craftElementContainer, false);
        craftElementDisplays.Add(obj,craftElement);
        craftables.Add(obj,false);
        obj.transform.Find("Icon").GetComponent<Image>().sprite = craftElement.craftItem.icon;
        obj.transform.Find("ElementName").GetComponent<TextMeshProUGUI>().text = craftElement.craftItem.name;
        var totalAmount = 0;
        var totalHave = 0;
        var canCraft = true;
        foreach (var cr in craftElement.requirements)
        {
            totalAmount += cr.amount;
            var amount = (cr.type == CraftRequirementType.Item)
                ? InventoryManager.Instance.items.Find(x => x.item == cr.item).amount
                : InventoryManager.Instance.fishes.FindAll(fish => fish.fishType == cr.fish).Count;
            totalHave += amount;
            canCraft = (amount >= cr.amount) ? canCraft : false;
        }
        craftables[obj] = canCraft;
        var percentage = (float)totalHave / (float)totalAmount;
        obj.transform.Find("ProgressText").GetComponent<TextMeshProUGUI>().text = $"{totalHave}/{totalAmount}";
        obj.transform.Find("Bar").GetComponent<RectTransform>().DOScaleX(math.clamp(percentage,0,1), 0.5f).SetEase(Ease.InOutSine);
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
        craftables.Clear();
    }
    private void SetupCraftView()
    {
        CleanElements();
        var elements = DataPersistenceManager.Instance.craftElements;
        foreach (var ce in elements)
        {
            SetupCraftElementDisplay(ce);
        }
        currentCraftElementGameObject = null;
        SelectElement(craftElementDisplays.First().Key);
    }

    private void RefreshCraftElements()
    {
        foreach (var kp in craftElementDisplays)
        {
            var obj = kp.Key;
            var ce = kp.Value;
            var totalAmount = 0;
            var totalHave = 0;
            var canCraft = true;
            foreach (var cr in ce.requirements)
            {
                totalAmount += cr.amount;
                var amount = (cr.type == CraftRequirementType.Item)
                    ? InventoryManager.Instance.items.Find(x => x.item == cr.item).amount
                    : InventoryManager.Instance.fishes.FindAll(fish => fish.fishType == cr.fish).Count;
                totalHave += amount;
                canCraft = (amount >= cr.amount) ? canCraft : false;
            }
            craftables[obj] = canCraft;
            var percentage = (float)totalHave / (float)totalAmount;
            obj.transform.Find("ProgressText").GetComponent<TextMeshProUGUI>().text = $"{totalHave}/{totalAmount}";
            obj.transform.Find("Bar").GetComponent<RectTransform>().DOScaleX(math.clamp(percentage,0,1), 0.5f).SetEase(Ease.InOutSine);
        }
    }
    void RemoveN<T>(List<T> list, System.Predicate<T> match, int n)
    {
        int removed = 0;
        for (int i = 0; i < list.Count && removed < n;)
        {
            if (match(list[i]))
            {
                list.RemoveAt(i);
                removed++;
            }
            else
            {
                i++;
            }
        }
    }
    private void SubtractMaterials(CraftElement craftElement)
    {
        foreach (var cr in craftElement.requirements)
        {
            if (cr.type == CraftRequirementType.Item)
            {
                InventoryManager.Instance.items.ForEach(x =>
                {
                    if (x.item == cr.item)
                    {
                        x.amount-= cr.amount;
                    }
                });
            }
            else
            {
                RemoveN(InventoryManager.Instance.fishes, x => x.fishType == cr.fish, cr.amount);
            }
        }
    }

    private void GiveCraftItem(CraftElement craftElement)
    {
        var item = new GameItem(craftElement.craftItem,craftElement.amount);
        LootManager.Instance.AddItem(item);
        LootTagDisplayManager.instance.AddTag(craftElement.craftItem.icon,craftElement.craftItem.name,craftElement.amount,0.7f,"+","");
    }
    public void craft()
    {
        if (currentCraftElementGameObject == null) return;
        if (!craftables[currentCraftElementGameObject]) return;
        //remove requirements
        var ce = craftElementDisplays[currentCraftElementGameObject];
        SubtractMaterials(ce);
        RefreshCraftElements();
        var temp = currentCraftElementGameObject;
        currentCraftElementGameObject = null;
        SelectElement(temp);
        GiveCraftItem(ce);
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
        MainCanvasGroup.interactable = false;
        MainCanvasGroup.blocksRaycasts = false;
        MainCanvasGroup.DOFade(0, 0.3f);
        eventSystem.SetSelectedGameObject(null);
    }
}