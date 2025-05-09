using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class FishingRodCellHandler : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public FishingRod fishingRod;
    public RectTransform cell;
    public RectTransform wave;
    public RectTransform nameLabel;
    public RectTransform container;
    public TextMeshProUGUI damageLabel;
    public TextMeshProUGUI resilienceLabel;
    public TextMeshProUGUI luckLabel;
    public TextMeshProUGUI timeUsedLabel;
    public TextMeshProUGUI durabilityLabel;
    public TextMeshProUGUI aquireDateLabel;
    public Image cellImage;
    public Image fishingRodImage;
    public bool active = false;
    [Space] public Color normalColor;
    public Color hoverColor;
    public bool selected = false;
    [FormerlySerializedAs("SelectedHeight")] [Space] [SerializeField] private float selectedHeight = -170;
    [FormerlySerializedAs("DeselectHeight")] [SerializeField] private float deselectHeight = -480;
    [FormerlySerializedAs("ScrollStart")] [SerializeField] private float scrollStart = 111.7f;
    [FormerlySerializedAs("ScrollEnd")] [SerializeField] private float scrollEnd = -141.81f;
    private Player player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    public void OnPointerEnter(PointerEventData data)
    {
        if (!active) return;
        DoHoverEffect();
    }

    public void OnPointerExit(PointerEventData data)
    {
        if (!active) return;
        DisableHoverEffect();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!active || !selected) return;
        DoEquipEffect();
        player.EquipFishingRod(fishingRod);
    }
    public void Init()
    {
        nameLabel.GetComponent<TextMeshProUGUI>().text = fishingRod.fishingRodSO.name;
        fishingRodImage.sprite = fishingRod.fishingRodSO.spriteDisplay;
        cell.localRotation = Quaternion.Euler(new Vector3(0, 0, 70));
        cell.DOLocalRotate(new Vector3(0, 0, 0), 0.35f).SetEase(Ease.OutBack);
        damageLabel.text = "Damage: " + fishingRod.fishingRodSO.damage;
        resilienceLabel.text = "Resilience: " + fishingRod.fishingRodSO.damage;
        luckLabel.text = "Luck: " + fishingRod.fishingRodSO.damage;
        timeUsedLabel.text = "Fish caught: " + fishingRod.fishCaught;
        durabilityLabel.text = "Durability: " + fishingRod.durability;
        aquireDateLabel.text = "Aquire Date: " + fishingRod.aquireDate;
    }

    private async UniTask DoEquipEffect()
    {
        cellImage.color = normalColor;
        await UniTask.WaitForSeconds(0.15f);
        cellImage.color = hoverColor;
        await UniTask.WaitForSeconds(0.15f);
        cellImage.color = normalColor;
        await UniTask.WaitForSeconds(0.15f);
        cellImage.color = hoverColor;
    }
    private void DoHoverEffect()
    {
        if (selected) return;
        wave.anchoredPosition = new Vector2(scrollStart, wave.anchoredPosition.y);
        wave.DOKill();
        cellImage.color = hoverColor;
        selected = true;
        wave.DOAnchorPosY(selectedHeight, 1f).SetEase(Ease.OutBack);
        wave.DOAnchorPosX(scrollEnd, 1.3f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
        nameLabel.DOAnchorPosY(35.38f, 1f).SetEase(Ease.OutBack);
        container.DOAnchorPosY(35.38f, 1f).SetEase(Ease.OutBack);
    }

    private void DisableHoverEffect()
    {
        selected = false;
        cellImage.color = normalColor;
        wave.DOKill();
        wave.DOAnchorPosY(deselectHeight, 1f).SetEase(Ease.OutBack);
        nameLabel.DOAnchorPosY(-254.3f, 1f).SetEase(Ease.OutBack);
        container.DOAnchorPosY(-254.3f, 1f).SetEase(Ease.OutBack);
    }
}