using System;
using Halfmoon.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.Serialization;
using Cysharp.Threading.Tasks;
using Random = UnityEngine.Random;
using Timer = Halfmoon.Utilities.Timer;

public class FishingController : PlayerSystem
{
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    private static readonly int CastFishingRod = Animator.StringToHash("CastFishingRod");
    private static readonly int RetractFishingRod = Animator.StringToHash("RetractFishingRod");
    public BiteNoticeUIManager biteNoticeUIManager;
    public float castRodCooldownlength = 3;
    public float retractDebounceLength = 0.6f;
    public float FishingCameraDistance = 10f;
    public float pullProgress;
    public bool castDebounce;
    public bool retractDebounce;
    public bool isFishing;
    public bool fishOnBait;
    public bool boostApplied;

    [FormerlySerializedAs("controlBarOverlaping")]
    public bool ReelingBarOverlaping;

    public Vector3 bobberOffset = new(0, 2f, 0);
    [FormerlySerializedAs("Zones")] public ZoneDisplayer[] zones;
    [Header("effect")] public Image fishHealthBar;
    public Sprite normalHealthBar;
    public Sprite whiteHealthBar;

    [FormerlySerializedAs("ZoneContainer")]
    public GameObject zoneContainer;

    public Fish currentFish;
    public float fishMaxHealth = 100;
    public float fishHealth = 100;

    [Header("Pull state")] public bool pointerLanded;

    [FormerlySerializedAs("attakFish")] [Header("Sound effects")]
    public AudioClip attackFish;

    public AudioClip biteNotifySound;

    public AudioClip pedalFlip;
    [FormerlySerializedAs("BiteNotify")] public AudioClip biteNotify;

    [FormerlySerializedAs("LandOnGreenSoundFX")]
    public AudioClip landOnGreenSoundFX;

    [FormerlySerializedAs("LandOnOrangeSoundFX")]
    public AudioClip landOnOrangeSoundFX;

    [FormerlySerializedAs("LandOnRedSoundFX")]
    public AudioClip landOnRedSoundFX;

    [FormerlySerializedAs("FishCatchedSoundFX")]
    public AudioClip fishCatchedSoundFX;

    [FormerlySerializedAs("FishFailedSoundFX")]
    public AudioClip fishFailedSoundFX;

    private AudioSource _reelSoundSource;
    private Transform _playerTransform;
    private InputAction _controlBarAction;
    private Animator _animator;
    private Countdowntimer _castRodCooldownTimer;
    private Countdowntimer _retractDebounceTimer;
    private Countdowntimer _damageCooldownTimer;
    public SectionTimer damageBoostTimer;
    private CancellationTokenSource _castRodCancellationTokenSource;
    private List<Timer> _timers;
    private Vector2 _biteNoticeScreenPosition = new Vector2(-0.3777781f, 0.09147596f);

    [FormerlySerializedAs("RumbleLowFreq")] [Header("Gamepad")]
    public float rumbleLowFreq = 0.25f;

    private static bool _isInside(Vector2 a, Vector2 sizeA, Vector2 b)
    {
        var result = a.x - sizeA.x / 2f <= b.x && a.x + sizeA.x / 2f >= b.x && a.y - sizeA.y / 2f <= b.y &&
                     a.y + sizeA.y / 2f >= b.y;
        return result;
    }

    private bool _isReelingBarOverlap()
    {
        var a1 = player.ReelCanvaManager.controlBarPosition - player.ID.pullBarSize / 2;
        var a2 = player.ReelCanvaManager.controlBarPosition + player.ID.pullBarSize / 2;
        var b1 = player.ReelCanvaManager.fishNeedlePosition - 14;
        var b2 = player.ReelCanvaManager.fishNeedlePosition + 14f;
        if (b2 > a2) return a2 >= b1;
        if (b2 < a1) return a1 <= b2;
        return true;
    }

    private void ConfigureCamera(float distance)
    {
        player.cinemachineCamera.CameraDistance = distance;
    }

