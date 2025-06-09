using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryDisplayManager : MonoBehaviour
{
    public RectTransform InventoryDisplay;
    public ItemIconDisplayer itemIconDisplayer;
    public FishIconDisplayer fishIconDisplayer;
    
    [Header("contents")]
    public RectTransform baitBoxContent;
    public RectTransform itemContent;
    public RectTransform fishContent;
    [SerializeField]private List<ItemIconDisplayer> _itemIconDisplayers = new List<ItemIconDisplayer>();
    [SerializeField]private List<FishIconDisplayer> _fishIconDisplayers = new List<FishIconDisplayer>();
    
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
        
        var FishList = InventoryManager.Instance.fishes.ToList();
        foreach (var fish in FishList)
        {
            var icon = Instantiate(fishIconDisplayer.gameObject, fishContent);
            var iconDisplayer = icon.GetComponent<FishIconDisplayer>();
            iconDisplayer.fish = fish;
            iconDisplayer.Init();
            _fishIconDisplayers.Add(iconDisplayer);
        }
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

    public void ShowInventory()
    {
        InventoryDisplay.localScale = Vector3.one;
    }

    public void HideIventory()
    {
        InventoryDisplay.localScale = Vector3.zero;
    }
    
    public void OpenInventory()
    {
        ShowInventory();
        PrepareItems();
    }

    public void CloseInventory()
    {
        HideIventory();
        CleanItems();
    }
}