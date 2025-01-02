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
                Debug.LogError("Kayýt hatasý: " + www.error);
            }
            else
            {
                string responseText = www.downloadHandler.text;

                if (responseText.Contains("Bu kullanýcý adý zaten alýnmýþ"))
                {
                    Debug.LogError("Hata: Bu kullanýcý adý zaten kullanýlýyor. Lütfen baþka bir kullanýcý adý seçin.");
                }
                else if (responseText.Contains("Kayýt baþarýlý"))
                {
                    Debug.Log("Kayýt baþarýlý!");
                    registerButtonController.Back();
                }
                else
                {
                    Debug.LogError("Kayýt sýrasýnda beklenmeyen bir hata oluþtu: " + responseText);
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
                Debug.LogError("Giriþ hatasý: " + www.error);
            }
            else
            {
                Debug.Log("Giriþ cevabý: " + www.downloadHandler.text);
                if (www.downloadHandler.text == "Geçersiz kullanýcý adý veya þifre" || www.downloadHandler.text == "Hata: Kullanýcý zaten oyunda")
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
                Debug.LogError("Çýkýþ hatasý: " + www.error);
            }
            else
            {
                Debug.Log("Çýkýþ baþarýlý!");
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
            // Hata durumunda renk deðiþimi ve sarsma baþlat
            StartCoroutine(ShakeAndColorChange(nicknameInputField, color));
            StartCoroutine(ShakeAndColorChange(passwordInputField, color));
        }
    }

    private IEnumerator ShakeAndColorChange(TMP_InputField inputField, Color color)
    {
        // Orijinal pozisyon ve renk bilgilerini sakla
        Vector3 originalPosition = inputField.transform.localPosition;
        Color originalColor = inputField.textComponent.color;

        // Renk deðiþimini baþlat
        inputField.textComponent.color = color;

        float elapsedTime = 0f;

        // Belirli bir süre boyunca nesneyi sars
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

        // Pozisyonu ve rengi geri yükle
        inputField.transform.localPosition = originalPosition;
        inputField.textComponent.color = originalColor;
    }
}
