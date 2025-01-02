using UnityEngine;

public class RegisterScreen : MonoBehaviour
{
    public UnityEngine.UI.Button Apply;
    public TMPro.TMP_InputField password, nickname;

    private void Start()
    {
        Apply.onClick.AddListener(RegisterApply);
    }

    private void RegisterApply()
    {
        StartCoroutine(MySQLManager.instance.Register(nickname.text, password.text));
    }
}
