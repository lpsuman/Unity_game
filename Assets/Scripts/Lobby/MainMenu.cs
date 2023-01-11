using Bluaniman.SpaceGame.Debugging;
using Bluaniman.SpaceGame.General;
using Bluaniman.SpaceGame.Networking;
using UnityEngine;
using UnityEngine.UI;

namespace Bluaniman.SpaceGame.Lobby
{
    public class MainMenu : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private GameObject nameInputPanel = null;
        [SerializeField] private GameObject landingPanel = null;
        [SerializeField] private GameObject ipAddressInputPanel = null;

        [Header("Debug")]
        [SerializeField] private Button confirmNameButton = null;
        [SerializeField] private Button hostButton = null;
        [SerializeField] private Button joinButton = null;
        [SerializeField] private Button confirmIpButton = null;
        [SerializeField] private Button exitGameButton = null;

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
            Globals.networkManager.StartHost();
            HideAllPanels();
        }

        public void ShowNameInputPanel()
        {
            nameInputPanel.SetActive(true);
            SetLandingPageVisible(false);
            ipAddressInputPanel.SetActive(false);
        }

        public void ShowLandingPage()
        {
            nameInputPanel.SetActive(false);
            SetLandingPageVisible(true);
            ipAddressInputPanel.SetActive(false);
        }

        public void ShowIpAddressInputPanel()
        {
            nameInputPanel.SetActive(false);
            SetLandingPageVisible(false);
            ipAddressInputPanel.SetActive(true);
        }

        public void HideAllPanels()
        {
            nameInputPanel.SetActive(false);
            SetLandingPageVisible(false);
            ipAddressInputPanel.SetActive(false);
        }

        private void SetLandingPageVisible(bool isVisible)
        {
            landingPanel.SetActive(isVisible);
            exitGameButton.gameObject.SetActive(isVisible);
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}
