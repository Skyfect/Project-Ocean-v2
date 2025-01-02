using System.Collections.Generic;
using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    public List<string> gameModes;
    public List<GameModeButon> buttons;

    private void Start()
    {
       for (int i = 0; i < gameModes.Count; i++)
        {
            buttons[i].myMode = gameModes[i];
            buttons[i].UpdateChild();
        }
    }

    public void ChangeMode(string mode)
    {
        LobyManager.Instance.UpdateGameMode(mode);
    }
}
