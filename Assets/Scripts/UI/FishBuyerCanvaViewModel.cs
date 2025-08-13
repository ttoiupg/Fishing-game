using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FishBuyerCanvaViewModel : MonoBehaviour,IViewFrame
{
    [SerializeField] private List<FishIconDisplayer> _fishIconDisplayers = new List<FishIconDisplayer>();
    public List<FishIconDisplayer> selectedIconDisplayer = new List<FishIconDisplayer>();
    public FishIconDisplayer fishIconDisplayer;
    public RectTransform fishContent;
    public GameObject inventoryBody;
    public Button sellButton;
    public Button closeButton;
    private EventSystem _eventSystem;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _eventSystem = EventSystem.current;
    }
    public void SelectIcon(FishIconDisplayer icon)
    {
        var index = selectedIconDisplayer.IndexOf(icon);
        if (index == -1)
        {
            icon.hover.gameObject.SetActive(true);
            selectedIconDisplayer.Add(icon);
        }
        else
        {
            icon.hover.gameObject.SetActive(false);
            selectedIconDisplayer.RemoveAt(index);
        };
    }
    public void PrepareItems()
    {
        CleanItems();
        var FishList = InventoryManager.Instance.fishes.ToList().OrderBy(item => 1 / item.fishType.Rarity.OneIn);
        foreach (var fish in FishList)
        {
            var icon = Instantiate(fishIconDisplayer.gameObject, fishContent);
            var iconDisplayer = icon.GetComponent<FishIconDisplayer>();
            var button = icon.GetComponent<Button>();
            iconDisplayer.fish = fish;
            iconDisplayer.Init();
            button.onClick.AddListener(()=>SelectIcon(iconDisplayer));
            _fishIconDisplayers.Add(iconDisplayer);
        }
        for (int i = 0; i < _fishIconDisplayers.Count; i++)
        {
            var icon = _fishIconDisplayers[i].GetComponent<Button>();
            var nav = new Navigation
            {
                mode = Navigation.Mode.Explicit
            };
            var up = (i - 6 < 0) ? sellButton : _fishIconDisplayers[i - 6].GetComponent<Button>();
            var down = (i + 6 > _fishIconDisplayers.Count - 1) ? sellButton : _fishIconDisplayers[i + 6].GetComponent<Button>();
            var left = (i - 1 < 0) ? sellButton : _fishIconDisplayers[i - 1].GetComponent<Button>();
            var right = (i + 1 > _fishIconDisplayers.Count - 1) ? sellButton : _fishIconDisplayers[i + 1].GetComponent<Button>();
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

        var closeNav = new Navigation { mode = Navigation.Mode.Explicit };
        var sellNav = new Navigation { mode = Navigation.Mode.Explicit };
        closeNav.selectOnUp = _fishIconDisplayers[^1].GetComponent<Button>();
        closeNav.selectOnDown = _fishIconDisplayers[0].GetComponent<Button>();
        closeNav.selectOnLeft = sellButton;
        closeNav.selectOnRight = sellButton;
        sellNav.selectOnUp = _fishIconDisplayers[^1].GetComponent<Button>();
        sellNav.selectOnDown = _fishIconDisplayers[0].GetComponent<Button>();
        sellNav.selectOnLeft = closeButton;
        sellNav.selectOnRight = closeButton;
        closeButton.navigation = closeNav;
        sellButton.navigation = sellNav;
        Debug.Log("Order finished");
        _eventSystem.SetSelectedGameObject(_fishIconDisplayers[0].gameObject, null);
    }
    public void ShowInventory()
    {
        inventoryBody.gameObject.SetActive(true);
    }

    public void HideInventory()
    {
        _eventSystem.SetSelectedGameObject(null);
        inventoryBody.gameObject.SetActive(false);
    }
    public void CleanItems()
    {
        for (int i = _fishIconDisplayers.Count - 1; i >= 0; i--)
        {
            Destroy(_fishIconDisplayers[i].gameObject);
            _fishIconDisplayers.RemoveAt(i);
        }
        for (int i = selectedIconDisplayer.Count - 1; i >= 0; i--)
        {
            selectedIconDisplayer.RemoveAt(i);
        }
    }
    public void CloseFishBuyer()
    {
        ViewManager.instance.CloseView();
    }
    public void Sell()
    {
        foreach(var icon in selectedIconDisplayer)
        {
            GameManager.Instance.SellFish(icon.fish);
            var inventoryIndex = InventoryManager.Instance.fishes.IndexOf(icon.fish);
            InventoryManager.Instance.fishes.RemoveAt(inventoryIndex);
        }
        CleanItems();
        PrepareItems();
    }
    public void Begin()
    {
        PauseViewModel.Instance.PauseLock = true;
        GameManager.Instance.player.isActive = false;
        ShowInventory();
        PrepareItems();
    }
    public void End()
    {
        PauseViewModel.Instance.PauseLock = false;
        GameManager.Instance.player.isActive = true;
        HideInventory();
    }
}
