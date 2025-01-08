using UnityEngine;

public class CubePlayer : MonoBehaviour
{
    private float moveSpeed = 5f;
    private string playerId;
    private NetworkConnection matchManager;
    private bool isLocalPlayer;
    private SpriteRenderer spriteRenderer;
    private float updateInterval = 0.05f; // Send updates every 0.05 seconds
    private float nextUpdateTime = 0f;
    public void Initialize(string id, bool isLocal, NetworkConnection manager)
    {
        playerId = id;
        isLocalPlayer = isLocal;
        matchManager = manager;

        Debug.Log($"Initializing player. ID: {id}, IsLocal: {isLocal}");
        // Set different colors for local and remote players
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = isLocalPlayer ? Color.blue : Color.red;
            Debug.Log($"Set player color to {(isLocalPlayer ? "blue" : "red")}");
        }
    }
    void Update()
    {
        if (!isLocalPlayer) return;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0) * moveSpeed * Time.deltaTime;
        if (movement != Vector3.zero)
        {
            transform.position += movement;
        }

        // Send position updates regularly, even when not moving
        if (matchManager != null && Time.time >= nextUpdateTime)
        {
            matchManager.SendPositionUpdate(transform.position);
            nextUpdateTime = Time.time + updateInterval;
        }
    }
    public void UpdatePosition(Vector3 newPosition)
    {
        if (!isLocalPlayer)
        {
            transform.position = newPosition;
        }
    }
}
