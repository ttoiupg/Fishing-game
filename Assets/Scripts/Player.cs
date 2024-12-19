using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerID ID;
    public bool isControllerConnected = false;

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
            while (_experience >= expRequire)
            {
                _experience -= expRequire;
                level += 1;
                expRequire = (float)GetExpRQ(level);
            }
        }
    }
    public List<BaseFish> UnlockedFishes;

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
    IEnumerator CheckForControllers()
    {
        while (true)
        {
            var controllers = Input.GetJoystickNames();

            if (!isControllerConnected && controllers.Length > 0)
            {
                isControllerConnected = true;
                Debug.Log("Connected");

            }
            else if (isControllerConnected && controllers.Length == 0)
            {
                isControllerConnected = false;
                Debug.Log("Disconnected");
            }

            yield return new WaitForSeconds(1f);
        }
    }

    void Awake()
    {
        StartCoroutine(CheckForControllers());
    }
}
