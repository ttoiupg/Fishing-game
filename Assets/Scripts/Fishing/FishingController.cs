using Halfmoon.Utilities;
using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Cinemachine;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.UI;
using System.Threading.Tasks;

public class FishingController : PlayerSystem
{
    public float castRodCooldownlength = 3;
    public float retractDebounceLength = 0.6f;
    public ZoneDisplayer[] Zones;
    [Header("effect")]
    public Image fishHealthBar;
    public Sprite normalHealthBar;
    public Sprite whiteHealthBar;
    public Slider fishCatchTimer;
    public ScreenEffectsHandler screenEffectsHandler;
    public CinemachinePositionComposer cinemachineCamera;
    public GameObject ZoneContainer;
    public float fishMaxHealth = 100;
    public float fishHealth = 100;

    [Header("Pull state")]
    public bool pointerLanded = false;

    [Header("Sound effects")]
    public AudioClip attakFish;
    public AudioClip pedalFlip;
    public AudioClip pedalFlip2;
    public AudioClip pedalFlip3;
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
    private InputAction ControlBarAction;
    private Animator animator;

    private Countdowntimer castRodCooldownTimer;
    private Countdowntimer retractDebounceTimer;
    private Countdowntimer overlapFirstTimer;
    private Countdowntimer overlapSecondTimer;
    private Countdowntimer overlapThirdTimer;
    private Countdowntimer damageCooldownTimer;

    private List<Timer> timers;

    Coroutine FishingCoroutine;
    Coroutine PullCoroutine;

