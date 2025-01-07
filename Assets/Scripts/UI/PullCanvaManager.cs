using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.VisualScripting;

public class PullCanvaManager : PlayerSystem
{
    public RectTransform pullCanva;
    public RectTransform controlBar;
    public RectTransform fishNeedle;
    public RectTransform fishPedal;
    public RectTransform fishPedalL1;
    public RectTransform fishPedalL2;
    public RectTransform fishPedalR1;
    public RectTransform fishPedalR2;


    public float controlBarPosition = 0f;
    public float controlBarGravity = 0f;
    public float controlBarVelocity = 0f;
    public float fishNeedleTargetPosition = 0f;
    public float fishNeedlePosition = 0f;
    public float PullProgress = 0f;
    public float fishNeedleSpeed = 6f;
    public bool isFishBarOverlaping = false;
    public bool firstFliped = false;
    public bool secondFliped = false;
    public bool thirdFliped = false;

    public float rawLeftBound = -286;
    public float rawRightBound = 286f;

    private Animator animator;
    private float Lerpfloat(float a, float b, float t)
    {
        return a + (b - a) * t;
    }
    public void Init()
    {
        controlBar.sizeDelta = new Vector2(player.ID.pullBarSize, 94.7651f);
        controlBar.anchoredPosition = new Vector2(0, -74.718f);
        fishNeedle.anchoredPosition = new Vector2(0, -139.5762f);
        fishPedal.localScale = new Vector3(1,0,1);
        fishPedalL1.localScale = new Vector3(1, 0, 1);
        fishPedalR1.localScale = new Vector3(1, 0, 1);
        fishPedalL2.localScale = new Vector3(1, 0, 1);
        fishPedalR2.localScale = new Vector3(1, 0, 1);
        controlBarGravity = -9.8f;
        controlBarPosition = 0f;
        fishNeedlePosition = 0f;
        fishNeedlePosition = 0f;
        fishNeedleSpeed = 6f;
        PullProgress = 0f;
        isFishBarOverlaping = false;
        StartCoroutine(RandomFishBarPosition());
    }
    public void UpdatePosition()
    {
        float leftBound = rawLeftBound + controlBar.sizeDelta.x / 2;
        float rightBound = rawRightBound - controlBar.sizeDelta.x / 2;
        controlBarVelocity += controlBarGravity * Time.deltaTime;
        float nextPosition = controlBarPosition + controlBarVelocity * Time.deltaTime;
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
        controlBar.anchoredPosition = new Vector2(controlBarPosition, -74.718f);

        fishNeedlePosition = Lerpfloat(fishNeedlePosition, fishNeedleTargetPosition, fishNeedleSpeed * Time.deltaTime);
        fishNeedle.anchoredPosition = new Vector2(fishNeedlePosition, -139.5762f);
    }
    public void FlipFirst()
    {
        firstFliped = true;
        fishPedal.DOScaleY(1, 0.25f).SetEase(Ease.OutBounce);
    }
    public void FlipSecond()
    {
        player.attackBuff = 1f;
        secondFliped = true;
        fishPedalL1.DOScaleY(1, 0.25f).SetEase(Ease.OutBounce);
        fishPedalR1.DOScaleY(1, 0.25f).SetEase(Ease.OutBounce);
    }
    public void FlipThird()
    {
        player.attackBuff = 1.5f;
        thirdFliped = true;
        fishPedalL2.DOScaleY(1, 0.25f).SetEase(Ease.OutBounce);
        fishPedalR2.DOScaleY(1, 0.25f).SetEase(Ease.OutBounce);
    }
    public void FlipDownAll()
    {
        firstFliped = false;
        secondFliped = false;
        thirdFliped = false;
        fishPedal.DOScaleY(0, 0.25f).SetEase(Ease.OutBounce);
        fishPedalL1.DOScaleY(0, 0.25f).SetEase(Ease.OutBounce);
        fishPedalR1.DOScaleY(0, 0.25f).SetEase(Ease.OutBounce);
        fishPedalL2.DOScaleY(0, 0.25f).SetEase(Ease.OutBounce);
        fishPedalR2.DOScaleY(0, 0.25f).SetEase(Ease.OutBounce);
    }
    public IEnumerator RandomFishBarPosition()
    {
        while (player.pullstate == true)
        {
            float RandSecond = UnityEngine.Random.Range(player.currentFish.fishType.MinFishBarChangeTime, player.currentFish.fishType.MaxFishBarChangeTime);
            float RandPosition = UnityEngine.Random.Range(-283.5f, 283.5f);
            while (Mathf.Abs(RandPosition - fishNeedleTargetPosition) > player.currentFish.fishType.MaxFishBarChangeDistance)
            {
                RandPosition = UnityEngine.Random.Range(-283.5f, 283.5f);
            }
            fishNeedleTargetPosition = RandPosition;
            fishNeedleSpeed = UnityEngine.Random.Range(0.05f, 6f);
            yield return new WaitForSeconds(RandSecond);
        }
    }
    public void GamepadVibration()
    {
        float lowfreq = 0.5f;
        float highfreq = player.playerInputs.Fishing.ControlFishingRod.ReadValue<float>() * 0.6f;
        if (isFishBarOverlaping)
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
        pullCanva.DOScale(Vector3.one,0.35f).SetEase(Ease.OutBack);
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
