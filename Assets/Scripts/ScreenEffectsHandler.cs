using DG.Tweening;
using System.Collections;
using TMPro;
using TMPro.Examples;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ScreenEffectsHandler : MonoBehaviour
{
    public Player player;
    [Header("FishCatch")]
    public CanvasGroup fishCatchScreenCanvaGroup;
    public Animator fishCatchScreenAnimator;
    public Animator fishAnimator;
    public ParticleSystem whiteMiddle;
    public ParticleSystem leftSmall;
    public ParticleSystem leftBig;
    public ParticleSystem leftBlue;
    public ParticleSystem rightSmall;
    public ParticleSystem rightBig;
    public ParticleSystem rightBlue;
    [Header("FishInspect")]
    public CanvasGroup fishInspectGroup;
    public GameObject fishInspectBlackCover;
    public RectTransform brightSlice;
    public RectTransform darkSlice;
    public RectTransform fishImage;
    public RectTransform mutation;
    public RectTransform weight;
    public RectTransform fishName;
    public TextMeshProUGUI weightText;
    public TextMeshProUGUI mutationText;
    public IEnumerator PlayFishFirstCatchAnimation(Fish fish)
    {
        player.inspecting = true;
        fishCatchScreenCanvaGroup.alpha = 1.0f;
        fishCatchScreenCanvaGroup.blocksRaycasts = true;
        fishCatchScreenCanvaGroup.interactable = true;
        whiteMiddle.Play();
        leftSmall.Play();
        leftBig.Play();
        leftBlue.Play();
        rightSmall.Play();
        rightBig.Play();
        rightBlue.Play();
        fishCatchScreenAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(0.45f);
        fishAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(3.15f);
        PlayFishInspect(fish);
        yield return new WaitForSeconds(0.5f);
        whiteMiddle.Stop();
        leftSmall.Stop();
        leftBig.Stop();
        leftBlue.Stop();
        rightSmall.Stop();
        rightBig.Stop();
        rightBlue.Stop();
        fishCatchScreenCanvaGroup.alpha = 0f;
        fishCatchScreenCanvaGroup.blocksRaycasts = false;
        fishCatchScreenCanvaGroup.interactable = false;
    }
    public void PlayFishInspect(Fish fish)
    {
        fishInspectGroup.alpha = 1.0f;
        fishInspectGroup.blocksRaycasts = true;
        fishInspectGroup.interactable = true;
        fishInspectBlackCover.SetActive(true);
        fishImage.GetComponent<Image>().sprite = fish.fishType.Art;
        fishName.GetComponent<TextMeshProUGUI>().text = fish.fishType.name;
        weightText.text = fish.weight.ToString() + "kg";
        mutationText.text = fish.mutation.name;
        brightSlice.DOAnchorPosX(320.82f,0.4f).SetEase(Ease.OutBack);
        darkSlice.DOAnchorPosX(256f, 0.4f).SetEase(Ease.OutBack).SetDelay(0.15f);
        fishImage.DOAnchorPosX(516f, 0.4f).SetEase(Ease.OutBack).SetDelay(0.3f);
        fishName.DOAnchorPosX(1350.16f, 0.4f).SetEase(Ease.OutBack).SetDelay(0.45f);
        weight.DOAnchorPosY(507f, 0.4f).SetEase(Ease.OutBack).SetDelay(0.6f);
        mutation.DOAnchorPosY(374f, 0.4f).SetEase(Ease.OutBack).SetDelay(0.75f);
    }
    public void CloseInspectView()
    {
        player.inspecting = false;
        fishInspectGroup.alpha = 0f;
        fishInspectGroup.blocksRaycasts = false;
        fishInspectGroup.interactable = false;
    }
    public void StopFishInspect()
    {
        fishImage.DOAnchorPosX(-600f, 0.4f).SetEase(Ease.InBack);
        fishName.DOAnchorPosX(2500f, 0.4f).SetEase(Ease.InBack).SetDelay(0.1f);
        darkSlice.DOAnchorPosX(-600f, 0.4f).SetEase(Ease.InBack).SetDelay(0.2f);
        brightSlice.DOAnchorPosX(-600f, 0.4f).SetEase(Ease.InBack).SetDelay(0.3f);
        mutation.DOAnchorPosY(-120f, 0.4f).SetEase(Ease.InBack).SetDelay(0.4f);
        weight.DOAnchorPosY(-120f, 0.4f).SetEase(Ease.InBack).SetDelay(0.5f);
        Invoke("CloseInspectView",2f);
    }
}
