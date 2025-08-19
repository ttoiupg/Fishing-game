using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BoostCanvaManager : MonoBehaviour
{
    public AudioClip openSound;
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
    public Player player;

    public float moveRange = 17.5f;
    public float greenRange = 12.2f;
    public float orangeRange = 35.03f;

    public void StartBoost()
    {
        if (pointerTween != null)
        {
            pointerTween.Kill();
        }
        pointer.rotation = Quaternion.Euler(0, 0, 62.45f);
        pointerTween = pointer.DORotate(new Vector3(0, 0 , -62.45f),0.45f);
        pointerTween.SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }
    public string LandPointer()
    {
        pointerTween.Kill();
        float rotation = pointer.rotation.eulerAngles.z;
        if (rotation >= 180)
        {
            rotation -= 360;
        }
        float fixRotation = currentRotation * (28.34f / moveRange);
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
        SoundFXManger.Instance.PlaySoundFXClip(openSound, player.characterTransform, 1f);
        currentRotation = Random.Range(-moveRange, moveRange);
        greenZone.rotation = Quaternion.Euler(0, 0, currentRotation);
        orangeZone.rotation = Quaternion.Euler(0, 0, currentRotation);
        //do animation
        pointer.DORotate(new Vector3(0,0, 62.45f),0.5f).SetEase(Ease.OutBack);
        boostState.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutBack);
        rightGear.DORotate(new Vector3(0, 0, -41.727f), 0.5f).SetEase(Ease.OutCubic).SetDelay(0.2f);
        foreGround.DOFillAmount(0.81f, 0.5f).SetEase(Ease.OutCubic).SetDelay(0.2f);
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
