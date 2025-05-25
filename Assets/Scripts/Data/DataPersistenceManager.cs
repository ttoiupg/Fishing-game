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
    [SerializeField] private List<BaseMutation> gameMutationList = new List<BaseMutation>();
    [SerializeField] private List<GameItemSo> gameItemList = new List<GameItemSo>();
    [SerializeField] private List<ModifierBase> ModifierCardList = new List<ModifierBase>();
    public Dictionary<string, BaseFish> gameFish = new Dictionary<string, BaseFish>();
    public Dictionary<string, BaseMutation> gameMutations = new Dictionary<string, BaseMutation>();
    public Dictionary<string, FishingRodSO> gameFishingRods = new Dictionary<string, FishingRodSO>();
    public Dictionary<string, GameItemSo> gameItems = new Dictionary<string, GameItemSo>();
    public Dictionary<string, ModifierBase> ModifierCards = new Dictionary<string, ModifierBase>();
    public static DataPersistenceManager Instance { get; private set; }

    private void InitializeList()
    {
        foreach (var fish in gameFishList)
        {
            gameFish.Add(fish.id, fish);
        }

        foreach (var fishingRod in gameFishingRodsList)
        {
            gameFishingRods.Add(fishingRod.id, fishingRod);
        }

        foreach (var mutation in gameMutationList)
        {
            gameMutations.Add(mutation.id, mutation);
        }

        foreach (var item in gameItemList)
        {
            gameItems.Add(item.id, item);
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        InitializeList();
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
        FishingRod rod = new FishingRod(gameFishingRods["rod_starter"], 0, 100,
            System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        IDataFishingRod starterRod = new IDataFishingRod(rod);
        this.gameData.dataFishingRods.Add(starterRod);
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