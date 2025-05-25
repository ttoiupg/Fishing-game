using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ItemIconDisplayer : MonoBehaviour
{
    public GameItem item;
    [FormerlySerializedAs("Icon")] public Image icon;
    public TextMeshProUGUI amountText;
    
    public void Init()
    {
        GetComponent<Image>().sprite = item.item.Ring;
        icon.sprite = item.item.icon;
        amountText.text = item.amount.ToString();
    }
}