using Nakama;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkConnection : MonoBehaviour
{
    public static IClient client;
    public static ISession session;

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

    public ISocket socket;
    public static IMatch match;

    public string ticket;
    public string matchId;

    public static NetworkConnection instance;

    private void Awake()
    {
        instance = this;
    }

    public async Task Connect() {
        client = new Client("http", "127.0.0.1", 7350, "defaultkey");

        facebookLoginButton.onClick.AddListener(() => LoginWithFacebook());
        googleLoginButton.onClick.AddListener(() => LoginWithGoogle());
        deviceIdLoginButton.onClick.AddListener(() => LoginWithDeviceId());
        emailRegisterButton.onClick.AddListener(() => RegisterWithEmail());
        emailLoginButton.onClick.AddListener(() => LoginWithEmail());
    }
    void Start()
    {
        
    }

    private async void LoginWithFacebook()
    {
        string facebookToken = "Kullan�c�n�nFacebookToken";
        await AuthenticateWithProvider("facebook", facebookToken);
    }

    private async void LoginWithGoogle()
    {
        string googleToken = "Kullan�c�n�nGoogleToken";
        await AuthenticateWithProvider("google", googleToken);
    }

    private async void LoginWithDeviceId()
    {
        string deviceId = SystemInfo.deviceUniqueIdentifier;
        session = await client.AuthenticateDeviceAsync(deviceId);
        Debug.Log($"Device ID ile giri� ba�ar�l�. User ID: {session.UserId}");
        ShowLobbyPanel();
    }

    private async void RegisterWithEmail()
    {
        string email = emailInputField.text.Trim();
        string password = passwordInputField.text.Trim();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Debug.LogWarning("E-posta ve �ifre bo� olamaz!");
            return;
        }

        try
        {
            session = await client.AuthenticateEmailAsync(email, password);
            Debug.Log($"Email ile kay�t ba�ar�l�. User ID: {session.UserId}");
            ShowLobbyPanel();
        }
        catch (Exception e)
        {
            Debug.LogError($"Email ile kay�t ba�ar�s�z: {e.Message}");
        }
    }

    private async void LoginWithEmail()
    {
        string email = emailInputField.text.Trim();
        string password = passwordInputField.text.Trim();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Debug.LogWarning("E-posta ve �ifre bo� olamaz!");
            return;
        }

        try
        {
            session = await client.AuthenticateEmailAsync(email, password);
            Debug.Log($"Email ile giri� ba�ar�l�. User ID: {session.UserId}");
            ShowLobbyPanel();
        }
        catch (Exception e)
        {
            Debug.LogError($"Email ile giri� ba�ar�s�z: {e.Message}");
        }
    }

    private async Task AuthenticateWithProvider(string provider, string token)
    {
        try
        {
            session = await client.AuthenticateCustomAsync(token);
            Debug.Log($"{provider} ile giri� ba�ar�l�. User ID: {session.UserId}");
            ShowLobbyPanel();
        }
        catch (Exception e)
        {
            Debug.LogError($"Giri� i�lemi ba�ar�s�z: {e.Message}");
        }
    }

    private async void ShowLobbyPanel()
    {
        loginPanel.SetActive(false);
        lobbyPanel.SetActive(true);

        socket = client.NewSocket();
        await socket.ConnectAsync(session);
        Debug.Log("Socket ba�lant�s� kuruldu.");
        NetworkManager.instance.CreateSocket();
        FindMatch();
        //StartMatchmaking();
    }

    public async void FindMatch()
    {
        Debug.Log("Finding match");
        lobbyStatusText.text = "E�le�me aran�yor...";

        var matchmakingTicket = await socket.AddMatchmakerAsync("*", 2, 2);
        ticket = matchmakingTicket.Ticket;

    }
    /*
    private async void StartMatchmaking()
    {
        try
        {
            lobbyStatusText.text = "E�le�me aran�yor...";
            var matchmakingTicket = await socket.AddMatchmakerAsync("*", 2, 2);
            ticket = matchmakingTicket.Ticket;

            socket.ReceivedMatchmakerMatched += async (IMatchmakerMatched matched) =>
            {
              
                match = await socket.JoinMatchAsync(matched);
                Debug.Log($"E�le�me bulundu! Match ID: {match.Id}");
                lobbyStatusText.text = "E�le�me bulundu, oyun ba�l�yor...";

                foreach (var user in match.Presences)
                {
                    Debug.Log($"Player Session ID: {user.SessionId}");
                }
           

                lobbyStatusText.text = "E�le�me bulundu, oyun ba�l�yor...";
                await Task.Delay(2000);
                lobbyPanel.SetActive(false);
                // GoToNextScene();
            };
        }
        catch (Exception e)
        {
            Debug.LogError($"E�le�me s�ras�nda hata: {e.Message}");
            lobbyStatusText.text = "E�le�me ba�ar�s�z.";
        }
   
    }*/

    private void GoToNextScene()
    {
        SceneManager.LoadScene("Level01");
    }
}
