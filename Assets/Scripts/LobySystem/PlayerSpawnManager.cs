using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    public static PlayerSpawnManager instance;

    public GameObject[] setPlatyers;
    public TMPro.TextMeshProUGUI[] nickField;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public void SetActivePlayer(int Count, string[] nicks)
    {
        for (int i = 0; i < setPlatyers.Length; i++)
        {
            setPlatyers[i].SetActive(false);
        }
        for (int i = 0; i < Count; i++)
        {
            setPlatyers[i].SetActive(true);
            nickField[i].text = nicks[i];
        }
    }

    public void CloseSetActiveAll()
    {
        for (int i = 0; i < setPlatyers.Length; i++)
        {
            setPlatyers[i].SetActive(false);
        }
    }
}
