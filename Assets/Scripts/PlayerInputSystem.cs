using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputSystem : MonoBehaviour
{
    public static PlayerInputSystem Instance;
    public DefaultInputActions playerInput;
    
    void Awake() {
        var duplicates = FindObjectsOfType<PlayerInputSystem>();
        if (duplicates.Length > 1) { Destroy(gameObject); return; }
        DontDestroyOnLoad(gameObject);
        Instance = this;
        playerInput = new DefaultInputActions();
    }

    public void load()
    {
        playerInput = new DefaultInputActions();
    }

    public void unload()
    {
        playerInput.Dispose();
        playerInput.Disable();
    }
}
