using Mirror;
using UnityEngine;

namespace Bluaniman.SpaceGame.Debugging
{
	public class DebugHandler : NetworkBehaviour
	{
		private static DebugHandler singleton;
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

		// ~SH fields (serialization helper) are used to show static fields in Unity's inspector
		[Header("Debug")]
		[SerializeField] private bool isDebugEnabled = false;
		public static bool IsDebugEnabled() => singleton.isDebugEnabled;
		[SerializeField] private bool updateOrder = false;
		public static bool UpdateOrder() => singleton.updateOrder;
		[SerializeField] private OriginShiftLoggingMode originShift = OriginShiftLoggingMode.Disabled;
		public static OriginShiftLoggingMode OriginShift() => singleton.originShift;
		[SerializeField] private bool cinemachineBrainUpdating = false;
		public static bool CinemachineBrainUpdating() => singleton.cinemachineBrainUpdating;
		[SerializeField] private bool mainMenu = false;
		public static bool MainMenu() => singleton.mainMenu;
		[SerializeField] private bool input = false;
		public static bool Input() => singleton.input;
		[SerializeField] private bool coroutine = false;
		public static bool Coroutine() => singleton.coroutine;

		[Header("Network debug")]
		[SerializeField] private bool serverMessages = false;
		public static bool ServerMessages() => singleton.serverMessages;
		[SerializeField] private bool remoteClientMessages = false;
		public static bool RemoteClientMessages() => singleton.remoteClientMessages;
		[SerializeField] private bool hostClientMessages = false;
		public static bool HostClientMessages() => singleton.hostClientMessages;

		[Header("Lobby")]
		[SerializeField] private AutoLobbyAction autoHost = AutoLobbyAction.Disabled;
		public static AutoLobbyAction AutoHost() => singleton.autoHost;
		[SerializeField] private AutoLobbyAction autoJoin = AutoLobbyAction.Disabled;
		public static AutoLobbyAction AutoJoin() => singleton.autoJoin;
		[SerializeField] private AutoLobbyAction autoReady = AutoLobbyAction.Disabled;
		public static AutoLobbyAction AutoReady() => singleton.autoReady;
		[SerializeField] private AutoLobbyAction autoStart = AutoLobbyAction.Disabled;
		public static AutoLobbyAction AutoStart() => singleton.autoStart;
		[SerializeField] private AutoLobbyAction autoStartNotAlone = AutoLobbyAction.Disabled;
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
            DebugNetworkMessage debugNetworkMessage = new DebugNetworkMessage()
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
				msg += "No network context provided.";
			}
			else
			{
				msg += $"\nisServer={debugNetworkMessage.isServer} isClient={debugNetworkMessage.isClient} isOwned={debugNetworkMessage.isOwned}";

			}
			Debug.Log(msg);
		}

        private void Awake()
        {
			singleton = this;
		}

        private void Start()
        {
			if (ShouldDebug())
			{
				if (isServer)
				{
					NetworkServer.RegisterHandler<DebugNetworkMessage>(OnDebugNetworkMessageFromClient);

				}
				if (isClient)
                {
					NetworkClient.RegisterHandler<DebugNetworkMessage>(OnDebugNetworkMessageFromServer);
				}
			}
			DontDestroyOnLoad(this);
		}
    }
}