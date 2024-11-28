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
    public UIDocument BoostStateUI;
    public VisualElement OrangeChunk;
    public VisualElement GreenChunk;
    public VisualElement Needle;
    [Header("Pull UI")]
    public UIDocument PullStateUI;
    public VisualElement ControlBarUI;
    public VisualElement FishBarUI;
    public VisualElement ProgressBarUI;

    public GameObject ZoneContainer;
    private List<BaseMutation> AvaliableMutations;
    private List<BaseFish> AvailableFishes;
    private ZoneDisplayer[] Zones;
    private Transform playerTransform;
    private Fish CurrentFish;
    private InputAction ControlBarAction;

    Coroutine FishingCoroutine;
    Coroutine PullCoroutine;

    [Header("Pull state")]
    public StyleLength ControlBarStylePosition;
    public float ControlBarPosition = 50f;
    public float ControlBarGravity = 0f;
    public float ControlBarVelocity = 0f;
    public StyleLength FishBarStylePosition;
    public float FishBarTargetPosition = 0f;
    public float FishBarPosition = 0f;
    public float PullProgress = 0f;

    [Header("Boost state")]
    public StyleLength OrangeChunkStylePosition;
    public StyleLength GreenChunkStylePosition;
    public StyleLength NeedleStylePosition;
    public float OrangeChunkPosition;
    public float GreenChunkPosition;
    public float NeedlePosition;
    public float needleSpeed = 3f;
    public int needleDirection = 1;

    private float Lerpfloat(float a, float b,float t)
    {
        return a + (b - a) * t;
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
    private bool IsOverlap(float a1,float a2,float b1,float b2)
    {
        float a_width = a2 - a1;
        float b_width = b2 - b1;
        if (b2 > a2) return a2 >= b1;
        if (b2 < a1) return a1 <= b2;
        if (b2 < a2 && a1 > b2) return true;
        return true;
        /*if (InBetween(b1, b2, a1) || InBetween(b1, b2, a2) || InBetween(a1, a2, b1) || InBetween(a1, a2, b2))
        {
            return true;
        }

        return false;*/
    }
    #region Update functions
    private void UpdateControlBarPosition()
    {

        ControlBarVelocity += ControlBarGravity * Time.deltaTime;
        float NextPosition = ControlBarPosition + ControlBarVelocity * Time.deltaTime;
        //check if the bar is touching the sides, mean the position-(width/2) is smaller than "0"
        if ((NextPosition - player.ID.pullBarSize/2) <= 0)
        {
            ControlBarVelocity *= -0.4f;
            NextPosition = player.ID.pullBarSize / 2;
        }
        else if((NextPosition + player.ID.pullBarSize / 2) >= 100) 
        {
            ControlBarVelocity *= -0.4f;
            NextPosition = 100 - player.ID.pullBarSize / 2;
        }
        ControlBarPosition = NextPosition;
        ControlBarStylePosition = new StyleLength(Length.Percent((float)ControlBarPosition - 15f));
        ControlBarUI.style.left = ControlBarStylePosition;
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
            float NextPosition = NeedlePosition + needleDirection * needleSpeed * Time.deltaTime;
            if (NextPosition >= 96f)
            {
                needleDirection *= -1;
                NextPosition = 96f;
            }
            else if (NextPosition <= 0)
            {
                needleDirection *= -1;
                NextPosition = 0;
            }
            NeedlePosition = NextPosition;
            NeedleStylePosition = new StyleLength(Length.Percent(NeedlePosition));
            Needle.style.left = NeedleStylePosition;

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
            FishBarUI.style.left = new StyleLength(Length.Percent(FishBarPosition));
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
        float totalWeight = AvaliableMutations.Sum(x => 1f / x.OneIn);
        float randomValue = UnityEngine.Random.Range(0f, totalWeight);
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
        float totalWeight = AvailableFishes.Sum(x => 1f / x.Rarity.OneIn);
        float randomValue = UnityEngine.Random.Range(0f, totalWeight);
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
        PullStateUI.rootVisualElement.style.display = DisplayStyle.None;
        player.ID.playerEvents.OnExitFishingState?.Invoke();
        player.ID.playerEvents.OnFishCatched?.Invoke(CurrentFish);
        PullProgress = 0f;
        ControlBarPosition = 50f;
    }
    public void ControlPullingBar()
    {
        if (player.ID.isPullState == true)
        {
            float Value = ControlBarAction.ReadValue<float>();
            if (Value > 0)
            {
                ControlBarGravity = 300f * Value;
            }
            else
            {
                ControlBarGravity = -300f;
            }
        }
    }
    public IEnumerator RandomFishBarPosition()
    {
        while (player.ID.isPullState)
        {
            float RandSecond = UnityEngine.Random.Range(CurrentFish.fishType.MinFishBarChangeTime, CurrentFish.fishType.MaxFishBarChangeTime);
            FishBarTargetPosition = UnityEngine.Random.Range(0, 98f);
            yield return new WaitForSeconds(RandSecond);
        }
    }
    private IEnumerator EnterPullState(float Buff)
    {
        BaseFish catchedBaseFish = RollForFish();
        BaseMutation catchedBaseMutation = RollForMutation();
        CurrentFish = new Fish(catchedBaseFish, catchedBaseMutation);
        player.ID.playerEvents.OnPullStage?.Invoke();
        player.ID.isBoostState = false;
        PullProgress = Buff;
        ControlBarPosition = 50f;
        ControlBarGravity = 0f;
        ProgressBarUI.style.width = new StyleLength(Length.Percent(PullProgress));
        ControlBarStylePosition = new StyleLength(Length.Percent((float)ControlBarPosition - 15f));
        ControlBarUI.style.left = ControlBarStylePosition;
        PullStateUI.rootVisualElement.style.display = DisplayStyle.Flex;
        yield return new WaitForSeconds(0.5f);
        player.ID.isPullState = true;
        PullCoroutine = StartCoroutine(RandomFishBarPosition());
    }
    public void LandNeedle(InputAction.CallbackContext callbackContext)
    {
        StartCoroutine(LandNeedleIEnumerator());
    }
    public IEnumerator LandNeedleIEnumerator()
    {
        //Debug.Log(player.ID.isBoostState);
        if (player.ID.isBoostState && player.ID.FishOnBait)
        {
            
            player.ID.isBoostState = false;
            float buff = 10f;
            if (IsOverlap(GreenChunkPosition, GreenChunkPosition + 20f, NeedlePosition, NeedlePosition + 4f))
            {
                buff = player.ID.GreenZonebuff;
            }
            else if (IsOverlap(OrangeChunkPosition, OrangeChunkPosition + 44f, NeedlePosition, NeedlePosition + 4f))
            {
                buff = player.ID.OrangeZonebuff;
            }
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(EnterPullState(buff));
            BoostStateUI.rootVisualElement.style.display = DisplayStyle.None;
        };
    }
    private void EnterBoostState()
    {
        player.ID.playerEvents.OnBoostStage?.Invoke();
        player.ID.isBoostState = true;
        BoostStateUI.rootVisualElement.style.display = DisplayStyle.Flex;
        OrangeChunkPosition = UnityEngine.Random.Range(0, 66f);
        GreenChunkPosition = OrangeChunkPosition + 12f;
        NeedlePosition = 48f;
        OrangeChunkStylePosition = new StyleLength(Length.Percent(OrangeChunkPosition));
        GreenChunkStylePosition = new StyleLength(Length.Percent(GreenChunkPosition));
        NeedleStylePosition = new StyleLength(Length.Percent(NeedlePosition));
        OrangeChunk.style.left = OrangeChunkStylePosition;
        GreenChunk.style.left = GreenChunkStylePosition;
        Needle.style.left = NeedleStylePosition;
    }
    private void SetHook()
    {
        player.ID.FishOnBait = true;
        EnterBoostState();
    }
    public IEnumerator WaitForbite()
    {
        float randTime = UnityEngine.Random.Range(2.5f, 3.5f);
        //yield�^�Ǩө���
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
    }
    private void OnDisable()
    {
        playerInput.Fishing.CastFishingRod.performed -= CastOrRetract;
        playerInput.Fishing.CastFishingRod.performed -= LandNeedle;
        playerInput.Fishing.Disable();
    }
    private void Start()
    {
        Zones = ZoneContainer.GetComponentsInChildren<ZoneDisplayer>();
        playerTransform = player.GetComponent<Transform>();
        ControlBarUI = PullStateUI.rootVisualElement.Q<VisualElement>("ControlBar");
        FishBarUI = PullStateUI.rootVisualElement.Q<VisualElement>("FishBar");
        ProgressBarUI = PullStateUI.rootVisualElement.Q<VisualElement>("Bar");
        OrangeChunk = BoostStateUI.rootVisualElement.Q<VisualElement>("OrangeChunk");
        GreenChunk = BoostStateUI.rootVisualElement.Q<VisualElement>("GreenChunk");
        Needle = BoostStateUI.rootVisualElement.Q<VisualElement>("Needle");
        PullStateUI.rootVisualElement.style.display = DisplayStyle.None;
        BoostStateUI.rootVisualElement.style.display = DisplayStyle.None;
        ControlBarAction = playerInput.Fishing.ControlFishingRod;
    }
    // Update is called once per frame
    private void Update()
    {
        ZoneCheck();
        BoostStateUpdateFunction();
        PullStateUpdateFunction();
        ControlPullingBar();
    }
   
    #endregion
}