    private void ConfigureCamera(float distance, Vector2 position)
    {
        player.cinemachineCamera.CameraDistance = distance;
        player.cinemachineCamera.Composition.ScreenPosition = position;
    }

    public void HandleInput(InputAction.CallbackContext callbackContext)
    {
        if (player.currentZone == null) return;
        if (!isFishing)
        {
            if (castDebounce) return;
            castDebounce = true;
            isFishing = true;
            StartFishing();
            _castRodCooldownTimer.Start();
        }
        else
        {
            if (castDebounce) return;
            castDebounce = true;
            isFishing = false;
            StopFishing();
            _retractDebounceTimer.Start();
        }
    }

    public void ZoneCheck()
    {
        foreach (var t in zones)
        {
            if (_isInside(t.zone.position, t.zone.size,
                    new Vector2(_playerTransform.position.x, _playerTransform.position.z)))
            {
                player.currentZone = t.zone;
                break;
            }
            else
            {
                player.currentZone = null;
            }
        }
    }

    #region UnityFunction

    private void SetUpTimers()
    {
        TimerSection[] sections = { new(0), new(0.6f), new(1.2f), new(1.5f) };
        _castRodCooldownTimer = new Countdowntimer(castRodCooldownlength);
        _retractDebounceTimer = new Countdowntimer(retractDebounceLength);
        damageBoostTimer = new SectionTimer(sections);
        _damageCooldownTimer = new Countdowntimer(0.5f);
        _timers = new List<Timer>(5)
            { _castRodCooldownTimer, _retractDebounceTimer, damageBoostTimer, _damageCooldownTimer };
        damageBoostTimer.OnSectionMeet += DamageSectionMet;
        damageBoostTimer.OnTimerStop += AttackFish;
        _castRodCooldownTimer.OnTimerStop += () => castDebounce = false;
        _retractDebounceTimer.OnTimerStop += () => castDebounce = false;
        _damageCooldownTimer.OnTimerStop += () => player.canDamage = true;
    }

    public override void Awake()
    {
        player = transform.root.GetComponent<Player>();
        playerInput = new DefaultInputActions();
        SetUpTimers();
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
        _animator = player.GetComponent<Animator>();
        _reelSoundSource = GetComponent<AudioSource>();
        zones = zoneContainer.GetComponentsInChildren<ZoneDisplayer>();
        _playerTransform = player.GetComponent<Transform>();
        _controlBarAction = playerInput.Fishing.ControlFishingRod;
        player.ID.playerEvents.OnFishCatched += fish => { FishCatched(); };
        player.ID.playerEvents.OnFishFailed += () => FishFailed();
    }

    private void Update()
    {
        if (Input.anyKeyDown && Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            //HandleInput(new InputAction.CallbackContext());
        }
        TickTimer();
    }

    #endregion

    #region StartFishing

    private void PreparePlayerForFishing()
    {
        _animator.SetBool(IsMoving, false);
        player.ID.playerEvents.OnEnterFishingState?.Invoke();
    }

    private void SpawnFishingBobber()
    {
        VisualFXManager.Instance.SpawnBobber(_playerTransform.position + bobberOffset, player.Facing);
    }

    private async void StartFishing()
    {
        Reset();
        PreparePlayerForFishing();
        ConfigureCamera(FishingCameraDistance);
        SpawnFishingBobber();
        _castRodCancellationTokenSource ??= new CancellationTokenSource();
        _animator.SetTrigger(CastFishingRod);
        await WaitForbite(_castRodCancellationTokenSource.Token);
    }

    #endregion

    #region StopFishing

    private void CleanCencellationToken()
    {
        _castRodCancellationTokenSource.Cancel();
        _castRodCancellationTokenSource.Dispose();
        _castRodCancellationTokenSource = null;
    }

    private void CleanFishingBobber()
    {
        VisualFXManager.Instance.DestroyBobber();
    }

    private void StopFishing()
    {
        CleanCencellationToken();
        _animator.SetTrigger(RetractFishingRod);
        ConfigureCamera(player.defaultCameraDistance);
        CleanFishingBobber();
    }

