using UnityEngine;
using Photon.Pun;
using System;

[RequireComponent(typeof(CharacterController))]
public class PhotonFPSController : MonoBehaviourPunCallbacks
{
    [Header("Player Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float gravity;

    [Header("References")]
    [SerializeField] private Camera playerCamera;

    private CharacterController characterController;
    private float verticalVelocity;
    private float xRotation = 0f;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        // If this is not our local player, disable camera and audio listener
        // (so that we don't hear or see duplicates from other players).
        if (!photonView.IsMine)
        {
            // Destroy or disable camera components to avoid duplications
            if (playerCamera != null)
            {
                Destroy(playerCamera.gameObject);
            }
        }
        else
        {
            // Lock and hide the cursor for a better FPS experience
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        // Only process input and movement if this is the local player's view
        if (!photonView.IsMine) return;

        HandleLook();
        HandleMovement();
    }

    private void HandleLook()
    {
        // Mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Vertical rotation (pitch)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Limit vertical rotation

        // Apply rotation to camera
        if (playerCamera)
        {
            playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }

        // Horizontal rotation (yaw)
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");  // A, D (or left, right)
        float moveZ = Input.GetAxis("Vertical");    // W, S (or up, down)

        // Direction based on local forward/right
        Vector3 move = (transform.right * moveX + transform.forward * moveZ);

        // Apply gravity
        if (characterController.isGrounded)
        {
            verticalVelocity = -2f; // small downward force to keep grounded
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        move.y = verticalVelocity;

        // Move the controller
        characterController.Move(move * moveSpeed * Time.deltaTime);
    }
}
