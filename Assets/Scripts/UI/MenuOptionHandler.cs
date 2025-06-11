using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuOptionHandler : MonoBehaviour,ISelectHandler,IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform menuOption;

    private Button _button;

    private void Start()
    {
        _button = GetComponent<Button>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnPointerEnter(PointerEventData data)
    {
        if (!_button.interactable) return;
        menuOption.DOScale(new Vector3(1.1f,1.1f,1.1f), 0.1f).SetEase(Ease.OutBack);
    }
    public void OnPointerExit(PointerEventData data)
    {
        if (!_button.interactable) return;
        menuOption.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutBack);
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (!_button.interactable) return;
        menuOption.DOScale(new Vector3(1.1f,1.1f,1.1f), 0.1f).SetEase(Ease.OutBack);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (!_button.interactable) return;
        menuOption.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutBack);
    }
}
