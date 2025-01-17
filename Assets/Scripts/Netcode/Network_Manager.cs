using Unity.Netcode;
using UnityEngine;

public class Network_Manager : MonoBehaviour
{
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        Debug.Log("Host ba�lat�ld�.");
    }
    public void StartClient()
    {
        NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes("CustomConnectionData");
        NetworkManager.Singleton.StartClient();
        Debug.Log("�stemci ba�lan�yor...");
    }
}
