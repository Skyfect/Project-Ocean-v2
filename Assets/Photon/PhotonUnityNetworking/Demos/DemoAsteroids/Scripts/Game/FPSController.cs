// --------------------------------------------------------------------------------------------------------------------
// <copyright>
//   Example of a simple Photon FPS Controller
// </copyright>
// <summary>
//  Basic movement & camera look for an FPS-style character
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviourPun
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float gravity = 9.8f;

    [Header("Look Settings")]
    public float mouseSensitivity = 3f;
    public Transform cameraTransform;

    private CharacterController controller;
    private float verticalVelocity = 0f;
    private float xRotation = 0f;

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        // Lock cursor
        if (photonView.IsMine)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        HandleMovement();
        HandleMouseLook();
    }

    private void HandleMovement()
    {
        // WASD input
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Apply gravity
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f; // small downward force
        }

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
        {
            verticalVelocity = jumpForce;
        }

        verticalVelocity -= gravity * Time.deltaTime;

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        move *= moveSpeed;

        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);
    }

    private void HandleMouseLook()
    {
        // Mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Horizontal rotation (y-axis) on character
        transform.Rotate(Vector3.up * mouseX);

        // Vertical rotation (x-axis) on camera
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -75f, 75f);

        if (cameraTransform)
        {
            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }
}
