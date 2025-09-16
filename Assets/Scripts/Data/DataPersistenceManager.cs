using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class DataPersistenceManager : MonoBehaviour
{
    [FormerlySerializedAs("fileName")] [SerializeField] private string playerDataFileName;
    [SerializeField] private string fishingRodDataFileName;
    [SerializeField] private string itemDataFileName;
    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler<PlayerData> playerDataHandler;
    private FileDataHandler<FishingRodData> fishingRodDataHandler;
    private FileDataHandler<ItemData> itemDataHandler;
    [SerializeField] private List<BaseFish> gameFishList = new List<BaseFish>();
    [SerializeField] private List<FishingRodSO> gameFishingRodsList = new List<FishingRodSO>();
    [SerializeField] private List<BaseMutation> gameMutationList = new List<BaseMutation>();
    [SerializeField] private List<GameItemSo> gameItemList = new List<GameItemSo>();
    [SerializeField] private List<ModifierBase> ModifierCardList = new List<ModifierBase>();
    public List<CraftElement> craftElements = new List<CraftElement>();
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
        foreach (var modifier in ModifierCardList)
        {
            ModifierCards.Add(modifier.id, modifier);
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        InitializeList();
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        this.playerDataHandler = new FileDataHandler<PlayerData>(Application.persistentDataPath, playerDataFileName);
        this.fishingRodDataHandler = new FileDataHandler<FishingRodData>(Application.persistentDataPath, fishingRodDataFileName);
        this.itemDataHandler = new FileDataHandler<ItemData>(Application.persistentDataPath, itemDataFileName);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }
    public void NewFishingRodData()
    {
        FishingRod rod = new FishingRod(gameFishingRods["rod_starter"], 0, 100,
            System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        IDataFishingRod starterRod = new IDataFishingRod(rod);
        this.gameData.fishingRodData.fishingRods.Add(starterRod);
    }

    public void LoadGame()
    {
        this.gameData = new GameData();
        this.gameData.playerData = playerDataHandler.Load() ?? new PlayerData();
        this.gameData.itemData = itemDataHandler.Load() ?? new ItemData();
        this.gameData.fishingRodData = fishingRodDataHandler.Load();
        if (this.gameData.fishingRodData == null)
        {
            Debug.Log("there's no fishing rod data");
            this.gameData.fishingRodData = new FishingRodData();
            NewFishingRodData();
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

        string playerSave = playerDataHandler.Save(gameData.playerData);
        string fishingRodSave = fishingRodDataHandler.Save(gameData.fishingRodData);
        string itemSave = itemDataHandler.Save(gameData.itemData);
        Debug.Log(playerSave);
        Debug.Log(fishingRodSave);
        Debug.Log(itemSave);
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