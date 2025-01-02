using UnityEngine;

public class GameModeButon : MonoBehaviour
{
    public string myMode;

    UnityEngine.UI.Button myButton;
    public TMPro.TextMeshProUGUI _text;

    GameModeManager manager;

    private void Start()
    {
        manager = GetComponentInParent<GameModeManager>();
        myButton = GetComponent<UnityEngine.UI.Button>();
        _text = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        myButton.onClick.AddListener(ButtonClickEvent);
    }

    private void ButtonClickEvent()
    {
        manager.ChangeMode(myMode);
    }

    public void UpdateChild()
    {
        _text.text = myMode;
    }
}
