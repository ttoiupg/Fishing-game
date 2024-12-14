using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class FishipediaLabelHandler : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler,IPointerDownHandler,IPointerUpHandler
{
    private RectTransform rectTransform;
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    public void ToSetPosition(float value)
    {
        rectTransform.anchoredPosition = new Vector2(value, rectTransform.anchoredPosition.y);
    }
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        rectTransform.DOAnchorPosX(-76.6f, 0.2f).SetEase(Ease.OutBack).OnComplete(() => ToSetPosition(-76.6f));
    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        rectTransform.DOAnchorPosX(-51f, 0.2f).OnComplete(() => ToSetPosition(-51f));
    }
    public void OnPointerDown(PointerEventData pointerEventData)
    {
        rectTransform.DOAnchorPosX(-16.94f, 0.2f).SetEase(Ease.OutBack).OnComplete(() => ToSetPosition(-16.94f));
    }
    public void OnPointerUp(PointerEventData pointerEventData)
    {
        rectTransform.DOAnchorPosX(-51f, 0.2f).SetEase(Ease.OutBack).OnComplete(() => ToSetPosition(-51f));
    }
}
