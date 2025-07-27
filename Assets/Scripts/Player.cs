using System;
using Halfmoon.StateMachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Halfmoon.Utilities;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour, IDataPersistence
{
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    private static readonly int Speed = Animator.StringToHash("Speed");
    public PlayerID ID;

    public float defaultCameraDistance = 8f;
    public int currentFishingRod;
    public bool isActive;
    public DefaultInputActions PlayerInputs;
    public FishingController fishingController;
    public HUDController hudController;
    public Animator animator;
    public CinemachinePositionComposer cinemachineCamera;
    public SpriteRenderer fishingRodSprite;
    public SpriteRenderer fishingRodReelSprite;

    [FormerlySerializedAs("CharacterTransform")]
    public Transform characterTransform;

    public CharacterController controller;
    public Vector3 playerVelocity;
    public bool groundedPlayer;
    public float movementResponsiveness = 25f;
    public float playerSpeed;
    public float currentSpeed;
    [SerializeField] private float gravityValue = -9.81f;
    [Header("Stats")] public float expRequire = 1f;
    [SerializeField] private float _experience = 0.0f;
    public int level = 1;

    public float experience
    {
        get => _experience;
        set
        {
            _experience = value;
            expRequire = (float)GetExpRequirement(level);
            while (_experience >= expRequire)
            {
                _experience -= expRequire;
                level += 1;
                expRequire = (float)GetExpRequirement(level);
            }
        }
    }
    public Dictionary<string, DiscoveredFish> discoveredFish = new Dictionary<string, DiscoveredFish>();
    private StateMachine _playerStateMachine;

    [Header("Fishing")] public float damage = 10f;
    public float attackBuff = 1;
    public float pullProgressBuff = 10f;
    public BoostCanvaManager boostCanvaManager;

    [FormerlySerializedAs("pullCanvaManager")]
    public ReelCanvaManager ReelCanvaManager;

    public Fish currentFish;
    public BaseZone currentZone;
    public bool pullstate;
    public bool inspecting;
    public bool CardOpened;
    public bool canDamage = true;

    [Header("Character")] public int Facing = 1;

    [Header("Interaction")] [SerializeField]
    private Transform _interactionPoint;

    [SerializeField] private float _interactionRange;
    [SerializeField] private LayerMask _interactionLayerMask;
    private readonly Collider[] _colliders = new Collider[3];
    [SerializeField] private int _numFound;
    [SerializeField] private bool isEnter = true;
    private bool interactionDebounce = false;
    public bool interacted;
    private Countdowntimer interactionDebounceTimer;
    public IInteractable currentInteract;
    private IInteractable lastInteract;
    [SerializeField] private string currentPrompt;
    [SerializeField] private float currentLength;
    [Header("Testing")] public bool CanInteract = true;

    private void Start()
    {
        Debug.Log("Graphics API: " + SystemInfo.graphicsDeviceType);
        AudioListener.volume = PlayerPrefs.GetFloat("Volume");
    }

    public void Setup()
    {
        fishingController.Setup();
        hudController = GameObject.Find("View(Clone)").GetComponent<HUDController>();
        cinemachineCamera = GameObject.Find("CinemachineCamera").GetComponent<CinemachinePositionComposer>();
        cinemachineCamera.transform.GetComponent<CinemachineCamera>().Follow = this.transform;
        interactionDebounceTimer = new Countdowntimer(0.5f);
        interactionDebounceTimer.OnTimerStop += () => interactionDebounce = false;
        PlayerInputs = new DefaultInputActions();
        _playerStateMachine = new StateMachine();
        
        //declare what states we have
        var locomotionState = new LocomotionState(this, animator);
        var fishingState = new FishingState(this, animator);
        var inactiveState = new InactiveState(this, animator);
        var fishingBoostState = new FishingBoostState(this, animator);
        var fishingReelState = new FishingReelState(this, animator);
        
        //add transition
        At(locomotionState, fishingState, new FuncPredicate(() => currentZone && fishingController.isFishing));
        At(locomotionState, inactiveState, new FuncPredicate(() => isActive == false));
        At(inactiveState, locomotionState, new FuncPredicate(() => isActive == true));
        At(fishingState, locomotionState, new FuncPredicate(() => !fishingController.isFishing));
        At(fishingState, fishingBoostState, new FuncPredicate(() => fishingController.fishOnBait));
        At(fishingBoostState, fishingReelState, new FuncPredicate(() => fishingController.boostApplied));
        At(fishingReelState, locomotionState, new FuncPredicate(() => !fishingController.isFishing));
        _playerStateMachine.SetState(locomotionState);
        PlayerInputs?.Player.Enable();
    }
    private void Awake()
    {
        // interactionDebounceTimer = new Countdowntimer(0.5f);
        // interactionDebounceTimer.OnTimerStop += () => interactionDebounce = false;
        // PlayerInputs = new DefaultInputActions();
        // _playerStateMachine = new StateMachine();
        //
        // //declare what states we have
        // var locomotionState = new LocomotionState(this, animator);
        // var fishingState = new FishingState(this, animator);
        // var inactiveState = new InactiveState(this, animator);
        // var fishingBoostState = new FishingBoostState(this, animator);
        // var fishingReelState = new FishingReelState(this, animator);
        //
        // //add transition
        // At(locomotionState, fishingState, new FuncPredicate(() => currentZone && fishingController.isFishing));
        // At(locomotionState, inactiveState, new FuncPredicate(() => isActive == false));
        // At(inactiveState, locomotionState, new FuncPredicate(() => isActive == true));
        // At(fishingState, locomotionState, new FuncPredicate(() => !fishingController.isFishing));
        // At(fishingState, fishingBoostState, new FuncPredicate(() => fishingController.fishOnBait));
        // At(fishingBoostState, fishingReelState, new FuncPredicate(() => fishingController.boostApplied));
        // At(fishingReelState, locomotionState, new FuncPredicate(() => !fishingController.isFishing));
        // _playerStateMachine.SetState(locomotionState);
    }

    private void OnEnable()
    {
        PlayerInputs?.Player.Enable();
    }

    private void OnDisable()
    {
        PlayerInputs.Player.Disable();
    }

    void At(IState from, IState to, IPredicate condition) => _playerStateMachine.AddTransition(from, to, condition);
    void Any(IState to, IPredicate condition) => _playerStateMachine.AddAnyTransition(to, condition);

    private void Update()
    {
        _playerStateMachine?.Update();
    }

    private void FixedUpdate()
    {
        _playerStateMachine?.FixedUpdate();
    }
    
    private Collider GetClosestCollider(Collider[] colliders)
    {
        var distance = 10000f;
        Collider result = null;
        foreach (var c in colliders)
        {
            if (!c) continue;
            var d = (c.gameObject.transform.position - characterTransform.position).magnitude;
            if (!(d <= distance)) continue;
            distance = d;
            result = c;
        }

        return result;
    }

    float GetExpRequirement(int level)
    {
        return Mathf.Round((4 * (Mathf.Pow((float)level, 3f))) / 5);
    }

    public FishingRod GetFishingRod()
    {
        return InventoryManager.Instance.fishingRods[currentFishingRod];
    }

    public void LoadData(GameData gameData)
    {
        level = gameData.playerData.level;
        experience = gameData.playerData.experience;
        expRequire = (float)GetExpRequirement(level);
        currentFishingRod = gameData.playerData.equipedFishingRod;
        discoveredFish.Clear();
        foreach (var dis in gameData.playerData.discoverFishList)
        {
            discoveredFish.Add(dis.id, new DiscoveredFish(dis));
        }
    }

    public void SaveData(ref GameData gameData)
    {
        gameData.playerData.level = level;
        gameData.playerData.experience = experience;
        gameData.playerData.discoverFishList.Clear();
        gameData.playerData.equipedFishingRod = currentFishingRod;
        foreach (var fish in discoveredFish.Values)
        {
            gameData.playerData.discoverFishList.Add(new IDataDiscoverFish(fish));
        }
    }

    public void UpdateMovement()
    {
        var moveHorizontal = Input.GetAxisRaw("Horizontal");
        var moveVertical = Input.GetAxisRaw("Vertical");
        // DOING CHARACTER MOVEMENT
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        var move = Vector3.ClampMagnitude(new Vector3(moveHorizontal, playerVelocity.y, moveVertical), 1f);
        currentSpeed = Mathf.Lerp(currentSpeed, playerSpeed * move.magnitude,
            1 - Mathf.Exp(-movementResponsiveness * Time.deltaTime));
        controller.Move(move * (Time.deltaTime * currentSpeed));
        UpdateAnimator(move);
    }

    private void UpdateAnimator(Vector3 direction)
    {
        if (playerSpeed == 0) return;
        if (direction.x != 0)
        {
            if (direction.x > 0)
            {
                Facing = 1;
                characterTransform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                Facing = -1;
                characterTransform.localScale = new Vector3(-1, 1, 1);
            }
        }

        var flatDirection = new Vector3(direction.x, 0, direction.z);
        if (flatDirection.magnitude > 0.05f)
        {
            animator.SetBool(IsMoving, true);
            animator.SetFloat(Speed, currentSpeed / 3.5f);
        }
        else
        {
            animator.SetFloat(Speed, 1f);
            animator.SetBool(IsMoving, false);
        }
    }

    public void UpdateInteraction()
    {
        interactionDebounceTimer.Tick(Time.deltaTime);
        if (_numFound == 0) isEnter = true;
        _numFound = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactionRange, _colliders,
            _interactionLayerMask);
        if (interactionDebounce) return;
        if (_numFound > 0)
        {
            var closest = GetClosestCollider(_colliders);
            currentInteract = closest.GetComponent<IInteractable>();
            if (!isEnter && currentInteract == lastInteract) return;
            lastInteract?.PromptHide(this);
            hudController.interactionPrompt.localScale = Vector3.zero;
            interacted = false;
            lastInteract = currentInteract;
            currentPrompt = currentInteract.InteractionPrompt;
            currentLength = currentInteract.length;
            hudController.requiredInteractTime = currentInteract.length;
            isEnter = false;
            hudController.ShowInteractionPrompt(currentPrompt, closest.name);
            currentInteract.PromptShow(this);
        }
        else if (!isEnter)
        {
            interactionDebounce = true;
            hudController.HideInteractionPrompt();
            currentInteract.PromptHide(this);
            interactionDebounceTimer.Start();
            currentInteract = null;
        }
    }

    public void InteractInput(InputAction.CallbackContext callbackContext)
    {
        if (currentInteract == null) return;
        switch (callbackContext.phase)
        {
            case InputActionPhase.Started:
                hudController.interactionPrompt.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.15f).SetEase(Ease.OutBack);
                currentInteract.InteractionStart(this);
                hudController.promptImage.DOKill();
                hudController.promptImage.DOFillAmount(1, currentLength).onComplete += () =>
                {
                    currentInteract.Interact(this);
                    hudController.HideInteractionPrompt();
                    hudController.promptImage.fillAmount = 0;
                };
                hudController.interacting = true;
                break;
            case InputActionPhase.Canceled when interacted == false:
                hudController.interactionPrompt.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutBack);
                currentInteract.InteractionStop(this);
                hudController.promptImage.DOKill();
                hudController.promptImage.DOFillAmount(0, 0.26f);
                hudController.interacting = false;
                break;
        }
    }

    public void EquipFishingRod(FishingRod fishingRod)
    {
        currentFishingRod = InventoryManager.Instance.fishingRods.FindIndex(fRod => fRod == fishingRod);
        ChangeFishingRodAppearance(fishingRod);
    }

    private void ChangeFishingRodAppearance(FishingRod fishingRod)
    {
        fishingRodSprite.sprite = fishingRod.fishingRodSO.spriteWorldRod;
        fishingRodReelSprite.sprite = fishingRod.fishingRodSO.spriteWorldReel;
    }
}