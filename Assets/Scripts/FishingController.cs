using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using TMPro;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.InputSystem;

public class FishingController : PlayerSystem
{
    public Transform playerTransform;
    public Transform ZoneContainer;
    private List<BaseMutation> AvaliableMutations;
    private List<BaseFish> AvailableFishes;
    private ZoneDisplayer[] Zones;
    Coroutine FishingCoroutine;

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
    private void StartCatchingFish()
    {
        BaseFish catchedBaseFish = RollForFish();
        BaseMutation catchedBaseMutation = RollForMutation();
        Fish catchedFish = new Fish(catchedBaseFish, catchedBaseMutation);
        player.ID.playerEvents.OnFishCatched.Invoke(catchedFish);
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
        playerTransform = player.GetComponent<Transform>();
    }
    // Update is called once per frame
    void Update()
    {
        Zones = ZoneContainer.GetComponentsInChildren<ZoneDisplayer>();
        for(int i=0;i< Zones.Length; i++)
        {
            if (IsInside(Zones[i].zone.position, Zones[i].zone.size, new Vector2(playerTransform.position.x, playerTransform.position.z))){
                player.ID.currentZone = Zones[i].zone;
                break;
            }
            else
            {
                player.ID.currentZone = null;
            };
        };
    }


    public void PendingFishing(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed == true)
        {
            if (player.ID.isFishing == false && player.ID.canFish)
            {
                player.ID.isFishing = true;
                ThrowFishingRod();
            }
            else if (player.ID.isFishing)
            {
                player.ID.isFishing = false;
                RetrackFishingRod();
            }
        }
    }
    public void ThrowFishingRod()
    {
        player.ID.playerEvents.OnEnterFishingState?.Invoke();
        if (player.ID.currentZone != null)
        {
            AvailableFishes = player.ID.currentZone.GetSortedFeaturedFish();
            AvaliableMutations = player.ID.currentZone.GetSortedFeaturedMutations();
            if (FishingCoroutine != null)
            {
                StopCoroutine(FishingCoroutine);
            }
            FishingCoroutine = StartCoroutine(WaitingFish());
        }
    }
    public void RetrackFishingRod()
    {
        if (FishingCoroutine != null)
        {
            StopCoroutine(FishingCoroutine);
        }
        player.ID.playerEvents.OnExitFishingState?.Invoke();
    }
    //�Q��IEnumerator �ӹ�{����N�X
    public IEnumerator WaitingFish()
    {
        float randTime = Random.Range(2.5f, 3.5f);
        //yield�^�Ǩө���
        yield return new WaitForSeconds(randTime);
        StartCatchingFish();
    }
}