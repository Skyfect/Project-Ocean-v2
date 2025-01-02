using UnityEngine;
using UnityEngine.UI;

public class RegisterButtonController : MonoBehaviour
{
    public GameObject RegisterScreen, LoginScreen, MainScreen;
    public Button RegisterButton, LoginButton, BackButton;

    private void Start()
    {
        LoginButton.onClick.AddListener(Login);
        RegisterButton.onClick.AddListener(Register);
        BackButton.onClick.AddListener(Back);
    }

    public void Back()
    {
        RegisterScreen.SetActive(false);
        LoginScreen.SetActive(false);
        MainScreen.SetActive(true);
    }

    private void Register()
    {
        RegisterScreen.SetActive(true);
        LoginScreen.SetActive(false);
        MainScreen.SetActive(false);
    }

    private void Login()
    {
        RegisterScreen.SetActive(false);
        LoginScreen.SetActive(true);
        MainScreen.SetActive(false);
    }
}
