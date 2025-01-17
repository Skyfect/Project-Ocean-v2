using UnityEngine;
using Unity.Netcode;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Core; // To start Unity Services
using Unity.Services.Authentication;
using UnityEngine.UI;
public class Relay_Manager : MonoBehaviour
{
    [SerializeField] private Text joinCodeText;
    [SerializeField] private GameObject otherButtonsPanel;

    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in:" + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    //public async void CreateRelay()
    //{
    //    try
    //    {
    //        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);

    //        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

    //        Debug.Log("joinCode:" + joinCode);

    //        NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
    //            allocation.RelayServer.IpV4,
    //            (ushort)allocation.RelayServer.Port,
    //            allocation.AllocationIdBytes,
    //            allocation.Key,
    //            allocation.ConnectionData
    //        );

    //        if (NetworkManager.Singleton.IsConnectedClient || NetworkManager.Singleton.IsHost)
    //        {
    //            NetworkManager.Singleton.Shutdown();
    //        }

    //        NetworkManager.Singleton.StartHost();

    //        joinCodeText.text = joinCode.ToString();
    //    }
    //    catch (RelayServiceException e)
    //    {
    //        Debug.Log(e);
    //    }

    //    otherButtonsPanel.SetActive(false);
    //}

    //public async void JoinRelay(string joinCode)
    //{
    //    try
    //    {
    //        Debug.Log("Joining Relay With" + joinCode);

    //        JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

    //        NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
    //            joinAllocation.RelayServer.IpV4,
    //            (ushort)joinAllocation.RelayServer.Port,
    //            joinAllocation.AllocationIdBytes,
    //            joinAllocation.Key,
    //            joinAllocation.ConnectionData,
    //            joinAllocation.HostConnectionData
    //        );

    //        if (NetworkManager.Singleton.IsConnectedClient || NetworkManager.Singleton.IsHost)
    //        {
    //            NetworkManager.Singleton.Shutdown();
    //        }

    //        NetworkManager.Singleton.StartClient();

    //        Debug.Log("Successfully joined the game!");

    //        joinCodeText.text = joinCode.ToString();
    //    }
    //    catch (RelayServiceException e)
    //    {
    //        Debug.LogError("Failed to join the game. Reason: " + e.Message);
    //    }

    //    otherButtonsPanel.SetActive(false);
    //}
}
