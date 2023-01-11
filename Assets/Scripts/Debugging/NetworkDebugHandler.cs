using Bluaniman.SpaceGame.General;
using Mirror;

namespace Bluaniman.SpaceGame.Debugging
{
	public class NetworkDebugHandler : NetworkBehaviour
	{
        private void Start()
		{
			if (DebugHandler.ShouldDebug() && !Globals.hasRegisteredDbgNetMsgHandlers)
			{
				if (isServer)
				{
					NetworkServer.RegisterHandler<DebugHandler.DebugNetworkMessage>(DebugHandler.OnDebugNetworkMessageFromClient);
				}
				if (isClient)
				{
					NetworkClient.RegisterHandler<DebugHandler.DebugNetworkMessage>(DebugHandler.OnDebugNetworkMessageFromServer);
				}
				Globals.hasRegisteredDbgNetMsgHandlers = true;
			}
		}
	}
}