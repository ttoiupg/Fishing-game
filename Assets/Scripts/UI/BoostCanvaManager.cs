using DG.Tweening;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BoostCanvaManager : PlayerSystem
{
    public RectTransform boostState;
    public Image foreGround;
    public RectTransform redZone;
    public RectTransform orangeZone;
    public RectTransform greenZone;
    public Image redImage;
    public Image orangeImage;
    public Image greenImage;
    public RectTransform rightGear;
    public RectTransform pointer;

    private float currentRotation;
    private Tween pointerTween;

    public float moveRange = 17.5f;
    public float greenRange = 12.2f;
    public float orangeRange = 35.03f;

    public void StartBoost()
    {
        pointerTween = pointer.DORotate(new Vector3(0, 0 , -62.45f),0.45f);
        pointerTween.SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }
    public string LandPointer()
    {
        pointerTween.Pause();
        float rotation = pointer.rotation.eulerAngles.z;
        if (rotation >= 180)
        {
            rotation -= 360;
        }
        float fixRotation = currentRotation * (28.34f / moveRange);
        //Debug.Log(rotation);
        //Debug.Log(currentRotation);
        //Debug.Log(fixRotation);
        //Debug.Log(fixRotation + greenRange);
        //Debug.Log(fixRotation - greenRange);
        //Debug.Log(fixRotation + orangeRange);
        //Debug.Log(fixRotation - orangeRange);
        if (rotation <= fixRotation + greenRange && rotation >= fixRotation - greenRange)
        {
            return "green";
        }
        else if (rotation <= fixRotation + orangeRange && rotation >= fixRotation - orangeRange)
        {
            return "orange";
        }
        else
        {
            return "red";
        }
    }
    public void ShowBoostUI()
    {
        currentRotation = Random.Range(-moveRange, moveRange);
        greenZone.rotation = Quaternion.Euler(0, 0, currentRotation);
        orangeZone.rotation = Quaternion.Euler(0, 0, currentRotation);
        //do animation
        pointer.DORotate(new Vector3(0,0, 62.45f),0.5f).SetEase(Ease.OutBack);
        boostState.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutBack);
        rightGear.DORotate(new Vector3(0, 0, -41.727f), 0.7f).SetEase(Ease.OutQuint).SetDelay(0.2f);
        foreGround.DOFillAmount(0.81f, 0.7f).SetEase(Ease.OutQuint).SetDelay(0.2f);
        redImage.DOFillAmount(1, 0.7f).SetEase(Ease.OutQuint).SetDelay(0.2f);
        orangeImage.DOFillAmount(1, 0.7f).SetEase(Ease.OutQuint).SetDelay(0.2f);
        greenImage.DOFillAmount(1, 0.7f).SetEase(Ease.OutQuint).SetDelay(0.2f);
        Invoke("StartBoost",0.9f);
    }
    public void HideBoostUI()
    {
        pointer.DORotate(new Vector3(0,0,180), 0.5f).SetEase(Ease.OutBack);
        rightGear.DORotate(new Vector3(0, 0, 41.727f), 0.7f).SetEase(Ease.OutQuint);
        foreGround.DOFillAmount(0.2f, 0.7f).SetEase(Ease.OutQuint);
        redImage.DOFillAmount(0, 0.7f).SetEase(Ease.OutQuint);
        orangeImage.DOFillAmount(0, 0.7f).SetEase(Ease.OutQuint);
        greenImage.DOFillAmount(0, 0.7f).SetEase(Ease.OutQuint);
        boostState.DOScale(Vector3.zero, 0.3f).SetEase(Ease.OutBack).SetDelay(0.2f);
    }
}
