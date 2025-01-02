using System;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public static ButtonManager Instance;

    public Button Create, List, Join, Apply, CopyID, RefreshPlayerList;
    [Space]
    public Toggle isLobbyPrivateToggle;
    [Header("For Create")]
    public GameObject InputField, GameModeArea;

    private bool isCreate;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ChecMyLoby();
        Create.onClick.AddListener(SetCreate);
        Apply.onClick.AddListener(SetApply);
        Join.onClick.AddListener(SetJoin);
        List.onClick.AddListener(SetList);
        CopyID.onClick.AddListener(CopyLobyID);
        RefreshPlayerList.onClick.AddListener(_RefreshPlayerList);
        isLobbyPrivateToggle.onValueChanged.AddListener(ChangeLobbySetting);
    }

    private void _RefreshPlayerList()
    {
        LobyManager.Instance.UpdatePlayerLists();
    }

    private void SetCreate()
    {
        InputField.SetActive(true);
        isCreate = true;
    }

    private void SetList()
    {
        LobyManager.Instance.ListLobbies();
    }

    private void SetJoin()
    {
        InputField.SetActive(true);
        isCreate = false;
    }

    private void SetApply()
    {
        if (isCreate)
        {
            //create Lobby
            LobyManager.Instance.CreateLobby();
            InputField.GetComponent<TMPro.TMP_InputField>().text = "";
            CopyID.gameObject.SetActive(true);
            isLobbyPrivateToggle.gameObject.SetActive(true);
            GameModeArea.SetActive(true);
        }
        else
        {
            //Join Lobby
            LobyManager.Instance.JoinLobby();
            InputField.GetComponent<TMPro.TMP_InputField>().text = "";
            CopyID.gameObject.SetActive(false);
            isLobbyPrivateToggle.gameObject.SetActive(false);
            GameModeArea.SetActive(false);
        }
        InputField.SetActive(false);
        PlayerListManager.instance.SetActiveMe(true);
    }

    private void ChecMyLoby()
    {
        if (LobyManager.Instance.CreatedLobby != null)
            return;
        else
        {
            CopyID.gameObject.SetActive(false);
            isLobbyPrivateToggle.gameObject.SetActive(false);
        }
    }


    public void UpdateButton()
    {
        if (LobyManager.Instance.IsLobbyHost())
        {
            CopyID.gameObject.SetActive(true);
            isLobbyPrivateToggle.gameObject.SetActive(true);
            GameModeArea.SetActive(true);
        }
        else
        {
            CopyID.gameObject.SetActive(false);
            isLobbyPrivateToggle.gameObject.SetActive(false);
            GameModeArea.SetActive(false);
        }
        PlayerListManager.instance.SetActiveMe(true);
        PlayerListManager.instance.CreateChild(LobyManager.Instance.PlayerCountInLobby());
    }

    private void CopyLobyID()
    {
        GUIUtility.systemCopyBuffer = LobyManager.Instance.CreatedLobby.LobbyCode;
    }
    private void ChangeLobbySetting(bool mode)
    {
        LobyManager.Instance.UpdateLobbyPrivateSetting(mode);
        SetList();
    }
}
