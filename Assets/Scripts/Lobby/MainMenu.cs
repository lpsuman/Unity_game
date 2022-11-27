using System;
using Bluaniman.SpaceGame.Debugging;
using Bluaniman.SpaceGame.Networking;
using UnityEngine;
using UnityEngine.UI;

namespace Bluaniman.SpaceGame.Lobby
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private MyNetworkManager networkManager = null;

        [Header("UI")]
        [SerializeField] private GameObject nameInputPanel = null;
        [SerializeField] private GameObject landingPanel = null;
        [SerializeField] private GameObject ipAddressInputPanel = null;

        [Header("Debug")]
        [SerializeField] private Button confirmNameButton = null;
        [SerializeField] private Button hostButton = null;

        private void Start()
        {
            nameInputPanel.SetActive(true);
            landingPanel.SetActive(false);
            ipAddressInputPanel.SetActive(false);
            if (DebugHandler.ShouldDebug(DebugHandler.autoHost))
            {
                confirmNameButton.onClick.Invoke();
                hostButton.onClick.Invoke();
                return;
            }
        }
        public void HostLobby()
        {
            networkManager.StartHost();
            landingPanel.SetActive(false);
        }
    }
}
