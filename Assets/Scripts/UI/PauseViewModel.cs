using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PauseViewModel : MonoBehaviour, IViewFrame
{
    public GameObject menuContainer;
    public AudioClip OpenSound;
    public AudioClip CloseSound;
    public AudioClip FrameOpenSound;
    public AudioClip FrameCloseSound;
    public RectTransform currentPage;
    public RectTransform resumeButton;
    public RectTransform fishipediaButton;
    public RectTransform settingButton;
    public RectTransform quitButton;
    public Volume globalVolume;
    public bool isPageOpen = false;
    public bool isDpadCurrentInputing = false;
    public bool MenuDebounce = false;
    private Player _player;
    private EventSystem _eventSystem;
    void Start()
    {
        _player = FindAnyObjectByType<Player>();
    }

    public void MenuOpenAnimation()
    {
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
        fishipediaButton.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack).SetDelay(0.1f);
        settingButton.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack).SetDelay(0.2f);
        quitButton.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack).SetDelay(0.3f);
        globalVolume.weight = 0;
    }
    public void MenuCloseAnimation()
    {
        _eventSystem.SetSelectedGameObject(null);
        SoundFXManger.Instance.PlaySoundFXClip(CloseSound, _player.transform, 0.5f);
        resumeButton.GetComponent<Image>().raycastTarget = false;
        fishipediaButton.GetComponent<Image>().raycastTarget = false;
        settingButton.GetComponent<Image>().raycastTarget = false;
        quitButton.GetComponent<Image>().raycastTarget = false;
        resumeButton.DOScale(Vector3.zero, 0.15f).SetEase(Ease.OutQuint);
        fishipediaButton.DOScale(Vector3.zero, 0.15f).SetEase(Ease.OutQuint).SetDelay(0.1f);
        settingButton.DOScale(Vector3.zero, 0.15f).SetEase(Ease.OutQuint).SetDelay(0.2f);
        quitButton.DOScale(Vector3.zero, 0.15f).SetEase(Ease.OutQuint).SetDelay(0.3f);
        resumeButton.GetComponent<Button>().interactable = false;
        fishipediaButton.GetComponent<Button>().interactable = false;
        settingButton.GetComponent<Button>().interactable = false;
        quitButton.GetComponent<Button>().interactable = false;
        globalVolume.weight = 1;
    }
    public void begin()
    {

    }
    public void End()
    {

    }
}