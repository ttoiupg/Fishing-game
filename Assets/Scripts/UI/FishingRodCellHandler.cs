using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FishingRodCellHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform cell;
    public RectTransform wave;
    public RectTransform nameLabel;
    public Image cellImage;
    [Space]
    public Color normalColor;
    public Color hoverColor;
    public bool selected = false;
    [Space]
    [SerializeField] private float _selectedHeight = -170;
    [SerializeField] private float _deselectHeight = -480;
    [SerializeField] private float _scrollStart = 141.81f;
    [SerializeField] private float _scrollEnd = -141.81f;
    public void OnPointerEnter(PointerEventData data)
    {
        Select();
        cellImage.color = hoverColor;
    }
    public void OnPointerExit(PointerEventData data)
    {
        Deselect();
        cellImage.color = normalColor;
    }

    public void Select()
    {
        if (selected) return;
        wave.anchoredPosition = new Vector2(_scrollStart,wave.anchoredPosition.y);
        wave.DOKill();
        selected = true;
        wave.DOAnchorPosY(_selectedHeight, 1f).SetEase(Ease.OutBack);
        wave.DOAnchorPosX(_scrollEnd, 1.3f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
        nameLabel.DOAnchorPosY(35.38f, 1f).SetEase(Ease.OutBack);
    }
    public void Deselect()
    {
        selected = false;
        wave.DOKill();
        wave.DOAnchorPosY(_deselectHeight, 1f).SetEase(Ease.OutBack);
        nameLabel.DOAnchorPosY(-254.3f,1f).SetEase(Ease.OutBack);
    }
}
