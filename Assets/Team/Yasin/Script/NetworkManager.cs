using Nakama;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;
    public GameObject NetworkLocalPlayerPrefab;
    public GameObject NetworkRemotePlayerPrefab;

    public TextMeshProUGUI matchIdText;
    public TextMeshProUGUI userText;

    private IDictionary<string, GameObject> players;
    private IUserPresence localUser;
    private GameObject localPlayer;
    private IMatch currentMatch;
    private string matchId;

    public Transform spawnPoint, spawnPoint2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        instance = this;
    }
    private async void Start()
    {
        players = new Dictionary<string, GameObject>();

        await NetworkConnection.instance.Connect();      
    }

    public void CreateSocket()
    {
        NetworkConnection.instance.socket.ReceivedMatchmakerMatched += OnReceivedMatchmakerMatched;
        NetworkConnection.instance.socket.ReceivedMatchPresence += OnReceivedMatchPresence;
    }

    private async void OnReceivedMatchmakerMatched(IMatchmakerMatched matched)
    {
        localUser = matched.Self.Presence;

        var match = await NetworkConnection.instance.socket.JoinMatchAsync(matched);
        matchId = match.Id;
        Debug.Log($"Eþleþme bulundu! Match ID: {match.Id}");
        NetworkConnection.instance.lobbyStatusText.text = "Eþleþme bulundu, oyun baþlýyor...";

        Debug.Log("Our Session Id: " + match.Self.SessionId);
        NetworkConnection.instance.lobbyPanel.SetActive(false);
        matchIdText.text = match.Id;
        foreach (var user in match.Presences)
        {
            Debug.Log("Connected User Session Id: " + user.SessionId);
            userText.text = user.SessionId;
        }

        foreach (var user in match.Presences)
        {
            SpawnPlayer(match.Id, user);
        }

        currentMatch = match;
    }

    private void OnReceivedMatchPresence(IMatchPresenceEvent matchPresenceEvent)
    {
        foreach (var user in matchPresenceEvent.Joins)
        {
            SpawnPlayer(matchPresenceEvent.MatchId, user);
        }

        foreach (var user in matchPresenceEvent.Leaves)
        {
            if (players.ContainsKey(user.SessionId))
            {
                Destroy(players[user.SessionId]);
                players.Remove(user.SessionId);
            }
        }
    }

    private void SpawnPlayer(string matchId, IUserPresence user, int spawnIndex = -1)
    {
        var isLocal = user.SessionId == localUser.SessionId;

        var playerPrefab = isLocal ? NetworkLocalPlayerPrefab : NetworkRemotePlayerPrefab;

        if(playerPrefab == isLocal)
        {
            var player = Instantiate(playerPrefab, spawnPoint.transform.position, Quaternion.identity);
        }
        else
        {
            var player = Instantiate(playerPrefab, spawnPoint2.transform.position, Quaternion.identity);
        }

       

    }

    // Update is called once per frame
    void Update()
    {

    }
}
