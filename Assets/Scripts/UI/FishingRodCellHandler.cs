using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FishingRodCellHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public FishingRodSO fishingRodSO;
    public RectTransform cell;
    public RectTransform wave;
    public RectTransform nameLabel;
    public RectTransform container;
    public TextMeshProUGUI damageLabel;
    public TextMeshProUGUI resilienceLabel;
    public TextMeshProUGUI luckLabel;
    public Image cellImage;
    public Image fishingRodImage;
    public bool active = false;
    [Space]
    public Color normalColor;
    public Color hoverColor;
    public bool selected = false;
    [Space]
    [SerializeField] private float SelectedHeight = -170;
    [SerializeField] private float DeselectHeight = -480;
    [SerializeField] private float ScrollStart = 134.81f;
    [SerializeField] private float ScrollEnd = -141.81f;
    public void OnPointerEnter(PointerEventData data)
    {
        if (!active) return;
        Select();
        cellImage.color = hoverColor;
    }
    public void OnPointerExit(PointerEventData data)
    {
        if (!active) return;
        Deselect();
        cellImage.color = normalColor;
    }
    public void Init()
    {
        nameLabel.GetComponent<TextMeshProUGUI>().text = fishingRodSO.name;
        fishingRodImage.sprite = fishingRodSO.spriteDisplay;
        cell.localRotation = Quaternion.Euler(new Vector3(0,0,70));
        cell.DOLocalRotate(new Vector3(0, 0, 0), 0.35f).SetEase(Ease.OutBack);
    }
    public void Select()
    {
        if (selected) return;
        wave.anchoredPosition = new Vector2(ScrollStart,wave.anchoredPosition.y);
        wave.DOKill();
        selected = true;
        wave.DOAnchorPosY(SelectedHeight, 1f).SetEase(Ease.OutBack);
        wave.DOAnchorPosX(ScrollEnd, 1.3f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
        nameLabel.DOAnchorPosY(35.38f, 1f).SetEase(Ease.OutBack);
        container.DOAnchorPosY(35.38f, 1f).SetEase(Ease.OutBack);
    }
    public void Deselect()
    {
        selected = false;
        wave.DOKill();
        wave.DOAnchorPosY(DeselectHeight, 1f).SetEase(Ease.OutBack);
        nameLabel.DOAnchorPosY(-254.3f,1f).SetEase(Ease.OutBack);
        container.DOAnchorPosY(-254.3f, 1f).SetEase(Ease.OutBack);
    }
}
