using TMPro;
using UnityEngine;

public class LobyScript : MonoBehaviour
{
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI CountText;
    public string MyLobyID;

    public void Call(string myName, string myCount, string CurrentPlayerCount,string myID)
    {
        NameText.text = myName;
        CountText.text = CurrentPlayerCount + "/" + myCount;
        MyLobyID = myID;
    }
}
