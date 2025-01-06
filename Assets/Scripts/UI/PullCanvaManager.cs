using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using UnityEngine.InputSystem;
using System.Collections;

public class PullCanvaManager : MonoBehaviour
{
    public RectTransform pullCanva;
    public RectTransform controlBar;
    public RectTransform fishNeedle;
    public RectTransform fishPedal;
    public RectTransform fishPedalL1;
    public RectTransform fishPedalL2;
    public RectTransform fishPedalR1;
    public RectTransform fishPedalR2;


    public float controlBarPosition = 50f;
    public float controlBarGravity = 0f;
    public float controlBarVelocity = 0f;
    public float fishNeedleTargetPosition = 0f;
    public float fishNeedlePosition = 0f;
    public float PullProgress = 0f;
    public float fishNeedleSpeed = 6f;
    public bool isFishBarOverlaping = false;

    public float rawLeftBound = -286;
    public float rawRightBound = 286f;

    private InputAction controlBarAction;
    private bool IsOverlap(float a1, float a2, float b1, float b2)
    {
        float a_width = a2 - a1;
        float b_width = b2 - b1;
        if (b2 > a2) return a2 >= b1;
        if (b2 < a1) return a1 <= b2;
        if (b2 < a2 && a1 > b2) return true;
        return true;
        /*if (InBetween(b1, b2, a1) || InBetween(b1, b2, a2) || InBetween(a1, a2, b1) || InBetween(a1, a2, b2))
        {
            return true;
        }

        return false;*/
    }
    public void Init()
    {
        controlBar.anchoredPosition = new Vector2(0, -74.718f);
        fishNeedle.anchoredPosition = new Vector2(0, -139.5762f);
        controlBarGravity = -9.8f;
        controlBarPosition = 0f;
        fishNeedlePosition = 0f;
        fishNeedlePosition = 0f;
        fishNeedleSpeed = 6f;
        PullProgress = 0f;
        isFishBarOverlaping = false;
    }
    public void UpdateControlBarPosition()
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
        controlBar.anchoredPosition = new Vector2(nextPosition, -74.718f);
    }
    //public bool checkControlOverlap()
    //{

    //}
    //public IEnumerator RandomFishBarPosition()
    //{
    //    while (player.pullstate == true)
    //    {
    //        float RandSecond = UnityEngine.Random.Range(CurrentFish.fishType.MinFishBarChangeTime, CurrentFish.fishType.MaxFishBarChangeTime);

    //        float RandPosition = UnityEngine.Random.Range(0, 98f);
    //        while (Mathf.Abs(RandPosition - fishNeedleTargetPosition) > CurrentFish.fishType.MaxFishBarChangeDistance)
    //        {
    //            RandPosition = UnityEngine.Random.Range(0, 98f);
    //        }
    //        fishNeedleTargetPosition = UnityEngine.Random.Range(0, 98f);
    //        fishNeedleSpeed = UnityEngine.Random.Range(0.05f, 6f);
    //        yield return new WaitForSeconds(RandSecond);
    //    }
    //}
    public void GamepadVibration()
    {
        float lowfreq = 0.5f;
        float highfreq = controlBarAction.ReadValue<float>() * 0.6f;
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
}
