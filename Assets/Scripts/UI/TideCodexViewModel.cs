using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = System.Random;

public class TideCodexViewModel : MonoBehaviour, IViewFrame
{
    [Header("Setup")] public Player player;
    public RectTransform mainFrame;
    public RectTransform codex;
    [Header("Flip")] public RectTransform flipLeft;
    public RectTransform flipRight;
    public List<Sprite> flipSprites = new List<Sprite>();
    [FormerlySerializedAs("currentCatagory")] [Header("Catagory")]
    public int currentCategory = 0;
    [FormerlySerializedAs("catagoryButton")] public List<Image> categoryButton = new List<Image>();
    [FormerlySerializedAs("selectedCatagory")] public Sprite selectedCategory;
    [FormerlySerializedAs("normalCatagory")] public Sprite normalCategory;
    public List<TideCodexCategory> categories = new List<TideCodexCategory>();
    public List<GameObject> tabContainers = new List<GameObject>();
    public List<GameObject[]> tabs = new List<GameObject[]>();
    [Header("SFX")] public AudioClip pageTurn;
    public AudioClip openSound;
    public AudioClip closeSound;

    [FormerlySerializedAs("cardController")]
    public FishCardHandler cardHandler;

    public DofController dofController;

    private bool _flipDebounce;
    private bool _changeCategoryDebounce;
    private Player _player;
    private EventSystem _eventSystem;
    private int _prevCatagory = 0;
    private void Start()
    {
        _eventSystem = EventSystem.current;
        dofController = FindAnyObjectByType<DofController>();
        _player = GameManager.Instance.player;
    }

    public async UniTask TurnEffect(bool isLeft)
    {
        var display = (isLeft) ? flipLeft.GetComponent<Image>() : flipRight.GetComponent<Image>();
        display.gameObject.SetActive(true);
        SoundFXManger.Instance.PlaySoundFXClip(pageTurn, player.transform, 1f);
        foreach (var sprite in flipSprites)
        {
            display.sprite = sprite;
            await UniTask.WaitForSeconds(0.04f);
        }

        display.sprite = null;
        display.gameObject.SetActive(false);
        _flipDebounce = false;
    }
    public void TurnPage(bool isLeft)
    {
        if (_flipDebounce) return;
        _flipDebounce = true;
        TurnEffect(isLeft);
    }

    public void OpenUI()
    {
        mainFrame.gameObject.SetActive(true);
        SoundFXManger.Instance.PlaySoundFXClip(openSound, player.characterTransform, 1f);
        codex.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        dofController.SetFocusDistance(0.1f);
    }

    public async UniTask CloseUI()
    {
        FishCardHandler.instance.CloseCard();
        SoundFXManger.Instance.PlaySoundFXClip(closeSound, player.characterTransform, 1f);
        codex.DOScale(Vector3.zero, 0.15f).SetEase(Ease.OutQuint);
        dofController.SetFocusDistance(2f);
        await UniTask.WaitForSeconds(0.15f);
        mainFrame.gameObject.SetActive(false);
    }
    private void SwitchCatagory(bool isLeft)
    {
        currentCategory = isLeft ? (currentCategory+1)>categoryButton.Count-1 ? 0 : currentCategory + 1 : 
            (currentCategory-1)< 0 ? categoryButton.Count-1 : currentCategory - 1 ;
        //ChangeCatagory(currentCategory);
        _eventSystem.SetSelectedGameObject(categoryButton[currentCategory].gameObject);
        for (var i = 0; i < categories.Count; i++)
        {
            var select = i == currentCategory;
            categoryButton[i].sprite = (select)? selectedCategory: normalCategory;
        }
    }
    

    private void SwithLeft(InputAction.CallbackContext ctx) => SwitchCatagory(true);
    private void SwithRight(InputAction.CallbackContext ctx) => SwitchCatagory(false);
    private async UniTask ChangeCategoryEffect(int index)
    {
        if (_changeCategoryDebounce) return;
        _changeCategoryDebounce = true;
        try
        {
            for (var i = 0; i < categories.Count; i++)
            {
                var select = i == index;
                categories[i].gameObject.SetActive(false);
                tabContainers[i].SetActive(select);
                categoryButton[i].sprite = (select)? selectedCategory: normalCategory;
            }
            await UniTask.Delay(50);
            await TurnEffect(index%2 == 0);
            await UniTask.Delay(150);
            for (var i = 0; i < categoryButton.Count; i++)
            {
                var select = i == index;
                categories[i].gameObject.SetActive(select);
                tabContainers[i].SetActive(select);
            }
            foreach (var x in categoryButton)
            {
                var navigation = x.GetComponent<Button>().navigation;
                navigation.selectOnDown = categories[index].tabButtons[0].GetComponent<Button>();
                x.GetComponent<Button>().navigation = navigation;
            }
        }
        finally
        {
            await UniTask.Delay(100);
            _changeCategoryDebounce = false;
        }
    }

    public void ChangeCatagory(int index)
    {
        _ = ChangeCategoryEffect(index);
    }

    private void SetupInput()
    {
        _player.PlayerInputs.UI.LeftSwitch.performed += SwithLeft;
        _player.PlayerInputs.UI.RightSwitch.performed += SwithRight;
    }
    private void CleanupInput()
    {
        _player.PlayerInputs.UI.LeftSwitch.performed -= SwithLeft;
        _player.PlayerInputs.UI.RightSwitch.performed -= SwithRight;
    }
    public void Begin()
    {
        OpenUI();
        SetupInput();
    }

    public void End()
    {
        CloseUI();
        CleanupInput();
    }

    public void Update()
    {
        if (!_changeCategoryDebounce && _prevCatagory != currentCategory)
        {
            _prevCatagory = currentCategory;
            ChangeCatagory(currentCategory);
        };
    }
}