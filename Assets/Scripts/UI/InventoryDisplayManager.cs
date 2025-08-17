using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryDisplayManager : MonoBehaviour,IViewFrame
{
    public static InventoryDisplayManager instance { get; private set; }
    public RectTransform InventoryBody;
    public ItemIconDisplayer itemIconDisplayer;
    public FishIconDisplayer fishIconDisplayer;
    public FishingRodCellDisplayer fishingRodCellDisplayer;
    public GameObject closeButton;
    public RectTransform inventoryContext;
    [Header("contents")]
    public RectTransform baitBoxContent;
    public RectTransform itemContent;
    public RectTransform fishContent;
    public RectTransform fishingRodContent;
    public List<GameObject> scrollViews;
    public List<CanvasGroup> categoryButtons;
    [SerializeField]private List<ItemIconDisplayer> _itemIconDisplayers = new List<ItemIconDisplayer>();
    [SerializeField]private List<FishIconDisplayer> _fishIconDisplayers = new List<FishIconDisplayer>();
    private List<FishingRodCellDisplayer> _fishingRodCellDisplayers = new List<FishingRodCellDisplayer>();
    [Header("Detail")] 
    public GameObject itemDetail;
    public RectTransform detailContent;
    public Image itemImage;
    public Image tagImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI detailText;
    [Header("Fish detail")]
    public GameObject fishDetail;

    public TextMeshProUGUI fishNameText;
    public TextMeshProUGUI weightText;
    public RectTransform weightBar;
    public TextMeshProUGUI rarityText;
    public TextMeshProUGUI mutationText;
    [Header("Fishing rod detail")]
    public GameObject fishingRodDetail;
    public Image fishingRodImage;
    public TextMeshProUGUI fishingRodName;
    public TextMeshProUGUI rodDamageText;
    public RectTransform rodDamageBar;
    public TextMeshProUGUI rodHandlingText;
    public RectTransform rodHandlingBar;
    public TextMeshProUGUI rodCriticalChanceText;
    public RectTransform rodCriticalChanceBar;
    public TextMeshProUGUI rodCriticalMultiplierText;
    public RectTransform rodCriticalMultiplierBar;
    public TextMeshProUGUI rodLuckText;
    public RectTransform rodLuckBar;
    public TextMeshProUGUI rodResilienceText;
    public RectTransform rodResilienceBar;
    public TextMeshProUGUI rodDurabilityText;
    public RectTransform rodDurabilityBar;
    public TextMeshProUGUI rodRarityText;
    [Header("money")]
    public TextMeshProUGUI moneyText;
    [Space]
    public GameObject currentInspecting;
    public FishingRodCellDisplayer currentFishingRod;
    private EventSystem _eventSystem;

    private List<Fish> _cacheFishList = new();
    private List<FishingRod> _cacheFishingRodList = new();
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        _eventSystem = EventSystem.current;
    }
    public void ChangeCategory(int index)
    {
        for(int i = 0; i < scrollViews.Count; i++)
        {
            categoryButtons[i].alpha = (i==index)?1:0.5f;
            scrollViews[i].SetActive(i == index);
        }
        CloseDetailView();
        switch (index)
        {
         case 0:
             SetupFishNavigation();
             itemDetail.SetActive(false);
             fishDetail.SetActive(true);
             fishingRodDetail.SetActive(false);
             break;
         case 1:
             itemDetail.SetActive(true);
             fishDetail.SetActive(false);
             fishingRodDetail.SetActive(false);
             break;
         case 2:
             SetupFishingRodsNavigation();
             itemDetail.SetActive(false);
             fishDetail.SetActive(false);
             fishingRodDetail.SetActive(true);
             break;
         default:
             break;
        }

        currentInspecting = null;
        currentFishingRod = null;
    }

    public void PrepareFish()
    {
        _cacheFishList = InventoryManager.Instance.fishes;
        var FishList = _cacheFishList.ToList().OrderBy(item => 1 / item.fishType.Rarity.OneIn);

        foreach (var fish in FishList)
        {
            var icon = Instantiate(fishIconDisplayer.gameObject, fishContent);
            var iconDisplayer = icon.GetComponent<FishIconDisplayer>();
            var button = icon.GetComponent<Button>();
            iconDisplayer.fish = fish;
            iconDisplayer.Init();
            button.onClick.AddListener(() => SetFishDetailView(iconDisplayer));
            _fishIconDisplayers.Add(iconDisplayer);
        }
        for (int i = 0; i < _fishIconDisplayers.Count; i++)
        {
            var icon = _fishIconDisplayers[i].GetComponent<Button>();
            var nav = new Navigation
            {
                mode = Navigation.Mode.Explicit
            };
            var up = (i - 6 < 0) ? categoryButtons[0].GetComponent<Button>() : _fishIconDisplayers[i - 6].GetComponent<Button>();
            var down = (i + 6 > _fishIconDisplayers.Count - 1) ? null : _fishIconDisplayers[i + 6].GetComponent<Button>();
            var left = (i - 1 < 0) ? null : _fishIconDisplayers[i - 1].GetComponent<Button>();
            var right = (i + 1 > _fishIconDisplayers.Count - 1) ? null : _fishIconDisplayers[i + 1].GetComponent<Button>();
            nav.selectOnUp = up;
            nav.selectOnDown = down;
            nav.selectOnLeft = left;
            nav.selectOnRight = right;
            if ((i + 1) % 6 == 0)
            {
                nav.selectOnRight = null;
            }
            else if ((i + 1) % 6 == 1)
            {
                nav.selectOnLeft = null;
            }
            icon.navigation = nav;
        }

        SetupFishNavigation();
    }

    public void SetupFishNavigation()
    {
        for (int i = 0; i < categoryButtons.Count; i++)
        {
            var nav = new Navigation { mode = Navigation.Mode.Explicit };
            nav.selectOnUp = _fishIconDisplayers[^1].GetComponent<Button>();
            nav.selectOnDown = _fishIconDisplayers[0].GetComponent<Button>();
            nav.selectOnLeft = (i-1<0) ? categoryButtons[^1].GetComponent<Button>() :  categoryButtons[i-1].GetComponent<Button>();
            nav.selectOnRight = (i+1>categoryButtons.Count-1) ? categoryButtons[0].GetComponent<Button>() :  categoryButtons[i+1].GetComponent<Button>();
            categoryButtons[i].GetComponent<Button>().navigation = nav;
        }
    }

    public void PrepareItems()
    {
        //filter out other item, for this, we're filtering out non bait item
        var baitList = InventoryManager.Instance.items.FindAll((item => item.item.itemType == ItemType.Bait))
            .OrderBy(item => item.item.name).ToList();
        foreach (var item in baitList)
        {
            var icon = Instantiate(itemIconDisplayer.gameObject, baitBoxContent);
            var iconDisplayer = icon.GetComponent<ItemIconDisplayer>();
            iconDisplayer.item = item;
            iconDisplayer.Init();
            _itemIconDisplayers.Add(iconDisplayer);
        }

        var itemList = InventoryManager.Instance.items.FindAll((item => item.item.itemType == ItemType.StoryItem))
            .OrderBy(item => item.item.name).ToList();
        foreach (var item in itemList)
        {
            var icon = Instantiate(itemIconDisplayer.gameObject, itemContent);
            var iconDisplayer = icon.GetComponent<ItemIconDisplayer>();
            iconDisplayer.item = item;
            iconDisplayer.Init();
            _itemIconDisplayers.Add(iconDisplayer);
        }
    }

    public void PrepareFishingRods()
    {
        _cacheFishingRodList = InventoryManager.Instance.fishingRods;
        var RodList = _cacheFishingRodList.ToList().OrderBy(item => 1 / item.fishingRodSO.rarity.OneIn);
        foreach (var rod in RodList)
        {
            var icon = Instantiate(fishingRodCellDisplayer.gameObject,fishingRodContent );
            var iconDisplayer = icon.GetComponent<FishingRodCellDisplayer>();
            var button = icon.GetComponent<Button>();
            iconDisplayer.item = rod;
            iconDisplayer.Init();
            button.onClick.AddListener(() =>SetFishingRodDetailView(iconDisplayer));
            _fishingRodCellDisplayers.Add(iconDisplayer);
        }
        for (int i = 0; i < _fishingRodCellDisplayers.Count; i++)
        {
            var icon = _fishingRodCellDisplayers[i].GetComponent<Button>();
            var nav = new Navigation
            {
                mode = Navigation.Mode.Explicit
            };
            var up = categoryButtons[0].GetComponent<Button>();
            var left = (i - 1 < 0) ? null : _fishingRodCellDisplayers[i - 1].GetComponent<Button>();
            var right = (i + 1 > _fishingRodCellDisplayers.Count - 1) ? null : _fishingRodCellDisplayers[i + 1].GetComponent<Button>();
            nav.selectOnUp = up;
            nav.selectOnDown = null;
            nav.selectOnLeft = left;
            nav.selectOnRight = right;
            if ((i + 1) % 6 == 0)
            {
                nav.selectOnRight = null;
            }
            else if ((i + 1) % 6 == 1)
            {
                nav.selectOnLeft = null;
            }
            icon.navigation = nav;
        }
        SetupFishingRodsNavigation();
    }
    public void SetupFishingRodsNavigation()
    {
        for (int i = 0; i < categoryButtons.Count; i++)
        {
            var nav = new Navigation { mode = Navigation.Mode.Explicit };
            nav.selectOnUp = _fishingRodCellDisplayers[^1].GetComponent<Button>();
            nav.selectOnDown = _fishingRodCellDisplayers[0].GetComponent<Button>();
            nav.selectOnLeft = (i-1<0) ? categoryButtons[^1].GetComponent<Button>() :  categoryButtons[i-1].GetComponent<Button>();
            nav.selectOnRight = (i+1>categoryButtons.Count-1) ? categoryButtons[0].GetComponent<Button>() :  categoryButtons[i+1].GetComponent<Button>();
            categoryButtons[i].GetComponent<Button>().navigation = nav;
        }
    }
    public void PrepareIcons()
    {
        //PrepareItems();
        CleanItems();
        PrepareFish();
        PrepareFishingRods();
        _eventSystem.SetSelectedGameObject(_fishIconDisplayers[0].gameObject, null);
    }

    public void CleanItems()
    {
        for (int i = _itemIconDisplayers.Count - 1; i >= 0; i--)
        {
            Destroy(_itemIconDisplayers[i].gameObject);
            _itemIconDisplayers.RemoveAt(i);
        }
        for (int i = _fishIconDisplayers.Count - 1; i >= 0; i--)
        {
            Destroy(_fishIconDisplayers[i].gameObject);
            _fishIconDisplayers.RemoveAt(i);
        }
        for (int i = _fishingRodCellDisplayers.Count - 1; i >= 0; i--)
        {
            Destroy(_fishingRodCellDisplayers[i].gameObject);
            _fishingRodCellDisplayers.RemoveAt(i);
        }
    }

    public void SetFishingRodDetailView(FishingRodCellDisplayer rod)
    {
        itemImage.gameObject.SetActive(false);
        fishingRodImage.gameObject.SetActive(true);
        if (rod == currentFishingRod)
        {
            currentFishingRod = null;
            detailContent.GetComponent<CanvasGroup>().DOFade(0,0.2f);
            rod.hover.SetActive(false);
            inventoryContext.DOLocalMoveX(0, 0.5f).SetEase(Ease.OutBack);
            detailContent.DOLocalMoveX(0, 0.5f).SetEase(Ease.OutBack);
        }
        else
        {
            if (currentFishingRod != null)
            {
                currentFishingRod.hover.SetActive(false);
            }
            detailContent.GetComponent<CanvasGroup>().DOKill();
            detailContent.GetComponent<CanvasGroup>().alpha = 0;
            detailContent.GetComponent<CanvasGroup>().DOFade(1,0.5f);
            rod.hover.SetActive(true);
            currentFishingRod = rod;
            var detailStartPos = detailContent.localPosition;
            detailStartPos.x = 0f;
            //inventoryContext.localPosition = inventoryStartPos;
            detailContent.DOKill();
            detailContent.localPosition = detailStartPos;
            inventoryContext.DOLocalMoveX(-351, 0.5f).SetEase(Ease.OutBack);
            detailContent.DOLocalMoveX(598, 0.5f).SetEase(Ease.OutBack);
            fishingRodImage.sprite = rod.item.fishingRodSO.spriteDisplay;
            fishingRodName.text = rod.item.fishingRodSO.name;
            var fishingRodSO = rod.item.fishingRodSO;
            var damage = fishingRodSO.damage;
            var handling = fishingRodSO.accuracy;
            var critChance = fishingRodSO.critChance;
            var critMultiplier = fishingRodSO.critMultiplier;
            var luck = fishingRodSO.luck;
            var resilience = fishingRodSO.resilience;
            var durability = rod.item.durability;
            var maxDurability = fishingRodSO.durability;
            rodDamageText.text = $"Damage: {damage}";
            rodDamageBar.localScale = new Vector3(damage/100, 1f, 1f);
            rodHandlingText.text = $"Accuracy: {handling}";
            rodHandlingBar.localScale = new Vector3(handling/100, 1f, 1f);
            rodCriticalChanceText.text = $"Critical Chance: {critChance}%";
            rodCriticalChanceBar.localScale = new Vector3(critChance/100, 1f, 1f);
            rodCriticalMultiplierText.text = $"Critical Multiplier: {critMultiplier}x";
            rodCriticalChanceBar.localScale = new Vector3(critMultiplier/100, 1f, 1f);
            rodLuckText.text = $"Luck: {luck}%";
            rodLuckBar.localScale = new Vector3(luck/100, 1f, 1f);
            rodResilienceText.text = $"Resilience: {resilience}";
            rodResilienceBar.localScale = new Vector3(resilience/100, 1f, 1f);
            rodDurabilityText.text = $"Durability: {durability}/{maxDurability}";
            rodDurabilityBar.localScale = new Vector3(durability/maxDurability, 1f, 1f);
            rodRarityText.text = fishingRodSO.rarity.name;
            rodRarityText.color = fishingRodSO.rarity.InventoryColor;
            tagImage.sprite = fishingRodSO.rarity.TagSprite;
        }
    }
    public void SetFishDetailView(FishIconDisplayer item)
    {
        itemImage.gameObject.SetActive(true);
        fishingRodImage.gameObject.SetActive(false);
        if (item == currentInspecting)
        {
            currentInspecting = null;
            detailContent.GetComponent<CanvasGroup>().DOFade(0,0.2f);
            item.transform.Find("Hover").gameObject.SetActive(false);
            inventoryContext.DOLocalMoveX(0, 0.5f).SetEase(Ease.OutBack);
            detailContent.DOLocalMoveX(0, 0.5f).SetEase(Ease.OutBack);
        }
        else
        {
            if (currentInspecting != null)
            {
                currentInspecting.transform.Find("Hover").gameObject.SetActive(false);
            }
            detailContent.GetComponent<CanvasGroup>().DOKill();
            detailContent.GetComponent<CanvasGroup>().alpha = 0;
            detailContent.GetComponent<CanvasGroup>().DOFade(1,0.5f);
            item.transform.Find("Hover").gameObject.SetActive(true);
            currentInspecting = item.gameObject;
            var detailStartPos = detailContent.localPosition;
            detailStartPos.x = 0f;
            //inventoryContext.localPosition = inventoryStartPos;
            detailContent.DOKill();
            detailContent.localPosition = detailStartPos;
            inventoryContext.DOLocalMoveX(-351, 0.5f).SetEase(Ease.OutBack);
            detailContent.DOLocalMoveX(598, 0.5f).SetEase(Ease.OutBack);
            itemImage.sprite = item.fish.fishType.Art;
            tagImage.sprite = item.fish.fishType.Tag;
            fishNameText.text = item.fish.fishType.name;
            weightText.text = $"Weight: {item.fish.weight}";
            weightBar.localScale = new Vector3(item.fish.weight/item.fish.fishType.MaxWeight, 1f, 1f);
            rarityText.text = item.fish.fishType.Rarity.name;
            rarityText.color = item.fish.fishType.Rarity.InventoryColor;
            mutationText.text = item.fish.mutation.name;
        }
    }

    public void CloseDetailView()
    {
        detailContent.GetComponent<CanvasGroup>().DOFade(0,0.2f);
        if (currentInspecting != null)
        {
            currentInspecting?.transform.Find("Hover").gameObject.SetActive(false);
        }
        if (currentFishingRod != null)
        {
            currentFishingRod?.hover.SetActive(false);
        }
        inventoryContext.DOLocalMoveX(0, 0.5f).SetEase(Ease.OutBack);
        detailContent.DOLocalMoveX(0, 0.5f).SetEase(Ease.OutBack);
    }

    public void SetupMoney()
    {
        moneyText.text = $"Gold: {GameManager.Instance.player.gold}";
    }
    public void ShowInventory()
    {
        InventoryBody.gameObject.SetActive(true);
    }

    public void HideIventory()
    {
        _eventSystem.SetSelectedGameObject(null);
        InventoryBody.gameObject.SetActive(false);
    }

    public void ToggleInventory(InputAction.CallbackContext ctx)
    {
        Debug.Log("open");
        ViewManager.instance.OpenView(this);
    }
    public void Begin()
    {
        ShowInventory();
        PrepareIcons();
        SetupMoney();
    }

    public void End()
    {
        HideIventory();
        //CleanItems();
    }
}