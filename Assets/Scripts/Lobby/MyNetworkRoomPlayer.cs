using System;
using UnityEngine;
using Mirror;
using Bluaniman.SpaceGame.Networking;
using TMPro;
using UnityEngine.UI;

namespace Bluaniman.SpaceGame.Lobby
{
    public class MyNetworkRoomPlayer : NetworkBehaviour
    {
        [Header("UI")]
        [SerializeField] private GameObject lobbyUI = null;
        [SerializeField] private TMP_Text[] playerNameTexts = null;
        [SerializeField] private TMP_Text[] playerReadyTexts = null;
        [SerializeField] private Button startGameButton = null;

        [SyncVar(hook = nameof(HandleDisplayNameChanged))]
        public string DisplayName = "Loading...";
        [SyncVar(hook = nameof(HandleReadyStatusChanged))]
        public bool IsReady = false;

        private bool isLeader;

        public bool IsLeader
        {
            set
            {
                isLeader = value;
                startGameButton.gameObject.SetActive(value);
            }
        }

        private MyNetworkManager room;

        private MyNetworkManager Room
        {
            get
            {
                if (room != null) { return room; }
                return room = NetworkManager.singleton as MyNetworkManager;
            }
        }

        public override void OnStartAuthority()
        {
            CmdSetDisplayName(PlayerNameInput.DisplayName);
            lobbyUI.SetActive(true);
        }

        public override void OnStartClient()
        {
            if (!hasAuthority)
            {
                lobbyUI.SetActive(false);
            }
            Room.RoomPlayers.Add(this);
            UpdateDisplay();
        }

        public override void OnStopClient()
        {
            Room.RoomPlayers.Remove(this);
            UpdateDisplay();
        }

        private void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();
        private void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();

        private void UpdateDisplay()
        {
            if (!hasAuthority)
            {
                foreach (MyNetworkRoomPlayer player in Room.RoomPlayers)
                {
                    if (player.hasAuthority)
                    {
                        player.UpdateDisplay();
                        break;
                    }
                }
                return;
            }

            for (int i = 0; i < playerNameTexts.Length; i++)
            {
                if (i < Room.RoomPlayers.Count)
                {
                    playerNameTexts[i].text = Room.RoomPlayers[i].DisplayName;
                    playerReadyTexts[i].text = Room.RoomPlayers[i].IsReady ?
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
            if (!isLeader) { return; }
            startGameButton.interactable = readyToStart;
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
            Room.NotifyPlayersOfReadyState();
        }

        [Command]
        public void CmdStartGame()
        {
            if (Room.RoomPlayers[0].connectionToClient != connectionToClient) { return; }

            // Start game
        }
    }
}
