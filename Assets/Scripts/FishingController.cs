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
        //把此區域可釣到的變種的稀有度(倒數)加起來
        float totalWeight = AvaliableMutations.Sum(x => 1f / x.OneIn);
        //新增一個從0到totalWeight的隨機小數
        float randomValue = Random.Range(0f, totalWeight);
        //從最常見的變種開始減，當總數小於減數時，可得知此次釣到的變種稀有度
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
        //把此區域可釣到的魚類的稀有度(倒數)加起來
        float totalWeight = AvailableFishes.Sum(x => 1f / x.Rarity.OneIn);
        //新增一個從0到totalWeight的隨機小數
        float randomValue = Random.Range(0f, totalWeight);
        //從最常見的魚種開始減，當總數小於減數時，可得知此次釣到的魚類稀有度
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