    [Header("Gamepad")]
    public float RumbleLowFreq = 0.25f;
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
    public async Task AttackFish()
    {
        Debug.Log("Attack with " + player.attackBuff.ToString() + "buff");
        float damage = player.damage * player.attackBuff;
        fishHealth -= damage;
        player.pullCanvaManager.FlipDownAll();
        player.canDamage = false;
        damageCooldownTimer.Reset();
        damageCooldownTimer.Start();
        fishHealthBar.sprite = whiteHealthBar;
        SoundFXManger.Instance.PlaySoundFXClip(attakFish, playerTransform, player.attackBuff);
        fishHealthBar.DOFillAmount(fishHealth / fishMaxHealth, 0.2f).SetEase(Ease.OutBack);
        await Task.Delay(100);
        fishHealthBar.sprite = normalHealthBar;
        if (fishHealth <= 0)
        {
            FishCatched();
        }
        player.attackBuff = 0.4f;
    }
    public void PullStateUpdateFunction()
    {
        //check for overlap
        if (IsOverlap(player.pullCanvaManager.controlBarPosition - player.ID.pullBarSize / 2, 
                      player.pullCanvaManager.controlBarPosition + player.ID.pullBarSize / 2, 
                      player.pullCanvaManager.fishNeedlePosition-14, player.pullCanvaManager.fishNeedlePosition + 14f))
        {
            if (player.canDamage)
            {
                if (!player.pullCanvaManager.secondFliped && !overlapFirstTimer.IsRunning)
                {
                    overlapFirstTimer.Reset(0.6f);
                    overlapFirstTimer.Start();
                }
                if (!player.pullCanvaManager.thirdFliped && !overlapSecondTimer.IsRunning)
                {
                    overlapSecondTimer.Reset(1.2f);
                    overlapSecondTimer.Start();
                }
                if (!overlapThirdTimer.IsRunning)
                {
                    overlapThirdTimer.Reset(1.5f);
                    overlapThirdTimer.Start();
                }
                if (player.pullCanvaManager.firstFliped == false)
                {
                    player.pullCanvaManager.FlipFirst();
                    SoundFXManger.Instance.PlaySoundFXClip(pedalFlip, playerTransform, 0.7f);
                }
            }
            player.pullCanvaManager.isFishBarOverlaping = true;
            player.pullCanvaManager.controlBar.GetComponent<Image>().color = new Color32(217, 77, 88, 255);
            player.pullCanvaManager.controlBar.GetComponent<Outline>().effectColor = new Color32(0, 135, 164, 255);
            //PullProgress += player.ID.pullProgressSpeed * Time.deltaTime;
        }
        else 
        {
            if (player.pullCanvaManager.isFishBarOverlaping && player.canDamage)
            {
                Task j = AttackFish();
            }
            overlapFirstTimer.Pause();
            overlapSecondTimer.Pause();
            overlapThirdTimer.Pause();
            player.pullCanvaManager.isFishBarOverlaping = false;
            player.pullCanvaManager.controlBar.GetComponent<Image>().color = new Color32(0, 135, 164, 255);
            player.pullCanvaManager.controlBar.GetComponent<Outline>().effectColor = new Color32(217, 77, 88, 255);
            //PullProgress -= player.ID.pullProgressLooseSpeed * Time.deltaTime;
            //if (PullProgress < 0)
            //{
            //    PullProgress = 0;
            //    StartCoroutine(FishFailed());
            //}
        }
        //ProgressBarUI.style.width = new StyleLength(Length.Percent(PullProgress));
        //if (PullProgress >= 100f)
        //{
        //    FishCatched();
        //}
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
        if (!player.discoveredFish.ContainsKey(player.currentFish.fishType.id))
        {
            //StartCoroutine(screenEffectsHandler.PlayFishFirstCatchAnimation(player.currentFish));
            DiscoveredFish discoveredFish = new DiscoveredFish(player.currentFish.fishType, System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            player.discoveredFish.Add(discoveredFish.baseFish.id,discoveredFish);
            player.ID.playerEvents.OnFishUnlocked.Invoke(player.currentFish.fishType);
            player.retrackDebounce = true;
            yield return new WaitForSeconds(0.6f);
            player.retrackDebounce = false;
        }
        else
        {
            player.discoveredFish[player.currentFish.fishType.id].timeCatched += 1;
            player.retrackDebounce = true;
            yield return new WaitForSeconds(0.6f);
            player.retrackDebounce = false;
        }
        player.experience += player.currentFish.fishType.Experience;
    }
    private void FishCatched()
    {
        player.hudController.StartLootTag(player.currentFish.fishType.Art,player.currentFish.fishType.name,"Mutation:"+player.currentFish.mutation.name,player.currentFish.weight.ToString()+"Kg");
        StartCoroutine(FishCatchedEffects());
        StartCoroutine(ProcessFish());
        //StopCoroutine(PullCoroutine);
        player.ID.playerEvents.OnExitFishingState?.Invoke();
        player.ID.playerEvents.OnFishCatched?.Invoke(player.currentFish);
        player.pullstate = false;
        player.booststate = false;
        player.fishing = false;
        player.retrackDebounce = true;
        retractDebounceTimer.Start();
    }
    private void FishFailed()
    {
        VisualFXManager.Instance.DestroyBobber();
        animator.SetTrigger("CatchingEnd");
        ReelSoundSource.Stop();
        //StopCoroutine(PullCoroutine);
        player.ID.playerEvents.OnExitFishingState?.Invoke();
        cinemachineCamera.CameraDistance = 8f;
        SoundFXManger.Instance.PlaySoundFXClip(FishFailedSoundFX, playerTransform, 1f);
        InputSystem.ResetHaptics();
        player.retrackDebounce = true;
        player.pullstate = false;
        player.booststate = false;
        player.fishing = false;
        player.retrackDebounce = true;
        retractDebounceTimer.Start();
    }
    public void ControlPullingBar()
    {
        float Value = ControlBarAction.ReadValue<float>();
        if (Value > 0)
        {
            animator.SetFloat("PullingSpeed", 1f + Value);
            ReelSoundSource.pitch = 0.7f + Value * 0.35f;
            //Gamepad.current?.SetMotorSpeeds(RumbleLowFreq, Value * 0.6f);
            player.pullCanvaManager.controlBarGravity = 1500f * Value;
            cinemachineCamera.CameraDistance = 8f - 2f * Value;
        }
        else
        {
            animator.SetFloat("PullingSpeed", 0.6f);
            ReelSoundSource.pitch = 0.7f;
            //Gamepad.current?.SetMotorSpeeds(RumbleLowFreq, 0);
            player.pullCanvaManager.controlBarGravity = -1500;
            cinemachineCamera.CameraDistance = 8f;
        }
    }
    public void LandPointer(InputAction.CallbackContext callbackContext)
    {
        if (!pointerLanded)
        {
            pointerLanded = true;
            string result = player.boostCanvaManager.LandPointer();
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
        player.attackBuff = 0.4f;
        BaseFish catchedBaseFish = RollForFish();
        BaseMutation catchedBaseMutation = RollForMutation();
        player.currentFish = new Fish(catchedBaseFish, catchedBaseMutation);
        fishHealth = fishMaxHealth - player.pullProgressBuff;
        fishHealthBar.fillAmount = fishHealth / fishMaxHealth;
        player.ID.playerEvents.OnPullStage?.Invoke();
        yield return new WaitForSeconds(0.5f);
        player.boostCanvaManager.HideBoostUI();
        player.pullCanvaManager.Init();
        player.pullCanvaManager.ShowUI();
        yield return new WaitForSeconds(0.6f);
        player.pullstate = true;
        //PullCoroutine = StartCoroutine(RandomFishBarPosition());
    }
    public void EnterBoostState()
    {
        player.boostCanvaManager.ShowBoostUI();
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
        overlapFirstTimer = new Countdowntimer(0.6f);
        overlapSecondTimer = new Countdowntimer(1.2f);
        overlapThirdTimer = new Countdowntimer(1.5f);
        damageCooldownTimer = new Countdowntimer(0.5f);
        timers = new List<Timer>(6) { castRodCooldownTimer, retractDebounceTimer, 
            overlapFirstTimer, overlapSecondTimer, 
            overlapThirdTimer, damageCooldownTimer};
        castRodCooldownTimer.OnTimerStop += () => player.castRodDebounce = false;
        retractDebounceTimer.OnTimerStop += () => player.retrackDebounce = false;
        overlapFirstTimer.OnTimerStop += () => {
            player.pullCanvaManager.FlipSecond();
            SoundFXManger.Instance.PlaySoundFXClip(pedalFlip, playerTransform, 0.7f);
        };
        overlapSecondTimer.OnTimerStop += () => {
            player.pullCanvaManager.FlipThird();
            SoundFXManger.Instance.PlaySoundFXClip(pedalFlip, playerTransform, 0.7f);
        };
        overlapThirdTimer.OnTimerStop += async () =>
        {
            await AttackFish();
        };
        damageCooldownTimer.OnTimerStop += () => player.canDamage = true;
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