    #endregion

    #region AttackFish

    private void PlayAttackSound()
    {
        SoundFXManger.Instance.PlaySoundFXClip(attackFish, _playerTransform, player.attackBuff);
    }

    private void DoDamage()
    {
        var fishingRod = player.GetFishingRod();
        var info = fishingRod.GetDamage();
        GameManager.Instance.CurrentBattle.Attack(info);
    }

    private void AttackFish()
    {
        player.canDamage = false;
        _damageCooldownTimer.Start();
        PlayAttackSound();
        DoDamage();
    }

    private async UniTask FishCatchedEffects()
    {
        VisualFXManager.Instance.DestroyBobber();
        _animator.SetTrigger("FishCatched");
        SoundFXManger.Instance.PlaySoundFXClip(fishCatchedSoundFX, _playerTransform, 0.7f);
        _reelSoundSource.Stop();
        ConfigureCamera(player.defaultCameraDistance);
        Gamepad.current?.SetMotorSpeeds(1f, 1f);
        await UniTask.Delay(200);
        Gamepad.current?.SetMotorSpeeds(0, 0);
        await UniTask.Delay(100);
        Gamepad.current?.SetMotorSpeeds(1f, 1f);
        await UniTask.Delay(100);
        InputSystem.ResetHaptics();
    }

    private void FishCatched()
    {
        FishCatchedEffects();
        Reset();
        isFishing = false;
        castDebounce = true;
        _retractDebounceTimer.Start();
    }

    private void FishFailed()
    {
        VisualFXManager.Instance.DestroyBobber();
        _animator.SetTrigger("CatchingEnd");
        _reelSoundSource.Stop();
        ConfigureCamera(player.defaultCameraDistance);
        SoundFXManger.Instance.PlaySoundFXClip(fishFailedSoundFX, _playerTransform, 1f);
        InputSystem.ResetHaptics();
        Reset();
        isFishing = false;
        castDebounce = true;
        _retractDebounceTimer.Start();
    }

    #endregion

    #region ReelState

    public void ControlReelingBar()
    {
        var Value = _controlBarAction.ReadValue<float>();
        if (Value > 0)
        {
            _animator.SetFloat("PullingSpeed", 1f + Value);
            _reelSoundSource.pitch = 0.7f + Value * 0.35f;
            //Gamepad.current?.SetMotorSpeeds(RumbleLowFreq, Value * 0.6f);
            player.ReelCanvaManager.controlBarGravity = 1500f * Value;
            ConfigureCamera(8f - 2f * Value);
        }
        else
        {
            _animator.SetFloat("PullingSpeed", 0.6f);
            _reelSoundSource.pitch = 0.7f;
            //Gamepad.current?.SetMotorSpeeds(RumbleLowFreq, 0);
            player.ReelCanvaManager.controlBarGravity = -1500;
            ConfigureCamera(player.defaultCameraDistance);
        }
    }

    public void ReelStateUpdateFunction()
    {
        var overlap = _isReelingBarOverlap();
        GameManager.Instance.CurrentBattle.SetOverlapping(overlap);
        if (!player.canDamage) return;
        switch (overlap)
        {
            case true when !damageBoostTimer.IsRunning:
                damageBoostTimer.Reset();
                damageBoostTimer.Resume();
                break;
            case false when ReelingBarOverlaping:
                //AttackFish();
                damageBoostTimer.Stop();
                break;
        }

        ReelingBarOverlaping = overlap;
        player.ReelCanvaManager.SwitchReelingBarColor(ReelingBarOverlaping);
        player.ReelCanvaManager.RandomFishBarPosition();
    }

    private void PrepareReelUI()
    {
        player.ReelCanvaManager.ResetTimer();
        player.boostCanvaManager.HideBoostUI();
        player.ReelCanvaManager.Init();
        player.ReelCanvaManager.ShowUI();
    }

