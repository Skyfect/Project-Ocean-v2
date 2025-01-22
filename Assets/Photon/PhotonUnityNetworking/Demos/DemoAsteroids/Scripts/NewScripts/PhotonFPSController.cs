using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(CharacterController))]
public class PhotonFPSController : MonoBehaviourPun, IPunObservable
{
    [Header("Player Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float gravity = 9.81f;

    [Header("References")]
    [SerializeField] private Camera playerCamera;

    private CharacterController characterController;
    private float verticalVelocity;
    private float xRotation = 0f;

    // Variables for position and rotation sync
    private Vector3 networkPosition;
    private Quaternion networkRotation;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        if (!photonView.IsMine)
        {
            // Disable local control components for remote players
            if (playerCamera != null)
            {
                playerCamera.gameObject.SetActive(false);
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
        if (photonView.IsMine)
        {
            HandleLook();
            HandleMovement();
        }
        else
        {
            // Smoothly interpolate position and rotation for remote players
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 10f);
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * 10f);
        }
    }

    private void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        if (playerCamera != null)
        {
            playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }

        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        if (characterController.isGrounded)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        move.y = verticalVelocity;

        characterController.Move(move * moveSpeed * Time.deltaTime);
    }

    // Photon PUN's serialization method for syncing data
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) // Sending data
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else // Receiving data
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
