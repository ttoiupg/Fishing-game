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
    public CanvasGroup BoostCanva;
    public RectTransform OrangeZone;
    public RectTransform GreenZone;
    public RectTransform Needle;
    public Transform playerTransform;
    public Transform ZoneContainer;
    private List<BaseMutation> AvaliableMutations;
    private List<BaseFish> AvailableFishes;
    private ZoneDisplayer[] Zones;
    [SerializeField]
    private float needleSpeed = 3f;
    private int needleDirection = 1;
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
    private void EnterBoostState()
    {
        player.ID.playerEvents.OnBoostStage?.Invoke();
        player.ID.isBoostState = true;
        BoostCanva.alpha = 1f;
        float RandOrangePos = Random.Range(-350f, 350f);
        Vector2 RandPos = new Vector2(RandOrangePos, 0);
        OrangeZone.anchoredPosition = RandPos;
        GreenZone.anchoredPosition = RandPos;
        Needle.anchoredPosition = new Vector2(0, 0);
    }
    private void FishCatched()
    {
        player.ID.FishOnBait = false;
        player.ID.isFishing = false;
        player.ID.playerEvents.OnExitFishingState?.Invoke();
        if (IsOverlap(Needle, GreenZone, Needle.rect, GreenZone.rect))
        {
            Debug.Log("Great!(green)");
        }
        else if (IsOverlap(Needle, OrangeZone, Needle.rect, OrangeZone.rect))
        {
            Debug.Log("nice!(orange)");
        }
        else
        {
            Debug.Log("it's ok(red)");
        }
        BaseFish catchedBaseFish = RollForFish();
        BaseMutation catchedBaseMutation = RollForMutation();
        Fish catchedFish = new Fish(catchedBaseFish, catchedBaseMutation);
        player.ID.playerEvents.OnFishCatched?.Invoke(catchedFish);
    }

    private void StartCatchingFish()
    {
        player.ID.FishOnBait = true;
        EnterBoostState();
    }
    private bool IsOverlap(RectTransform At,RectTransform Bt,Rect A,Rect B)
    {
        if (At.localPosition.x < Bt.localPosition.x + B.width &&
            At.localPosition.x + A.width > Bt.localPosition.x &&
            At.localPosition.y < Bt.localPosition.y + B.height &&
            At.localPosition.y + A.height > Bt.localPosition.y)
        {
            return true;
        }
        else
        {
            return false;
        }
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
        if (player.ID.isBoostState == true)
        {
            Vector2 NeedlePosDelta = new Vector2(needleDirection * 700 * needleSpeed * Time.deltaTime, 0);
            Needle.anchoredPosition += NeedlePosDelta;
            if (Needle.anchoredPosition.x > 350f || Needle.anchoredPosition.x < -350f)
            {
                needleDirection *= -1;
            }
        }
    }
    public void PendingFishing(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed == true)
        {
            if (player.ID.FishOnBait == false) {

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
            };
        }
    }
    public void LandNeedle(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed == true)
        {
            Debug.Log(player.ID.isBoostState);
            if (player.ID.isBoostState && player.ID.FishOnBait)
            {
                BoostCanva.alpha = 0f;
                player.ID.isBoostState = false;
                FishCatched();
            };
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