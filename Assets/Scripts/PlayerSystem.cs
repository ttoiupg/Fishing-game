using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerSystem : MonoBehaviour
{
    protected Player player;
    protected DefaultInputActions playerInput;

    public virtual void Awake()
    {
        player = transform.root.GetComponent<Player>();
        playerInput = new DefaultInputActions();
    }
}