// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsteroidsGameManager.cs" company="Exit Games GmbH">
//   Modified for demonstration of a simple FPS instantiation
// </copyright>
// <summary>
//  Game Manager for a simplified Photon FPS setup
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using Photon.Pun;

namespace Photon.Pun.Demo.Asteroids
{
    public class AsteroidsGameManager : MonoBehaviourPunCallbacks
    {
        public static AsteroidsGameManager Instance = null;

        // Name of the prefab in the Resources folder. E.g. "Skeleton"
        [Tooltip("Name of the Player Prefab in Resources folder.")]
        public string playerPrefabName = "Skeleton";

        // Base spawn position
        private Vector3 spawnPosition = new Vector3(200f, 5.2f, 150f);

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                // If this is the second player in the room, offset z by +5
                // (First player sees PlayerCount == 1. Second sees == 2, etc.)
                if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
                {
                    spawnPosition.z += 5f;
                }

                // You can randomize rotation or keep it fixed
                Quaternion randomRotation = Quaternion.Euler(
                    0,
                    Random.Range(0f, 360f),
                    0f
                );

                // Instantiate local player
                GameObject newPlayer = PhotonNetwork.Instantiate(
                    playerPrefabName,
                    spawnPosition,
                    randomRotation,
                    0
                );

                // Force activation to avoid any 'inactive prefab' issues
                if (!newPlayer.activeSelf)
                {
                    newPlayer.SetActive(true);
                }
            }
        }

        #region PUN CALLBACKS

        public override void OnDisconnected(DisconnectCause cause)
        {
            // Return to lobby or a fallback scene on disconnect
            SceneManager.LoadScene("DemoAsteroids-LobbyScene");
        }

        public override void OnLeftRoom()
        {
            // Disconnect from Photon on leaving a room
            PhotonNetwork.Disconnect();
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            Debug.Log("Master Client switched to: " + newMasterClient.NickName);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log("Player left room: " + otherPlayer.NickName);
        }

        #endregion
    }
}
