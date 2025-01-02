using UnityEngine;

public class NetworkUIController : MonoBehaviour
{
    public UnityEngine.UI.Button[] buttons;

    private void Start()
    {
        buttons[0].onClick.AddListener(() => {
            Unity.Netcode.NetworkManager.Singleton.StartHost();
        });
        buttons[1].onClick.AddListener(() => {
            Unity.Netcode.NetworkManager.Singleton.StartServer();
        });
        buttons[2].onClick.AddListener(() => {
            Unity.Netcode.NetworkManager.Singleton.StartClient();
        });
    }
}
