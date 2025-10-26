using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;

public class AwardViewManager : MonoBehaviour,IViewFrame
{
    public static AwardViewManager Instance;
    private EventSystem _eventSystem;
    [SerializeField] private Animator animator;
    [SerializeField] private Image image;
    [SerializeField] private Image tag;
    [SerializeField] private TextMeshProUGUI NameLabel;
    [SerializeField] private TextMeshProUGUI WeightLabel;
    [SerializeField] private TextMeshProUGUI MutationLabel;
    [SerializeField] private GameObject TipButton;
    private PauseViewModel _pauseViewModel;
    private bool _debounce = false;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        _pauseViewModel = GameObject.FindFirstObjectByType<PauseViewModel>();
        _eventSystem = EventSystem.current;
    }

    public void DisableAnimator()
    {
        animator.enabled = false; 
    }
    private async UniTask On()
    {
        _pauseViewModel.PauseLock = true;
        GameManager.Instance.player.isActive = false;
        _eventSystem.SetSelectedGameObject(null);
        animator.enabled = true;
        animator.Play("AwardViewPopUP",0,0);
        await UniTask.WaitForSeconds(4f);
        TipButton.SetActive(true);
        _eventSystem.SetSelectedGameObject(TipButton);
        animator.enabled = false;
        _debounce = false;
    }

    private async UniTask Off()
    {
        animator.enabled = true;
        _eventSystem.SetSelectedGameObject(null);
        TipButton.SetActive(false);
        animator.Play("AwardViewClose",0,0);
        await UniTask.WaitForSeconds(1.2f);
        animator.enabled = false;
        GameManager.Instance.player.isActive = true;
        _pauseViewModel.PauseLock = false;
        _debounce = false;
    }

    public void ShowFishAward(Fish fish)
    {
        image.sprite = fish.fishType.Art;
        tag.sprite = fish.fishType.Tag; 
        NameLabel.text = fish.fishType.name;
        WeightLabel.text = $"{ fish.weight}kg";
        MutationLabel.text = $"Mutation: {fish.mutation.name}";
        ViewManager.instance.OpenView(this);
    }

    public void Begin()
    {
        On();
    }

    public void End()
    {
        Off();
    }
}