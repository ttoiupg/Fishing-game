using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using JetBrains.Annotations;
using TMPro;
using Unity.Burst.Intrinsics;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class FishCardHandler : MonoBehaviour
{
    public static FishCardHandler instance;
    public Player player;
    public AudioClip CardOpen;
    public AudioClip CardClose;
    public Transform cardTransform;
    public GameObject cardOverlay;
    public GameObject shadow;
    public GameObject closeButton;
    public SpriteRenderer Front;
    public SpriteRenderer Art;
    public TextMeshPro Fishname;
    public TextMeshPro Weight;
    public TextMeshPro DiscoverDate;
    public TextMeshPro Rarity;
    public TextMeshPro FavoriteFood;
    public Material material;
    public float x_RotateAmount;
    public float y_RotateAmount;

    public bool isOpen = false;
    private EventSystem _eventSystem;
    private GameObject _triggerObject;
    [SerializeField]private bool _debounce = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _eventSystem = EventSystem.current;
        cardTransform = GetComponent<Transform>();
        Debug.Log("started");
    }
    float Remap(float value, float from1,float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
    private async UniTask OpenCard(BaseFish fish)
    {
        if (_debounce) return;
        _debounce = true;
        cardOverlay.SetActive(true);
        cardTransform.DOScale(Vector3.zero, 0.15f).SetEase(Ease.OutQuint);
        SoundFXManger.Instance.PlaySoundFXClip(CardOpen, player.characterTransform, 0.8f);
        player.CardOpened = true;
        shadow.SetActive(true);
        cardTransform.rotation = Quaternion.Euler(0,180,0);
        cardTransform.DORotate(new Vector3(0,0,0),.5f).SetEase(Ease.OutBack);
        cardTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        Front.sprite = fish.Card;
        Art.sprite = fish.Art;
        DiscoveredFish discoveredFish;
        if (player.discoveredFish.TryGetValue(fish.id,out discoveredFish))
        {
            Art.color = Color.white;
            Fishname.text = fish.name;
            Rarity.text = fish.Rarity.name;
            FavoriteFood.text = fish.FavoriteFood;
            Weight.text = fish.MinWeight + "~" + fish.MaxWeight;
            DiscoverDate.text = "Discover date : " + discoveredFish.discoverDate;
        }
        else
        {
            Art.color = Color.black;
            Fishname.text = "???";
            Rarity.text = fish.Rarity.name;
            FavoriteFood.text = "???";
            Weight.text = "??? ~ ???";
            DiscoverDate.text = "Yet to be discovered!";
        }
        material.SetFloat("_EDITION", fish.FishipediaCardShader);
        Front.material = new Material(material);
        _eventSystem.SetSelectedGameObject(closeButton);
        await UniTask.WaitForSeconds(0.5f);
        isOpen = true;
        _debounce = false;
    }
    private async UniTask OpenCard(Fish fish)
    {
        if (_debounce) return;
        _debounce = true;
        cardOverlay.SetActive(true);
        SoundFXManger.Instance.PlaySoundFXClip(CardOpen, player.characterTransform, 0.8f);
        player.CardOpened = true;
        shadow.SetActive(true);
        cardTransform.rotation = Quaternion.Euler(0, 180, 0);
        cardTransform.DORotate(new Vector3(0, 0, 0), .5f).SetEase(Ease.OutBack);
        cardTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        Front.sprite = fish.fishType.Card;
        Art.sprite = fish.fishType.Art;
        DiscoveredFish discoveredFish;
        if (player.discoveredFish.TryGetValue(fish.fishType.id, out discoveredFish))
        {
            Art.color = Color.white;
            Fishname.text = fish.fishType.name;
            Rarity.text = fish.fishType.Rarity.name;
            FavoriteFood.text = "Mutation : " + fish.mutation.name;
            Weight.text = fish.weight.ToString() + " KG";
            DiscoverDate.text = "Discover date : " + discoveredFish.discoverDate;
        }
        material.SetFloat("_EDITION", fish.fishType.FishipediaCardShader);
        Front.material = new Material(material);
        _eventSystem.SetSelectedGameObject(closeButton);
        await UniTask.WaitForSeconds(0.5f);
        isOpen = true;
        _debounce = false;
    }

    public void CloseCard()
    {
        if (_debounce) return;
        _debounce = true;
        SoundFXManger.Instance.PlaySoundFXClip(CardClose, player.characterTransform, 0.8f);
        player.CardOpened = false;
        isOpen = false;
        cardTransform.DORotate(new Vector3(0, 180, 0), .4f).SetEase(Ease.OutBack);
        cardTransform.DOScale(Vector3.zero, 0.45f).SetEase(Ease.OutQuint).SetDelay(0.1f);
        cardOverlay.SetActive(false);
        shadow.SetActive(false);
        _debounce = false;
        if (_triggerObject != null)
        {
            _eventSystem.SetSelectedGameObject(_triggerObject);
        }
    }
    public void toggleCard(Fish fish,GameObject iconDisplayer)
    {
        if (_debounce) return;
        if (isOpen)
        {
            CloseCard();
        }
        else
        {
            OpenCard(fish);
            _triggerObject = iconDisplayer;
        }
    }
    public void toggleCard(BaseFish fish,GameObject displayer)
    {
        if (_debounce) return;
        if (isOpen)
        {
            CloseCard();
        }
        else
        {
            OpenCard(fish);
            _triggerObject = displayer;
        }
    }
    void Update()
    {
        if (isOpen == true && Gamepad.all.Count == 0)
        {
            int screenWidth = Screen.width;
            int screenHeight = Screen.height;
            Vector2 mousePos = Input.mousePosition;
            Vector2 screenCenter = new Vector2(screenWidth / 2, screenHeight / 2);
            Vector2 Converted = new Vector2((mousePos.x - screenCenter.x) / (screenWidth / 2), (mousePos.y - screenCenter.y) / (screenHeight / 2));
            float xAngle = -Converted.x * x_RotateAmount;
            float yAngle = Converted.y * y_RotateAmount;
            cardTransform.rotation = Quaternion.Euler(yAngle, xAngle, 0f);
            Front.material.SetVector("_Rotation", new Vector2(Remap(xAngle, -20, 20, -0.5f, 0.5f), Remap(yAngle, -20, 20, -0.5f, 0.5f)));
        }
    }
}
