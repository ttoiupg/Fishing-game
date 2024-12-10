using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerID ID;

    [Header("Stats")]
    public float expRequire = 1f;
    public float _experience = 0.0f;
    public int level = 1;
    public float experience
    {
        get => _experience;
        set
        {
            _experience = value;
            expRequire = (float)GetExpRQ(level);
            Debug.Log(expRequire);
            while (_experience >= expRequire)
            {
                _experience -= expRequire;
                level += 1;
                expRequire = (float)GetExpRQ(level);
                Debug.Log(expRequire);
            }
        }
    }

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

    float GetExpRQ(int level)
    {
        return Mathf.Round((4 * (Mathf.Pow((float)level,3f))) / 5);
    }
}
