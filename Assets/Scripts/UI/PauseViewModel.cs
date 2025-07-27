using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PauseViewModel : MonoBehaviour, IViewFrame
{
    public static PauseViewModel Instance { get; private set; }
    
    public GameObject menuContainer;
    public AudioClip OpenSound;
    public AudioClip CloseSound;
    public AudioClip FrameOpenSound;
    public AudioClip FrameCloseSound;
    public RectTransform currentPage;
    public RectTransform background;
    public RectTransform resumeButton;
    public RectTransform fishipediaButton;
    public RectTransform settingButton;
    public RectTransform quitButton;
    public DofController dofController;
    public bool isPageOpen = false;
    public bool isDpadCurrentInputing = false;
    public bool MenuDebounce = false;
    public bool PauseLock = false;
    private Player _player;
    private EventSystem _eventSystem;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        _eventSystem = EventSystem.current;
    }

    void Start()
    {
        _player = FindAnyObjectByType<Player>();
        dofController = FindAnyObjectByType<DofController>();
    }

    public void Trigger(InputAction.CallbackContext context)
    {
        if (MenuDebounce || PauseLock) return;
        MenuDebounce = true;
        ViewManager.instance.OpenView(this);
        HandleDebounce();
    }
    private async UniTask HandleDebounce()
    {
        await UniTask.WaitForSeconds(0.25f);
        MenuDebounce = false;
    }
    public void MenuOpenAnimation()
    {
        background.gameObject.SetActive(true);
        resumeButton.GetComponent<Button>().interactable = true;
        fishipediaButton.GetComponent<Button>().interactable = true;
        settingButton.GetComponent<Button>().interactable = true;
        quitButton.GetComponent<Button>().interactable = true;
        SoundFXManger.Instance.PlaySoundFXClip(OpenSound, _player.transform, 0.5f);
        resumeButton.GetComponent<Image>().raycastTarget = true;
        fishipediaButton.GetComponent<Image>().raycastTarget = true;
        settingButton.GetComponent<Image>().raycastTarget = true;
        quitButton.GetComponent<Image>().raycastTarget = true;
        resumeButton.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
        fishipediaButton.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack).SetDelay(0.06f);
        settingButton.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack).SetDelay(0.12f);
        quitButton.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack).SetDelay(0.18f);
        dofController.SetFocusDistance(0f);
    }
    public void MenuCloseAnimation()
    {
        background.gameObject.SetActive(false);
        _eventSystem.SetSelectedGameObject(null);
        SoundFXManger.Instance.PlaySoundFXClip(CloseSound, _player.transform, 0.5f);
        resumeButton.GetComponent<Image>().raycastTarget = false;
        fishipediaButton.GetComponent<Image>().raycastTarget = false;
        settingButton.GetComponent<Image>().raycastTarget = false;
        quitButton.GetComponent<Image>().raycastTarget = false;
        resumeButton.DOScale(Vector3.zero, 0.15f).SetEase(Ease.OutQuint);
        fishipediaButton.DOScale(Vector3.zero, 0.15f).SetEase(Ease.OutQuint).SetDelay(0.06f);
        settingButton.DOScale(Vector3.zero, 0.15f).SetEase(Ease.OutQuint).SetDelay(0.12f);
        quitButton.DOScale(Vector3.zero, 0.15f).SetEase(Ease.OutQuint).SetDelay(0.18f);
        resumeButton.GetComponent<Button>().interactable = false;
        fishipediaButton.GetComponent<Button>().interactable = false;
        settingButton.GetComponent<Button>().interactable = false;
        quitButton.GetComponent<Button>().interactable = false;
        dofController.SetFocusDistance(100f);
    }
    public void Begin()
    {
        MenuOpenAnimation();
        _eventSystem.SetSelectedGameObject(resumeButton.gameObject);
    }
    public void End()
    {
        MenuCloseAnimation();
    }
}