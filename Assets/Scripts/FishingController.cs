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
using UnityEngine.UIElements;

public class FishingController : PlayerSystem
{
    [Header("Buff UI")]
    public CanvasGroup BoostCanva;
    public RectTransform OrangeZone;
    public RectTransform GreenZone;
    public RectTransform Needle;
    public Transform ZoneContainer;
    [Header("Pull UI")]
    public UIDocument PullStateUI;
    public VisualElement ControlBarUI;
    public VisualElement FishBarUI;
    public VisualElement ProgressBarUI;


    private List<BaseMutation> AvaliableMutations;
    private List<BaseFish> AvailableFishes;
    private ZoneDisplayer[] Zones;
    private Transform playerTransform;
    private Fish CurrentFish;

    private float needleSpeed = 3f;
    private int needleDirection = 1;
    Coroutine FishingCoroutine;
    Coroutine PullCoroutine;

    [Header("Fishing bar")]
    public StyleLength ControlBarStylePosition;
    public float ControlBarPosition = 50f;
    public float ControlBarGravity = 0f;
    public float ControlBarVelocity = 0f;
    public StyleLength FishBarStylePosition;
    public float FishBarTargetPosition = 0f;
    public float FishBarPosition = 0f;
    public float PullProgress = 0f;

    private float Lerpfloat(float a, float b,float t)
    {
        return a + (b - a) * t;
    }
    private bool IsOverlapOLD(RectTransform At,RectTransform Bt,Rect A,Rect B)
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
    private bool InBetween(float min,float max,float n)
    {
        if (n < max && n > min)
        {
            return true;
        }
        return false;
    }
    private bool IsOverlap(float a1,float a2,float b1,float b2)
    {
        float a_width = a2 - a1;
        float b_width = b2 - b1;
        if (InBetween(b1, b2, a1) || InBetween(b1, b2, a2) || InBetween(a1, a2, b1) || InBetween(a1, a2, b2))
        {
            return true;
        }

        return false;
    }
    #region Update functions
    private void UpdateControlBarPosition()
    {

        ControlBarVelocity += ControlBarGravity * Time.deltaTime;
        float NextPosition = ControlBarPosition + ControlBarVelocity * Time.deltaTime;
        //check if the bar is touching the sides, mean the position-(width/2) is smaller than "0"
        if ((NextPosition - player.ID.pullBarSize/2) <= 0)
        {
            ControlBarVelocity *= -0.6f;
            NextPosition = player.ID.pullBarSize / 2;
        }
        else if((NextPosition + player.ID.pullBarSize / 2) >= 100) 
        {
            ControlBarVelocity *= -0.6f;
            NextPosition = 100 - player.ID.pullBarSize / 2;
        }
        ControlBarPosition = NextPosition;
        ControlBarStylePosition = new StyleLength(Length.Percent((float)ControlBarPosition - 15f));
        ControlBarUI.style.marginLeft = ControlBarStylePosition;
        //Debug.Log(ControlBarPosition);
    }
    private void ZoneCheck()
    {
        for (int i = 0; i < Zones.Length; i++)
        {
            if (IsInside(Zones[i].zone.position, Zones[i].zone.size, new Vector2(playerTransform.position.x, playerTransform.position.z)))
            {
                player.ID.currentZone = Zones[i].zone;
                break;
            }
            else
            {
                player.ID.currentZone = null;
            };
        };
    }
    private void BoostStateUpdateFunction()
    {
        if (player.ID.isBoostState == true)
        {
            Vector2 NeedlePosDelta = new Vector2(needleDirection * 500 * needleSpeed * Time.deltaTime, 0);
            Needle.anchoredPosition += NeedlePosDelta;
            if (Needle.anchoredPosition.x >= 350f || Needle.anchoredPosition.x <= -350f)
            {
                needleDirection *= -1;
            }
        }
    }
    private void PullStateUpdateFunction()
    {
        if (player.ID.isPullState == true)
        {
            //Update control bar's position
            UpdateControlBarPosition();
            //lerping fish bar's position
            FishBarPosition = Lerpfloat(FishBarPosition, FishBarTargetPosition, 6f * Time.deltaTime);
            FishBarUI.style.marginLeft = new StyleLength(Length.Percent(FishBarPosition));
            //check for overlap
            if (IsOverlap(ControlBarPosition - player.ID.pullBarSize / 2, ControlBarPosition + player.ID.pullBarSize / 2, FishBarPosition, FishBarPosition + 6f))
            {
                PullProgress += player.ID.pullProgressSpeed * Time.deltaTime;
            }
            else
            {
                PullProgress -= player.ID.pullProgressLooseSpeed * Time.deltaTime;
                if (PullProgress < 0) { PullProgress = 0; }
            }
            ProgressBarUI.style.width = new StyleLength(Length.Percent(PullProgress));
            if (PullProgress >= 100f)
            {
                FishCatched();
            }
        }
    }
    #endregion
    #region Fishing
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
        for (int i = AvailableFishes.Count - 1; i >= 0; i--)
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
    private void FishCatched()
    {
        StopCoroutine(PullCoroutine);
        player.ID.FishOnBait = false;
        player.ID.isFishing = false;
        player.ID.isPullState = false;
        player.ID.playerEvents.OnExitFishingState?.Invoke();
        player.ID.playerEvents.OnFishCatched?.Invoke(CurrentFish);
        PullProgress = 0f;
    }
    private bool CheckFishBarOverlap(RectTransform Ft, RectTransform Ct, Rect F, Rect C)
    {
        if (F.yMax + Ft.localPosition.y < C.yMax + Ct.localPosition.y
            && F.yMin + Ft.localPosition.y > C.yMin + Ct.localPosition.y)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void ControlPullingBar(InputAction.CallbackContext callbackContext)
    {
        if (player.ID.isPullState == true)
        {
            float Value = callbackContext.ReadValue<float>();
            if (Value > 0)
            {
                Debug.Log("switched gravity!");
                ControlBarGravity = 200f * Value;
                //PullingControlBarRigid.gravityScale = -75f * Value;
            }
            else
            {
                Debug.Log("Back to normal!");
                ControlBarGravity = -200f;
                //PullingControlBarRigid.gravityScale = 75f;
            }
        }
    }
    public IEnumerator RandomFishBarPosition()
    {
        while (player.ID.isPullState)
        {
            float RandSecond = UnityEngine.Random.Range(CurrentFish.fishType.MinFishBarChangeTime, CurrentFish.fishType.MaxFishBarChangeTime);
            FishBarTargetPosition = UnityEngine.Random.Range(0, 94f);
            yield return new WaitForSeconds(RandSecond);
        }
    }
    private void EnterPullState(float Buff)
    {
        BaseFish catchedBaseFish = RollForFish();
        BaseMutation catchedBaseMutation = RollForMutation();
        CurrentFish = new Fish(catchedBaseFish, catchedBaseMutation);
        player.ID.playerEvents.OnPullStage?.Invoke();
        player.ID.isBoostState = false;
        player.ID.isPullState = true;
        PullProgress = Buff;
        PullCoroutine = StartCoroutine(RandomFishBarPosition());
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
                if (IsOverlapOLD(Needle, GreenZone, Needle.rect, GreenZone.rect))
                {
                    buff = player.ID.GreenZonebuff;
                }
                else if (IsOverlapOLD(Needle, OrangeZone, Needle.rect, OrangeZone.rect))
                {
                    buff = player.ID.OrangeZonebuff;
                }
                EnterPullState(buff);
            };
        }
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
    private void SetHook()
    {
        player.ID.FishOnBait = true;
        EnterBoostState();
    }
    public IEnumerator WaitForbite()
    {
        float randTime = UnityEngine.Random.Range(2.5f, 3.5f);
        //yield回傳來延遲
        yield return new WaitForSeconds(randTime);
        SetHook();
    }

