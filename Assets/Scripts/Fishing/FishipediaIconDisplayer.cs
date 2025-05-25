using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using DG.Tweening;

public class FishipediaIconDisplayer : MonoBehaviour
{
    public Player player;
    public BaseFish fish;
    public Image Icon;
    public FishipediaCardController cardController;
    private void OnEnable()
    {
        player.ID.playerEvents.OnFishUnlocked += Refresh;
    }
    private void OnDisable()
    {
        player.ID.playerEvents.OnFishUnlocked -= Refresh;
    }
    public void Init()
    {
        GetComponent<Image>().sprite = fish.Ring;
        Icon.sprite = fish.Art;
        if (!player.discoveredFish.ContainsKey(fish.id))
            Icon.color = Color.black;
        else
            Icon.color = Color.white;
    }
    public void Refresh(BaseFish _fish)
    {
        if (_fish == fish) Init();
    }
    public void Clicked()
    {
        StartCoroutine(cardController.OpenCard(fish));
    }
}