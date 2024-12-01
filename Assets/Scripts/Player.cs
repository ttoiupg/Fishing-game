using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerID ID;
    private void Start()
    {
        ID.canFish = true;
        ID.canRetract = false;
        ID.isFishing = false;
        ID.isBoostState = false;
        ID.FishOnBait = false;
        ID.isPullState = false;
    }
}
