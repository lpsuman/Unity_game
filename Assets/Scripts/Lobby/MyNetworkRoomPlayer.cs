using System;
using UnityEngine;
using Mirror;
using Bluaniman.SpaceGame.Networking;
using TMPro;
using UnityEngine.UI;
using Bluaniman.SpaceGame.Debugging;
using Bluaniman.SpaceGame.Network;

namespace Bluaniman.SpaceGame.Lobby
{
    public class MyNetworkRoomPlayer : MyNetworkBehavior
    {
        [Header("UI")]
        [SerializeField] private GameObject lobbyUI = null;
        [SerializeField] private TMP_Text[] playerNameTexts = null;
        [SerializeField] private TMP_Text[] playerReadyTexts = null;
        [SerializeField] private Button startGameButton = null;

        [Header("Debug")]
        [SerializeField] private Button readyButton = null;

        [SyncVar(hook = nameof(HandleDisplayNameChanged))]
        public string DisplayName = "Loading...";
        [SyncVar(hook = nameof(HandleReadyStatusChanged))]
        public bool IsReady = false;

        private bool isLeader;

        public bool IsLeader
        {
            get
            {
                return isLeader;
            }
            set
            {
                isLeader = value;
                startGameButton.interactable = value;
            }
        }

        public override void OnStartAuthority()
        {
            CmdSetDisplayName(PlayerNameInput.DisplayName);
        }

        public override void OnStartClient()
        {
            lobbyUI.SetActive(isOwned);
            startGameButton.interactable = false;
            networkManager.RoomPlayers.Add(this);
            UpdateDisplay();
            if (isOwned && DebugHandler.ShouldAutoLobbyAction(DebugHandler.AutoReady()))
            {
                readyButton.onClick.Invoke();
            }
        }

        public override void OnStopClient()
        {
            networkManager.RoomPlayers.Remove(this);
            if (isOwned)
            {
                FindObjectOfType<MainMenu>().ShowLandingPage();
            }
        }

        private void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();
        private void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();

        private void UpdateDisplay()
        {
            if (!isOwned)
            {
                foreach (MyNetworkRoomPlayer player in networkManager.RoomPlayers)
                {
                    if (player.isOwned)
                    {
                        player.UpdateDisplay();
                        break;
                    }
                }
                return;
            }

            for (int i = 0; i < playerNameTexts.Length; i++)
            {
                if (i < networkManager.RoomPlayers.Count)
                {
                    playerNameTexts[i].text = networkManager.RoomPlayers[i].DisplayName;
                    playerReadyTexts[i].text = networkManager.RoomPlayers[i].IsReady ?
                        "<color=green>Ready</color>" :
                        "<color=red>Not Ready</color>";
                } else
                {
                    playerNameTexts[i].text = "Waiting For Player...";
                    playerNameTexts[i].text = string.Empty;
                }
                
            }
        }

        public void HandleReadyToStart(bool readyToStart)
        {
            if (!IsLeader) { return; }
            startGameButton.interactable = readyToStart;
            if (startGameButton.interactable && 
                   (DebugHandler.ShouldAutoLobbyAction(DebugHandler.AutoStart())
                || (DebugHandler.ShouldAutoLobbyAction(DebugHandler.AutoStartNotAlone()) && networkManager.RoomPlayers.Count > 1)))
            {
                startGameButton.onClick.Invoke();
            }
        }

        [Command]
        private void CmdSetDisplayName(string displayName)
        {
            DisplayName = displayName;
        }

        [Command]
        public void CmdReadyUp()
        {
            IsReady = !IsReady;
            networkManager.NotifyPlayersOfReadyState();
        }

        [Command]
        public void CmdStartGame()
        {
            if (networkManager.RoomPlayers[0].connectionToClient != connectionToClient) { return; }
            networkManager.StartGame();
        }
    }
}
