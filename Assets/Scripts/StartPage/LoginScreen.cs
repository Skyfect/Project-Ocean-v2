using UnityEngine;

public class LoginScreen : MonoBehaviour
{
    public UnityEngine.UI.Button Apply;
    public TMPro.TMP_InputField password, nickname;

    private void Start()
    {
        Apply.onClick.AddListener(LoinApply);
    }

    private void LoinApply()
    {
        StartCoroutine(MySQLManager.instance.Login(nickname.text, password.text));
    }
}
