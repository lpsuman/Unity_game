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
        [SerializeField] private Button joinButton = null;
        [SerializeField] private Button confirmIpButton = null;

        private void Start()
        {
            nameInputPanel.SetActive(true);
            landingPanel.SetActive(false);
            ipAddressInputPanel.SetActive(false);
            if (DebugHandler.ShouldAutoLobbyAction(DebugHandler.autoHost))
            {
                confirmNameButton.onClick.Invoke();
                DebugHandler.CheckAndDebugLog(DebugHandler.mainMenu, "Autoclicked confirm name button.");
                hostButton.onClick.Invoke();
                DebugHandler.CheckAndDebugLog(DebugHandler.mainMenu, "Autoclicked host button.");
            } else if (DebugHandler.ShouldAutoLobbyAction(DebugHandler.autoJoin)) {
                confirmNameButton.onClick.Invoke();
                DebugHandler.CheckAndDebugLog(DebugHandler.mainMenu, "Autoclicked confirm name button.");
                joinButton.onClick.Invoke();
                DebugHandler.CheckAndDebugLog(DebugHandler.mainMenu, "Autoclicked join button.");
                confirmIpButton.onClick.Invoke();
                DebugHandler.CheckAndDebugLog(DebugHandler.mainMenu, "Autoclicked confirm IP button.");
            }

        }
        public void HostLobby()
        {
            networkManager.StartHost();
            landingPanel.SetActive(false);
        }
    }
}
