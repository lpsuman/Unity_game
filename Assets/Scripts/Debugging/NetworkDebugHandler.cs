using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Bluaniman.SpaceGame.Debugging
{
	public class NetworkDebugHandler : NetworkBehaviour
	{
		private void Start()
		{
			if (DebugHandler.ShouldDebug())
			{
				if (isServer)
				{
					NetworkServer.RegisterHandler<DebugHandler.DebugNetworkMessage>(DebugHandler.OnDebugNetworkMessageFromClient);

				}
				if (isClient)
				{
					NetworkClient.RegisterHandler<DebugHandler.DebugNetworkMessage>(DebugHandler.OnDebugNetworkMessageFromServer);
				}
			}
		}
	}
}