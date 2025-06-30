using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TideCodexViewModel : MonoBehaviour, IViewFrame
{
    [Header("Setup")]
    public Player player;
    public RectTransform mainFrame;
    public RectTransform codex;
    [Header("Flip")]
    public RectTransform flipLeft;
    public RectTransform flipRight;
    public List<Sprite> flipSprites = new List<Sprite>();
    [Header("SFX")]
    public AudioClip pageTurn;
    public AudioClip openSound;
    public AudioClip closeSound;
    [FormerlySerializedAs("cardController")] public FishCardHandler cardHandler;
    public DofController dofController;

    private bool _flipDebounce;
    private void Start()
    {
        dofController = FindAnyObjectByType<DofController>();
    }
    
    private async UniTask TurnEffect(bool isLeft)
    {
        var display = (isLeft)? flipLeft.GetComponent<Image>() : flipRight.GetComponent<Image>();
        display.gameObject.SetActive(true);
        SoundFXManger.Instance.PlaySoundFXClip(pageTurn,player.transform, 1f);
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

    public void Begin()
    {
        OpenUI();
    }

    public void End()
    {
        CloseUI();
    }
}