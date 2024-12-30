using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuOptionHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform menuOption;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnPointerEnter(PointerEventData data)
    {
        menuOption.DOScale(new Vector3(1.1f,1.1f,1.1f), 0.1f).SetEase(Ease.OutBack);
    }
    public void OnPointerExit(PointerEventData data)
    {
        menuOption.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutBack);
    }
}
