using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using DG.Tweening;

using System.Runtime.CompilerServices;
using TMPro;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UI;
using Unity.Collections;
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

    [Header("Menu UI")]
    public AudioClip OpenSound;
    public AudioClip CloseSound;
    public AudioClip RotateSound;
    public float openTime = 0.22f;
    public float closeTime = 0.17f;
    public float spinSpeed = 0.26f;
    public float needleDegree = 0f;
    public RectTransform CurrentPage;
    public RectTransform MenuCenterCircle;
    public RectTransform MenuNeedle;
    public RectTransform UpMenu;
    public RectTransform DownMenu;
    public RectTransform LeftMenu;
    public RectTransform RightMenu;
    public string currentNeedleLocation = "None";
    public string lastNeedleLocation = "None";
    public bool isPageOpen = false;
    public bool isMenuOpen = false;
    public bool isDpadCurrentInputing = false;
    public bool MenuDebounce = false;
    [Header("Fishipedia")]
    public RectTransform FishipediaPage;

    public Dictionary<string, RectTransform> Pages = new Dictionary<string, RectTransform>();
    float BackLerp(float x)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1;

        return 1 + c3 * Mathf.Pow(x - 1, 3) + c1 * Mathf.Pow(x - 1, 2);
    }
    private void ResetMenuState()
    {
        MenuDebounce = false;
    }
    private void MenuOpenAnimation()
    {
        SoundFXManger.Instance.PlaySoundFXClip(OpenSound, player.transform, 0.5f);
        MenuCenterCircle.DOScale(new Vector3(1, 1, 1), openTime).SetEase(Ease.OutBack);
        UpMenu.DOLocalMoveY(341f,0.25f).SetEase(Ease.OutBack).SetDelay(0.1f);
        UpMenu.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack).SetDelay(0.1f);
        LeftMenu.DOLocalMoveX(-418f, 0.25f).SetEase(Ease.OutBack).SetDelay(0.2f);
        LeftMenu.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack).SetDelay(0.2f);
        DownMenu.DOLocalMoveY(-341f, 0.25f).SetEase(Ease.OutBack).SetDelay(0.3f);
        DownMenu.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack).SetDelay(0.3f);
        RightMenu.DOLocalMoveX(418f, 0.25f).SetEase(Ease.OutBack).SetDelay(0.4f);
        RightMenu.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack).SetDelay(0.4f);
    }
    private void MenuCloseAnimation()
    {
        SoundFXManger.Instance.PlaySoundFXClip(CloseSound, player.transform, 0.5f);
        UpMenu.DOLocalMoveY(0, 0.25f).SetEase(Ease.InBack);
        UpMenu.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack);
        LeftMenu.DOLocalMoveX(0, 0.25f).SetEase(Ease.InBack).SetDelay(0.1f);
        LeftMenu.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack).SetDelay(0.1f);
        DownMenu.DOLocalMoveY(0, 0.25f).SetEase(Ease.InBack).SetDelay(0.2f);
        DownMenu.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack).SetDelay(0.2f);
        RightMenu.DOLocalMoveX(0, 0.25f).SetEase(Ease.InBack).SetDelay(0.3f);
        RightMenu.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack).SetDelay(0.3f);
        MenuCenterCircle.DOScale(Vector3.zero, closeTime).SetEase(Ease.InBack).SetDelay(0.4f);
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
    private void SwitchMenu(InputAction.CallbackContext callbackContext)
    {
        if (MenuDebounce) return;
        if (player.inspecting == true || player.CardOpened == true) return;
        if (isPageOpen)
        {
            player.menuOpen = false;
            isPageOpen = false;
            CurrentPage.DOScale(Vector3.zero, 0.2f).SetEase(Ease.OutBack);
        }
        else
        {
            isMenuOpen = !isMenuOpen;
            if (isMenuOpen)
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
    private void RotateNeedle()
    {
        if (currentNeedleLocation != lastNeedleLocation)
        {
            lastNeedleLocation = currentNeedleLocation;
            if (lastNeedleLocation == "up")
            {
                needleDegree = 0f;
            }
            else if (lastNeedleLocation == "right")
            {
                needleDegree = -90f;
            }
            else if (lastNeedleLocation == "down")
            {
                needleDegree = 180f;
            }
            else if (lastNeedleLocation == "left")
            {
                needleDegree = 90f;
            }
            SoundFXManger.Instance.PlaySoundFXClip(RotateSound, player.transform, 1f);
            MenuNeedle.DORotate(new Vector3(0, 0, needleDegree), spinSpeed).SetEase(Ease.OutBack);
        }
    }
    private void TrackNeedlePosition()
    {
        if (isMenuOpen) {
            if (!isDpadCurrentInputing)
            {
                Vector2 mousePosition = Mouse.current.position.value;
                Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
                Vector2 deltaVector = mousePosition - screenCenter;
                if (deltaVector.y > 0f && deltaVector.y > Mathf.Abs(deltaVector.x))
                {
                    currentNeedleLocation = "up";
                }
                else if (deltaVector.x > 0f && deltaVector.x > Mathf.Abs(deltaVector.y))
                {
                    currentNeedleLocation = "right";
                }
                else if (deltaVector.y < 0f && Mathf.Abs(deltaVector.x) < Mathf.Abs(deltaVector.y))
                {
                    currentNeedleLocation = "down";
                }
                else if (deltaVector.x < 0f && Mathf.Abs(deltaVector.y) < Mathf.Abs(deltaVector.x))
                {
                    currentNeedleLocation = "left";
                }
            }
            RotateNeedle();
        }
    }
    private void SelectMenu(InputAction.CallbackContext callbackContext)
    {
        if (!isMenuOpen) return;
        if (player.isControllerConnected)
        {
            isDpadCurrentInputing = false;
            currentNeedleLocation = callbackContext.control.name;
            RotateNeedle();
            isMenuOpen = false;
        }
        else
        {
            isMenuOpen = false;
        }
        if (Pages.TryGetValue(currentNeedleLocation,out CurrentPage))
        {
            isPageOpen = true;
            CurrentPage.DOScale(Vector3.one,0.3f).SetEase(Ease.OutBack);
        }
        MenuCloseAnimation();
        MenuDebounce = true;
        Invoke("ResetMenuState",0.5f);
    }
    private void OnEnable()
    {
        playerInput.UI.Enable();
        playerInput.UI.OpenMenu.performed += SwitchMenu;
        playerInput.UI.SelectMenu.performed += SelectMenu;
    }
    private void OnDisable()
    {
        playerInput.UI.OpenMenu.performed -= SwitchMenu;
        playerInput.UI.SelectMenu.performed -= SelectMenu;
    }
    private void Start()
    {
        Pages.Add("left", FishipediaPage);
    }
    private void Update()
    {
        UpdateRadialProgress();
        TrackNeedlePosition();
    }
}
