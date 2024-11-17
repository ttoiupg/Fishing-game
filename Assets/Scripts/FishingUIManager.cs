using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FishingUIManager : PlayerSystem
{
    public CanvasGroup BoostCanva;
    public RectTransform RedZone;
    public RectTransform OrangeZone;
    public RectTransform GreenZone;
    public RectTransform Needle;
    private bool IsBoostState = false;
    [SerializeField]
    private float needleSpeed = 3f;
    private int needleDirection = 1;
    //private float OrangeWidth = 200f;
    //private float GreenWidth = 100f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsBoostState)
        {
            Vector2 NeedlePosDelta = new Vector2(needleDirection * 700 * needleSpeed * Time.deltaTime,0);
            Needle.anchoredPosition += NeedlePosDelta;
            if (Needle.anchoredPosition.x > 350f || Needle.anchoredPosition.x < -350f)
            {
                needleDirection *= -1;
            }
        }
    }
    public void LandNeedle(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed == true)
        {
            if (player.ID.FishOnBait) {
                IsBoostState = false;
                player.ID.playerEvents.FinishedBoostStage.Invoke(Needle.anchoredPosition.x);
            };
        }
    }
    private void enterBoostState()
    {
        IsBoostState = true;
        float RandOrangePos = Random.Range(-350f,350f);
        Vector2 RandPos = new Vector2(RandOrangePos, 0);
        OrangeZone.anchoredPosition = RandPos;
        GreenZone.anchoredPosition = RandPos;
    }
    private void OnEnable()
    {
        player.ID.playerEvents.OnBoostStage += enterBoostState;
    }
    private void OnDisable()
    {
        player.ID.playerEvents.OnBoostStage -= enterBoostState;
    }
}