    private async UniTask EnterReelState()
    {
        GameManager.Instance.FishEnemy = new FishEnemy();
        GameManager.Instance.NewBattle(BattleType.Fish,GameManager.Instance.FishEnemy);
        PrepareReelUI();
        await UniTask.WaitForSeconds(1.38f);
        GameManager.Instance.StartBattle();
        player.ReelCanvaManager.StartTimer(15f);
    }

    #endregion

    #region BoostState

    public async void LandPointer(InputAction.CallbackContext callbackContext)
    {
        if (pointerLanded) return;
        pointerLanded = true;
        var result = player.boostCanvaManager.LandPointer();
        switch (result)
        {
            case "green":
                player.pullProgressBuff = player.ID.GreenZonebuff;
                SoundFXManger.Instance.PlaySoundFXClip(landOnGreenSoundFX, _playerTransform, 0.45f);
                break;
            case "orange":
                player.pullProgressBuff = player.ID.OrangeZonebuff;
                SoundFXManger.Instance.PlaySoundFXClip(landOnOrangeSoundFX, _playerTransform, 0.4f);
                break;
            default:
                player.pullProgressBuff = 10f;
                SoundFXManger.Instance.PlaySoundFXClip(landOnRedSoundFX, _playerTransform, 1.2f);
                break;
        }

        await UniTask.WaitForSeconds(0.5f);
        EnterReelState();
        boostApplied = true;
    }

    public void EnterBoostState()
    {
        player.boostCanvaManager.ShowBoostUI();
        pointerLanded = false;
        player.ID.playerEvents.OnBoostStage?.Invoke();
    }

    private async UniTask SetHook()
    {
        castDebounce = true;
        biteNoticeUIManager.StartAnimation();
        SoundFXManger.Instance.PlaySoundFXClip(biteNotifySound, _playerTransform, 1f);
        ConfigureCamera(2.15f, _biteNoticeScreenPosition);
        await UniTask.WaitForSeconds(2);
        fishOnBait = true;
        castDebounce = false;
        ConfigureCamera(6f, Vector2.zero);
        _animator.SetTrigger("FishBite");
        _reelSoundSource.Play();
        SoundFXManger.Instance.PlaySoundFXClip(biteNotify, _playerTransform, 1f);
        Gamepad.current?.SetMotorSpeeds(rumbleLowFreq, 0);
    }

    #endregion

    #region WaitState

    private async UniTask WaitForbite(CancellationToken token)
    {
        var randTime = (int)(Random.Range(3.5f, 6f) * 1000);
        try
        {
            await UniTask.Delay(randTime, false, PlayerLoopTiming.Update, token);
            SetHook();
        }
        catch (OperationCanceledException)
        {
        }
    }

    #endregion

    private void Reset()
    {
        fishOnBait = false;
        boostApplied = false;
        currentFish = null;
    }

    private void DamageSectionMet(int index)
    {
        switch (index)
        {
            case 0:
                player.ReelCanvaManager.FlipFirst();
                SoundFXManger.Instance.PlaySoundFXClip(pedalFlip, _playerTransform, 0.7f);
                GameManager.Instance.CurrentBattle.SetDamageStage(DamageStage.Stage1);
                break;
            case 1:
                player.ReelCanvaManager.FlipSecond();
                SoundFXManger.Instance.PlaySoundFXClip(pedalFlip, _playerTransform, 0.7f);
                GameManager.Instance.CurrentBattle.SetDamageStage(DamageStage.Stage2);
                break;
            case 2:
                player.ReelCanvaManager.FlipThird();
                SoundFXManger.Instance.PlaySoundFXClip(pedalFlip, _playerTransform, 0.7f);
                GameManager.Instance.CurrentBattle.SetDamageStage(DamageStage.Stage3);
                break;
            case 3:
                //AttackFish();
                damageBoostTimer.Stop();
                break;
        }
    }

    private void TickTimer()
    {
        foreach (var timer in _timers)
        {
            timer.Tick(Time.deltaTime);
        }
    }

    private void OnApplicationQuit()
    {
        InputSystem.ResetHaptics();
    }
}