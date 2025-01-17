using UnityEngine;
using Unity.Netcode;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab; 
    [SerializeField] private Transform[] spawnPoints;

    private Network_UI _networkUI;

    private void Start()
    {
        
        _networkUI = FindFirstObjectByType<Network_UI>();
        //StartHost();
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        SpawnPlayer(NetworkManager.Singleton.LocalClientId);
        _networkUI.PanelDisabled();
        Debug.Log("Host baþlatýldý.");
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        SpawnPlayer(NetworkManager.Singleton.LocalClientId);
        _networkUI.PanelDisabled();
        Debug.Log("Ýstemci baðlanýyor...");

    }

    public void StartServer()
    {
        NetworkManager.Singleton.StartServer();
        SpawnPlayer(NetworkManager.Singleton.LocalClientId);
        _networkUI.PanelDisabled();
    }

    private void SpawnPlayer(ulong clientId)
    {
        int random = Random.Range(0, spawnPoints.Length);
        if (NetworkManager.Singleton.IsServer)
        {
            GameObject playerInstance = Instantiate(playerPrefab, spawnPoints[random].position, Quaternion.identity);
            playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }
    }
}
