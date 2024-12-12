using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using DG.Tweening;
using UnityEditor.Rendering;
public class HUDController : PlayerSystem
{
    public UIDocument UIDocument;
    TideAndFinsUILibrary.RadialProgress radialProgress;

    int prevLevel = 0;
    float prevProgress = 0.0f;
    int targetLevel = 0;
    float targetProgress = 0.0f;
    float _levelTween = 0.0f;
    float _ExpTween = 0.0f;
    [Header("level progress ui")]
    public float LevelSpeed = 0.2f;
    public float ExpSpeed = 0.5f;

    [Header("Menu UI")]
    public RectTransform MenuCenterCircle;
    public RectTransform MenuNeedle;
    public string currentMouseLocation = "None";
    public string lastMouseLocation = "None";
    public bool isMenuOpen = false;
    float BackLerp(float x)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1;

        return 1 + c3 * Mathf.Pow(x - 1, 3) + c1 * Mathf.Pow(x - 1, 2);
    }


    private void UpdateLevelProgress(Fish fish)
    {
        targetLevel = player.level;
        targetProgress = player.experience / player.expRequire * 100f;
    }
    private void UpdateRadialProgress()
    {
        if (targetLevel > prevLevel)
        {
            float fixedDt = Time.deltaTime / LevelSpeed;
            _levelTween += fixedDt;

            if (_levelTween > 1f)
            {
                prevLevel += 1;
                radialProgress.level += 1;
                prevProgress = 0.0f;
                _levelTween = 0.0f;
                radialProgress.Progress = 0.0f;
            }
            else
            {
                float deltaP = (100 - prevProgress);
                radialProgress.Progress = prevProgress + deltaP * _levelTween;
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
                radialProgress.Progress = targetProgress;
            }
            else
            {
                float deltaP = (targetProgress - prevProgress);
                radialProgress.Progress = prevProgress + deltaP * BackLerp(_ExpTween);
            }
        }
    }
    private void SwitchMenu(InputAction.CallbackContext callbackContext)
    {
        isMenuOpen = !isMenuOpen;
        Debug.Log("Switch menu");
        if (isMenuOpen)
        {
            MenuCenterCircle.DOScale(new Vector3(1, 1, 1), 0.35f).SetEase(Ease.OutBack);
        }
        else
        {
            MenuCenterCircle.DOScale(new Vector3(0, 0, 0), 0.35f).SetEase(Ease.InBack);
        }
    }
    private void RotateNeedle()
    {
        if (currentMouseLocation != lastMouseLocation)
        {
            lastMouseLocation = currentMouseLocation;
            if (lastMouseLocation == "Up")
            {
                MenuNeedle.DORotate(new Vector3(0,0,0),0.35f).SetEase(Ease.OutBack);
            }else if (lastMouseLocation == "Right")
            {
                MenuNeedle.DORotate(new Vector3(0, 0, -90f), 0.35f).SetEase(Ease.OutBack);
            }
            else if (lastMouseLocation == "Down")
            {
                MenuNeedle.DORotate(new Vector3(0, 0, 180f), 0.35f).SetEase(Ease.OutBack);
            }
            else if (lastMouseLocation == "Left")
            {
                MenuNeedle.DORotate(new Vector3(0, 0, 90f), 0.35f).SetEase(Ease.OutBack);
            }
        }
    }
    private void TrackMousePosition()
    {
        if (isMenuOpen) {
            Vector2 mousePosition =  Mouse.current.position.value;
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Vector2 deltaVector = mousePosition - screenCenter;
            if (deltaVector.y > 0f && deltaVector.y > Mathf.Abs(deltaVector.x))
            {
                currentMouseLocation = "Up";
            }else if (deltaVector.x > 0f && deltaVector.x > Mathf.Abs(deltaVector.y))
            {
                currentMouseLocation = "Right";
            }else if (deltaVector.y < 0f && Mathf.Abs(deltaVector.x) < Mathf.Abs(deltaVector.y))
            {
                currentMouseLocation = "Down";
            }else if (deltaVector.x < 0f && Mathf.Abs(deltaVector.y) < Mathf.Abs(deltaVector.x))
            {
                currentMouseLocation = "Left";
            }
            RotateNeedle();
        }
    }
    private void OnEnable()
    {
        playerInput.UI.Enable();
        player.ID.playerEvents.OnFishCatched += UpdateLevelProgress;
        playerInput.UI.OpenMenu.performed += SwitchMenu;
    }
    private void OnDisable()
    {
        player.ID.playerEvents.OnFishCatched -= UpdateLevelProgress;
        playerInput.UI.OpenMenu.performed -= SwitchMenu;
    }
    private void Start()
    {
        radialProgress = UIDocument.rootVisualElement.Q("RadialProgress") as TideAndFinsUILibrary.RadialProgress;
        //MenuContainer = UIDocument.rootVisualElement.Q("MenuContainer");
    }
    private void Update()
    {
        UpdateRadialProgress();
        TrackMousePosition();
    }
}
