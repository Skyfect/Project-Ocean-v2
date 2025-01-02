using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using QFSW.QC;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;

public class LobyManager : MonoBehaviour
{
    public static LobyManager Instance;

    public List<GameObject> lobbyList;
    public GameObject LobbyPrefab;
    public Transform ParentPrefab;
    public TMP_InputField LobbyInputField;

    public Lobby CreatedLobby;
    public Lobby JoinedLobby;

    private float heartBeatTimer;
    private float lobbyUpdateTimer;

    private const string PLAYER_NÝCK_KEY = "PlayerName";
    private const string MAP_KEY = "Map";
    private const string GAME_MODE_KEY = "GameMode";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private async void Start()
    {

        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

    }

    private void Update()
    {
        HeartBeatLobby();
        UpdateLobby();
    }


    private async void UpdateLobby()
    {
        if (JoinedLobby == null) return;

        lobbyUpdateTimer -= Time.deltaTime;
        if(lobbyUpdateTimer <= 0)
        {
            lobbyUpdateTimer = 1.1f;
            JoinedLobby = await LobbyService.Instance.GetLobbyAsync(JoinedLobby.Id);
            UpdatePlayer();
        }
    }

    private async void HeartBeatLobby()
    {
        if (CreatedLobby != null)
        {
            heartBeatTimer -= Time.deltaTime;
            if (heartBeatTimer <= 0)
            {
                heartBeatTimer = 15;
                await LobbyService.Instance.SendHeartbeatPingAsync(CreatedLobby.Id);
            }
        }
    }

    private void UpdatePlayer()
    {
        if (JoinedLobby == null) return;

        foreach (Player player in JoinedLobby.Players)
        {
            if (IsMe(player.Id))
            {
                if (PlayerListManager.instance.ReturnChildCount() < PlayerCountInLobby() || PlayerListManager.instance.isAnyChildisNull())
                {
                    Debug.Log($"PLAYER LÝST UPDATED");
                    UpdatePlayerLists();
                }

                ButtonManager.Instance.UpdateButton();
            }
        }
    }

    [Command]
    public void UpdatePlayerLists()
    {
        if (JoinedLobby == null) return;

        string[] nicks = new string[JoinedLobby.Players.Count];
        string[] ids = new string[JoinedLobby.Players.Count];

        Debug.Log($"Nick Count,{nicks.Length}, ID's Count,{ids.Length}");

        GivePlayerID(ids, JoinedLobby);
        SetNicksArray(nicks, JoinedLobby);

        PlayerListManager.instance.GetID_Nick(ids, nicks);
        PlayerSpawnManager.instance.SetActivePlayer(JoinedLobby.Players.Count, nicks);

        ButtonManager.Instance.UpdateButton();
    }

    private void SetNicksArray(string[] nicks, Lobby lobby)
    {
        for (int i = 0; i < nicks.Length; i++)
        {
            nicks[i] = lobby.Players[i].Data[PLAYER_NÝCK_KEY].Value;
        }
    }

    public void GivePlayerID(string[] ids, Lobby lobby)
    {
        for (int i = 0; i < ids.Length; i++)
        {
            ids[i] = lobby.Players[i].Id;
        }
    }

