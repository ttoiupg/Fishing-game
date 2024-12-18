using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using DG.Tweening;

public class FishIconDisplayer : MonoBehaviour
{
    public BaseFish fish;
    public FishipediaCardController cardController;
    public void Init()
    {
        GetComponent<Image>().sprite = fish.Ring;
    }
    public void Clicked()
    {
        cardController.OpenCard(fish);
    }
}
