using TMPro;
using UnityEngine;

[CreateAssetMenu]
public class PlayerID : ScriptableObject
{
    [Header("Properties")]
    public string playerName;
    public bool canFish = true;
    public bool canRetract = false;
    public bool isFishing = false;
    public bool isBoostState = false;
    public bool isPullState = false;
    public bool FishOnBait = false;
    public int stanima = 100;
    public BaseZone currentZone;
    public float pullProgressSpeed = 8f;
    public float pullProgressLooseSpeed = 5f;
    public float pullBarSize = 30f;
    public float luckBoost = 0;
    public float pullSpeedBoost = 0;
    public float pullBarSizeBoost = 0;
    public float GreenZonebuff = 35f;
    public float OrangeZonebuff = 15f;

    [Header("Events")]
    public PlayerEvents playerEvents;

}
