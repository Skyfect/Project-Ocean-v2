using UnityEngine;

using Nakama;
using System.Collections.Generic;
using System.Collections.Concurrent;

using UnityEngine.UI;

public class MatchManager : MonoBehaviour
{
    public Button findMatchButton;

    private string scheme = "http";
    private string host = "localhost";
    private int port = 7350;
    private string serverKey = "defaultkey";

    private IClient client;
    public ISession session;
    public ISocket socket;

    private string ticket;
    public string matchId;
    private IMatch currentMatch;

    private IDictionary<string, GameObject> players;
    private string localPlayerId;
    public GameObject playerPrefab;

    public static MatchManager instance;

    // Thread-safe kuyruk için
    private ConcurrentQueue<PlayerSpawnInfo> spawnQueue = new ConcurrentQueue<PlayerSpawnInfo>();

    // Spawn bilgilerini tutmak için yardımcı sınıf
    private class PlayerSpawnInfo
    {
        public string UserId;
        public Vector3 Position;
        public bool IsLocalPlayer;

        public PlayerSpawnInfo(string userId, Vector3 position, bool isLocalPlayer)
        {
            UserId = userId;
            Position = position;
            IsLocalPlayer = isLocalPlayer;
        }
    }

    public void Awake()
    {
        instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        players = new Dictionary<string, GameObject>(); // Sözlüğü başlatıyoruz.

        client = new Nakama.Client(scheme, host, port, serverKey, UnityWebRequestAdapter.Instance);
        var deviceId = System.Guid.NewGuid().ToString(); // Benzersiz bir kimlik oluştur
        session = await client.AuthenticateDeviceAsync(deviceId);
        socket = client.NewSocket();
        await socket.ConnectAsync(session, true);

        // Eşleşme ile ilgili tüm olaylar bu fonksiyonda yapılacak.
        socket.ReceivedMatchmakerMatched += OnReceivedMatchmakerMatched;
        socket.ReceivedMatchPresence += OnReceivedMatchPresence;
        socket.ReceivedMatchState += OnReceivedMatchState;

        findMatchButton.onClick.AddListener(FindMatch);

        Debug.Log(session);
        Debug.Log(socket);
    }

    public async void FindMatch()
    {
        Debug.Log("Finding Match...");

        var matchmakingTicket = await socket.AddMatchmakerAsync("*", 2, 2);
        ticket = matchmakingTicket.Ticket;
        findMatchButton.gameObject.SetActive(false);
    }
    private async void OnReceivedMatchmakerMatched(IMatchmakerMatched matchmakerMatched)
    {
        currentMatch = await socket.JoinMatchAsync(matchmakerMatched);
        matchId = currentMatch.Id;
        localPlayerId = currentMatch.Self.UserId;

        Debug.Log("MatchID : " + matchId);
        Debug.Log("Local Player ID: " + localPlayerId);
        
        // Tüm oyuncuları spawn et
        foreach (var presence in currentMatch.Presences)
        {
            // Pozisyonu ve local/remote durumunu belirle
            bool isLocalPlayer = presence.UserId == localPlayerId;
            Vector3 position = isLocalPlayer ? Vector3.zero : new Vector3(2f, 0f, 0f);
            Debug.Log($"Spawning player {presence.UserId}, isLocal: {isLocalPlayer}");
            spawnQueue.Enqueue(new PlayerSpawnInfo(presence.UserId, position, isLocalPlayer));
        }
    }

