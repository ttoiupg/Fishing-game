using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour,IDataPersistence
{
    public PlayerID ID;
    public HUDController HUDController;
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
    public List<DiscoveredFish> discoveredFish;
    private List<IDataDiscoverFish> dataDiscoverFish;
    public PlayerState currentState;

    [Header("Fishing")]
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
    private void Start()
    {
        HUDController.UpdateLevelProgress();
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
public enum PlayerState
{
    None,
    Freeze,
    Fishing,
    CastingRod,
    FishingBoost,
    FishingPull,
    MenuOpened,
    CardOpened,
    InspectingFish,
}