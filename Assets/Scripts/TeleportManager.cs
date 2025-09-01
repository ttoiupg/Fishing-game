using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;

[System.Serializable]
public class sceneData
{
    public string name;
    public bool loaded;
}
public class TeleportManager : MonoBehaviour,IDataPersistence
{
    public UnityEvent<string> TeleportStarted;
    public UnityEvent TeleportEnded;
    public static TeleportManager Instance;
    public List<sceneData> sceneDatas;
    public string currentScene;
    public bool teleporting;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
        InitializeSceneData();
    }

    public void InitializeSceneData()
    {
        foreach (var data in sceneDatas)
        {
            Scene scene = SceneManager.GetSceneByName(data.name);
            data.loaded = (scene.name == data.name);
        }
    }
    public void MovePlayer(Vector3 newPosition)
    {
        var player = GameManager.Instance.player;
        var controller = player.GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;
        player.transform.position = newPosition;
        if (controller != null) controller.enabled = true;
    }
    public void TeleportImmediate([CanBeNull] string sceneName, string portName)
    {
        if (teleporting) return;
        teleporting = true;
        if (sceneName != null)
        {
            var cs = currentScene;
            currentScene = sceneName;
            LoadScene(sceneName);
            Teleport(null,portName);
            UnloadScene(cs);
        }
        else
        {
            var target = GameObject.Find(portName).GetComponent<Teleporter>();
            MovePlayer(target.teleportPoint.position);
        }

        teleporting = false;
    }

    public async UniTask Teleport([CanBeNull] string sceneName, string portName)
    {
        if (teleporting) return;
        teleporting = true;
        if (sceneName != null)
        {
            var player = GameManager.Instance.player;
            TeleportStarted?.Invoke(sceneName);
            await UniTask.WaitForSeconds(1.2f);
            var cs = currentScene;
            currentScene = sceneName;
            LoadScene(sceneName);
            UnloadScene(cs);
            await UniTask.WaitForSeconds(1f);
            Scene scene = SceneManager.GetSceneByName(sceneName);
            SceneManager.SetActiveScene(scene);
            var target = GameObject.Find(portName).GetComponent<Teleporter>();
            player.transform.position = target.teleportPoint.position;
            await UniTask.WaitForSeconds(2.1f);
            TeleportEnded?.Invoke();
        }
        else
        {
            var target = GameObject.Find(portName).GetComponent<Teleporter>();
            MovePlayer(target.teleportPoint.position);
        }
        teleporting = false;
    }
    public void LoadScene(string sceneName)
    {
        var scData = sceneDatas.Find(x => x.name == sceneName);
        if (scData.loaded == false)
        {
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            scData.loaded = true;
        }
    }

    public void UnloadScene(string sceneName)
    {
        var scData = sceneDatas.Find(x => x.name == sceneName);
        if (scData.loaded)
        {
            SceneManager.UnloadSceneAsync(sceneName);
            scData.loaded = false;
        } 
    }

    public void LoadData(GameData data)
    {
        currentScene = data.playerData.Scene;
        LoadScene(currentScene);
    }

    public void SaveData(ref GameData data)
    {
        data.playerData.Scene = currentScene;
    }
}
