using System;
using Bluaniman.SpaceGame.Debugging;
using Bluaniman.SpaceGame.General;
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
            if (DebugHandler.ShouldAutoLobbyAction(DebugHandler.AutoHost()))
            {
                confirmNameButton.onClick.Invoke();
                DebugHandler.CheckAndDebugLog(DebugHandler.MainMenu(), "Autoclicked confirm name button.");
                hostButton.onClick.Invoke();
                DebugHandler.CheckAndDebugLog(DebugHandler.MainMenu(), "Autoclicked host button.");
            }
            else if (DebugHandler.ShouldAutoLobbyAction(DebugHandler.AutoJoin()))
            {
                confirmNameButton.onClick.Invoke();
                DebugHandler.CheckAndDebugLog(DebugHandler.MainMenu(), "Autoclicked confirm name button.");
                joinButton.onClick.Invoke();
                DebugHandler.CheckAndDebugLog(DebugHandler.MainMenu(), "Autoclicked join button.");
                StartCoroutine(CoroutineHelper.CheckDoWait(2f, () => ipAddressInputPanel.activeInHierarchy, () =>
                {
                    confirmIpButton.onClick.Invoke();
                    DebugHandler.CheckAndDebugLog(DebugHandler.MainMenu(), "Autoclicked confirm IP button.");
                }));
            }
        }
        public void HostLobby()
        {
            networkManager.StartHost();
            landingPanel.SetActive(false);
        }
    }
}
