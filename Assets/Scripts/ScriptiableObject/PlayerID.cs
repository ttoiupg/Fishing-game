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
    public bool isPullState = false;
    public bool FishOnBait = false;
    public int stanima = 100;
    public BaseZone currentZone;
    public float pullProgressSpeed = 60f;
    public float pullProgressLooseSpeed = 30f;
    public float pullBarSize = -350;
    public float luckBoost = 0;
    public float pullSpeedBoost = 0;
    public float pullBarSizeBoost = 0;
    public float GreenZonebuff = 0.35f;
    public float OrangeZonebuff = 0.15f;

    [Header("Events")]
    public PlayerEvents playerEvents;

}
