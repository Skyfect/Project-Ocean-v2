using Nakama;
using Unity.Services.Matchmaker.Models;
using UnityEngine;

public class Nakama_Connection : MonoBehaviour
{
    private string _scheme = "http";
    private string _host = "localhost";
    private int _port = 7350;
    private string _serverKey = "defaultkey";

    private IClient _client;
    private ISession _session;
    private ISocket _socket;

    private async void Start()
    {
        _client = new Client(_scheme, _host, _port, _serverKey, UnityWebRequestAdapter.Instance);
        _session = await _client.AuthenticateDeviceAsync(SystemInfo.deviceUniqueIdentifier);
        _socket = _client.NewSocket();
        await _socket.ConnectAsync(_session, true);

        Debug.Log(_session);
        Debug.Log(_socket);
    }
}
