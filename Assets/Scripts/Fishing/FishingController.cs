using Halfmoon.Utilities;
using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class FishingController : PlayerSystem
{
    public HUDController hudController;
    public BoostCanvaManager boostCanvaManager;
    public float castRodCooldownlength = 3;
    public float retractDebounceLength = 0.6f;
    public ZoneDisplayer[] Zones;
    [Header("effect")]
    public ScreenEffectsHandler screenEffectsHandler;
    public CinemachinePositionComposer cinemachineCamera;
    public GameObject ZoneContainer;

    [Header("Pull state")]
    public bool pointerLanded = false;

    [Header("Sound effects")]
    public AudioClip BiteNotify;
    public AudioClip LandOnGreenSoundFX;
    public AudioClip LandOnOrangeSoundFX;
    public AudioClip LandOnRedSoundFX;
    public AudioClip PullStateSoundFX;
    public AudioClip SpinningReel;
    public AudioClip FishCatchedSoundFX;
    public AudioClip FishFailedSoundFX;
    public AudioMixer FishingAudioMixer;

    private AudioSource ReelSoundSource;
    private List<BaseMutation> AvaliableMutations;
    private List<BaseFish> AvailableFishes;
    private Transform playerTransform;
    private Fish CurrentFish;
    private InputAction ControlBarAction;
    private Animator animator;
    private Countdowntimer castRodCooldownTimer;
    private Countdowntimer retractDebounceTimer;
    private List<Timer> timers;

    Coroutine FishingCoroutine;
    Coroutine PullCoroutine;

    [Header("Gamepad")]
    public float RumbleLowFreq = 0.25f;

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
    public void ZoneCheck()
    {
        for (int i = 0; i < Zones.Length; i++)
        {
            if (IsInside(Zones[i].zone.position, Zones[i].zone.size, new Vector2(playerTransform.position.x, playerTransform.position.z)))
            {
                player.currentZone = Zones[i].zone;
                break;
            }
            else
            {
                player.currentZone = null;
            };
        };
    }
    //public void PullStateUpdateFunction()
    //{
    //    //Update control bar's position
    //    UpdateControlBarPosition();
    //    GamepadVibration();
    //    //lerping fish bar's position
    //    FishBarPosition = Lerpfloat(FishBarPosition, FishBarTargetPosition, FishBarSpeed * Time.deltaTime);
    //    FishBarUI.style.left = new StyleLength(Length.Percent(FishBarPosition));
    //    //check for overlap
    //    if (IsOverlap(ControlBarPosition - player.ID.pullBarSize / 2, ControlBarPosition + player.ID.pullBarSize / 2, FishBarPosition, FishBarPosition + 2f))
    //    {
    //        IsFishBarOverlaping = true;
    //        ControlBarUI.style.borderBottomColor = new StyleColor(new Color32(0, 228, 199, 255));
    //        ControlBarUI.style.borderTopColor = new StyleColor(new Color32(0, 228, 199, 255));
    //        ControlBarUI.style.borderLeftColor = new StyleColor(new Color32(0, 228, 199, 255));
    //        ControlBarUI.style.borderRightColor = new StyleColor(new Color32(0, 228, 199, 255));


    //        PullProgress += player.ID.pullProgressSpeed * Time.deltaTime;
    //    }
    //    else
    //    {
    //        IsFishBarOverlaping = false;
    //        ControlBarUI.style.borderBottomColor = new StyleColor(new Color32(217, 77, 88, 255));
    //        ControlBarUI.style.borderTopColor = new StyleColor(new Color32(217, 77, 88, 255));
    //        ControlBarUI.style.borderLeftColor = new StyleColor(new Color32(217, 77, 88, 255));
    //        ControlBarUI.style.borderRightColor = new StyleColor(new Color32(217, 77, 88, 255));
    //        PullProgress -= player.ID.pullProgressLooseSpeed * Time.deltaTime;
    //        if (PullProgress < 0)
    //        {
    //            PullProgress = 0;
    //            StartCoroutine(FishFailed());
    //        }
    //    }
    //    ProgressBarUI.style.width = new StyleLength(Length.Percent(PullProgress));
    //    if (PullProgress >= 100f)
    //    {
    //        FishCatched();
    //    }
    //}
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
    private IEnumerator FishCatchedEffects()
    {
        VisualFXManager.Instance.DestroyBobber();
        animator.SetTrigger("FishCatched");
        SoundFXManger.Instance.PlaySoundFXClip(FishCatchedSoundFX, playerTransform, 0.7f);
        ReelSoundSource.Stop();
        cinemachineCamera.CameraDistance = 8f;
        Gamepad.current?.SetMotorSpeeds(1f, 1f);
        yield return new WaitForSeconds(0.2f);
        Gamepad.current?.SetMotorSpeeds(0, 0);
        yield return new WaitForSeconds(0.1f);
        Gamepad.current?.SetMotorSpeeds(1f, 1f);
        yield return new WaitForSeconds(0.1f);
        InputSystem.ResetHaptics();
    }
    private IEnumerator ProcessFish()
    {
        if (!player.discoveredFish.Exists((x) => x.baseFish == CurrentFish.fishType))
        {
            StartCoroutine(screenEffectsHandler.PlayFishFirstCatchAnimation(CurrentFish));
            DiscoveredFish discoveredFish = new DiscoveredFish(CurrentFish.fishType, System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            player.discoveredFish.Add(discoveredFish);
            player.ID.playerEvents.OnFishUnlocked.Invoke(CurrentFish.fishType);
        }
        else
        {
            player.retrackDebounce = true;
            yield return new WaitForSeconds(0.6f);
            player.retrackDebounce = false;
        }
        player.experience += CurrentFish.fishType.Experience;
    }
    private void FishCatched()
    {
        hudController.StartLootTag(CurrentFish.fishType.Art,CurrentFish.fishType.name,"Mutation:"+CurrentFish.mutation.name,CurrentFish.weight.ToString()+"Kg");
        StartCoroutine(FishCatchedEffects());
        StartCoroutine(ProcessFish());
        StopCoroutine(PullCoroutine);
        player.ID.playerEvents.OnExitFishingState?.Invoke();
        player.ID.playerEvents.OnFishCatched?.Invoke(CurrentFish);
        player.pullstate = false;
        player.booststate = false;
        player.fishing = false;
    }
    private IEnumerator FishFailed()
    {
        VisualFXManager.Instance.DestroyBobber();
        animator.SetTrigger("CatchingEnd");
        ReelSoundSource.Stop();
        StopCoroutine(PullCoroutine);
        player.ID.playerEvents.OnExitFishingState?.Invoke();
        cinemachineCamera.CameraDistance = 8f;
        SoundFXManger.Instance.PlaySoundFXClip(FishFailedSoundFX, playerTransform, 1f);
        InputSystem.ResetHaptics();
        player.retrackDebounce = true;
        player.pullstate = false;
        player.booststate = false;
        player.fishing = false;
        yield return new WaitForSeconds(0.6f);
        player.retrackDebounce = false;
    }
    //public void ControlPullingBar()
    //{
    //    float Value = ControlBarAction.ReadValue<float>();
    //    if (Value > 0)
    //    {
    //        animator.SetFloat("PullingSpeed", 1f + Value);
    //        ReelSoundSource.pitch = 0.7f + Value * 0.35f;
    //        //Gamepad.current?.SetMotorSpeeds(RumbleLowFreq, Value * 0.6f);
    //        ControlBarGravity = 300f * Value;
    //        cinemachineCamera.CameraDistance = 8f - 2f * Value;
    //    }
    //    else
    //    {
    //        animator.SetFloat("PullingSpeed", 0.6f);
    //        ReelSoundSource.pitch = 0.7f;
    //        //Gamepad.current?.SetMotorSpeeds(RumbleLowFreq, 0);
    //        ControlBarGravity = -300f;
    //        cinemachineCamera.CameraDistance = 8f;
    //    }
    //}
    public void LandPointer(InputAction.CallbackContext callbackContext)
    {
        if (!pointerLanded)
        {
            pointerLanded = true;
            string result = boostCanvaManager.LandPointer();
            if (result == "green")
            {
                player.pullProgressBuff = player.ID.GreenZonebuff;
                SoundFXManger.Instance.PlaySoundFXClip(LandOnGreenSoundFX, playerTransform, 0.45f);
            }
            else if (result == "orange")
            {
                player.pullProgressBuff = player.ID.OrangeZonebuff;
                SoundFXManger.Instance.PlaySoundFXClip(LandOnOrangeSoundFX, playerTransform, 0.4f);
            }
            else
            {
                player.pullProgressBuff = 10f;
                SoundFXManger.Instance.PlaySoundFXClip(LandOnRedSoundFX, playerTransform, 1.2f);
            }
            player.booststate = false;
            StartCoroutine(EnterPullState());
        }
    }

    private IEnumerator EnterPullState()
    {
        BaseFish catchedBaseFish = RollForFish();
        BaseMutation catchedBaseMutation = RollForMutation();
        CurrentFish = new Fish(catchedBaseFish, catchedBaseMutation);
        player.ID.playerEvents.OnPullStage?.Invoke();
        yield return new WaitForSeconds(0.5f);
        boostCanvaManager.HideBoostUI();
        player.pullstate = true;
        //PullCoroutine = StartCoroutine(RandomFishBarPosition());
    }
    public void EnterBoostState()
    {
        boostCanvaManager.ShowBoostUI();
        pointerLanded = false;
        player.ID.playerEvents.OnBoostStage?.Invoke();
    }
    private void SetHook()
    {
        animator.SetTrigger("FishBite");
        ReelSoundSource.Play();
        SoundFXManger.Instance.PlaySoundFXClip(BiteNotify, playerTransform, 1f);
        cinemachineCamera.CameraDistance = 7f;
        Gamepad.current?.SetMotorSpeeds(RumbleLowFreq, 0);
        player.booststate = true;
    }
    public IEnumerator WaitForbite()
    {
        float randTime = UnityEngine.Random.Range(3.5f, 6f);
        yield return new WaitForSeconds(randTime);
        SetHook();
    }

    #endregion
    #region Casting fishing rod
    public void CastOrRetract(InputAction.CallbackContext callbackContext)
    {
        if (player.currentZone != null)
        {
            if (!player.fishing)
            {
                if (!player.castRodDebounce)
                {
                    player.castRodDebounce = true;
                    player.fishing = true;
                    CastFishingRod();
                    castRodCooldownTimer.Start();
                }
            }
            else
            {
                if (!player.castRodDebounce)
                {
                    player.retrackDebounce = true;
                    player.fishing = false;
                    RetractFishingRod();
                    retractDebounceTimer.Start();
                }
            }
        }
    }

    public void CastFishingRod()
    {
        animator.SetBool("IsMoving", false);
        player.ID.playerEvents.OnEnterFishingState?.Invoke();
        AvailableFishes = player.currentZone.GetSortedFeaturedFish();
        AvaliableMutations = player.currentZone.GetSortedFeaturedMutations();
        if (FishingCoroutine != null)
        {
            StopCoroutine(FishingCoroutine);
        }
        animator.SetTrigger("CastFishingRod");
        cinemachineCamera.CameraDistance = 12f;
        VisualFXManager.Instance.SpawnBobber(playerTransform.position + new Vector3(0, 2f, 0), player.Facing);
        FishingCoroutine = StartCoroutine(WaitForbite());
    }
    public void RetractFishingRod()
    {
        Debug.Log("retract");
        if (FishingCoroutine != null)
        {
            StopCoroutine(FishingCoroutine);
        }
        animator.SetTrigger("RetractFishingRod");
        cinemachineCamera.CameraDistance = 8f;
        player.ID.playerEvents.OnExitFishingState?.Invoke();
        VisualFXManager.Instance.DestroyBobber();
    }

    #endregion
    #region Unity functions
    public override void Awake()
    {
        player = transform.root.GetComponent<Player>();
        playerInput = new PlayerInputActions();
        castRodCooldownTimer = new Countdowntimer(castRodCooldownlength);
        retractDebounceTimer = new Countdowntimer(retractDebounceLength);
        timers = new List<Timer>(2) { castRodCooldownTimer, retractDebounceTimer };
        castRodCooldownTimer.OnTimerStop += () => player.castRodDebounce = false;
        retractDebounceTimer.OnTimerStop += () => player.retrackDebounce = false;
    }
    void HandleTimer()
    {
        foreach (var timer in timers)
        {
            timer.Tick(Time.deltaTime);
        }
    }
    private void OnEnable()
    {
        playerInput.Fishing.Enable();
    }
    private void OnDisable()
    {
        playerInput.Fishing.Disable();
    }
    private void Start()
    {
        animator = player.GetComponent<Animator>();
        ReelSoundSource = GetComponent<AudioSource>();
        Zones = ZoneContainer.GetComponentsInChildren<ZoneDisplayer>();
        playerTransform = player.GetComponent<Transform>();
        ControlBarAction = playerInput.Fishing.ControlFishingRod;
    }
    // Update is called once per frame
    private void Update()
    {
        HandleTimer();
    }
    private void OnApplicationQuit()
    {
        InputSystem.ResetHaptics();
    }
    #endregion
}