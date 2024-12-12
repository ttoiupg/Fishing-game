using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
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

    private VisualElement MenuContainer;
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
        MenuContainer.SetEnabled(isMenuOpen);
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
        MenuContainer = UIDocument.rootVisualElement.Q("MenuContainer");
    }
    private void Update()
    {
        UpdateRadialProgress();
    }
}
