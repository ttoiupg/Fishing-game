using UnityEngine;
using UnityEngine.UI;

public class FishIconDisplayer : MonoBehaviour
{
    public Fish fish;
    public Image Icon;
    public void Init()
    {
        GetComponent<Image>().sprite = fish.fishType.Ring;
        Icon.sprite = fish.fishType.Art;
    }
    public void Clicked()
    {
        FishipediaCardController.instance.toggleCard(fish,gameObject);
    }
}