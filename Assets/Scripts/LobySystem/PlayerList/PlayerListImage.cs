using UnityEngine;

public class PlayerListImage : MonoBehaviour
{
    private UnityEngine.UI.Button kickButton;
    public TMPro.TextMeshProUGUI nicktext;
    public string myID;

    private void Start()
    {
        kickButton = GetComponentInChildren<UnityEngine.UI.Button>();

        kickButton.interactable = LobyManager.Instance.IsLobbyHost();

        if (LobyManager.Instance.IsMe(myID))
            kickButton.gameObject.SetActive(false);

        kickButton.onClick.AddListener(YouKicked);
    }


    public void YouKicked()
    {
        LobyManager.Instance.KickPlayer(myID);
    }
}
