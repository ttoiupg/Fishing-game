using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public struct PlayerEvents
{
    //input events
    public Action OnThrowingRodInput;
    public Action OnChatInput;

    //Fishing events
    public Action OnEnterFishingState;
    public Action OnExitFishingState;
    public Action<BaseZone> OnZoneEnter;
    public Action OnZoneExit;
    public Action OnRodThrew;
    public Action OnFishBite;
    public Action<Fish> OnFishCatched;
    public Action OnBoostStage;
    public Action<float> FinishedBoostStage;
}