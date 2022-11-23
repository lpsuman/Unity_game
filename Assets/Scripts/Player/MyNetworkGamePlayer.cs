using System;
using UnityEngine;
using Mirror;
using Bluaniman.SpaceGame.Networking;
using TMPro;
using UnityEngine.UI;

namespace Bluaniman.SpaceGame.Lobby
{
    public class MyNetworkGamePlayer : NetworkBehaviour
    {

        [SyncVar]
        private string displayName = "Loading...";

        private MyNetworkManager room;

        private MyNetworkManager Room
        {
            get
            {
                if (room != null) { return room; }
                return room = NetworkManager.singleton as MyNetworkManager;
            }
        }

        public override void OnStartClient()
        {
            DontDestroyOnLoad(gameObject);
            Room.GamePlayers.Add(this);
        }

        public override void OnStopClient()
        {
            Room.GamePlayers.Remove(this);
        }

        [Server]
        public void SetDisplayName(string displayName)
        {
            this.displayName = displayName;
        }
    }
}
