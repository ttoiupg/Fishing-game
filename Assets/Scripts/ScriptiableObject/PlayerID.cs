using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu]
public class PlayerID : ScriptableObject
{
    [Header("Properties")]
    public string playerName;
    public bool canFish = true;
    public bool isFishing = false;
    public bool isBoostState = false;
    public bool FishOnBait = false;
    public int stanima = 100;
    public BaseZone currentZone;
    public float luckBoost = 0;
    public float lureSpeedBoost = 0;
    public float lureBarSizeBoost = 0;

    [Header("Events")]
    public PlayerEvents playerEvents;

}
