using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour,IDataPersistence
{
    public PlayerID ID;
    public bool isControllerConnected = false;

    [Header("Stats")]
    public float expRequire = 1f;
    [SerializeField]
    private float _experience = 0.0f;
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
    public List<DiscoveredFish> discoveredFish;
    public List<StatusEffect> PlayerEffects;
    private List<IDataDiscoverFish> dataDiscoverFish;
    public PlayerState currentState;

    [Header("Fishing")]
    public BaseZone currentZone;
    public bool castRodDebounce;
    public bool retrackDebounce;
    public bool booststate;
    public bool pullstate;
    public bool menuOpen;
    public bool inspecting;
    public bool CardOpened;
    public bool fishing;

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
    private void Update()
    {
        for(int i=0;i<PlayerEffects.Count;i++)
        {
            StatusEffect effect = PlayerEffects[i];
            if (effect.TimeLimited == true)
            {
                if (effect.length <= Time.deltaTime)
                {
                    PlayerEffects.RemoveAt(i);
                }
                effect.length -= Time.deltaTime;
            }
        }
    }
    public void LoadData(GameData gameData)
    {
        level = gameData.level;
        experience = gameData.experience;
        expRequire = (float)GetExpRQ(level);
        discoveredFish.Clear();
        foreach(IDataDiscoverFish dataFish in gameData.discoveredFish)
        {
            BaseFish b_fish = DataPersistenceManager.Instance.gameFish.Find((x) => x.name == dataFish.name);
            discoveredFish.Add(new DiscoveredFish(b_fish,dataFish.discoverDate));
        }
    }
    public void SaveData(ref GameData gameData)
    {
        gameData.level = level;
        gameData.experience = experience;
        List<IDataDiscoverFish> tempDisFish = new List<IDataDiscoverFish>();
        foreach (DiscoveredFish Fish in discoveredFish)
        {
            tempDisFish.Add(new IDataDiscoverFish(Fish));
        }
        gameData.discoveredFish = tempDisFish;
    }
    public void AddStatusEffect(StatusEffect effect)
    {
        StatusEffect PreEffect = PlayerEffects.Find((x) => x.name == effect.name);
        if (PreEffect == null)
        {
            PlayerEffects.Add(effect);
        }
        else
        {
            PreEffect.length = effect.length;
        }
    }
    public void RemoveStatusEffect(StatusEffect effect)
    {
        StatusEffect PrevEffect = PlayerEffects.Find((x) => x.name == effect.name);
        if (PrevEffect != null)
        {
            PlayerEffects.Remove(PrevEffect);
        }
    }
    public bool HaveStatusEffect(string effectName)
    {
        return PlayerEffects.Exists((x) => x.name == effectName);
    }
}
[System.Serializable]
public class IDataDiscoverFish
{
    public string name;
    public string discoverDate;
    public IDataDiscoverFish(DiscoveredFish fish)
    {
        name = fish.baseFish.name;
        discoverDate = fish.discoverDate;
    }
}
[System.Serializable]
public class StatusEffect
{
    public string name;
    public bool TimeLimited;
    public float length;

    public StatusEffect(string _name, float _length)
    {
        name = _name;
        TimeLimited = true;
        length = _length;
    }
    public StatusEffect(string _name)
    {
        name = _name;
        TimeLimited = false;
    }
}
[System.Serializable]
public enum PlayerState
{
    None,
    Freeze,
    Fishing,
    CastingRod,
    RetractingRod,
    FishingBoost,
    FishingPull,
    MenuOpened,
    CardOpened,
    InspectingFish,
}