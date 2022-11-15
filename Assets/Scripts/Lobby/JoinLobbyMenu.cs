using System;
using Bluaniman.SpaceGame.Networking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Bluaniman.SpaceGame.Lobby
{
    public class JoinLobbyMenu : MonoBehaviour
    {
        [SerializeField] private MyNetworkManager networkManager = null;

        [Header("UI")]
        [SerializeField] private GameObject landingPanel = null;
        [SerializeField] private TMP_InputField ipAddressInputField = null;
        [SerializeField] private Button joinButton = null;
        [SerializeField] private Button cancelButton = null;

        private void OnEnable()
        {
            MyNetworkManager.OnClientConnected += HandleClientConnected;
            MyNetworkManager.OnClientDisconnected += HandleClientDisconnected;
        }

        private void OnDisable()
        {
            MyNetworkManager.OnClientConnected -= HandleClientConnected;
            MyNetworkManager.OnClientDisconnected -= HandleClientDisconnected;
        }

        private void HandleClientConnected()
        {
            SetButtonsInteractable(true);
            gameObject.SetActive(false);
            landingPanel.SetActive(false);
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
            Debug.Log($"Trying to join with address: {ipAddress}");
            SetButtonsInteractable(false);
            networkManager.networkAddress = ipAddress;
            networkManager.StartClient();
        }
    }
}
