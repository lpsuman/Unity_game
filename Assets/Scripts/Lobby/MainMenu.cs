using System;
using Bluaniman.SpaceGame.Networking;
using UnityEngine;

namespace Bluaniman.SpaceGame.Lobby
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private MyNetworkManager networkManager = null;

        [Header("UI")]
        [SerializeField] private GameObject nameInputPanel = null;
        [SerializeField] private GameObject landingPanel = null;
        [SerializeField] private GameObject ipAddressInputPanel = null;

        private void Start()
        {
            nameInputPanel.SetActive(true);
            landingPanel.SetActive(false);
            ipAddressInputPanel.SetActive(false);
        }
        public void HostLobby()
        {
            networkManager.StartHost();
            landingPanel.SetActive(false);
        }
    }
}
