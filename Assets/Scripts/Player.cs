using Halfmoon.StateMachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Halfmoon.Utilities;
using System.Threading;
using UnityEngine.InputSystem;
using DG.Tweening;
public class Player : MonoBehaviour,IDataPersistence
{
    public PlayerID ID;

    public FishingRod currentFishingRod;
    public PlayerInputActions playerInputs;
    public FishingController fishingController;
    public HUDController hudController;
    public Animator animator;
    public Transform CharacterTransform;
    public CharacterController controller;
    public Vector3 playerVelocity;
    public bool groundedPlayer;
    public float movementResponsiveness = 25f;
    public float playerSpeed;
    public float currentSpeed;
    public Vector3 currentDirection;
    public float lastSpeed;
    public bool isControllerConnected = false;
    [SerializeField]
    private float gravityValue = -9.81f;
    [Header("Stats")]
    public float expRequire = 1f;
    [SerializeField]
    private float _experience = 0.0f;
    public int level = 1;
    public float experience
    {
        get => _experience;
        set
        {
            _experience = value;
            expRequire = (float)GetExpRQ(level);
            while (_experience >= expRequire)
            {
                _experience -= expRequire;
                level += 1;
                expRequire = (float)GetExpRQ(level);
            }
        }
    }
    public Dictionary<string,DiscoveredFish> discoveredFish = new Dictionary<string, DiscoveredFish>();
    private StateMachine stateMachine;

    [Header("Fishing")]
    public float damage = 10f;
    public float attackBuff = 1;
    public float pullProgressBuff = 10f;
    public BoostCanvaManager boostCanvaManager;
    public PullCanvaManager pullCanvaManager;
    public Fish currentFish;
    public BaseZone currentZone;
    public bool fishOnBait;
    public bool castRodDebounce;
    public bool retrackDebounce;
    public bool booststate;
    public bool pullstate;
    public bool menuOpen;
    public bool inspecting;
    public bool CardOpened;
    public bool fishing;
    public bool canDamage = true;

    [Header("Character")]
    public int Facing = 1;

