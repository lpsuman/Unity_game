using Bluaniman.SpaceGame.General;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Bluaniman.SpaceGame.Lobby
{
    public class JoinLobbyMenu : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private MainMenu mainMenu = null;
        [SerializeField] private TMP_InputField ipAddressInputField = null;
        [SerializeField] private Button joinButton = null;
        [SerializeField] private Button cancelButton = null;

        private void OnEnable()
        {
            Globals.networkManager.OnClientConnected += HandleClientConnected;
            Globals.networkManager.OnClientDisconnected += HandleClientDisconnected;
        }

        private void OnDisable()
        {
            Globals.networkManager.OnClientConnected -= HandleClientConnected;
            Globals.networkManager.OnClientDisconnected -= HandleClientDisconnected;
        }

        private void OnDestroy()
        {
            Globals.networkManager.OnClientConnected -= HandleClientConnected;
            Globals.networkManager.OnClientDisconnected -= HandleClientDisconnected;
        }

        private void HandleClientConnected()
        {
            SetButtonsInteractable(true);
            gameObject.SetActive(false);
            mainMenu.HideAllPanels();
        }

        private void HandleClientDisconnected()
        {
            SetButtonsInteractable(true);
        }

        private void SetButtonsInteractable(bool isInteractable)
        {
            joinButton.interactable = isInteractable;
            cancelButton.interactable = isInteractable;
        }

        public void JoinLobby()
        {
            string ipAddress = ipAddressInputField.text;
            //Debug.Log($"Trying to join with address: {ipAddress}");
            SetButtonsInteractable(false);
            Globals.networkManager.networkAddress = ipAddress;
            Globals.networkManager.StartClient();
        }
    }
}
