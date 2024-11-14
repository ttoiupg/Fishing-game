using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TMPro;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public class FishingController : MonoBehaviour
{
    public Transform playerTransform;
    public PlayerControl playerControl;
    public bool CanFish = true;
    public bool IsFishing = false;
    public Transform ZoneContainer;
    public BaseZone CurrentZone;
    public TextMeshProUGUI CatchedFishText;
    public TextMeshProUGUI CatchedMutationText;
    public TextMeshProUGUI CurrentZoneText;
    public TextMeshProUGUI IsFishingText;
    public TextMeshProUGUI RarityText;
    private List<BaseMutation> AvaliableMutations;
    private List<BaseFish> AvailableFishes;
    private ZoneDisplayer[] Zones;

    private BaseMutation RollForMutation()
    {
        BaseMutation catchedMutation = null;
        //�⦹�ϰ�i���쪺�ܺت��}����(�˼�)�[�_��
        float totalWeight = AvaliableMutations.Sum(x => 1f / x.OneIn);
        //�s�W�@�ӱq0��totalWeight���H���p��
        float randomValue = Random.Range(0f, totalWeight);
        //�q�̱`�����ܺض}�l��A���`�Ƥp���ƮɡA�i�o���������쪺�ܺص}����
        for (int i = AvaliableMutations.Count - 1; i >= 0; i--)
        {
            totalWeight -= 1f / AvaliableMutations[i].OneIn;
            if (totalWeight <= randomValue)
            {
                catchedMutation = AvaliableMutations[i];
                //Debug.Log(AvaliableMutations[i].name);
                break;
            }
        }
        return catchedMutation;
    }
    private BaseFish RollForFish()
    {
        BaseFish catchedFish = null;
        //�⦹�ϰ�i���쪺�������}����(�˼�)�[�_��
        float totalWeight = AvailableFishes.Sum(x => 1f / x.Rarity.OneIn);
        //�s�W�@�ӱq0��totalWeight���H���p��
        float randomValue = Random.Range(0f, totalWeight);
        //�q�̱`�������ض}�l��A���`�Ƥp���ƮɡA�i�o���������쪺�����}����
        for (int i = AvailableFishes.Count-1; i >= 0 ; i--)
        {
            totalWeight -= 1f / AvailableFishes[i].Rarity.OneIn;
            if (totalWeight <= randomValue)
            {
                catchedFish = AvailableFishes[i]; 
                //Debug.Log(AvailableFishes[i].name);
                break;
            }
        }
        return catchedFish;
    }
    private void CatchFish()
    {
        BaseFish catchedFish = RollForFish();
        BaseMutation catchedMutation = RollForMutation();
        CatchedFishText.text = $"Catched : {catchedFish.name}";
        RarityText.text = $"Rarity : {catchedFish.Rarity.name}";
        CatchedMutationText.text = $"With mutation : {catchedMutation.name}";
    }
    private bool IsInside(Vector2 A,Vector2 SizeA,Vector2 B)
    {
        bool result = false;
        if (A.x - SizeA.x/2f <= B.x && A.x + SizeA.x/2f >= B.x && A.y - SizeA.y/2f <= B.y && A.y + SizeA.y/2f >= B.y)
        {
            result = true;
        }
        return result;
    }
    private void Start()
    {
        playerTransform = gameObject.GetComponent<Transform>();
        playerControl = gameObject.GetComponent<PlayerControl>();
    }
    // Update is called once per frame
    void Update()
    {
        Zones = ZoneContainer.GetComponentsInChildren<ZoneDisplayer>();
        for(int i=0;i< Zones.Length; i++)
        {
            if (IsInside(Zones[i].zone.position, Zones[i].zone.size, new Vector2(playerTransform.position.x, playerTransform.position.z))){
                CurrentZone = Zones[i].zone;
                break;
            }
            else
            {
                CurrentZone = null;
            };
        };
        if (CurrentZone != null)
        {
            CurrentZoneText.text = $"In {CurrentZone.name}";
        }
        else
        {
            CurrentZoneText.text = "Not in fishing zone";
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PendingFishing();
        }
    }


    public void PendingFishing()
    {
        if (IsFishing == false && CanFish)
        {
            IsFishing = true;
            IsFishingText.text = "Is fishing";
            ThrowFishingRod();
        }
        else if (IsFishing)
        {
            IsFishing = false;
            IsFishingText.text = "Not fishing";
            RetrackFishingRod();
        }
    }
    public void ThrowFishingRod()
    {
        playerControl.playerSpeed = 0;
        if (CurrentZone != null)
        {
            AvailableFishes = CurrentZone.GetSortedFeaturedFish();
            AvaliableMutations = CurrentZone.GetSortedFeaturedMutations();
            CatchFish();
        }
    }
    public void RetrackFishingRod()
    {
        CatchedFishText.text = "Catched : ";
        CatchedMutationText.text = "With mutation : ";
        RarityText.text = "Rarity : ";
        playerControl.playerSpeed = 7.0f;
    }
}