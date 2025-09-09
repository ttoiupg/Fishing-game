using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class FishIconDisplayer : MonoBehaviour, ISelectHandler
{
    public Fish fish;
    public Image Icon;
    public Image background;
    public Image hover;
    public TextMeshProUGUI weightText;
    public TextMeshProUGUI mutationText;
    public UnityEvent Select;
    public void Init()
    {
        Icon.sprite = fish.fishType.Art;
        background.color = fish.fishType.Rarity.InventoryColor;
        weightText.text = $"{fish.weight}kg";
        mutationText.text = (fish.mutation.id == "mut_none") ? "": $"{fish.mutation.name}";
    }

    public void OnSelect(BaseEventData eventData)
    {
        Select.Invoke();
    }
}