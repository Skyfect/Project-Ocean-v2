using Crest.Examples;
using Unity.Netcode;
using UnityEngine;

public class PlayerSetup : NetworkBehaviour
{
    [SerializeField] GameObject Camera;
    [SerializeField] BoatAlignNormal Controller;

    private void Update()
    {
        if (IsOwner)
            Setup();
    }

    public void Setup()
    {
        Camera.SetActive(true);
        Controller.enabled = true;
    }
}