    private void OnReceivedMatchPresence(IMatchPresenceEvent presenceEvent)
    {
        foreach (var presence in presenceEvent.Joins)
        {
            if (!players.ContainsKey(presence.UserId))
            {
                bool isLocalPlayer = presence.UserId == localPlayerId;
                Vector3 position = isLocalPlayer ? Vector3.zero : new Vector3(2f, 0f, 0f);
                Debug.Log($"New player joined {presence.UserId}, isLocal: {isLocalPlayer}");
                spawnQueue.Enqueue(new PlayerSpawnInfo(presence.UserId, position, isLocalPlayer));
            }
        }

        foreach (var presence in presenceEvent.Leaves)
        {
            if (players.ContainsKey(presence.UserId))
            {
                // Destroy işlemini de ana thread'e taşıyalım
                var playerToRemove = players[presence.UserId];
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    if (playerToRemove != null)
                    {
                        Destroy(playerToRemove);
                        players.Remove(presence.UserId);
                    }
                });
            }
        }
    }

    private void OnReceivedMatchState(IMatchState matchState)
    {
        try
        {
            if (matchState.UserPresence.UserId != localPlayerId)
            {
                var state = System.Text.Encoding.UTF8.GetString(matchState.State);
                
                if (matchState.OpCode == 1) // Position update
                {
                    var position = JsonUtility.FromJson<PlayerPosition>(state);
                    if (players.TryGetValue(matchState.UserPresence.UserId, out GameObject player))
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        {
                            player.transform.position = new Vector3(position.x, position.y, position.z);
                        });
                    }
                }
                else if (matchState.OpCode == 2) // Rotation update
                {
                    var rotation = JsonUtility.FromJson<PlayerRotation>(state);
                    if (players.TryGetValue(matchState.UserPresence.UserId, out GameObject player))
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        {
                            player.transform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
                        });
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error processing match state: {e.Message}");
        }
    }

    private void Update()
    {
        // Kuyruktaki spawn işlemlerini ana thread'de gerçekleştir
        while (spawnQueue.TryDequeue(out PlayerSpawnInfo spawnInfo))
        {
            SpawnPlayerInternal(spawnInfo.UserId, spawnInfo.Position, spawnInfo.IsLocalPlayer);
        }
    }

    // Ana thread'de çalışacak asıl spawn metodu
    private void SpawnPlayerInternal(string userId, Vector3 position, bool isLocalPlayer)
    {
        if (!players.ContainsKey(userId))
        {
            GameObject playerObj = Instantiate(playerPrefab, position, Quaternion.identity);
            playerObj.name = isLocalPlayer ? "LocalPlayer" : "RemotePlayer";
            players[userId] = playerObj;
            
            // Kamera referansını al
            Camera playerCamera = playerObj.GetComponentInChildren<Camera>();
            
            if (isLocalPlayer)
            {
                Debug.Log($"Adding PlayerController to {userId}");
                playerObj.AddComponent<PlayerController>();
                playerObj.GetComponent<Renderer>().material.color = Color.blue;
                
                // Local player'ın kamerasını aktif et
                if (playerCamera != null)
                {
                    playerCamera.gameObject.SetActive(true);
                }
            }
            else
            {
                Debug.Log($"Remote player spawned: {userId}");
                playerObj.GetComponent<Renderer>().material.color = Color.red;
                
                // Remote player'ın kamerasını deaktif et
                if (playerCamera != null)
                {
                    playerCamera.gameObject.SetActive(false);
                }
            }
        }
    }

    private void OnDestroy()
    {
        socket?.CloseAsync();
    }

    // Pozisyon verisi için yardımcı sınıf
    [System.Serializable]
    private class PlayerPosition
    {
        public float x;
        public float y;
        public float z;

        public PlayerPosition(Vector3 position)
        {
            x = position.x;
            y = position.y;
            z = position.z;
        }
    }

    // Rotasyon verisi için yardımcı sınıf
    [System.Serializable]
    private class PlayerRotation
    {
        public float x;
        public float y;
        public float z;

        public PlayerRotation(Vector3 rotation)
        {
            x = rotation.x;
            y = rotation.y;
            z = rotation.z;
        }
    }

    // Rotasyon güncellemesi gönderme metodu
    public async void SendRotationUpdate(Vector3 rotation)
    {
        if (currentMatch != null)
        {
            var rotationData = new PlayerRotation(rotation);
            var state = JsonUtility.ToJson(rotationData);
            await socket.SendMatchStateAsync(currentMatch.Id, 2, state); // OpCode 2 for rotation
        }
    }

    // Pozisyon güncellemesi gönderme metodu
    public async void SendPositionUpdate(Vector3 position)
    {
        if (currentMatch != null)
        {
            var positionData = new PlayerPosition(position);
            var state = JsonUtility.ToJson(positionData);
            await socket.SendMatchStateAsync(currentMatch.Id, 1, state);
        }
    }
} 