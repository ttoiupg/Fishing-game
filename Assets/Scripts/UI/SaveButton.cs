using UnityEngine;

public class SaveButton : MonoBehaviour
{
    private bool pressed = false;
    public void OnClick()
    {
        if (pressed) return;
        pressed = true;
        Debug.Log("Request save and return to menu");
        DataPersistenceManager.Instance.OnGameLeave();
    }
}