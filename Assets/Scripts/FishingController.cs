using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using TMPro;
using Unity.Cinemachine;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.iOS;
using UnityEngine.UIElements;

public class FishingController : PlayerSystem
{
    [Header("effect")]
    public CinemachinePositionComposer camera;
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

    [Header("Pull state")]
    public StyleLength ControlBarStylePosition;
    public float ControlBarPosition = 50f;
    public float ControlBarGravity = 0f;
    public float ControlBarVelocity = 0f;
    public StyleLength FishBarStylePosition;
    public float FishBarTargetPosition = 0f;
    public float FishBarPosition = 0f;
    public float PullProgress = 0f;
    public float FishBarSpeed = 6f;
    public bool IsFishBarOverlaping = false;
    [Header("Boost state")]
    public StyleLength OrangeChunkStylePosition;
    public StyleLength GreenChunkStylePosition;
    public StyleLength NeedleStylePosition;
    public float OrangeChunkPosition;
    public float GreenChunkPosition;
    public float NeedlePosition;
    public float needleSpeed = 3f;
    public int needleDirection = 1;

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
    private ZoneDisplayer[] Zones;
    private Transform playerTransform;
    private Fish CurrentFish;
    private InputAction ControlBarAction;
    private Animator animator;
    private Camera Camera;

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
    private void GamepadVibration()
    {
        float lowfreq = 0.5f;
        float highfreq = ControlBarAction.ReadValue<float>() * 0.6f;
        if (IsFishBarOverlaping)
        {
            lowfreq = 0f;
        }
        if (Gamepad.current != null)
        {
            if (Gamepad.current?.name != "DualShock4GamepadHID")
            {
                Gamepad.current?.SetMotorSpeeds(lowfreq, highfreq);
            }
        }
    }
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
        ControlBarStylePosition = new StyleLength(Length.Percent((float)ControlBarPosition - player.ID.pullBarSize / 2));
        ControlBarUI.style.left = ControlBarStylePosition;
        //Debug.Log(ControlBarPosition);
    }
    private void ZoneCheck()
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
    private void BoostStateUpdateFunction()
    {
        if (player.isBoostState == true)
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
        if (player.isPullState == true)
        {
            //Update control bar's position
            UpdateControlBarPosition();
            GamepadVibration();
            //lerping fish bar's position
            FishBarPosition = Lerpfloat(FishBarPosition, FishBarTargetPosition, FishBarSpeed * Time.deltaTime);
            FishBarUI.style.left = new StyleLength(Length.Percent(FishBarPosition));
            //check for overlap
            if (IsOverlap(ControlBarPosition - player.ID.pullBarSize / 2, ControlBarPosition + player.ID.pullBarSize / 2, FishBarPosition, FishBarPosition + 2f))
            {
                IsFishBarOverlaping = true;
                ControlBarUI.style.borderBottomColor = new StyleColor(new Color32(0,228,199,255));
                ControlBarUI.style.borderTopColor = new StyleColor(new Color32(0, 228, 199, 255));
                ControlBarUI.style.borderLeftColor = new StyleColor(new Color32(0, 228, 199, 255));
                ControlBarUI.style.borderRightColor = new StyleColor(new Color32(0, 228, 199, 255));


                PullProgress += player.ID.pullProgressSpeed * Time.deltaTime;
            }
            else
            {
                IsFishBarOverlaping = false;
                ControlBarUI.style.borderBottomColor = new StyleColor(new Color32(217, 77, 88, 255));
                ControlBarUI.style.borderTopColor = new StyleColor(new Color32(217, 77, 88, 255));
                ControlBarUI.style.borderLeftColor = new StyleColor(new Color32(217, 77, 88, 255));
                ControlBarUI.style.borderRightColor = new StyleColor(new Color32(217, 77, 88, 255));
                PullProgress -= player.ID.pullProgressLooseSpeed * Time.deltaTime;
                if (PullProgress < 0) { 
                    PullProgress = 0;
                    FishFailed();
                }
            }
            ProgressBarUI.style.width = new StyleLength(Length.Percent(PullProgress));
            if (PullProgress >= 100f)
            {
                StartCoroutine(FishCatched());
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
    private IEnumerator FishCatched()
    {
        VisualFXManager.Instance.DestroyBobber();
        animator.SetTrigger("FishCatched");
        SoundFXManger.Instance.PlaySoundFXClip(FishCatchedSoundFX, playerTransform, 0.8f);
        ReelSoundSource.Stop();
        StopCoroutine(PullCoroutine);
        player.FishOnBait = false;
        player.isFishing = false;
        player.isPullState = false;
        PullStateUI.rootVisualElement.style.display = DisplayStyle.None;
        player.ID.playerEvents.OnExitFishingState?.Invoke();
        player.ID.playerEvents.OnFishCatched?.Invoke(CurrentFish);
        PullProgress = 0f;
        ControlBarPosition = 50f;
        FishBarPosition = 49f;
        FishBarTargetPosition = 49;
        camera.CameraDistance = 10f;
        Gamepad.current?.SetMotorSpeeds(1f, 1f);
        yield return new WaitForSeconds(0.2f);
        Gamepad.current?.SetMotorSpeeds(0, 0);
        yield return new WaitForSeconds(0.1f);
        Gamepad.current?.SetMotorSpeeds(1f, 1f);
        yield return new WaitForSeconds(0.1f);
        InputSystem.ResetHaptics();
    }
    private void FishFailed()
    {
        VisualFXManager.Instance.DestroyBobber();
        animator.SetTrigger("CatchingEnd");
        ReelSoundSource.Stop();
        StopCoroutine(PullCoroutine);
        player.FishOnBait = false;
        player.isFishing = false;
        player.isPullState = false;
        player.ID.playerEvents.OnExitFishingState?.Invoke();
        PullStateUI.rootVisualElement.style.display = DisplayStyle.None;
        PullProgress = 0f;
        ControlBarPosition = 50f;
        FishBarPosition = 49f;
        FishBarTargetPosition = 49;
        camera.CameraDistance = 10f;
        SoundFXManger.Instance.PlaySoundFXClip(FishFailedSoundFX, playerTransform, 1f);
        InputSystem.ResetHaptics();
    }
    public void ControlPullingBar()
    {
        if (player.isPullState == true)
        {
            float Value = ControlBarAction.ReadValue<float>();
            if (Value > 0)
            {
                animator.SetFloat("PullingSpeed",1f + Value);
                ReelSoundSource.pitch = 0.7f+Value*0.35f;
                //Gamepad.current?.SetMotorSpeeds(RumbleLowFreq, Value * 0.6f);
                ControlBarGravity = 300f * Value;
                camera.CameraDistance = 10f - 4f * Value;
            }
            else
            {
                animator.SetFloat("PullingSpeed", 0.6f);
                ReelSoundSource.pitch = 0.7f;
                //Gamepad.current?.SetMotorSpeeds(RumbleLowFreq, 0);
                ControlBarGravity = -300f;
                camera.CameraDistance = 10f;
            }
        }
    }
    public IEnumerator RandomFishBarPosition()
    {
        while (player.isPullState)
        {
            float RandSecond = UnityEngine.Random.Range(CurrentFish.fishType.MinFishBarChangeTime, CurrentFish.fishType.MaxFishBarChangeTime);

            float RandPosition = UnityEngine.Random.Range(0, 98f);
            while (Math.Abs(RandPosition - FishBarTargetPosition) > CurrentFish.fishType.MaxFishBarChangeDistance)
            {
                RandPosition = UnityEngine.Random.Range(0, 98f);
            }
            FishBarTargetPosition = UnityEngine.Random.Range(0, 98f);
            FishBarSpeed = UnityEngine.Random.Range(0.05f, 6f);
            yield return new WaitForSeconds(RandSecond);
        }
    }
    private IEnumerator EnterPullState(float Buff)
    {
        BaseFish catchedBaseFish = RollForFish();
        BaseMutation catchedBaseMutation = RollForMutation();
        CurrentFish = new Fish(catchedBaseFish, catchedBaseMutation);
        player.ID.playerEvents.OnPullStage?.Invoke();
        player.isBoostState = false;
        PullProgress = Buff;
        ControlBarPosition = 50f;
        ControlBarGravity = 0f;
        ProgressBarUI.style.width = new StyleLength(Length.Percent(PullProgress));
        ControlBarUI.style.width = new StyleLength(Length.Percent(player.ID.pullBarSize));
        ControlBarStylePosition = new StyleLength(Length.Percent((float)ControlBarPosition - player.ID.pullBarSize / 2));
        FishBarUI.style.left = new StyleLength(Length.Percent(FishBarPosition));
        ControlBarUI.style.left = ControlBarStylePosition;
        PullStateUI.rootVisualElement.style.display = DisplayStyle.Flex;
        yield return new WaitForSeconds(0.5f);
        player.isPullState = true;
        PullCoroutine = StartCoroutine(RandomFishBarPosition());
    }
    public void LandNeedle(InputAction.CallbackContext callbackContext)
    {
        StartCoroutine(LandNeedleCoroutine());
    }
    public IEnumerator LandNeedleCoroutine()
    {
        //Debug.Log(player.ID.isBoostState);
        if (player.isBoostState && player.FishOnBait)
        {
            
            player.isBoostState = false;
            float buff = 10f;
            if (IsOverlap(GreenChunkPosition, GreenChunkPosition + 20f, NeedlePosition, NeedlePosition + 4f))
            {

                SoundFXManger.Instance.PlaySoundFXClip(LandOnGreenSoundFX, playerTransform, 0.65f);
                buff = player.ID.GreenZonebuff;
            }
            else if (IsOverlap(OrangeChunkPosition, OrangeChunkPosition + 44f, NeedlePosition, NeedlePosition + 4f))
            {
                SoundFXManger.Instance.PlaySoundFXClip(LandOnOrangeSoundFX, playerTransform, 0.6f);
                buff = player.ID.OrangeZonebuff;
            }
            else
            {
                SoundFXManger.Instance.PlaySoundFXClip(LandOnRedSoundFX, playerTransform, 1f);
            }
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(EnterPullState(buff));
            BoostStateUI.rootVisualElement.style.display = DisplayStyle.None;
        };
    }
    private void EnterBoostState()
    {
        player.ID.playerEvents.OnBoostStage?.Invoke();
        player.isBoostState = true;
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
        animator.SetTrigger("FishBite");
        ReelSoundSource.Play();
        SoundFXManger.Instance.PlaySoundFXClip(BiteNotify, playerTransform, 1f);
        player.FishOnBait = true;
        camera.CameraDistance = 8f;
        Gamepad.current?.SetMotorSpeeds(RumbleLowFreq, 0);
        EnterBoostState();
    }
    public IEnumerator WaitForbite()
    {
        yield return new WaitForSeconds(3f);
        player.canRetract = true;
        float randTime = UnityEngine.Random.Range(3.5f, 6f);
        yield return new WaitForSeconds(randTime);
        SetHook();
    }

    #endregion
    #region Casting fishing rod
    public void CastOrRetract(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed == true)
        {
            if (player.FishOnBait == false)
            {
                if (player.isFishing == false && player.canFish)
                {
                    if (player.currentZone != null)
                    {
                        animator.SetBool("IsMoving", false);
                        player.isFishing = true;
                        player.ID.playerEvents.OnEnterFishingState.Invoke();
                        player.canRetract = false;
                        AvailableFishes = player.currentZone.GetSortedFeaturedFish();
                        AvaliableMutations = player.currentZone.GetSortedFeaturedMutations();

                        if (FishingCoroutine != null)
                        {
                            StopCoroutine(FishingCoroutine);
                        }
                        animator.SetTrigger("CastFishingRod");
                        camera.CameraDistance = 14f;
                        Invoke("CastFishingRod", 0.65f);
                    }
                }
                else if (player.isFishing)
                {
                    RetractFishingRod();
                }
            };
        }
    }
    public void CastFishingRod()
    {
        if (player.currentZone != null)
        {
            VisualFXManager.Instance.SpawnBobber(playerTransform.position + new Vector3(0,2f,0),player.Facing);
            FishingCoroutine = StartCoroutine(WaitForbite());
        }
    }
    public void RetractFishingRod()
    {
        if (player.canRetract == true)
        {
            player.isFishing = false;
            if (FishingCoroutine != null)
            {
                StopCoroutine(FishingCoroutine);
            }
            animator.SetTrigger("RetractFishingRod");
            camera.CameraDistance = 10f;
            player.ID.playerEvents.OnExitFishingState?.Invoke();
            VisualFXManager.Instance.DestroyBobber();
        }
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
        animator = player.GetComponent<Animator>();
        ReelSoundSource = GetComponent<AudioSource>();
        Camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
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
    private void OnApplicationQuit()
    {
        InputSystem.ResetHaptics();
    }
    #endregion
}