using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : PlayerSystem
{
    private Vector3 CameraDisplace = new Vector3(0,5.22f,-8.47f);
    private Vector3 CameraRotation = new Vector3(32.02f,0,0);
    public Transform CameraTransform;
    private Animator animator;
    private Transform CharacterTransform;
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    public float playerSpeed;
    private float lastSpeed;

    [SerializeField]
    private float gravityValue = -9.81f;


    // Start is called before the first frame update
    void Start()
    {
        animator = player.GetComponent<Animator>();
        CharacterTransform = player.transform;
        controller = player.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void freezePlayer()
    {
        lastSpeed = playerSpeed;
        playerSpeed = 0;
    }
    private void unfreezePlayer()
    {
        playerSpeed = lastSpeed;
    }
    private void OnEnable()
    {
        player.ID.playerEvents.OnEnterFishingState += freezePlayer;
        player.ID.playerEvents.OnExitFishingState += unfreezePlayer;
    }
    private void OnDisable()
    {
        player.ID.playerEvents.OnEnterFishingState -= freezePlayer;
        player.ID.playerEvents.OnExitFishingState -= unfreezePlayer;
    }
}
