using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class FishingRodCellDisplayer : MonoBehaviour
{
    public FishingRod item;
    [FormerlySerializedAs("Icon")] public Image icon;
    public Image background;
    public GameObject hover;
    public RectTransform durabilityBar;
    public TextMeshProUGUI amountText;
    public TextMeshProUGUI fishCaughtText;
    
    public void Init()
    {
        background.color = item.fishingRodSO.rarity.InventoryColor;
        icon.sprite = item.fishingRodSO.spriteDisplay;
        amountText.text = $"{item.fishingRodSO.name}";
        fishCaughtText.text = $"x{item.fishCaught}";
        durabilityBar.localScale = new Vector3(1,item.durability/item.fishingRodSO.durability,1);
    }
}