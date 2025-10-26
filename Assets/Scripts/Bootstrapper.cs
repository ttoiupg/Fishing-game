using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrapper : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(gameObject);          // if this object holds managers
        LoadScene();
    }

    private async UniTask LoadScene()
    { 
        await SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Single);
        await UniTask.Delay(100);
        DataPersistenceManager.Instance.LoadGlobalSettings();
    }
}