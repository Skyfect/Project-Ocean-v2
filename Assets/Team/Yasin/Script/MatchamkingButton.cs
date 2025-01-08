using UnityEngine;
using UnityEngine.UI;

public class MatchamkingButton : MonoBehaviour
{
    private Button button;
    private NetworkConnection matchmakingManager;
    private Text buttonText;

    private void Start()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<Text>();
        matchmakingManager = FindObjectOfType<NetworkConnection>();

        if (matchmakingManager == null)
        {
            Debug.LogError("MatchmakingManager not found in scene!");
            button.interactable = false;
            if (buttonText != null) buttonText.text = "Error: No MatchmakingManager";
            return;
        }

        button.onClick.AddListener(OnMatchmakingButtonClick);
    }
    private void OnMatchmakingButtonClick()
    {
        matchmakingManager.StartMatchmaking();
        button.interactable = false; // Disable button while matchmaking
        if (buttonText != null) buttonText.text = "Finding Match...";
    }
    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnMatchmakingButtonClick);
        }
    }
}
