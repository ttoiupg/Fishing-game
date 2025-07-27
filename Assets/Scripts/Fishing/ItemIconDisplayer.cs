using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ItemIconDisplayer : MonoBehaviour
{
    public GameItem item;
    [FormerlySerializedAs("Icon")] public Image icon;
    public Image background;
    public TextMeshProUGUI amountText;
    
    public void Init()
    {
        background.color = item.item.rarity.InventoryColor;
        icon.sprite = item.item.icon;
        amountText.text = $"x{item.amount.ToString()}";
    }
}