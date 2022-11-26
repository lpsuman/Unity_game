using System;
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
        [SerializeField] private bool autoHost = false;
        [SerializeField] private Button confirmNameButton = null;
        [SerializeField] private Button hostButton = null;

        private void Start()
        {
            if (autoHost && Application.isEditor)
            {
                confirmNameButton.onClick.Invoke();
                hostButton.onClick.Invoke();
                return;
            }
            nameInputPanel.SetActive(true);
            landingPanel.SetActive(false);
            ipAddressInputPanel.SetActive(false);
        }
        public void HostLobby()
        {
            //Debug.Log("Starting host");
            networkManager.StartHost();
            landingPanel.SetActive(false);
        }
    }
}
