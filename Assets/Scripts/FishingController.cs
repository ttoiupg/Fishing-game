using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using TMPro;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.InputSystem;

public static class RectTransformExtensions
{

    public static bool Overlaps(this RectTransform a, RectTransform b)
    {
        Debug.Log(a.WorldRect());
        Debug.Log(b.WorldRect());
        return a.WorldRect().Overlaps(b.WorldRect());
    }
    public static bool Overlaps(this RectTransform a, RectTransform b, bool allowInverse)
    {
        return a.WorldRect().Overlaps(b.WorldRect(), allowInverse);
    }

    public static Rect WorldRect(this RectTransform rectTransform)
    {
        Vector2 sizeDelta = rectTransform.sizeDelta;
        float rectTransformWidth = sizeDelta.x * rectTransform.lossyScale.x;
        float rectTransformHeight = sizeDelta.y * rectTransform.lossyScale.y;

        Vector3 position = rectTransform.position;
        return new Rect(position.x - rectTransformWidth / 2f, position.y - rectTransformHeight / 2f, Math.Abs(rectTransformWidth), Math.Abs(rectTransformHeight));
    }
}

public class FishingController : PlayerSystem
{
    [Header("Buff UI")]
    public CanvasGroup BoostCanva;
    public RectTransform OrangeZone;
    public RectTransform GreenZone;
    public RectTransform Needle;
    public Transform ZoneContainer;
    [Header("Pull UI")]
    public CanvasGroup PullCanva;
    public Rigidbody2D PullingControlBarRigid;
    public RectTransform PullingControlBar;
    public RectTransform FishBar;
    public RectTransform ProgressBar;
    private List<BaseMutation> AvaliableMutations;
    private List<BaseFish> AvailableFishes;
    private ZoneDisplayer[] Zones;
    private Transform playerTransform;
    private Fish CurrentFish;

    private float needleSpeed = 3f;
    private int needleDirection = 1;
    private float pullProgress = 0f;
    private Vector2 fishBarPosition = Vector2.zero;
    Coroutine FishingCoroutine;
    Coroutine PullCoroutine;

    private BaseMutation RollForMutation()
    {
        BaseMutation catchedMutation = null;
        //把此區域可釣到的變種的稀有度(倒數)加起來
        float totalWeight = AvaliableMutations.Sum(x => 1f / x.OneIn);
        //新增一個從0到totalWeight的隨機小數
        float randomValue = UnityEngine.Random.Range(0f, totalWeight);
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
        float randomValue = UnityEngine.Random.Range(0f, totalWeight);
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
    private void EnterBoostState()
    {
        player.ID.playerEvents.OnBoostStage?.Invoke();
        player.ID.isBoostState = true;
        BoostCanva.alpha = 1f;
        float RandOrangePos = UnityEngine.Random.Range(-350f, 350f);
        Vector2 RandPos = new Vector2(RandOrangePos, 0);
        OrangeZone.anchoredPosition = RandPos;
        GreenZone.anchoredPosition = RandPos;
        Needle.anchoredPosition = new Vector2(0, 0);
    }
    private void EnterPullState(float Buff)
    {
        BaseFish catchedBaseFish = RollForFish();
        BaseMutation catchedBaseMutation = RollForMutation();
        CurrentFish = new Fish(catchedBaseFish, catchedBaseMutation);
        player.ID.playerEvents.OnPullStage?.Invoke();
        player.ID.isBoostState = false;
        player.ID.isPullState = true;
        PullCanva.alpha = 1f;
        pullProgress = 0f;
        PullingControlBarRigid.gravityScale = 50f;
        PullingControlBar.anchoredPosition = new Vector2(108f,0);
        ProgressBar.anchoredPosition = new Vector2(0, -558.7f);
        FishBar.anchoredPosition = new Vector2(0, 0);
        PullCoroutine = StartCoroutine(RandomFishBarPosition());
    }
    private void FishCatched()
    {
        StopCoroutine(PullCoroutine);
        player.ID.FishOnBait = false;
        player.ID.isFishing = false;
        player.ID.isPullState = false;
        player.ID.playerEvents.OnExitFishingState?.Invoke();
        PullCanva.alpha = 0f;
        player.ID.playerEvents.OnFishCatched?.Invoke(CurrentFish);
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
    private bool CheckFishBarOverlap(RectTransform Ft, RectTransform Ct,Rect F,Rect C)
    {
        if(F.yMax+ Ft.localPosition.y< C.yMax + Ct.localPosition.y
            && F.yMin + Ft.localPosition.y > C.yMin + Ct.localPosition.y)
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
    private void Update()
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
            Vector2 NeedlePosDelta = new Vector2(needleDirection * 500 * needleSpeed * Time.deltaTime, 0);
            Needle.anchoredPosition += NeedlePosDelta;
            if (Needle.anchoredPosition.x >= 350f || Needle.anchoredPosition.x <= -350f)
            {
                needleDirection *= -1;
            }
        }
        if (player.ID.isPullState == true)
        {
            FishBar.anchoredPosition = Vector2.Lerp(FishBar.anchoredPosition,fishBarPosition, 6f*Time.deltaTime);
            if (CheckFishBarOverlap(FishBar,PullingControlBar,FishBar.rect,PullingControlBar.rect))
            {
                pullProgress += player.ID.pullProgressSpeed*Time.deltaTime;
            }
            else
            {
                pullProgress -= player.ID.pullProgressLooseSpeed * Time.deltaTime;
                if(pullProgress <0) { pullProgress = 0; }
            }
            ProgressBar.anchoredPosition = new Vector2(0, -558.7f + pullProgress);
            if (pullProgress >= 558.7f)
            {
                FishCatched();
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
            //Debug.Log(player.ID.isBoostState);
            if (player.ID.isBoostState && player.ID.FishOnBait)
            {
                BoostCanva.alpha = 0f;
                player.ID.isBoostState = false;
                float buff = 0f;
                if (IsOverlap(Needle, GreenZone, Needle.rect, GreenZone.rect))
                {
                    buff = player.ID.GreenZonebuff;
                }
                else if (IsOverlap(Needle, OrangeZone, Needle.rect, OrangeZone.rect))
                {
                    buff = player.ID.OrangeZonebuff;
                }
                EnterPullState(buff);
            };
        }
    }

    public void ControlPullingBar(InputAction.CallbackContext callbackContext)
    {
        if (player.ID.isPullState == true)
        {
            float Value = callbackContext.ReadValue<float>();
            if (Value > 0)
            {
                PullingControlBarRigid.gravityScale = -75f* Value;
            }
            else
            {
                PullingControlBarRigid.gravityScale = 75f;
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
    //利用IEnumerator 來實現平行代碼
    public IEnumerator WaitingFish()
    {
        float randTime = UnityEngine.Random.Range(2.5f, 3.5f);
        //yield回傳來延遲
        yield return new WaitForSeconds(randTime);
        StartCatchingFish();
    }
    public IEnumerator RandomFishBarPosition()
    {
        while (player.ID.isPullState)
        {
            float RandSecond = UnityEngine.Random.Range(CurrentFish.fishType.MinFishBarChangeTime, CurrentFish.fishType.MaxFishBarChangeTime);
            float randY = UnityEngine.Random.Range(-269.24f, 269.24f);
            fishBarPosition = new Vector2(0, randY);
            yield return new WaitForSeconds(RandSecond);
        }
    }
}