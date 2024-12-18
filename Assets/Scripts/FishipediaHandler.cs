using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class FishipediaHandler : MonoBehaviour
{
    public GameConfiguration gameConfiguration;
    public GameObject fishipediaDisplayIcon;
    public FishipediaCardController cardController;
    public List<RectTransform> Labels = new List<RectTransform>();
    public int CurrentCategory;
    public List<RectTransform> Categories = new List<RectTransform>();
    public void CategorySelected(int number)
    {
        CurrentCategory = number;
        for (int i = 0; i < 4; i++) { 
            if (number == i)
            {
                Categories[number].gameObject.SetActive(true);
                Labels[number].DOAnchorPosX(-130f, 0.2f).SetEase(Ease.OutBack);
            }
            else
            {
                Categories[i].gameObject.SetActive(false);
                Labels[i].DOAnchorPosX(-51f, 0.2f);
            }
        }
    }
    private void Start()
    {
        List<BaseFish> pondFish = gameConfiguration.PondFishes;
        pondFish.Sort();
        foreach(BaseFish baseFish in pondFish)
        {
            FishIconDisplayer icon = Instantiate(fishipediaDisplayIcon, Categories[0]).GetComponent<FishIconDisplayer>();
            icon.fish = baseFish;
            icon.cardController = cardController;
            icon.Init();
        }
    }
}
