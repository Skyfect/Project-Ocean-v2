using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        Application.quitting += QuitApp;
    }

    private void OnApplicationQuit()
    {
        // Uygulama kapandýðýnda oyuncuyu çevrimdýþý yap
        StartCoroutine(MySQLManager.instance.Logout(MySQLManager.instance.PlayerNick));
    }

    private void QuitApp()
    {
        if(MySQLManager.instance.PlayerNick != null || MySQLManager.instance.PlayerNick != "")
        {
            StartCoroutine(MySQLManager.instance.Logout(MySQLManager.instance.PlayerNick));
        }
    }
}
