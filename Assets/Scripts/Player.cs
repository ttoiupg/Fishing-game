using Halfmoon.StateMachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class Player : MonoBehaviour,IDataPersistence
{
    public PlayerID ID;

    public PlayerInputActions playerInputs;
    public FishingController fishingController;
    public HUDController hudController;
    public Animator animator;
    public Transform CharacterTransform;
    public CharacterController controller;
    public Vector3 playerVelocity;
    public bool groundedPlayer;
    public float playerSpeed;
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
    public List<DiscoveredFish> discoveredFish;
    private List<IDataDiscoverFish> dataDiscoverFish;
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
    private void Awake()
    {
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
        foreach(IDataDiscoverFish dataFish in gameData.discoveredFish)
        {
            BaseFish b_fish = DataPersistenceManager.Instance.gameFish.Find((x) => x.name == dataFish.name);
            discoveredFish.Add(new DiscoveredFish(b_fish,dataFish.discoverDate));
        }
    }
    public void SaveData(ref GameData gameData)
    {
        gameData.level = level;
        gameData.experience = experience;
        List<IDataDiscoverFish> tempDisFish = new List<IDataDiscoverFish>();
        foreach (DiscoveredFish Fish in discoveredFish)
        {
            tempDisFish.Add(new IDataDiscoverFish(Fish));
        }
        gameData.discoveredFish = tempDisFish;
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
        Vector3 move = new Vector3(moveHorizontal, 0, moveVertical);
        if (move.magnitude > 1f)
        {
            move = move / move.magnitude;
        };
        controller.Move(move * Time.deltaTime * playerSpeed);
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
            if (move != Vector3.zero)
            {
                animator.SetBool("IsMoving", true);
                animator.SetFloat("Speed", move.magnitude * playerSpeed / 3f);
            }
            else
            {
                animator.SetFloat("Speed", 1f);
                animator.SetBool("IsMoving", false);
            }
        }
    }
}

[System.Serializable]
public class IDataDiscoverFish
{
    public string name;
    public string discoverDate;
    public IDataDiscoverFish(DiscoveredFish fish)
    {
        name = fish.baseFish.name;
        discoverDate = fish.discoverDate;
    }
}