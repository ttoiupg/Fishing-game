using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.Burst.Intrinsics;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class FishipediaCardController : MonoBehaviour
{
    public Transform cardTransform;
    public GameObject shadow;
    public SpriteRenderer Front;
    public SpriteRenderer Art;
    public TextMeshPro Fishname;
    public TextMeshPro Weight;
    public TextMeshPro Rarity;
    public TextMeshPro FavoriteFood;
    public TextMeshPro DiscoverDate;
    public Material material;
    public float x_RotateAmount;
    public float y_RotateAmount;

    public bool isOpen = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cardTransform = GetComponent<Transform>();
        material = new Material(Front.material);
        Front.material = material;
    }
    float Remap(float value, float from1,float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
    void SetOpen()
    {
        isOpen = true;
    }
    public void OpenCard(BaseFish fish)
    {
        shadow.SetActive(true);
        cardTransform.rotation = Quaternion.Euler(0,180,0);
        cardTransform.DORotate(new Vector3(0,0,0),.5f).SetEase(Ease.OutBack);
        cardTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        Invoke("SetOpen",0.5f);
        Front.sprite = fish.Card;
        Art.sprite = fish.Art;
        Fishname.text = fish.name;
        Rarity.text = fish.Rarity.name;
        FavoriteFood.text = fish.FavoriteFood;
        Weight.text = fish.MinWeight + "~" + fish.MaxWeight;
        DiscoverDate.text = "Discover date : " + System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
    }
    public void CloseCard()
    {
        isOpen = false;
        cardTransform.DORotate(new Vector3(0, 180, 0), .5f).SetEase(Ease.OutBack);
        cardTransform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutBack);
    }
    void Update()
    {
        if (isOpen == true)
        {
            int screenWidth = Screen.width;
            int screenHeight = Screen.height;
            Vector2 mousePos = Input.mousePosition;
            Vector2 screenCenter = new Vector2(screenWidth/2,screenHeight/2);
            Vector2 Converted = new Vector2((mousePos.x-screenCenter.x)/(screenWidth/2), (mousePos.y-screenCenter.y)/(screenHeight/2));
            float xAngle = -Converted.x*x_RotateAmount;
            float yAngle = Converted.y*y_RotateAmount;
            cardTransform.rotation = Quaternion.Euler(yAngle, xAngle, 0f);
            Front.material.SetVector("_Rotation",new Vector2(Remap(xAngle,-20,20,-0.5f,0.5f), Remap(yAngle,-20,20,-0.5f,0.5f)));
        }
    }
}