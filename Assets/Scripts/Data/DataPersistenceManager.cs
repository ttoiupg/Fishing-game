using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

[System.Serializable]
public class Saves
{
    public string directory;
    public string Name;
    public Sprite screenShot;

    public Saves(string directory, string name)
    {
        this.directory = directory;
        this.Name = name;
    }
}
public class DataPersistenceManager : MonoBehaviour
{
    [FormerlySerializedAs("fileName")] [SerializeField] private string playerDataFileName;
    public List<Saves> saves = new List<Saves>();
    [SerializeField] private string fishingRodDataFileName;
    [SerializeField] private string itemDataFileName;
    [SerializeField] private string globalSettingsFileName;
    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler<PlayerData> playerDataHandler;
    private FileDataHandler<FishingRodData> fishingRodDataHandler;
    private FileDataHandler<ItemData> itemDataHandler;
    private FileDataHandler<GlobalSettings> globalSettingsHandler;
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
    public bool ingame;
    public int currentSave;
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
    }

    private void Start()
    {
        string[] folders = Directory.GetDirectories(Application.persistentDataPath);
        foreach (var dirc in folders)
        {
            print(dirc);
            if (File.Exists(dirc + "/Tide.FishingRodData") &&
                File.Exists(dirc + "/Tide.PlayerData") &&
                File.Exists(dirc + "/Tide.ItemData"))
            {
                var filename = Path.GetFileName(dirc);
                var dic = Application.persistentDataPath + "/" + filename;
                saves.Add(new Saves(dic,filename));
            }
        }
        this.playerDataHandler = new FileDataHandler<PlayerData>(Application.persistentDataPath, playerDataFileName);
        this.fishingRodDataHandler = new FileDataHandler<FishingRodData>(Application.persistentDataPath, fishingRodDataFileName);
        this.itemDataHandler = new FileDataHandler<ItemData>(Application.persistentDataPath, itemDataFileName);
        
        this.globalSettingsHandler =
            new FileDataHandler<GlobalSettings>(Application.persistentDataPath, globalSettingsFileName);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        SceneManager.sceneLoaded += backToMenu;
    }

    private void backToMenu(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name == "Menu")
        {
            LoadGlobalSettings(); 
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= backToMenu;
    }

    public void NewFishingRodData()
    {
        FishingRod rod = new FishingRod(gameFishingRods["rod_starter"], 0, 100,
            System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        IDataFishingRod starterRod = new IDataFishingRod(rod);
        this.gameData.fishingRodData.fishingRods.Add(starterRod);
    }

    public async UniTask CreateNewSave(string Direc)
    {
        SaveGlobalSettings();
        if (Directory.Exists(Direc))
        {
            Directory.Delete(Direc, true);
        }
        DirectoryInfo info = Directory.CreateDirectory(Direc);
        this.gameData = new GameData();
        this.gameData.globalSettings = globalSettingsHandler.Load() ?? new GlobalSettings();
        this.gameData.playerData = playerDataHandler.Load(Direc) ?? new PlayerData();
        this.gameData.itemData = itemDataHandler.Load(Direc) ?? new ItemData();
        this.gameData.fishingRodData = fishingRodDataHandler.Load(Direc);
        if (this.gameData.fishingRodData == null)
        {
            Debug.Log("there's no fishing rod data");
            this.gameData.fishingRodData = new FishingRodData();
            NewFishingRodData();
        }

        var direct = Path.Combine(Application.persistentDataPath, Direc);
        var filename = Path.GetFileName(direct);
        saves.Add(new Saves(direct,filename));
        currentSave = saves.Count-1;
        PlayerInputSystem.Instance.load();
        ingame = true;
        await GameObject.FindAnyObjectByType<TransitionHandler>().LoadScene("BaseScene");
        await UniTask.Delay(500);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
        SaveGame(currentSave,true,false);
    }
    public void LoadGlobalSettings()
    {
        this.gameData = new GameData();
        this.gameData.globalSettings = globalSettingsHandler.Load() ?? new GlobalSettings();
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }
    public async UniTask LoadGame(int saveIndex)
    {
        SaveGlobalSettings();
        var save = saves[saveIndex];
        currentSave = saveIndex;
        string directory = Application.persistentDataPath + "/" + save.directory;
        this.gameData = new GameData();
        this.gameData.globalSettings = globalSettingsHandler.Load() ?? new GlobalSettings();
        this.gameData.playerData = playerDataHandler.Load(save.Name) ?? new PlayerData();
        this.gameData.itemData = itemDataHandler.Load(save.Name) ?? new ItemData();
        this.gameData.fishingRodData = fishingRodDataHandler.Load(save.Name);
        if (this.gameData.fishingRodData == null)
        {
            Debug.Log("there's no fishing rod data");
            this.gameData.fishingRodData = new FishingRodData();
            NewFishingRodData();
        }
        PlayerInputSystem.Instance.load();
        ingame = true;
        await GameObject.FindAnyObjectByType<TransitionHandler>().LoadScene("BaseScene");
        await UniTask.Delay(500);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
        SaveGame(saveIndex,true,false);
    }

    public void SaveGlobalSettings()
    {
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }

        string settings = globalSettingsHandler.Save(gameData.globalSettings);
        Debug.Log(settings);
    }
    public void SaveGame(int saveIndex, bool screenshot,bool immidiate)
    {
        var save = saves[saveIndex];
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }

        string playerSave = playerDataHandler.Save(save.directory,gameData.playerData);
        string fishingRodSave = fishingRodDataHandler.Save(save.directory,gameData.fishingRodData);
        string itemSave = itemDataHandler.Save(save.directory,gameData.itemData);
        var pngPath = save.directory +"/SaveThumbnail.png";
        if (screenshot)
        {
            if (File.Exists(pngPath))
            {
                File.Delete(pngPath);
            }
            if (immidiate)
            {
                ScreenCapture.CaptureScreenshot(pngPath);
            }
            else
            {
                ScreenShot(1500, pngPath);
            }
        }
        Debug.Log(playerSave);
        Debug.Log(fishingRodSave);
        Debug.Log(itemSave);
    }

    private async UniTask ScreenShot(int time,string pngPath)
    {
        await UniTask.Delay(time);
        ScreenCapture.CaptureScreenshot(pngPath);
    }
    
    public void OnGameLeave()
    {
        ViewManager.instance.CloseView();
        SaveGlobalSettings();
        SaveGame(currentSave,true,true);
        leave();
    }

    public async UniTask leave()
    {
        PlayerInputSystem.Instance.unload();
        await UniTask.Delay(400);
        ingame = false;
        await GameObject.FindAnyObjectByType<TransitionHandler>().LoadScene("Menu");
    }
    private void OnApplicationQuit()
    {
        if (ingame)
        {
            SaveGame(currentSave,false,true);
        }
        SaveGlobalSettings();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistences = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistences);
    }
}