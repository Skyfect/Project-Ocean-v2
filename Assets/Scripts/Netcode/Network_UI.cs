using UnityEngine;

public class Network_UI : MonoBehaviour
{
    public GameObject panel;
    void Start()
    {
        panel.SetActive(true);
    }

    public void PanelDisabled()
    {
        panel.SetActive(false);
    }
}
