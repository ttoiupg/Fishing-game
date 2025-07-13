
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using TMPro;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine.UI;
using Unity.Mathematics;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using DepthOfField = UnityEngine.Rendering.Universal.DepthOfField;

public class HUDController : PlayerSystem
{
    int prevLevel = 0;
    float prevProgress = 0.0f;
    int targetLevel = 0;
    float targetProgress = 0.0f;
    float _levelTween = 0.0f;
    float _ExpTween = 0.0f;
    private string _MenuOpenAnimatorTrigger = "MenuOpen";
    public GameObject hudContainer;
    [Header("level progress ui")] 
    public RectTransform needle;
    public RectTransform paper;
    public RectTransform closeButton;
    public RectTransform levelContainer;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI expRqText;
    public TextMeshProUGUI expCurrentText; 
    public UnityEngine.UI.Image fill;
    public float LevelSpeed = 0.2f;
    public float ExpSpeed = 0.5f;
    [Header("Loot tag")]
    public RectTransform lootTag;
    public Image lootImage;
    public TextMeshProUGUI lootNameText;
    public TextMeshProUGUI lootDesciptionText;
    public TextMeshProUGUI lootValueText;
    [Header("Interaction prompt")]
    public RectTransform interactionPrompt;
    public Image promptImage;
    public TextMeshProUGUI promptText;
    public TextMeshProUGUI promptName;
    public bool interacting = false;
    public float interactTime = 0;
    public float requiredInteractTime = 1;
    [SerializeField] private AudioClip popUpSound;
    [SerializeField] private AudioClip hideSound;
    private EventSystem _eventSystem;
    public static HUDController instance;

    public override void Awake()
    {
        base.Awake();
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        _eventSystem = EventSystem.current;
    }

    float BackLerp(float x)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1;

        return 1 + c3 * Mathf.Pow(x - 1, 3) + c1 * Mathf.Pow(x - 1, 2);
    }
    public void ShowInteractionPrompt(string prompt, string name)
    {
        promptText.text = prompt;
        promptName.text = name;
        SoundFXManger.Instance.PlaySoundFXClip(popUpSound, player.characterTransform, 0.3f);
        interactionPrompt.DOScale(Vector3.one,0.2f).SetEase(Ease.OutBack);
    }
    public void HideInteractionPrompt()
    {
        SoundFXManger.Instance.PlaySoundFXClip(hideSound, player.characterTransform, 0.3f);
        interactionPrompt.DOScale(Vector3.zero, 0.15f);
    }
    public void StartLootTag(Sprite image, string name, string desc, string value)
    {
        lootImage.sprite = image;
        lootNameText.text = name;
        lootDesciptionText.text = desc;
        lootValueText.text = value;
        lootTag.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutBack);
        lootTag.DOShakeAnchorPos(0.5f,50,100,90).SetDelay(0.15f);
        lootTag.DOScale(Vector3.zero, 0.1f).SetEase(Ease.OutQuint).SetDelay(3f);
    }
    private void UpdateRadialProgress()
    {
        targetLevel = player.level;
        targetProgress = player.experience / player.expRequire;
        expRqText.text = player.expRequire.ToString();
        if (targetLevel > prevLevel)
        {
            float fixedDt = Time.deltaTime / LevelSpeed;
            _levelTween += fixedDt;

            if (_levelTween > 1f)
            {
                prevLevel += 1;
                levelText.text = prevLevel.ToString();
                prevProgress = 0.0f;
                _levelTween = 0.0f;
                fill.fillAmount = 0f;
                needle.rotation = Quaternion.Euler(0, 0, 0);
                expCurrentText.text = "0";
            }
            else
            {
                float deltaP = (1 - prevProgress);
                float Amount = prevProgress + deltaP * _levelTween;
                fill.fillAmount = Amount;
                needle.rotation = Quaternion.Euler(0, 0, Amount * -360);
                expCurrentText.text = (Amount * player.expRequire).ToSafeString();
            }
        }
        else if (targetProgress > prevProgress)
        {
            float fixedDt = Time.deltaTime / ExpSpeed;
            _ExpTween += fixedDt;
            if (_ExpTween > 1f)
            {
                prevProgress = targetProgress;
                _ExpTween = 0f;
                fill.fillAmount = targetProgress;
                needle.rotation = Quaternion.Euler(0, 0, targetProgress * -360);
                expCurrentText.text = (targetProgress * player.expRequire).ToString();
            }
            else
            {
                float factor = BackLerp(_ExpTween);
                float deltaP = (targetProgress - prevProgress);
                float Amount = prevProgress + deltaP * factor;
                fill.fillAmount = Amount;
                needle.rotation = Quaternion.Euler(0,0, Amount * -360);
                expCurrentText.text = (Amount * player.expRequire).ToSafeString();
            }
        }
    }
    private void Update()
    {
        UpdateRadialProgress();
        //TrackNeedlePosition();
    }
}