    [Command]
    public async void CreateLobby()
    {
        try
        {
            if (CreatedLobby == null)
            {
                string lobbyName = string.IsNullOrEmpty(LobbyInputField.text) ? "noName" : LobbyInputField.text;
                int maxPlayers = 3;

                CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
                {
                    Player = GetPlayer(),
                    Data = new Dictionary<string, DataObject>
                    {
                        { GAME_MODE_KEY, new DataObject(DataObject.VisibilityOptions.Public, "DeathMatch") },
                        { MAP_KEY, new DataObject(DataObject.VisibilityOptions.Public, "De_Dust2") }
                    }
                };

                CreatedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);
                JoinedLobby = CreatedLobby;
                Debug.Log($"Created Lobby: {CreatedLobby.Name}, Max Players: {CreatedLobby.MaxPlayers}");
                UpdatePlayer();
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    [Command]
    public async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Count = 10,
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                },
                Order = new List<QueryOrder>
                {
                    new QueryOrder(false, QueryOrder.FieldOptions.Created)
                }
            };

            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);
            Debug.Log($"Total Lobbies: {queryResponse.Results.Count}");

            foreach (GameObject lobby in lobbyList)
            {
                Destroy(lobby);
            }
            lobbyList.Clear();

            foreach (Lobby lobby in queryResponse.Results)
            {
                CreateLobbyScreen(lobby);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    private void CreateLobbyScreen(Lobby lobby)
    {
        GameObject lobbyUI = Instantiate(LobbyPrefab, ParentPrefab);
        lobbyUI.GetComponent<LobyScript>().Call(lobby.Name, lobby.MaxPlayers.ToString(), lobby.Players.Count.ToString(), lobby.LobbyCode);
        lobbyList.Add(lobbyUI);
        ButtonManager.Instance.UpdateButton();
    }

    public async void JoinLobby()
    {
        try
        {
            JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions { Player = GetPlayer() };
            JoinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(LobbyInputField.text, options);
            PrintPlayers(JoinedLobby);
            ButtonManager.Instance.UpdateButton();
            UpdatePlayer();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    [Command]
    public async void LeaveLobby()
    {
        try
        {
            if (JoinedLobby == null) return;

            await LobbyService.Instance.RemovePlayerAsync(JoinedLobby.Id, AuthenticationService.Instance.PlayerId);
            CreatedLobby = null;
            JoinedLobby = null;
            ListLobbies();
            PlayerListManager.instance.SetActiveMe(false);
            ButtonManager.Instance.UpdateButton();
            PlayerSpawnManager.instance.CloseSetActiveAll();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public async void KickPlayer(string playerID)
    {
        try
        {
            if (JoinedLobby == null) return;

            await LobbyService.Instance.RemovePlayerAsync(JoinedLobby.Id, playerID);
            if (IsMe(playerID))
            {
                CreatedLobby = null;
                JoinedLobby = null;
                ListLobbies();

                PlayerListManager.instance.SetActiveMe(false);
                ButtonManager.Instance.UpdateButton();
                PlayerSpawnManager.instance.CloseSetActiveAll();
            }
            UpdatePlayer();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public void UpdateLobbyHost()
    {
        try
        {
            if (JoinedLobby == null) return;

        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    [Command]
    public void PrintPlayerWithCommand()
    {
        try
        {
            if (JoinedLobby == null) return;

            Lobby lobby = JoinedLobby;
            Debug.Log($"Players in lobby: {lobby.Name}, GameMode: {lobby.Data[GAME_MODE_KEY].Value}, Map: {lobby.Data[MAP_KEY].Value}, Private: {lobby.IsPrivate}");
            foreach (var player in lobby.Players)
            {
                Debug.Log($"Player ID: {player.Id}, Player NickName: {player.Data[PLAYER_NÝCK_KEY].Value}");
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    private void PrintPlayers(Lobby lobby)
    {
        try
        {
            Debug.Log($"Players in lobby: {lobby.Name}, GameMode: {lobby.Data[GAME_MODE_KEY].Value}, Map: {lobby.Data[MAP_KEY].Value}, Private: {lobby.IsPrivate}");
            foreach (var player in lobby.Players)
            {
                Debug.Log($"Player ID: {player.Id}, Player NickName: {player.Data[PLAYER_NÝCK_KEY].Value}");
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }



    private Player GetPlayer()
    {
        Debug.Log("YeniPlayer eklendi");
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, MySQLManager.instance.PlayerNick) },
            }
        };
    }

    public async void UpdateLobbyPrivateSetting(bool isPrivate)
    {
        if (CreatedLobby != null)
        {
            UpdateLobbyOptions options = new UpdateLobbyOptions { IsPrivate = isPrivate };
            CreatedLobby = await LobbyService.Instance.UpdateLobbyAsync(CreatedLobby.Id, options);
            JoinedLobby = CreatedLobby;
        }
    }

    [Command]
    public async void UpdateGameMode(string mode)
    {
        try
        {
            UpdateLobbyOptions options = new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { "GameMode", new DataObject(DataObject.VisibilityOptions.Public, mode) }
                }
            };
            CreatedLobby = await LobbyService.Instance.UpdateLobbyAsync(CreatedLobby.Id, options);
            JoinedLobby = CreatedLobby;
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public bool IsLobbyHost() => CreatedLobby != null && AuthenticationService.Instance.PlayerId == CreatedLobby.HostId;
    public bool IsMe(string playerID) => AuthenticationService.Instance.PlayerId == playerID;

    public int PlayerCountInLobby()
    {
        if (JoinedLobby != null) 
        {
            return JoinedLobby.Players.Count;
        }
        else
        {
            return 0;
        }
    }
}
