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
using TMPro;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class Player : MonoBehaviour, IDataPersistence
{
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    private static readonly int Speed = Animator.StringToHash("Speed");
    public PlayerID ID;
    public StringVariable playerName;

    public Rigidbody2D body;
    public float defaultCameraDistance = 8f;
    public int currentFishingRod;
    public bool isActive;
    public DefaultInputActions PlayerInputs;
    public FishingController fishingController;
    public HUDController hudController;
    public Animator animator;
    public GameObject cameras;
    public CinemachinePositionComposer cinemachineCamera;
    public SpriteRenderer fishingRodSprite;
    public SpriteRenderer fishingRodReelSprite;

    [FormerlySerializedAs("CharacterTransform")]
    public Transform characterTransform;
    
    public Vector3 playerVelocity;
    public bool groundedPlayer;
    public float movementResponsiveness = 25f;
    public float playerSpeed;
    public float currentSpeed;
    [SerializeField] private float gravityValue = -9.81f;
    [Header("Stats")] public int gold;
    public float expRequire = 1f;
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
    public GameObject currentZoneDisplayer;
    public TMP_Text zoneText;
    public bool pullstate;
    public bool inspecting;
    public bool CardOpened;
    public bool canDamage = true;

    [Header("Character")] public int Facing = 1;

    [Header("Interaction")] [SerializeField]
    private Transform _interactionPoint;

    [SerializeField] private float _interactionRange;
    [SerializeField] private LayerMask _interactionLayerMask;
    private string closestName;
    private readonly Collider2D[] _colliders = new Collider2D[3];
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
    [Header("Modifer")]
    [SerializeField] public List<ModifierBase> modifiers = new();
    public float tempDamage;
    public float tempAccuracy;
    public float tempCritChance;
    public float tempCritMultiplier;
    public float tempresilience;
    public float templuck;
    private PauseViewModel _pauseViewModel;
    private InactiveState inactiveState;
    private FishingState fishingState;
    private LocomotionState locomotionState;
    private FishingBoostState fishingBoostState;
    private FishingReelState fishingReelState;
    public DebugAgent DebugAgent;
    public Vector3 inputMoveDirection;
    
    public void SetActive(bool isActive)
    {
        this.isActive = isActive;
    }
    private void Start()
    {
        _pauseViewModel = GameObject.Find("Pause").GetComponent<PauseViewModel>();
        Debug.Log("Graphics API: " + SystemInfo.graphicsDeviceType);
        AudioListener.volume = PlayerPrefs.GetFloat("Volume");
        UnityEngine.Rendering.DebugManager.instance.enableRuntimeUI = false;
        UnityEngine.Rendering.DebugManager.instance.displayRuntimeUI = false;
        fishingController.Setup();
        interactionDebounceTimer = new Countdowntimer(0.5f);
        interactionDebounceTimer.OnTimerStop += () => interactionDebounce = false;
        PlayerInputs = PlayerInputSystem.Instance.playerInput;
        _playerStateMachine = new StateMachine();

        //declare what states we have
        locomotionState = new LocomotionState(this, animator,_pauseViewModel);
        fishingState = new FishingState(this, animator);
        inactiveState = new InactiveState(this, animator,_pauseViewModel);
        fishingBoostState = new FishingBoostState(this, animator);
        fishingReelState = new FishingReelState(this, animator);

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
        PlayerInputSystem.Instance.playerInput.Player.Enable();
    }

    public void unload()
    {
        Debug.Log("unload ");
        DebugAgent.Unload();
        inactiveState.Unload();
        locomotionState.Unload();
        fishingState.Unload();
        fishingBoostState.Unload();
    }

    private void OnEnable()
    {
        PlayerInputs?.Player.Enable();
        PlayerInputSystem.Instance.playerInput.Player.Enable();
    }

    private void OnDisable()
    {
        PlayerInputs.Player.Disable();
        PlayerInputSystem.Instance.playerInput.Player.Disable();
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

    private Collider2D GetClosestCollider(Collider2D[] colliders)
    {
        var distance = 10000f;
        Collider2D result = null;
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
        playerName.Value = gameData.playerData.name;
        gold = gameData.playerData.gold;
        experience = gameData.playerData.experience;
        expRequire = (float)GetExpRequirement(level);
        currentFishingRod = gameData.playerData.equipedFishingRod;
        discoveredFish.Clear();
        foreach (var dis in gameData.playerData.discoverFishList)
        {
            discoveredFish.Add(dis.id, new DiscoveredFish(dis));
        }
        modifiers.Clear();
        gameData.playerData.modifiers.ForEach(id => this.modifiers.Add(DataPersistenceManager.Instance.ModifierCards[id]));
        transform.position = gameData.playerData.position;

    }

    public void SaveData(ref GameData gameData)
    {
        gameData.playerData.gold = gold;
        gameData.playerData.name = playerName.Value;
        gameData.playerData.level = level;
        gameData.playerData.experience = experience;
        gameData.playerData.discoverFishList.Clear();
        gameData.playerData.equipedFishingRod = currentFishingRod;
        foreach (var fish in discoveredFish.Values)
        {
            gameData.playerData.discoverFishList.Add(new IDataDiscoverFish(fish));
        }
        gameData.playerData.modifiers.Clear();
        foreach (var mod in modifiers)
        {
            gameData.playerData.modifiers.Add(mod.id);
        }
        gameData.playerData.position = transform.position;
    }

    public void UpdateMovement()
    {
        if (!isActive)
        {
            body.linearVelocity = Vector2.zero;
            return;
        };
        inputMoveDirection = PlayerInputSystem.Instance.playerInput.Player.Move.ReadValue<Vector2>();
        var moveHorizontal = inputMoveDirection.x;//Input.GetAxisRaw("Horizontal");
        var moveVertical = inputMoveDirection.y;//Input.GetAxisRaw("Vertical");
        // DOING CHARACTER MOVEMENT
        var move = Vector3.ClampMagnitude(new Vector3(moveHorizontal,moveVertical,0 ), 1f);
        currentSpeed = Mathf.Lerp(currentSpeed, playerSpeed * move.magnitude,
            1 - Mathf.Exp(-movementResponsiveness * Time.deltaTime));
        body.linearVelocity = move * currentSpeed;
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
        if (direction.magnitude > 0.05f)
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
    public void ShowInteractPrompt()
    {
        if (currentInteract == null) return;
        hudController.interactionPrompt.localScale = Vector3.zero;
        interacted = false;
        lastInteract = currentInteract;
        currentPrompt = currentInteract.InteractionPrompt;
        currentLength = currentInteract.length;
        hudController.requiredInteractTime = currentInteract.length;
        isEnter = false;
        hudController.ShowInteractionPrompt(currentPrompt, closestName);
        currentInteract.PromptShow(this);
    }
    public void HideInteractPrompt()
    {
        hudController.HideInteractionPrompt();
        currentInteract.PromptHide(this);
        interactionDebounceTimer.Start();
        currentInteract = null;
    }
    public void UpdateInteraction()
    {
        interactionDebounceTimer.Tick(Time.deltaTime);
        if (_numFound == 0) isEnter = true;
        var mask = new ContactFilter2D();
        mask.useLayerMask = true;
        mask.layerMask = _interactionLayerMask;
        _numFound = Physics2D.OverlapCircle(new Vector2(_interactionPoint.position.x,_interactionPoint.position.y), _interactionRange,mask, _colliders);
        if (interactionDebounce) return;
        if (_numFound > 0)
        {
            var closest = GetClosestCollider(_colliders);
            currentInteract = closest.GetComponent<IInteractable>();
            if (!isEnter && currentInteract == lastInteract) return;
            closestName = closest.name;
            ShowInteractPrompt();
        }
        else if (!isEnter)
        {
            interactionDebounce = true;
            HideInteractPrompt();
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
    public void ShowCurrentZone(string zoneName)
    {
        currentZoneDisplayer.transform.DOKill();
        currentZoneDisplayer.transform.DOLocalMoveY(452, 0.5f).SetEase(Ease.OutBack);
        zoneText.text = zoneName;
    }
    public void HideCurrentZone()
    {
        currentZoneDisplayer.transform.DOKill();
        currentZoneDisplayer.transform.DOLocalMoveY(652, 0.3f).SetEase(Ease.InBack);
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

    public void OnModifierEvent(BattleEvent battleEvent)
    {
        if (modifiers.Count == 0) return;
        foreach (var modifier in modifiers)
        {
            modifier?.OnBattleEvent(battleEvent);
        }
    }
}