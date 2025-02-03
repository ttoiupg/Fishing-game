using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

using System.Runtime.CompilerServices;
using TMPro;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UI;
using Unity.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class HUDController : PlayerSystem
{
    int prevLevel = 0;
    float prevProgress = 0.0f;
    int targetLevel = 0;
    float targetProgress = 0.0f;
    float _levelTween = 0.0f;
    float _ExpTween = 0.0f;
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
    [Header("Menu UI")]
    public AudioClip OpenSound;
    public AudioClip CloseSound;
    public AudioClip FrameOpenSound;
    public AudioClip FrameCloseSound;
    public RectTransform currentPage;
    public RectTransform resumeButton;
    public RectTransform fishipediaButton;
    public RectTransform settingButton;
    public RectTransform quitButton;
    public bool isPageOpen = false;
    public bool isDpadCurrentInputing = false;
    public bool MenuDebounce = false;
    [Header("Fishipedia")]
    public RectTransform fishipediaPage;
    [Header("Interaction prompt")]
    public RectTransform interactionPrompt;
    public Image promptImage;
    public TextMeshProUGUI promptText;

    float BackLerp(float x)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1;

        return 1 + c3 * Mathf.Pow(x - 1, 3) + c1 * Mathf.Pow(x - 1, 2);
    }
    public void ShowInteractionPrompt()
    {
        interactionPrompt.DOScale(Vector3.one,0.2f).SetEase(Ease.OutBack);
    }
    public void HideInteractionPrompt()
    {
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
    private void ResetMenuState()
    {
        MenuDebounce = false;
    }
    public void MenuOpenAnimation()
    {
        SoundFXManger.Instance.PlaySoundFXClip(OpenSound, player.transform, 0.5f);
        resumeButton.GetComponent<Image>().raycastTarget = true;
        fishipediaButton.GetComponent<Image>().raycastTarget = true;
        settingButton.GetComponent<Image>().raycastTarget = true;
        quitButton.GetComponent<Image>().raycastTarget = true;
        resumeButton.DOScale(Vector3.one,0.2f).SetEase(Ease.OutBack);
        fishipediaButton.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack).SetDelay(0.1f);
        settingButton.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack).SetDelay(0.2f);
        quitButton.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack).SetDelay(0.3f);
    }
    public void MenuCloseAnimation()
    {
        SoundFXManger.Instance.PlaySoundFXClip(CloseSound, player.transform, 0.5f);
        resumeButton.GetComponent<Image>().raycastTarget = false;
        fishipediaButton.GetComponent<Image>().raycastTarget = false;
        settingButton.GetComponent<Image>().raycastTarget = false;
        quitButton.GetComponent<Image>().raycastTarget = false;
        resumeButton.DOScale(Vector3.zero, 0.15f).SetEase(Ease.OutQuint);
        fishipediaButton.DOScale(Vector3.zero, 0.15f).SetEase(Ease.OutQuint).SetDelay(0.1f);
        settingButton.DOScale(Vector3.zero, 0.15f).SetEase(Ease.OutQuint).SetDelay(0.2f);
        quitButton.DOScale(Vector3.zero, 0.15f).SetEase(Ease.OutQuint).SetDelay(0.3f);
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
    public void SwitchMenu(InputAction.CallbackContext callbackContext)
    {
        if (MenuDebounce) return;
        if (player.inspecting == true || player.CardOpened == true) return;
        if (isPageOpen)
        {
            player.menuOpen = true;
            isPageOpen = false;
            CloseUI();
        }
        else
        {
            if (!player.menuOpen)
            {
                player.menuOpen = true;
                MenuOpenAnimation();
            }
            else
            {
                player.menuOpen = false;
                MenuCloseAnimation();
                MenuDebounce = true;
                Invoke("ResetMenuState", 0.5f);
            }
        }
    }
    public void SwitchMenu()
    {
        if (MenuDebounce) return;
        if (player.inspecting == true || player.CardOpened == true) return;
        if (isPageOpen)
        {
            player.menuOpen = true;
            isPageOpen = false;
            CloseUI();
        }
        else
        {
            if (!player.menuOpen)
            {
                player.menuOpen = true;
                MenuOpenAnimation();
            }
            else
            {
                player.menuOpen = false;
                MenuCloseAnimation();
                MenuDebounce = true;
                Invoke("ResetMenuState", 0.5f);
            }
        }
    }
    public void SetOpenSound(AudioClip sound)
    {
        FrameOpenSound = sound;
    }
    public void SetCloseSound(AudioClip sound)
    {
        FrameCloseSound = sound;
    }
    public void OpenUI(RectTransform ui)
    {
        SoundFXManger.Instance.PlaySoundFXClip(FrameOpenSound, player.CharacterTransform, 1f);
        currentPage = ui;
        isPageOpen = true;
        ui.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
    }
    public void CloseUI()
    {
        SoundFXManger.Instance.PlaySoundFXClip(FrameCloseSound, player.CharacterTransform, 1f);
        isPageOpen = false;
        currentPage.DOScale(Vector3.zero, 0.15f).SetEase(Ease.OutQuint);
    }
    //private void RotateNeedle()
    //{
    //    if (currentNeedleLocation != lastNeedleLocation)
    //    {
    //        lastNeedleLocation = currentNeedleLocation;
    //        if (lastNeedleLocation == "up")
    //        {
    //            needleDegree = 0f;
    //        }
    //        else if (lastNeedleLocation == "right")
    //        {
    //            needleDegree = -90f;
    //        }
    //        else if (lastNeedleLocation == "down")
    //        {
    //            needleDegree = 180f;
    //        }
    //        else if (lastNeedleLocation == "left")
    //        {
    //            needleDegree = 90f;
    //        }
    //        SoundFXManger.Instance.PlaySoundFXClip(RotateSound, player.transform, 1f);
    //        MenuNeedle.DORotate(new Vector3(0, 0, needleDegree), spinSpeed).SetEase(Ease.OutBack);
    //    }
    //}
    //private void TrackNeedlePosition()
    //{
    //    if (isMenuOpen) {
    //        if (!isDpadCurrentInputing)
    //        {
    //            Vector2 mousePosition = Mouse.current.position.value;
    //            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
    //            Vector2 deltaVector = mousePosition - screenCenter;
    //            if (deltaVector.y > 0f && deltaVector.y > Mathf.Abs(deltaVector.x))
    //            {
    //                currentNeedleLocation = "up";
    //            }
    //            else if (deltaVector.x > 0f && deltaVector.x > Mathf.Abs(deltaVector.y))
    //            {
    //                currentNeedleLocation = "right";
    //            }
    //            else if (deltaVector.y < 0f && Mathf.Abs(deltaVector.x) < Mathf.Abs(deltaVector.y))
    //            {
    //                currentNeedleLocation = "down";
    //            }
    //            else if (deltaVector.x < 0f && Mathf.Abs(deltaVector.y) < Mathf.Abs(deltaVector.x))
    //            {
    //                currentNeedleLocation = "left";
    //            }
    //        }
    //        RotateNeedle();
    //    }
    //}
    //private void SelectMenu(InputAction.CallbackContext callbackContext)
    //{
    //    if (!isMenuOpen) return;
    //    if (player.isControllerConnected)
    //    {
    //        isDpadCurrentInputing = false;
    //        currentNeedleLocation = callbackContext.control.name;
    //        RotateNeedle();
    //        isMenuOpen = false;
    //    }
    //    else
    //    {
    //        isMenuOpen = false;
    //    }
    //    if (Pages.TryGetValue(currentNeedleLocation,out CurrentPage))
    //    {
    //        isPageOpen = true;
    //        CurrentPage.DOScale(Vector3.one,0.3f).SetEase(Ease.OutBack);
    //    }
    //    MenuCloseAnimation();
    //    MenuDebounce = true;
    //    Invoke("ResetMenuState",0.5f);
    //}
    private void Update()
    {
        UpdateRadialProgress();
        //TrackNeedlePosition();
    }
}
