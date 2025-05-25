using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using System.Collections;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine.UI;

public class ReelCanvaManager : PlayerSystem
{
    public RectTransform pullCanva;
    public RectTransform controlCanva;
    public RectTransform controlBar;
    public RectTransform fishNeedle;
    public RectTransform fishPedal;
    public RectTransform fishPedalL1;
    public RectTransform fishPedalL2;
    public RectTransform fishPedalR1;
    public RectTransform fishPedalR2;
    public Slider timer;
    private Image _controlBarImage;
    private Outline _controlBarOutline;
    
    public float pullCanvaRotation = 0f;
    public float controlBarPosition = 0f;
    public float controlBarGravity = 0f;
    public float controlBarVelocity = 0f;
    public float fishNeedleTargetPosition = 0f;
    public float fishNeedlePosition = 0f;
    public float fishNeedleSpeed = 6f;


    public float rawLeftBound = -286;
    public float rawRightBound = 286f;

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
        _controlBarImage.color = on ? Red : Blue;
        _controlBarOutline.effectColor = on ? Blue : Red;
    }
    public void Init()
    {
        _controlBarImage = controlBar.GetComponent<Image>();
        _controlBarOutline = controlBar.GetComponent<Outline>();
        controlBar.sizeDelta = new Vector2(player.ID.pullBarSize, 94.7651f);
        controlBar.anchoredPosition = new Vector2(0, -338.718f);
        fishNeedle.anchoredPosition = new Vector2(0, -403.5762f);
        fishPedal.localScale = new Vector3(1, 0, 1);
        fishPedalL1.localScale = new Vector3(1, 0, 1);
        fishPedalR1.localScale = new Vector3(1, 0, 1);
        fishPedalL2.localScale = new Vector3(1, 0, 1);
        fishPedalR2.localScale = new Vector3(1, 0, 1);
        controlBarGravity = -1500f;
        controlBarPosition = 0f;
        fishNeedlePosition = 0f;
        fishNeedlePosition = 0f;
        fishNeedleSpeed = 6f;
    }

    public void ResetTimer()
    {
        timer.value = 100f;
    }
    public void StartTimer(float length)
    {
        timer.DOValue(0,length);
    }
    public void UpdatePosition()
    {
        var leftBound = rawLeftBound + controlBar.sizeDelta.x / 2;
        var rightBound = rawRightBound - controlBar.sizeDelta.x / 2;
        controlBarVelocity += controlBarGravity * Time.deltaTime;
        var nextPosition = controlBarPosition + controlBarVelocity * Time.deltaTime;
        if (nextPosition < leftBound)
        {
            controlBarVelocity *= -0.4f;
            nextPosition = leftBound;
        }
        else if (nextPosition > rightBound)
        {
            controlBarVelocity *= -0.4f;
            nextPosition = rightBound;
        }

        controlBarPosition = nextPosition;
        controlBar.anchoredPosition = new Vector2(controlBarPosition, -338.718f);

        fishNeedlePosition = Lerpfloat(fishNeedlePosition, fishNeedleTargetPosition, fishNeedleSpeed * Time.deltaTime);
        pullCanvaRotation = Lerpfloat(pullCanvaRotation, controlBarGravity / -250f, 5.5f * Time.deltaTime);
        controlCanva.rotation = Quaternion.Euler(0, 0, pullCanvaRotation);
        fishNeedle.anchoredPosition = new Vector2(fishNeedlePosition, -403.5762f);
    }

    public void FlipFirst()
    {
        fishPedal.DOScaleY(1, 0.25f).SetEase(Ease.OutBounce);
    }

    public void FlipSecond()
    {
        fishPedalL1.DOScaleY(1, 0.25f).SetEase(Ease.OutBounce);
        fishPedalR1.DOScaleY(1, 0.25f).SetEase(Ease.OutBounce);
    }

    public void FlipThird()
    {
        fishPedalL2.DOScaleY(1, 0.25f).SetEase(Ease.OutBounce);
        fishPedalR2.DOScaleY(1, 0.25f).SetEase(Ease.OutBounce);
    }

    public void FlipDownAll()
    {
        fishPedal.DOScaleY(0, 0.25f).SetEase(Ease.OutBounce);
        fishPedalL1.DOScaleY(0, 0.25f).SetEase(Ease.OutBounce);
        fishPedalR1.DOScaleY(0, 0.25f).SetEase(Ease.OutBounce);
        fishPedalL2.DOScaleY(0, 0.25f).SetEase(Ease.OutBounce);
        fishPedalR2.DOScaleY(0, 0.25f).SetEase(Ease.OutBounce);
    }

    public async UniTask RandomFishBarPosition()
    {
        if (!_fishBarMoved) return;
        _fishBarMoved = false;
        Debug.Log("RandomFishBarPosition");
        var battle = GameManager.Instance.CurrentBattle;
        var enemy = battle.battleStats.enemy;
        var fish = GameManager.Instance.FishEnemy;
        if (GameManager.Instance.CurrentBattle.battleStats.enemy.IsDead()) return;
        var RandSecond = Random.Range(
            fish.fish.fishType.MinFishBarChangeTime,
            fish.fish.fishType.MaxFishBarChangeTime);
        var RandPosition = Random.Range(-283.5f, 283.5f);
        while (Mathf.Abs(RandPosition - fishNeedleTargetPosition) >
               fish.fish.fishType.MaxFishBarChangeDistance)
        {
            RandPosition = UnityEngine.Random.Range(-283.5f, 283.5f);
        }
        fishNeedleTargetPosition = RandPosition;
        fishNeedleSpeed = UnityEngine.Random.Range(0.05f, 6f);
        await UniTask.WaitForSeconds(RandSecond);
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

    public void ShowUI()
    {
        pullCanva.DOScale(new Vector3(0.79f, 0.79f, 0.79f), 0.35f).SetEase(Ease.OutBack);
    }

    public void CloseUI()
    {
        pullCanva.DOScale(Vector3.zero, 0.15f).SetEase(Ease.OutQuad);
    }

    private void Start()
    {
        animator = player.GetComponent<Animator>();
    }
}