using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrapper : MonoBehaviour
{
    public Animator intro;
    void Start()
    {
        DontDestroyOnLoad(gameObject);          // if this object holds managers
        LoadScene();
    }

    private async UniTask LoadScene()
    {
        await UniTask.Delay(3000);
        await SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Single);
        intro.SetTrigger("Start");
        await UniTask.Delay(100);
        DataPersistenceManager.Instance.LoadGlobalSettings();
    }
}