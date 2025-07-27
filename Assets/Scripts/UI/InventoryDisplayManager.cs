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
    public GameObject closeButton;
    public RectTransform inventoryContext;
    [Header("contents")]
    public RectTransform baitBoxContent;
    public RectTransform itemContent;
    public RectTransform fishContent;
    [SerializeField]private List<ItemIconDisplayer> _itemIconDisplayers = new List<ItemIconDisplayer>();
    [SerializeField]private List<FishIconDisplayer> _fishIconDisplayers = new List<FishIconDisplayer>();
    [Header("Detail")] 
    public RectTransform detailContent;
    public Image itemImage;
    public Image tagImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI detailText;
    [Space]
    public GameObject currentInspecting;
    private EventSystem _eventSystem;

    private List<Fish> _cacheFishList = new();
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

        if (!_cacheFishList.Equals(InventoryManager.Instance.fishes))
        {
            Debug.Log("different clear cached");
            CleanItems();
            _cacheFishList = InventoryManager.Instance.fishes;
            var FishList = _cacheFishList.ToList().OrderBy(item => 1/item.fishType.Rarity.OneIn);
        
            foreach (var fish in FishList)
            {
                var icon = Instantiate(fishIconDisplayer.gameObject, fishContent);
                var iconDisplayer = icon.GetComponent<FishIconDisplayer>();
                var button = icon.GetComponent<Button>();
                iconDisplayer.fish = fish;
                iconDisplayer.Init();
                button.onClick.AddListener(() => SetDetailView(icon.gameObject,fish.fishType.Art,fish.fishType.Tag,
                    fish.fishType.name,
                    $"Weight:{fish.weight}kg\nRarity:{fish.fishType.Rarity.name}\nMutation:{fish.mutation.name}"));
                _fishIconDisplayers.Add(iconDisplayer);
            }
            Debug.Log("Order");
            for (int i = 0; i < _fishIconDisplayers.Count; i++)
            {
                var icon = _fishIconDisplayers[i].GetComponent<Button>();
                var nav = new Navigation
                {
                    mode = Navigation.Mode.Explicit
                };
                var up = (i - 6 < 0) ? null : _fishIconDisplayers[i - 6].GetComponent<Button>();
                var down = (i + 6 > _fishIconDisplayers.Count - 1) ? null : _fishIconDisplayers[i + 6].GetComponent<Button>();
                var left = (i - 1 < 0) ? null : _fishIconDisplayers[i - 1].GetComponent<Button>();
                var right = (i + 1 > _fishIconDisplayers.Count - 1) ? null : _fishIconDisplayers[i + 1].GetComponent<Button>();
                nav.selectOnUp = up;
                nav.selectOnDown = down;
                nav.selectOnLeft = left;
                nav.selectOnRight = right;
                if ((i+1) % 6== 0)
                {
                    nav.selectOnRight = null;
                }else if ((i + 1) % 6 == 1)
                {
                    nav.selectOnLeft = null;
                }
                icon.navigation = nav;
            }
            Debug.Log("Order finished");
        }
        else
        {
            Debug.Log("Same");
        }
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
    }

    public void SetDetailView(GameObject item,Sprite sprite,Sprite tag,string Name, string detail)
    {
        if (item == currentInspecting)
        {
            detailContent.GetComponent<CanvasGroup>().DOFade(0,0.5f);
            item.transform.Find("Hover").gameObject.SetActive(false);
            inventoryContext.DOLocalMoveX(0, 0.5f).SetEase(Ease.OutBack);
            detailContent.DOLocalMoveX(0, 0.5f).SetEase(Ease.OutBack);
        }
        else
        {
            detailContent.GetComponent<CanvasGroup>().DOFade(1,0.5f);
            item.transform.Find("Hover").gameObject.SetActive(false);
            currentInspecting = item;
            inventoryContext.DOLocalMoveX(-351, 0.5f).SetEase(Ease.OutBack);
            detailContent.DOLocalMoveX(598, 0.5f).SetEase(Ease.OutBack);
            itemImage.sprite = sprite;
            tagImage.sprite = tag;
            nameText.text = Name;
            detailText.text = detail;
        }
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
        PrepareItems();
    }

    public void End()
    {
        HideIventory();
        //CleanItems();
    }
}