    #endregion
    #region Casting fishing rod
    public void CastOrRetract(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed == true)
        {
            if (player.ID.FishOnBait == false)
            {

                if (player.ID.isFishing == false && player.ID.canFish)
                {
                    player.ID.isFishing = true;
                    CastFishingRod();
                }
                else if (player.ID.isFishing)
                {
                    player.ID.isFishing = false;
                    RetractFishingRod();
                }
            };
        }
    }
    public void CastFishingRod()
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
            FishingCoroutine = StartCoroutine(WaitForbite());
        }
    }
    public void RetractFishingRod()
    {
        if (FishingCoroutine != null)
        {
            StopCoroutine(FishingCoroutine);
        }
        player.ID.playerEvents.OnExitFishingState?.Invoke();
    }

    #endregion
    #region Unity functions
    private void OnEnable()
    {
        playerInput.Fishing.Enable();
        playerInput.Fishing.CastFishingRod.performed += CastOrRetract;
        playerInput.Fishing.CastFishingRod.performed += LandNeedle;
        playerInput.Fishing.ControlFishingRod.started += ControlPullingBar;
        playerInput.Fishing.ControlFishingRod.canceled += ControlPullingBar;
    }
    private void OnDisable()
    {
        playerInput.Fishing.CastFishingRod.performed -= CastOrRetract;
        playerInput.Fishing.CastFishingRod.performed -= LandNeedle;
        playerInput.Fishing.ControlFishingRod.started -= ControlPullingBar;
        playerInput.Fishing.ControlFishingRod.canceled -= ControlPullingBar;
        playerInput.Fishing.Disable();
    }
    private void Start()
    {
        Zones = ZoneContainer.GetComponentsInChildren<ZoneDisplayer>();
        playerTransform = player.GetComponent<Transform>();
        ControlBarUI = PullStateUI.rootVisualElement.Q<VisualElement>("ControlBar");
        FishBarUI = PullStateUI.rootVisualElement.Q<VisualElement>("FishBar");
        ProgressBarUI = PullStateUI.rootVisualElement.Q<VisualElement>("Bar");
    }
    // Update is called once per frame
    private void Update()
    {
        ZoneCheck();
        BoostStateUpdateFunction();
        PullStateUpdateFunction();
    }
   
    #endregion
}