using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class FishIconDisplayer : MonoBehaviour
{
    public BaseFish fish;
    private CanvasGroup displayObject;
    private Image card;
    private Image art;
    private TextMeshProUGUI discoverDate;
    private TextMeshProUGUI weight;
    private TextMeshProUGUI fishName;
    private TextMeshProUGUI rarity;
    private TextMeshProUGUI favoriteFood;
    private void Awake()
    {
        displayObject = GameObject.FindGameObjectWithTag("FishipediaCardDisplay").GetComponent<CanvasGroup>();
        card = GameObject.FindGameObjectWithTag("FishipediaCard").GetComponent<Image>();
        art = GameObject.FindGameObjectWithTag("FishipediaCardArt").GetComponent<Image>();
        discoverDate = GameObject.FindGameObjectWithTag("FishipediaCardDiscoverDate").GetComponent<TextMeshProUGUI>();
        weight = GameObject.FindGameObjectWithTag("FishipediaCardWeight").GetComponent<TextMeshProUGUI>();
        fishName = GameObject.FindGameObjectWithTag("FishipediaCardFishName").GetComponent<TextMeshProUGUI>();
        rarity = GameObject.FindGameObjectWithTag("FishipediaCardRarity").GetComponent<TextMeshProUGUI>();
        favoriteFood = GameObject.FindGameObjectWithTag("FishipediaCardFavoriteFood").GetComponent<TextMeshProUGUI>();
    }
    public void Init()
    {
        GetComponent<Image>().sprite = fish.Ring;
    }
    public void Clicked()
    {
        displayObject.alpha = 1;
        displayObject.interactable = true;
        displayObject.blocksRaycasts = true;
        card.sprite = fish.Card;
        art.sprite = fish.Art;
        fishName.text = fish.name;
        rarity.text = fish.Rarity.name;
        favoriteFood.text = fish.FavoriteFood;
        weight.text = fish.MinWeight + "~" + fish.MaxWeight;
        discoverDate.text = "Discover date : "+ System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
    }
}
