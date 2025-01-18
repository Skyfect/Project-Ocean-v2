using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float moveSpeed = 5f;
    private float mouseSensitivity = 2f;
    private float updateRate = 0.1f;
    private float nextUpdateTime = 0f;

    private Camera playerCamera;
    private float verticalRotation = 0f;
    private float horizontalRotation = 0f;

    private void Start()
    {
        // Kamera ayarları
        playerCamera = GetComponentInChildren<Camera>();
        if (playerCamera == null)
        {
            Debug.LogError("Player camera not found!");
            return;
        }

        // Mouse'u kilitle
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Mouse ile rotasyon
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Dikey rotasyonu sınırla (-90 ile 90 derece arası)
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        
        // Yatay rotasyon
        horizontalRotation += mouseX;

        // Kamera rotasyonunu güncelle
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.rotation = Quaternion.Euler(0f, horizontalRotation, 0f);

        // WASD hareketi
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = transform.right * horizontal + transform.forward * vertical;
        movement = movement.normalized * moveSpeed * Time.deltaTime;
        transform.position += movement;

        // Pozisyon ve rotasyon güncellemesi gönder
        if (Time.time >= nextUpdateTime)
        {
            nextUpdateTime = Time.time + updateRate;
            MatchManager.instance.SendPositionUpdate(transform.position);
            MatchManager.instance.SendRotationUpdate(transform.rotation.eulerAngles);
        }
    }

    private void OnDestroy()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
} 