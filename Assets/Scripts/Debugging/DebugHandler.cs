using Mirror;
using UnityEngine;

namespace Bluaniman.SpaceGame.Debugging
{
	public class DebugHandler : MonoBehaviour
	{
		public static DebugHandler singleton;
		public struct DebugNetworkMessage : NetworkMessage
        {
			public string debugMsg;
			public bool isServer;
			public bool isClient;
			public bool isOwned;
        }

		public enum AutoLobbyAction
        {
			Disabled,
			Editor,
			NonEditor,
			Always
        }

		public enum OriginShiftLoggingMode
        {
			Disabled,
			OnlyOnChange,
			Always
        }

		[Header("Debug")]
		[SerializeField] private bool isDebugEnabled;
		public static bool IsDebugEnabled() => singleton.isDebugEnabled;
		[SerializeField] private bool updateOrder;
		public static bool UpdateOrder() => singleton.updateOrder;
		[SerializeField] private OriginShiftLoggingMode originShift;
		public static OriginShiftLoggingMode OriginShift() => singleton.originShift;
		[SerializeField] private bool cinemachineBrainUpdating;
		public static bool CinemachineBrainUpdating() => singleton.cinemachineBrainUpdating;
		[SerializeField] private bool mainMenu;
		public static bool MainMenu() => singleton.mainMenu;
		[SerializeField] private bool input;
		public static bool Input() => singleton.input;
		[SerializeField] private bool coroutine;
		public static bool Coroutine() => singleton.coroutine;
		[SerializeField] private bool movement;
		public static bool Movement() => singleton.movement;

		[Header("Network debug")]
		[SerializeField] private bool serverMessages;
		public static bool ServerMessages() => singleton.serverMessages;
		[SerializeField] private bool remoteClientMessages;
		public static bool RemoteClientMessages() => singleton.remoteClientMessages;
		[SerializeField] private bool hostClientMessages;
		public static bool HostClientMessages() => singleton.hostClientMessages;

		[Header("Lobby")]
		[SerializeField] private AutoLobbyAction autoHost;
		public static AutoLobbyAction AutoHost() => singleton.autoHost;
		[SerializeField] private AutoLobbyAction autoJoin;
		public static AutoLobbyAction AutoJoin() => singleton.autoJoin;
		[SerializeField] private AutoLobbyAction autoReady;
		public static AutoLobbyAction AutoReady() => singleton.autoReady;
		[SerializeField] private AutoLobbyAction autoStart;
		public static AutoLobbyAction AutoStart() => singleton.autoStart;
		[SerializeField] private AutoLobbyAction autoStartNotAlone;
		public static AutoLobbyAction AutoStartNotAlone() => singleton.autoStartNotAlone;

		public static bool ShouldDebug(bool additionalCondition = true, NetworkBehaviour networkContext = null)
        {
			return IsDebugEnabled()
				&& (	networkContext == null
					|| (networkContext.isServer && ServerMessages())
					|| (networkContext.isServer && networkContext.isClient && HostClientMessages())
					|| (networkContext.isClientOnly && RemoteClientMessages()))
				&& additionalCondition;
		}

		public static bool ShouldAutoLobbyAction(AutoLobbyAction lobbyAction)
		{
			if (!IsDebugEnabled())
			{
				return false;
			}
			else if (lobbyAction == AutoLobbyAction.Disabled)
			{
				return false;
			}
			else if (lobbyAction == AutoLobbyAction.Always)
			{
				return true;
			}
			// using logical XOR to simplify the IF statement
			else return Application.isEditor ^ lobbyAction == AutoLobbyAction.NonEditor;
		}

		public static void CheckAndDebugLog(bool additionalCondition, string debugMsg, NetworkBehaviour networkContext = null)
        {
			if (additionalCondition)
            {
				NetworkLog(debugMsg, networkContext);
            }
        }

		public static void NetworkLog(string debugMsg, NetworkBehaviour networkContext = null)
		{
            if (!ShouldDebug()) { return; }
			if (NetworkClient.connection == null || !NetworkClient.connection.isReady)
            {
				Debug.Log($"{debugMsg}\nNo connection to server.");
				return;
            }
            DebugNetworkMessage debugNetworkMessage = new()
			{
				debugMsg = debugMsg,
				isServer = networkContext != null && networkContext.isServer,
				isClient = networkContext != null && networkContext.isClient,
				isOwned = networkContext != null && networkContext.isOwned
			};
			NetworkClient.Send(debugNetworkMessage);
		}

		public static void OnDebugNetworkMessageFromClient(NetworkConnectionToClient conn, DebugNetworkMessage debugNetworkMessage)
        {
			NetworkServer.SendToAll(debugNetworkMessage);
		}

		public static void OnDebugNetworkMessageFromServer(DebugNetworkMessage debugNetworkMessage)
		{
			string msg = debugNetworkMessage.debugMsg;
			if (!debugNetworkMessage.isServer && !debugNetworkMessage.isClient)
			{
				msg += "\nNo network context provided.";
			}
			else
			{
				msg += $"\nisServer={debugNetworkMessage.isServer} isClient={debugNetworkMessage.isClient} isOwned={debugNetworkMessage.isOwned}";

			}
			Debug.Log(msg);
		}

        private void Awake()
        {
			if (singleton == null)
			{
				singleton = this;
			}
		}

        private void Start()
		{
			DontDestroyOnLoad(this);
		}
    }
}