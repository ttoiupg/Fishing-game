using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class TeleportScreenViewModel : MonoBehaviour,IViewFrame
{
    public CanvasGroup canvasGroup;
    public RectTransform cover;
    [FormerlySerializedAs("Image")] public GameObject Container;
    public RectTransform picture;
    public TextMeshProUGUI targetText;
    public TextMeshProUGUI tipText;

    private void Start()
    {
        TeleportManager.Instance.TeleportStarted.AddListener(SetupTeleport);
    }

    public void SetupTeleport(string targetSceneName)
    {
        targetText.text = targetSceneName;
    }
    public void Begin()
    {
        ViewManager.instance.frameLock = true;
        picture.localPosition = Vector3.left * 90;
        var fade = canvasGroup.DOFade(1, 0.25f);
        fade.onComplete += () => { canvasGroup.blocksRaycasts = true; Container.SetActive(true);};
        var sequence = DOTween.Sequence();
        sequence.Append(fade);
        sequence.Append(cover.DOLocalMoveY(1700, 0.5f));
        sequence.Join(picture.DOLocalMoveX(90, 3f).SetEase(Ease.Linear));
        sequence.Play();
        sequence.onComplete += () => {ViewManager.instance.CloseView();};
    }

    public void End()
    {
        var fade = canvasGroup.DOFade(0, 0.25f);
        fade.onComplete += () => { canvasGroup.blocksRaycasts = false;};
        var move = cover.DOLocalMoveY(0, 0.5f);
        move.onComplete += () => { Container.SetActive(false); };
        var sequence = DOTween.Sequence();
        sequence.Append(move);
        sequence.Append(fade);
        sequence.Play();
    }
}