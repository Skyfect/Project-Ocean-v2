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
        string deviceId = SystemInfo.deviceUniqueIdentifier;
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
            ShowLobbyPanel();
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
        NetworkManager.instance.CreateSocket();
        FindMatch();
        //StartMatchmaking();
    }

    public async void FindMatch()
    {
        Debug.Log("Finding match");
        lobbyStatusText.text = "Eþleþme aranýyor...";

        var matchmakingTicket = await socket.AddMatchmakerAsync("*", 2, 2);
        ticket = matchmakingTicket.Ticket;

    }
    /*
    private async void StartMatchmaking()
    {
        try
        {
            lobbyStatusText.text = "Eþleþme aranýyor...";
            var matchmakingTicket = await socket.AddMatchmakerAsync("*", 2, 2);
            ticket = matchmakingTicket.Ticket;

            socket.ReceivedMatchmakerMatched += async (IMatchmakerMatched matched) =>
            {
              
                match = await socket.JoinMatchAsync(matched);
                Debug.Log($"Eþleþme bulundu! Match ID: {match.Id}");
                lobbyStatusText.text = "Eþleþme bulundu, oyun baþlýyor...";

                foreach (var user in match.Presences)
                {
                    Debug.Log($"Player Session ID: {user.SessionId}");
                }
           

                lobbyStatusText.text = "Eþleþme bulundu, oyun baþlýyor...";
                await Task.Delay(2000);
                lobbyPanel.SetActive(false);
                // GoToNextScene();
            };
        }
        catch (Exception e)
        {
            Debug.LogError($"Eþleþme sýrasýnda hata: {e.Message}");
            lobbyStatusText.text = "Eþleþme baþarýsýz.";
        }
   
    }*/

    private void GoToNextScene()
    {
        SceneManager.LoadScene("Level01");
    }
}
