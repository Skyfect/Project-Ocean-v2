using Nakama;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkConnection : MonoBehaviour
{
    public Button facebookLoginButton;
    public Button googleLoginButton;
    public Button deviceIdLoginButton;
    public Button emailRegisterButton;
    public Button emailLoginButton;
    public TMP_InputField emailInputField;
    public TMP_InputField passwordInputField;
    public GameObject loginPanel;
    public GameObject lobbyPanel;
    public TMP_Text lobbyStatusText;
    // Temel deðiþkenler
    private IClient client;        // Nakama sunucusuyla iletiþim için ana istemci
    private ISocket socket;        // Gerçek zamanlý iletiþim için WebSocket baðlantýsý
    private ISession session;      // Oyuncunun oturum bilgileri
    private IMatch currentMatch;   // Mevcut eþleþme bilgileri
    private bool isConnected;      // Sunucuya baðlantý durumu

    // Oyuncu yönetimi için veri yapýlarý
    private Dictionary<string, GameObject> players;  // Tüm oyuncularý tutan sözlük
    private Vector3[] spawnPositions;               // Oyuncularýn baþlangýç pozisyonlarý
    private Queue<SpawnInfo> spawnQueue;            // Spawn iþlemlerini ana thread'e taþýmak için kuyruk
    private Queue<PositionUpdate> positionQueue;    // Pozisyon güncellemelerini ana thread'e taþýmak için kuyruk

    [SerializeField] private GameObject playerPrefab;

    // Eþleþtirme sabitleri
    private const string MATCHMAKING_POOL = "2players_pool";
    private const int MIN_PLAYERS = 2;
    private const int MAX_PLAYERS = 2;
    private const int OP_CODE_POSITION = 1;  // Pozisyon güncelleme mesajlarý için kod
    private struct PositionUpdate
    {
        public string userId;
        public Vector3 position;

        public PositionUpdate(string id, Vector3 pos)
        {
            userId = id;
            position = pos;
        }
    }
    private struct SpawnInfo
    {
        public string userId;
        public Vector3 position;
        public bool isLocalPlayer;

        public SpawnInfo(string id, Vector3 pos, bool isLocal)
        {
            userId = id;
            position = pos;
            isLocalPlayer = isLocal;
        }
    }

    void Start()
    {
        // Define spawn positions for Player 1 and Player 2
        spawnPositions[0] = new Vector3(-2, 0, 0);  // Player 1 spawn position
        spawnPositions[1] = new Vector3(2, 0, 0);   // Player 2 spawn position
        _ = InitializeNakama();
    }

    private async Task InitializeNakama()
    {
        try
        {
            // Initialize the Nakama client
            client = new Client("http", "localhost", 7350, "defaultkey");

            facebookLoginButton.onClick.AddListener(() => LoginWithFacebook());
            googleLoginButton.onClick.AddListener(() => LoginWithGoogle());
            deviceIdLoginButton.onClick.AddListener(() => LoginWithDeviceId());
            emailRegisterButton.onClick.AddListener(() => RegisterWithEmail());
            emailLoginButton.onClick.AddListener(() => LoginWithEmail());

            

            // Add socket event listeners
            socket.ReceivedMatchPresence += OnReceivedMatchPresence;
            socket.ReceivedMatchState += OnReceivedMatchState;
            socket.ReceivedMatchmakerMatched += OnReceivedMatchmakerMatched;

            isConnected = true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error connecting to Nakama: {e.Message}");
            isConnected = false;
        }
    }

    public async void StartMatchmaking()
    {
        if (!isConnected)
        {
            Debug.LogError("Cannot start matchmaking - not connected to server");
            return;
        }

        try
        {
            Debug.Log("Starting matchmaking...");

            // Register for matchmaker matched callback
            socket.ReceivedMatchmakerMatched += OnReceivedMatchmakerMatched;

            // Add the current user to the matchmaking pool
            var matchmakingTicket = await socket.AddMatchmakerAsync(
                query: "*",
                minCount: MIN_PLAYERS,
                maxCount: MAX_PLAYERS,
                stringProperties: new Dictionary<string, string>(),
                numericProperties: new Dictionary<string, double>()
            );

            Debug.Log($"Matchmaking ticket: {matchmakingTicket.Ticket}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error starting matchmaking: {e.Message}");
        }
    }

    private async void LoginWithFacebook()
    {
        string facebookToken = "KullanýcýnýnFacebookToken";
        await AuthenticateWithProvider("facebook", facebookToken);
    }

    private async void LoginWithGoogle()
    {
        string googleToken = "KullanýcýnýnGoogleToken";
        await AuthenticateWithProvider("google", googleToken);
    }

    private async void LoginWithDeviceId()
    {
        // Add random suffix to device ID to support multiple instances on same machine
        var deviceId = SystemInfo.deviceUniqueIdentifier + UnityEngine.Random.Range(0, 10000).ToString();
        session = await client.AuthenticateDeviceAsync(deviceId);

        Debug.Log($"Device ID ile giriþ baþarýlý. User ID: {session.UserId}");
        ShowLobbyPanel();
    }

    private async void RegisterWithEmail()
    {
        string email = emailInputField.text.Trim();
        string password = passwordInputField.text.Trim();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Debug.LogWarning("E-posta ve þifre boþ olamaz!");
            return;
        }

        try
        {
            session = await client.AuthenticateEmailAsync(email, password);
            Debug.Log($"Email ile kayýt baþarýlý. User ID: {session.UserId}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Email ile kayýt baþarýsýz: {e.Message}");
        }
    }

    private async void LoginWithEmail()
    {
        string email = emailInputField.text.Trim();
        string password = passwordInputField.text.Trim();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Debug.LogWarning("E-posta ve þifre boþ olamaz!");
            return;
        }

        try
        {
            session = await client.AuthenticateEmailAsync(email, password);

            Debug.Log($"Email ile giriþ baþarýlý. User ID: {session.UserId}");
            ShowLobbyPanel();
        }
        catch (Exception e)
        {
            Debug.LogError($"Email ile giriþ baþarýsýz: {e.Message}");
        }
    }

    private async Task AuthenticateWithProvider(string provider, string token)
    {
        try
        {
            session = await client.AuthenticateCustomAsync(token);


            Debug.Log($"{provider} ile giriþ baþarýlý. User ID: {session.UserId}");
            ShowLobbyPanel();
        }
        catch (Exception e)
        {
            Debug.LogError($"Giriþ iþlemi baþarýsýz: {e.Message}");
        }
    }

    private async void ShowLobbyPanel()
    {
        loginPanel.SetActive(false);
        lobbyPanel.SetActive(true);

        socket = client.NewSocket();
        await socket.ConnectAsync(session);
        Debug.Log("Socket baðlantýsý kuruldu.");
    }

    private async void OnReceivedMatchmakerMatched(IMatchmakerMatched matched)
    {
        Debug.Log("Received matchmaker matched event!");
        try
        {
            // Join the match
            currentMatch = await socket.JoinMatchAsync(matched);
            Debug.Log($"Joined match: {currentMatch.Id}");
            Debug.Log($"Number of players in match: {currentMatch.Presences.Count()}");
            Debug.Log($"My user ID: {session.UserId}");
            foreach (var presence in currentMatch.Presences)
            {
                Debug.Log($"Player in match: {presence.UserId}");
            }

            // Determine if we're player 1 or 2 based on join order
            int playerIndex = 0;
            foreach (var presence in currentMatch.Presences)
            {
                if (presence.UserId == session.UserId)
                {
                    break;
                }
                playerIndex++;
            }

            Debug.Log($"I am player {playerIndex + 1}");
            // Queue local player spawn
            spawnQueue.Enqueue(new SpawnInfo(session.UserId, spawnPositions[playerIndex], true));

            // Queue other players spawn
            foreach (var presence in currentMatch.Presences)
            {
                if (presence.UserId != session.UserId && !players.ContainsKey(presence.UserId))
                {
                    int otherIndex = (playerIndex == 0) ? 1 : 0;
                    Debug.Log($"Queueing spawn for other player at index {otherIndex}");
                    spawnQueue.Enqueue(new SpawnInfo(presence.UserId, spawnPositions[otherIndex], false));
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error joining match: {e.Message}");
        }
    }

    private void OnReceivedMatchPresence(IMatchPresenceEvent presenceEvent)
    {
        Debug.Log($"Received match presence event. Joins: {presenceEvent.Joins.Count()}, Leaves: {presenceEvent.Leaves.Count()}");
        foreach (var presence in presenceEvent.Joins)
        {
            Debug.Log($"Player joined: {presence.Username}");
            if (presence.UserId != session.UserId)
            {
                int newPlayerIndex = players.Count % 2;
                Debug.Log($"Spawning joining player at index {newPlayerIndex}");
                spawnQueue.Enqueue(new SpawnInfo(presence.UserId, spawnPositions[newPlayerIndex], false));
            }
        }

        foreach (var presence in presenceEvent.Leaves)
        {
            Debug.Log($"Player left: {presence.Username}");
            if (players.ContainsKey(presence.UserId))
            {
                Destroy(players[presence.UserId]);
                players.Remove(presence.UserId);
            }
        }
    }

    private void OnReceivedMatchState(IMatchState matchState)
    {
        if (matchState.OpCode == OP_CODE_POSITION)
        {
            try
            {
                var positionData = System.Text.Encoding.UTF8.GetString(matchState.State);
                var position = JsonUtility.FromJson<Vector3>(positionData);

                Debug.Log("player position:" +position);

                // Queue the position update to be processed in the main thread
                positionQueue.Enqueue(new PositionUpdate(matchState.UserPresence.UserId, position));
            }
            catch (Exception e)
            {
                Debug.LogError($"Error processing position update: {e.Message}");
            }
        }
    }

    private void SpawnPlayerInternal(string userId, Vector3 position, bool isLocalPlayer)
    {
        if (!players.ContainsKey(userId))
        {
            if (playerPrefab != null)
            {
                var playerObj = Instantiate(playerPrefab, position, Quaternion.identity);
                var player = playerObj.GetComponent<CubePlayer>();

                if (player != null)
                {
                    player.Initialize(userId, isLocalPlayer, this);
                    players[userId] = playerObj;

                    if (isLocalPlayer)
                    {
                        // Immediately send initial position to other players
                        SendPositionUpdate(position);
                    }
                }
            }
            else
            {
                Debug.LogError("Player prefab not set!");
            }
        }
    }

    public async void SendPositionUpdate(Vector3 position)
    {
        if (currentMatch != null)
        {
            try
            {
                string positionJson = JsonUtility.ToJson(position);
                byte[] positionBytes = System.Text.Encoding.UTF8.GetBytes(positionJson);

                await socket.SendMatchStateAsync(currentMatch.Id, OP_CODE_POSITION, positionBytes);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error sending position update: {e.Message}");
            }
        }
    }

    private void OnDestroy()
    {
        // Clean up socket connection when the script is destroyed
        if (socket != null && socket.IsConnected)
        {
            socket.ReceivedMatchmakerMatched -= OnReceivedMatchmakerMatched;
            socket.CloseAsync();
        }

        foreach (var player in players.Values)
        {
            if (player != null)
            {
                Destroy(player);
            }
        }
        players.Clear();
    }

    private void UpdatePlayerPosition(string userId, Vector3 position)
    {
        if (players.ContainsKey(userId))
        {
            var player = players[userId].GetComponent<CubePlayer>();
            if (player != null)
            {
                player.UpdatePosition(position);
            }
        }
    }

}
