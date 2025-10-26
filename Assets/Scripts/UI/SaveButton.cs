using UnityEngine;

public class SaveButton : MonoBehaviour
{
    public void OnClick()
    {
        DataPersistenceManager.Instance.OnGameLeave();
    }
}