using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class AwardViewManager : MonoBehaviour
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
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        _eventSystem = EventSystem.current;
    }

    public void DisableAnimator()
    {
        animator.enabled = false; 
    }
    private async UniTask On()
    {
        PauseViewModel.Instance.PauseLock = true;
        GameManager.Instance.player.isActive = false;
        _eventSystem.SetSelectedGameObject(null);
        DofController.instance.SetBlur(true);
        animator.enabled = true;
        animator.Play("AwardViewPopUP",0,0);
        await UniTask.WaitForSeconds(4f);
        _eventSystem.SetSelectedGameObject(TipButton);
        animator.enabled = false;
    }

    private async UniTask Off()
    {
        animator.enabled = true;
        _eventSystem.SetSelectedGameObject(null);
        animator.Play("AwardViewClose",0,0);
        await UniTask.WaitForSeconds(1.2f);
        animator.enabled = false;
        GameManager.Instance.player.isActive = true;
        DofController.instance.SetBlur(false);
        PauseViewModel.Instance.PauseLock = false;
    }
    public void SetAwardView(bool isOn)
    {
        if (isOn)
        {
            On();
        }
        else
        {
            Off(); 
        }
    }

    public void ShowFishAward(Fish fish)
    {
        image.sprite = fish.fishType.Art;
        tag.sprite = fish.fishType.Tag; 
        NameLabel.text = fish.fishType.name;
        WeightLabel.text = $"{ fish.weight}kg";
        MutationLabel.text = $"Mutation: {fish.mutation.name}";
        SetAwardView(true);
    }
}
