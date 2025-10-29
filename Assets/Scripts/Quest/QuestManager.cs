using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.TextCore.Text;

public class QuestManager : MonoBehaviour,IDataPersistence
{
    public List<Quest> quests;
    public static async UniTask<QuestObject> LoadCharacterAsync(string guid)
    {
        
        AsyncOperationHandle<QuestObject> handle = Addressables.LoadAssetAsync<QuestObject>(guid);
        QuestObject questObject = await handle.ToUniTask();
        return questObject;
    }
    // private void ConverDataToObject(QuestObject)
    // {
    //     
    // }

    private async UniTask LoadAsync(GameData data)
    {
        var objs = await data.playerData.questList.Select(g => LoadCharacterAsync(g.id));
        var progress = data.playerData.questList.Select(g => g.progress).ToList();
        foreach (var q in objs)
        {
            
        }
    }
    public void LoadData(GameData data)
    {
        LoadAsync(data);
    }

    public void SaveData(ref GameData data)
    {
        
    }
}



[System.Serializable]
public class Quest
{
    public QuestObject questObject;
    public List<int> progress;
    
    public void Initialize()
    {
        foreach (var questItem in questObject.items)
        {
            
        }
    }

    public void UpdateProgress()
    {
        
    }
}