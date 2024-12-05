using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : PlayerSystem
{
    private Vector3 CameraDisplace = new Vector3(0,5.22f,-8.47f);
    private Vector3 CameraRotation = new Vector3(32.02f,0,0);
    public Transform CameraTransform;
    private Transform CharacterTransform;
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    public float playerSpeed = 7.0f;

    [SerializeField]
    private float gravityValue = -9.81f;
    // Start is called before the first frame update
    void Start()
    {
        CharacterTransform = player.transform;
        controller = player.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");
        // DOING CHARACTER MOVEMENT
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }
        Vector3 move = new Vector3(moveHorizontal, 0, moveVertical);
        controller.Move(move * Time.deltaTime * playerSpeed);
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
        CharacterTransform.rotation = Quaternion.LookRotation(new Vector3(0,0,moveHorizontal));
    }
    private void freezePlayer()
    {
        playerSpeed = 0;
    }
    private void unfreezePlayer()
    {
        playerSpeed = 7.0f;
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
