using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerID ID;

    [Header("Fishing")]
    public bool canFish = true;
    public bool canRetract = false;
    public bool isFishing = false;
    public bool isBoostState = false;
    public bool isPullState = false;
    public bool FishOnBait = false;
    public BaseZone currentZone;

    [Header("Character")]
    public int Facing = 1;
}
