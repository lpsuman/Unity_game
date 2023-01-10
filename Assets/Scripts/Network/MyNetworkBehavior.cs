using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Bluaniman.SpaceGame.Networking;
using Bluaniman.SpaceGame.General;

namespace Bluaniman.SpaceGame.Network
{
	public class MyNetworkBehavior : NetworkBehaviour
    {
        protected MyNetworkManager networkManager = Globals.networkManager;
        private PlayerEventHandler playerEventHandler = null;
        protected PlayerEventHandler PlayerEventHandler
        {
            get
            {
                if (playerEventHandler == null)
                {
                    foreach (PlayerEventHandler playerEventHandler in networkManager.playerEventHandlerList)
                    {
                        if (playerEventHandler.isOwned)
                        {
                            this.playerEventHandler = playerEventHandler;
                            break;
                        }
                    }
                }
                return playerEventHandler;
            }
        }

        public event Action OnDestroyed;

        public bool IsClientWithOwnership()
        {
            return isClient && isOwned;
        }

        public virtual bool IsClientWithLocalControls()
        {
            return false;
        }

        public void LeaveGame()
        {
            if (isServer && isClient)
            {
                networkManager.StopHost();
            }
            else if (isServer)
            {
                networkManager.StopServer();
            }
            else
            {
                networkManager.StopClient();
            }
        }

        private void OnDestroy()
        {
            OnDestroyed?.Invoke();
        }
    }
}