using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

public class MySQLManager : MonoBehaviour
{
    public static MySQLManager instance;
    public RegisterButtonController registerButtonController;
    public GameObject lobby;
    public string PlayerNick;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public IEnumerator Register(string nickname, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("myNickName", nickname);
        form.AddField("myPassword", password);

        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/UnityGame/register.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Kay�t hatas�: " + www.error);
            }
            else
            {
                string responseText = www.downloadHandler.text;

                if (responseText.Contains("Bu kullan�c� ad� zaten al�nm��"))
                {
                    Debug.LogError("Hata: Bu kullan�c� ad� zaten kullan�l�yor. L�tfen ba�ka bir kullan�c� ad� se�in.");
                }
                else if (responseText.Contains("Kay�t ba�ar�l�"))
                {
                    Debug.Log("Kay�t ba�ar�l�!");
                    registerButtonController.Back();
                }
                else
                {
                    Debug.LogError("Kay�t s�ras�nda beklenmeyen bir hata olu�tu: " + responseText);
                }
            }
        }
    }

    public IEnumerator Login(string nickname, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("myNickName", nickname);
        form.AddField("myPassword", password);

        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/UnityGame/login.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Giri� hatas�: " + www.error);
            }
            else
            {
                Debug.Log("Giri� cevab�: " + www.downloadHandler.text);
                if (www.downloadHandler.text == "Ge�ersiz kullan�c� ad� veya �ifre" || www.downloadHandler.text == "Hata: Kullan�c� zaten oyunda")
                {
                    SetInputFieldColors(Color.red);
                }
                else
                {
                    PlayerNick = nickname;
                    lobby.SetActive(true);
                    gameObject.SetActive(false);
                }
            }
        }
    }

    public IEnumerator Logout(string nickname)
    {
        WWWForm form = new WWWForm();
        form.AddField("myNickName", nickname);

        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/UnityGame/logout.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("��k�� hatas�: " + www.error);
            }
            else
            {
                Debug.Log("��k�� ba�ar�l�!");
            }
        }
    }

    [Header("UI Elements")]
    public TMP_InputField nicknameInputField;
    public TMP_InputField passwordInputField;
    public Color errorColor = Color.red;
    public Color defaultColor = Color.white;
    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 5f;

    private void SetInputFieldColors(Color color)
    {
        if (nicknameInputField != null && passwordInputField != null)
        {
            // Hata durumunda renk de�i�imi ve sarsma ba�lat
            StartCoroutine(ShakeAndColorChange(nicknameInputField, color));
            StartCoroutine(ShakeAndColorChange(passwordInputField, color));
        }
    }

    private IEnumerator ShakeAndColorChange(TMP_InputField inputField, Color color)
    {
        // Orijinal pozisyon ve renk bilgilerini sakla
        Vector3 originalPosition = inputField.transform.localPosition;
        Color originalColor = inputField.textComponent.color;

        // Renk de�i�imini ba�lat
        inputField.textComponent.color = color;

        float elapsedTime = 0f;

        // Belirli bir s�re boyunca nesneyi sars
        while (elapsedTime < shakeDuration)
        {
            float offsetX = Random.Range(-shakeMagnitude, shakeMagnitude) * 0.1f;
            float offsetY = Random.Range(-shakeMagnitude, shakeMagnitude) * 0.1f;

            inputField.transform.localPosition = new Vector3(
                originalPosition.x + offsetX,
                originalPosition.y + offsetY,
                originalPosition.z
            );

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Pozisyonu ve rengi geri y�kle
        inputField.transform.localPosition = originalPosition;
        inputField.textComponent.color = originalColor;
    }
}
