using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerSystem : MonoBehaviour
{
    protected Player player;
    protected PlayerInputActions playerInput;

    protected virtual void Awake()
    {
        player = transform.root.GetComponent<Player>();
        playerInput = new PlayerInputActions();
    }
}