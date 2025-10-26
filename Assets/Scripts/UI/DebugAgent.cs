using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugAgent : MonoBehaviour
{
    [Header("LootTag")] public Sprite testIcon;
    public DefaultInputActions input;

    private void Start()
    {
        input = PlayerInputSystem.Instance.playerInput;
        input.Debug.Enable();
        input.Debug.SpawnTag.started +=
            (context => LootTagDisplayManager.instance.AddTag(testIcon, "Aqua", 150, 2.2f,"x",""));
    }

    public void Unload()
    {
        input.Debug.SpawnTag.Dispose();
    }
}