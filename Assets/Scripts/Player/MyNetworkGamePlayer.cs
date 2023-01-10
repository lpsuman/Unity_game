using System;
using UnityEngine;
using Mirror;
using Bluaniman.SpaceGame.Networking;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Bluaniman.SpaceGame.Network;

namespace Bluaniman.SpaceGame.Lobby
{
    public class MyNetworkGamePlayer : MyNetworkBehavior
    {
        [SyncVar]
        public string displayName = "Loading...";

        public override void OnStartClient()
        {
            DontDestroyOnLoad(gameObject);
            networkManager.GamePlayers.Add(this);
        }

        public override void OnStopClient()
        {
            networkManager.GamePlayers.Remove(this);
            Destroy(gameObject);
        }

        public override void OnStopLocalPlayer()
        {
            SceneManager.LoadScene(networkManager.menuScene);
        }

        [Server]
        public void SetDisplayName(string displayName)
        {
            this.displayName = displayName;
        }
    }
}
