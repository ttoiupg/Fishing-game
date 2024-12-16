using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using DG.Tweening;

public class FishIconDisplayer : MonoBehaviour
{
    public BaseFish fish;
    private CanvasGroup displayObject;
    private Image card;
    private Image art;
    private Image cardBack;
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
        cardBack = GameObject.FindGameObjectWithTag("FishipediaCardBack").GetComponent<Image>();
    }
    public void Init()
    {
        GetComponent<Image>().sprite = fish.Ring;
    }
    public void Clicked()
    {
        //zoom from click position to view
        cardBack.enabled = true;
        RectTransform cardTransform = card.gameObject.GetComponent<RectTransform>();
        Vector3 mousePos = Input.mousePosition;
        Vector3 percentage = new Vector3(mousePos.x / Screen.width * 1920f, mousePos.y / Screen.height * 1080f, 0f);
        void HalfRotate()
        {
            cardBack.enabled = false;
            cardTransform.DORotateQuaternion(Quaternion.Euler(0, 0, 0), 0.175f);
        }
        cardTransform.rotation = Quaternion.Euler(0, 180f, 0);
        cardTransform.localScale = Vector3.zero;
        cardTransform.anchoredPosition = new Vector3(percentage.x, percentage.y, 0f);
        cardTransform.DORotateQuaternion(Quaternion.Euler(0, 90f, 0), 0.175f).OnComplete(HalfRotate).SetDelay(0.15f);
        cardTransform.DOAnchorPos(new Vector2(960, 540), 0.35f).SetEase(Ease.OutBack).SetDelay(0.15f);
        cardTransform.DOScale(Vector3.one, 0.35f).SetEase(Ease.OutBack).SetDelay(0.15f);
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
