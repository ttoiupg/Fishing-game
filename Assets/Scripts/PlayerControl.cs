using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private Vector3 CameraDisplace = new Vector3(0,5.22f,-8.47f);
    private Vector3 CameraRotation = new Vector3(32.02f,0,0);
    public Transform CameraTransform;
    private Transform CharacterTransform;
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    [SerializeField]
    private float playerSpeed = 7.0f;
    //private float jumpHeight = 1.0f;
    [SerializeField]
    private float gravityValue = -9.81f;
    // Start is called before the first frame update
    void Start()
    {
        CharacterTransform = gameObject.GetComponent<Transform>();
        controller = gameObject.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        // DOING CHARACTER MOVEMENT
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        controller.Move(move * Time.deltaTime * playerSpeed);

        /*if (move != Vector3.zero)
        {
           gameObject.transform.forward = move;
        }*/

        // Makes the player jump
        /*if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
        }*/

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);


        //CAMERA
         CameraTransform.position = CharacterTransform.position + CameraDisplace;
         CameraTransform.localRotation = Quaternion.Euler(CameraRotation);
    }
}
