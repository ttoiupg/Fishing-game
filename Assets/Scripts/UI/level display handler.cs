using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class leveldisplayhandler : MonoBehaviour
{
    public RectTransform needle;
    public RectTransform paper;
    public RectTransform closeButton;
    public RectTransform levelContainer;
    public TextMeshProUGUI levelText;
    public Image fill;

    private bool detailOpened = false;

    public void Trigger()
    {
        if (detailOpened)
        {
            closeDetail();
            detailOpened = false;
        }
        else
        {
            openDetail();
            detailOpened = true;
        }
    }
    public void openDetail()
    {
        closeButton.rotation = Quaternion.Euler(0, 0, 0);
        closeButton.DOLocalMoveX(71.1f, 0.4f).SetEase(Ease.OutBack);
        levelContainer.DOLocalMoveX(174.4f, 0.4f).SetEase(Ease.OutBack);
        paper.DOScaleX(1, 0.4f).SetEase(Ease.OutBack);
    }
    public void closeDetail()
    {
        closeButton.rotation = Quaternion.Euler(0, 0, 180);
        closeButton.DOLocalMoveX(-129.4f, 0.4f).SetEase(Ease.OutBack);
        levelContainer.DOLocalMoveX(-79f, 0.4f).SetEase(Ease.OutBack);
        paper.DOScaleX(0, 0.4f).SetEase(Ease.OutBack);
    }
}
