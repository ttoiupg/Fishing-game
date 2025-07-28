using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using System.Collections;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ReelCanvaManager : MonoBehaviour
{
    [Header("UI")]
    public Animator ReelCanvasAnimator;
    public RectTransform pullCanva;
    public RectTransform controllCanva;
    [FormerlySerializedAs("controlCanva")] public RectTransform topBarCanva;
    public RectTransform controlBar;
    public RectTransform fishNeedle;
    public RectTransform timer;
    public RectTransform BuffTimer;
    public Image FishBar;
    public RectTransform flash;
    public Image flashImage;
    public Image buffTimerImage;
    public Image buffTimerImageBackground;
    public RectTransform fishHealthBar;
    public TextMeshProUGUI fishHealthText;
    public Image seaBackground;
    public RectTransform bonusBar;
    public Image bonusBarEffect;
    public RectTransform bonusTimer;
    public bool haveBonusBar;
    
    [Header("Valus")]
    public float pullCanvaRotation = 0f;

    public float controlForwardGravity = 3500f;
    public float controlBackwardGravity = -3500f;
    public float controlBarMaxSpeed = 1000f;
    public float controlBarPosition = 0f;
    public float controlBarGravity = 0f;
    public float controlBarDirection = 0f;
    public float controlBarVelocity = 0f;
    public float fishNeedleTargetPosition = 0f;
    public float fishNeedlePosition = 0f;
    public float fishNeedleSpeed = 6f;
    public float bonusBarPosition = 0f;

    [Header("Crank ")] 
    public RectTransform crank;
    public RectTransform gearCover;
    public RectTransform gear;
    public float crankRotation;
    public float crankDirection;
    public float crankSpeed;
    
    public float rawLeftBound = -375;
    public float rawRightBound = 375;
    private Player player;

    private bool _fishBarMoved = true;
    private static readonly Color32 Red = new Color32(217, 77, 88, 255);
    private static readonly Color32 Blue = new Color32(0, 135, 164, 255);
    
    private Animator animator;

    private float Lerpfloat(float a, float b, float t)
    {
        return a + (b - a) * t;
    }

    public void SwitchReelingBarColor(bool on)
    {
        //_controlBarImage.color = on ? Red : Blue;
        //_controlBarOutline.effectColor = on ? Blue : Red;
    }
    public void Init()
    {
        //controlBar.sizeDelta = new Vector2(player.ID.pullBarSize, 94.7651f);
        controlBar.anchoredPosition = new Vector2(0, -338.718f);
        fishNeedle.anchoredPosition = new Vector2(0, -403.5762f);
        controlBarGravity = controlBarDirection;
        controlBarDirection = 0f;
        controlBarPosition = 0f;
        fishNeedlePosition = 0f;
        fishNeedlePosition = 0f;
        fishNeedleSpeed = 6f;
    }

    public void ResetTimer()
    {
        timer.localScale = new Vector3(1f, 1f, 1f);
    }
    public void StartTimer(float length)
    {
        timer.localScale = new Vector3(1f, 1f, 1f);
        controllCanva.DOShakePosition(length,10f,6,30f,false,false,ShakeRandomnessMode.Full);
        timer.DOKill();
        timer.DOScaleX(0,length);
    }

    public void SetSeaHeight(float height)
    {
        seaBackground.material.SetFloat("_Height",height);
    }

    public void UpdateFishHealth(float health, float maxHealth)
    {
        seaBackground.material.DOFloat(health / maxHealth,"_Height",0.25f).SetEase(Ease.OutBack);
        fishHealthBar
            .DOScaleX(health / maxHealth, 0.25f)
            .SetEase(Ease.OutBack);
        fishHealthText.text = $"{health}/{maxHealth}";
    }
    private void SpawnBonusBar()
    {
        if (haveBonusBar) return;
        haveBonusBar = true;
        bonusBar.gameObject.SetActive(true);
        var leftBound = rawLeftBound + bonusBar.sizeDelta.x;
        var rightBound = rawRightBound - bonusBar.sizeDelta.x;
        var RandPosition = Random.Range(leftBound, rightBound);
        bonusBarPosition = RandPosition;
        var pos = bonusBar.localPosition;
        pos.x = RandPosition;
        bonusBar.localPosition = pos;
    }
    private void DestroyBonusBar()
    {
        haveBonusBar = false;
        bonusBar.gameObject.SetActive(false);
    }
    public void UpdatePosition()
    {
        var leftBound = -338.4f;
        var rightBound = 338.4f;
        controlBarGravity = (controlBarDirection<0)? controlBackwardGravity : controlForwardGravity * controlBarDirection;
        controlBarVelocity += controlBarGravity * Time.deltaTime;
        controlBarVelocity = (Mathf.Abs(controlBarVelocity)>controlBarMaxSpeed) ?  Mathf.Sign(controlBarVelocity) * controlBarMaxSpeed : controlBarVelocity;
        //crankVelocity = Mathf.Abs((crankVelocity + crankDirection * Time.deltaTime)) > crankMaxSpeed ? crankMaxSpeed : crankVelocity + crankDirection * Time.deltaTime;
        crankRotation += crankDirection * Time.deltaTime;
        var nextPosition = controlBarPosition + controlBarVelocity * Time.deltaTime;
        if (nextPosition < leftBound)
        {
            controlBarVelocity *= -0.1f;
            nextPosition = leftBound;
        }
        else if (nextPosition > rightBound)
        {
            controlBarVelocity *= -0.1f;
            nextPosition = rightBound;
        }

        controlBarPosition = nextPosition;
        controlBar.anchoredPosition = new Vector2(controlBarPosition, 118.3823f);

        fishNeedlePosition = Lerpfloat(fishNeedlePosition, fishNeedleTargetPosition, fishNeedleSpeed * Time.deltaTime);
        pullCanvaRotation = Lerpfloat(pullCanvaRotation, controlBarVelocity / (-controlBarMaxSpeed / 10), 5.5f * Time.deltaTime);
        topBarCanva.rotation = Quaternion.Euler(0, 0, pullCanvaRotation);
        gearCover.rotation = Quaternion.Euler(0, 0, crankRotation*0.07f);
        gear.rotation = Quaternion.Euler(0, 0, -crankRotation* 0.14f);
        crank.rotation = Quaternion.Euler(0, 0, crankRotation);
        fishNeedle.anchoredPosition = new Vector2(fishNeedlePosition, 137.1f);
    }
    
    public void Flash()
    {
        flash.localScale = Vector3.one;
        flash.DOScale(Vector3.one * 2, 0.3f);
        flashImage.color = Color.white;
        flashImage.DOColor(new Color(1, 1, 1, 0f), 0.3f);
    }

    public void SetTimerFlowIntensity(float intensity)
    {
        buffTimerImage.material.SetFloat("_Speed",intensity);
        buffTimerImageBackground.material.SetFloat("_Speed",intensity);
    }
    public void TweenBuffTimer(float time)
    {
        BuffTimer.localScale = new Vector3(0,1,1);
        BuffTimer.DOKill();
        BuffTimer.DOScaleX(1, time);
    }
    public void StopBuffTimerTween()
    {
        BuffTimer.DOKill();
        BuffTimer.localScale = new Vector3(0, 1, 1);
    }
    public void TweenBonusTimer(float time)
    {
        bonusTimer.localScale = new Vector3(0, 1, 1);
        bonusTimer.DOKill();
        bonusTimer.DOScaleX(1, time);
    }
    public void StopBonusTimerTween(bool isFinished)
    {
        bonusTimer.DOKill();
        bonusTimer.localScale = new Vector3(0, 1, 1);
        if (isFinished)
        {
            DestroyBonusBar();
        }
    }
    public void ShakeUI()
    {
        pullCanva.DOShakePosition(0.3f,50f,10,90f,false,true,ShakeRandomnessMode.Full);
    }
    public async UniTask RandomFishBarPosition()
    {
        if (!_fishBarMoved) return;
        var leftBound = rawLeftBound + fishNeedle.sizeDelta.x;
        var rightBound = rawRightBound - fishNeedle.sizeDelta.x;
        _fishBarMoved = false;
        Debug.Log("RandomFishBarPosition");
        var battle = GameManager.Instance.CurrentBattle;
        var enemy = battle.battleStats.enemy;
        var fish = GameManager.Instance.FishEnemy;
        if (GameManager.Instance.CurrentBattle.battleStats.enemy.IsDead()) return;
        var RandSecond = Random.Range(
            fish.fish.fishType.MinFishBarChangeTime,
            fish.fish.fishType.MaxFishBarChangeTime);
        var RandPosition = Random.Range(leftBound, rightBound);
        while (Mathf.Abs(RandPosition - fishNeedleTargetPosition) >
               fish.fish.fishType.MaxFishBarChangeDistance)
        {
            RandPosition = UnityEngine.Random.Range(leftBound, rightBound);
        }
        fishNeedleTargetPosition = RandPosition;
        fishNeedleSpeed = UnityEngine.Random.Range(0.05f, 6f);
        await UniTask.WaitForSeconds(RandSecond);
        var chance = Random.Range(0, 100);
        if (chance < 20)
        {
            SpawnBonusBar();
        }
        Debug.Log("RandomFishBarPosition finished");
        _fishBarMoved = true;
    }

    public void GamepadVibration()
    {
        float lowfreq = 0.5f;
        float highfreq = player.PlayerInputs.Fishing.ControlFishingRod.ReadValue<float>() * 0.6f;
        if (player.fishingController.ReelingBarOverlaping)
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
    private async UniTask PlayAnimation()
    {
        //ReelCanvasAnimator.Play();
        ReelCanvasAnimator.enabled = true;
        ReelCanvasAnimator.Play("ReelCanvaPopUp",0,0);
        await UniTask.WaitForSeconds(2.1f);
        ReelCanvasAnimator.enabled = false;
    }
    public void ShowUI()
    {
        DofController.instance.SetBlur(true);
        DestroyBonusBar();
        SetSeaHeight(1f);
        PlayAnimation();
    }

    public void CloseUI()
    {
        DofController.instance.SetBlur(false);
        pullCanva.DOScale(Vector3.zero, 0.15f).SetEase(Ease.OutQuad);
        buffTimerImage.material.SetFloat("_Speed",0f);
        buffTimerImageBackground.material.SetFloat("_Speed",0f);
        timer.DOKill();
        controllCanva.DOKill();
        timer.localScale = Vector3.one;
    }

    private void Start()
    {
        // animator = GameManager.Instance.player.GetComponent<Animator>();
        // Material seaMat = seaBackground.material;
        // seaBackground.material = new Material(seaMat);
    }

    public void Setup()
    {
        player = GameManager.Instance.player;
        animator = player.GetComponent<Animator>();
        Material seaMat = seaBackground.material;
        seaBackground.material = new Material(seaMat);
    }
}