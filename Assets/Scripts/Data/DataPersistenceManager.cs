using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;
public class DataPersistenceManager : MonoBehaviour
{
    [SerializeField] private string fileName;
    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;
    [SerializeField] private List<BaseFish> gameFishList = new List<BaseFish>();
    [SerializeField] private List<FishingRodSO> gameFishingRodsList = new List<FishingRodSO>();
    public static DataPersistenceManager Instance { get; private set; }
    public Dictionary<string,BaseFish> gameFish = new Dictionary<string, BaseFish>();
    public Dictionary<string, FishingRodSO> gameFishingRods = new Dictionary<string, FishingRodSO>();
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        gameFish.Clear();
        foreach (var fish in gameFishList)
        {
            gameFish.Add(fish.id, fish);
        }
        gameFishingRods.Clear();
        foreach (var fishingRod in gameFishingRodsList)
        {
            gameFishingRods.Add(fishingRod.id, fishingRod);
        }
    }

    private void Start()
    {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }
    public void NewGame()
    {
        this.gameData = new GameData();
        FishingRod starterRod = new FishingRod(gameFishingRods["rod_starter"],0,100, System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        this.gameData.ownedFishingRods.Add(starterRod);
    }
    public void LoadGame()
    {
        this.gameData = dataHandler.Load();
        if (this.gameData == null)
        {
            Debug.Log("there's no game data");
            NewGame();
        }
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }
    public void SaveGame()
    {
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }
        string a = dataHandler.Save(gameData);
        Debug.Log(a);
    }
    private void OnApplicationQuit()
    {
        SaveGame();
    }
    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistences = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistences);
    }
}
