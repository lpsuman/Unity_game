using System;
using System.Collections;
using System.Collections.Generic;
using Bluaniman.SpaceGame.Lobby;
using Bluaniman.SpaceGame.Network;
using Mirror;
using UnityEngine;

namespace Bluaniman.SpaceGame
{
    public class PlayerEventHandler : MyNetworkBehavior
    {
        public bool isPaused;

        public event Action<string> OnGamePaused;
        public event Action<string> OnGameUnpaused;

        public override void OnStartClient()
        {
            DontDestroyOnLoad(gameObject);
        }

        [Command]
        public void CmdRequestPauseGame(NetworkConnectionToClient sender = null)
        {
            if (isPaused) { return; }
            isPaused = true;
            Time.timeScale = 0f;
            RpcGamePaused(networkManager.connIdToPlayerDict[sender.connectionId].displayName);
        }

        [ClientRpc]
        public void RpcGamePaused(string pausingPlayer)
        {
            Time.timeScale = 0f;
            if (NetworkClient.localPlayer.gameObject.GetComponent<MyNetworkGamePlayer>().displayName == pausingPlayer)
            {
                pausingPlayer = "you";
            }
            OnGamePaused?.Invoke(pausingPlayer);

        }

        [Command]
        public void CmdRequestUnpauseGame(NetworkConnectionToClient sender = null)
        {
            if (!isPaused) { return; }
            isPaused = false;
            Time.timeScale = 1f;
            RpcGameUnpaused(networkManager.connIdToPlayerDict[sender.connectionId].displayName);
        }

        [ClientRpc]
        public void RpcGameUnpaused(string unpausingPlayer)
        {
            Time.timeScale = 1f;
            OnGameUnpaused?.Invoke(unpausingPlayer);
        }
    }
}
