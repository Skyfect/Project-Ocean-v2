using Unity.Netcode;
using UnityEngine;

public class Network_Manager : MonoBehaviour
{
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        Debug.Log("Host baþlatýldý.");
    }
    public void StartClient()
    {
        NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes("CustomConnectionData");
        NetworkManager.Singleton.StartClient();
        Debug.Log("Ýstemci baðlanýyor...");
    }
}