    [Header("Interaction")]
    [SerializeField] private Transform _interactionPoint;
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
    [Header("Testing")]
    public bool CanInteract = true;
    private void Awake()
    {
        interactionDebounceTimer = new Countdowntimer(0.5f);
        interactionDebounceTimer.OnTimerStop += () => interactionDebounce = false;
        playerInputs = new PlayerInputActions();
        stateMachine = new StateMachine();
        var locomotionState = new LocomotionState(this, animator);
        var fishingState = new FishingState(this, animator);
        var fishingBoostState = new FishingBoostState(this,animator);
        var fishingPullState = new FishingPullState(this,animator);
        var menuOpenState = new MenuOpenState(this,animator);
        At(locomotionState, fishingState, new FuncPredicate(() => currentZone != null && fishing));
        At(locomotionState, menuOpenState, new FuncPredicate(() => menuOpen == true));
        At(menuOpenState, locomotionState, new FuncPredicate(() => menuOpen == false));
        At(fishingState, locomotionState, new FuncPredicate(() => fishing == false));
        At(fishingState, fishingBoostState, new FuncPredicate(() => booststate == true));
        At(fishingBoostState, fishingPullState, new FuncPredicate(() => pullstate == true));
        At(fishingPullState, locomotionState, new FuncPredicate(() => fishing == false));

        stateMachine.SetState(locomotionState);
    }
    private void OnEnable()
    {
        playerInputs.Player.Enable();
    }
    private void OnDisable()
    {
        playerInputs.Player.Disable();
    }
    void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
    void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);
    private void Update()
    {
        stateMachine.Update();
    }
    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }
    private Collider GetClosestCollider(Collider[] colliders)
    {
        float distance = 10000f;
        Collider result = null;
        foreach (Collider collider in colliders)
        {
            if (collider == null) continue;
            float d = (collider.gameObject.transform.position - CharacterTransform.position).magnitude;
            if (d <= distance)
            {
                distance = d;
                result = collider;
            }
        }
        return result;
    }
    float GetExpRQ(int level)
    {
        return Mathf.Round((4 * (Mathf.Pow((float)level,3f))) / 5);
    }
    IEnumerator CheckForControllers()
    {
        while (true)
        {
            var controllers = Input.GetJoystickNames();

            if (!isControllerConnected && controllers.Length > 0)
            {
                isControllerConnected = true;
                Debug.Log("Connected");

            }
            else if (isControllerConnected && controllers.Length == 0)
            {
                isControllerConnected = false;
                Debug.Log("Disconnected");
            }

            yield return new WaitForSeconds(1f);
        }
    }
    public void LoadData(GameData gameData)
    {
        level = gameData.level;
        experience = gameData.experience;
        expRequire = (float)GetExpRQ(level);
        discoveredFish.Clear();
        discoveredFish = gameData.discoveredFish;
    }
    public void SaveData(ref GameData gameData)
    {
        gameData.level = level;
        gameData.experience = experience;
        gameData.discoveredFish = discoveredFish;
    }
    public void HandleMovement()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");
        // DOING CHARACTER MOVEMENT
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }
        Vector3 move = Vector3.ClampMagnitude(new Vector3(moveHorizontal, 0, moveVertical),1f);
        currentSpeed = Mathf.Lerp(currentSpeed, playerSpeed * move.magnitude, 1-Mathf.Exp(-movementResponsiveness * Time.deltaTime));
        if (move.magnitude > 0)
        {
            currentDirection = Vector3.Normalize(move);
        }
        controller.Move(currentDirection * Time.deltaTime * currentSpeed);
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
        if (playerSpeed != 0)
        {
            if (move.x != 0)
            {
                if (move.x > 0)
                {
                    Facing = 1;
                    CharacterTransform.rotation = Quaternion.Euler(new Vector3(27.5f, 0, 0));
                }
                else
                {
                    Facing = -1;
                    CharacterTransform.rotation = Quaternion.Euler(new Vector3(-27.5f, 180, 0));
                };
            }
            if (move.magnitude != 0)
            {
                animator.SetBool("IsMoving", true);
                animator.SetFloat("Speed", currentSpeed / 3.5f);
            }
            else
            {
                animator.SetFloat("Speed", 1f);
                animator.SetBool("IsMoving", false);
            }
        }
    }
    public void HandleInteraction()
    {
        interactionDebounceTimer.Tick(Time.deltaTime);
        if (_numFound == 0) isEnter = true;
        _numFound = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactionRange, _colliders, _interactionLayerMask);
        if (interactionDebounce) return;
        if (_numFound > 0)
        {
            Collider closest = GetClosestCollider(_colliders);
            currentInteract = closest.GetComponent<IInteractable>();
            if (isEnter || currentInteract != lastInteract)
            {
                if (lastInteract != null)
                {
                    lastInteract.PromptHide(this);
                }
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
        }
        else
        {
            if (!isEnter)
            {
                interactionDebounce = true;
                hudController.HideInteractionPrompt();
                currentInteract.PromptHide(this);
                interactionDebounceTimer.Start();
                currentInteract = null;
            }
        }
    }
    public void InteractInput(InputAction.CallbackContext callbackContext)
    {
        if (currentInteract != null)
        {
            if (callbackContext.phase == InputActionPhase.Started)
            {
                hudController.interactionPrompt.DOScale(new Vector3(0.8f,0.8f,0.8f), 0.15f).SetEase(Ease.OutBack);
                currentInteract.InteractionStart(this);
                hudController.promptImage.DOKill();
                hudController.promptImage.DOFillAmount(1,currentLength).onComplete += ()=> {
                    currentInteract.Interact(this);
                    hudController.HideInteractionPrompt();
                    hudController.promptImage.fillAmount = 0;
                };
                hudController.interacting = true;
            }else if (callbackContext.phase == InputActionPhase.Canceled && interacted == false)
            {
                hudController.interactionPrompt.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutBack);
                currentInteract.InteractionStop(this);
                hudController.promptImage.DOKill();
                hudController.promptImage.DOFillAmount(0, 0.26f);
                hudController.interacting = false;
            }
        }
    }
}

[System.Serializable]
public class IDataDiscoverFish
{
    public string id;
    public string discoverDate;
    public IDataDiscoverFish(DiscoveredFish fish)
    {
        id = fish.baseFish.id;
        discoverDate = fish.discoverDate;
    }
}