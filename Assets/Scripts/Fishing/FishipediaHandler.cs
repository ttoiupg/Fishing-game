using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;

public class FishipediaHandler : MonoBehaviour
{
    public Player player;
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
        List<BaseFish> pondFish = gameConfiguration.PondFishes.OrderBy(x => 1f/x.Rarity.OneIn).ToList();
        Debug.Log(pondFish);
        foreach(BaseFish baseFish in pondFish)
        {
            FishIconDisplayer icon = Instantiate(fishipediaDisplayIcon, Categories[0]).GetComponent<FishIconDisplayer>();
            icon.fish = baseFish;
            icon.cardController = cardController;
            icon.player = player;
            icon.enabled = true;
            icon.Init();
        }
    }
}
[System.Serializable]
public class DiscoveredFish
{
    public string discoverDate;
    public BaseFish baseFish;
    public DiscoveredFish(BaseFish fish, string _discoverDate)
    {
        discoverDate = _discoverDate;
        baseFish = fish;
    }
}