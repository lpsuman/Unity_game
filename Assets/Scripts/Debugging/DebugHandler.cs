using Mirror;
using UnityEngine;

namespace Bluaniman.SpaceGame.Debugging
{
	public class DebugHandler : MonoBehaviour, ISerializationCallbackReceiver
	{
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
		[SerializeField] private bool isDebugEnabledSH;
		public static bool isDebugEnabled = false;
		[SerializeField] private bool updateOrderSH;
		public static bool updateOrder = false;
		[SerializeField] private OriginShiftLoggingMode originShiftSH;
		public static OriginShiftLoggingMode originShift = OriginShiftLoggingMode.Disabled;
		[SerializeField] private bool cinemachineBrainUpdatingSH;
		public static bool cinemachineBrainUpdating = false;
		[SerializeField] private bool mainMenuSH;
		public static bool mainMenu = false;

		[Header("Network debug")]
		[SerializeField] private bool serverMessagesSH;
		public static bool serverMessages = false;
		[SerializeField] private bool remoteClientMessagesSH;
		public static bool remoteClientMessages = false;
		[SerializeField] private bool hostClientMessagesSH;
		public static bool hostClientMessages = false;

		[Header("Lobby")]
		[SerializeField] private AutoLobbyAction autoHostSH;
		public static AutoLobbyAction autoHost = AutoLobbyAction.Disabled;
		[SerializeField] private AutoLobbyAction autoJoinSH;
		public static AutoLobbyAction autoJoin = AutoLobbyAction.Disabled;
		[SerializeField] private AutoLobbyAction autoReadySH;
		public static AutoLobbyAction autoReady = AutoLobbyAction.Disabled;
		[SerializeField] private AutoLobbyAction autoStartSH;
		public static AutoLobbyAction autoStart = AutoLobbyAction.Disabled;
		[SerializeField] private AutoLobbyAction autoStartNotAloneSH;
		public static AutoLobbyAction autoStartNotAlone = AutoLobbyAction.Disabled;


		public void OnAfterDeserialize()
		{
			isDebugEnabled = isDebugEnabledSH;
			updateOrder = updateOrderSH;
			originShift = originShiftSH;
			cinemachineBrainUpdating = cinemachineBrainUpdatingSH;
			mainMenu = mainMenuSH;

			serverMessages = serverMessagesSH;
			remoteClientMessages = remoteClientMessagesSH;
			hostClientMessages = hostClientMessagesSH;

			autoHost = autoHostSH;
			autoJoin = autoJoinSH;
			autoReady = autoReadySH;
			autoStart = autoStartSH;
			autoStartNotAlone = autoStartNotAloneSH;
		}

		public static bool ShouldDebug(bool additionalCondition = true)
        {
			return isDebugEnabled && Application.isEditor && additionalCondition;
		}

		public static bool ShouldAutoLobbyAction(AutoLobbyAction lobbyAction)
		{
			if (!ShouldDebug())
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
			if (ShouldDebug(additionalCondition))
            {
				NetworkLog(debugMsg, networkContext);
            }
        }

		public static void NetworkLog(string debugMsg, NetworkBehaviour networkContext = null)
		{
			if (!ShouldDebug()) { return; }
			if (networkContext == null)
            {
				Debug.Log($"{debugMsg}\nNo network context was provided.");
            }
			else if ((networkContext.isServer && serverMessages)
				|| (networkContext.isServer && networkContext.isClient && hostClientMessages))
            {
				Log(debugMsg, networkContext);
			}
			else if (networkContext.isClientOnly && remoteClientMessages)
            {
				CmdLog(debugMsg, networkContext);
            }


		}

		private static void Log(string debugMsg, NetworkBehaviour networkContext)
        {
			Debug.Log($"{debugMsg}\nisServer={networkContext.isServer} isClient={networkContext.isClient} isOwned={networkContext.isOwned}");
		}

		// TODO implement as network message
		private static void CmdLog(string debugMsg, NetworkBehaviour networkContext)
        {
			Log(debugMsg, networkContext);
        }

        private void Start()
        {
			DontDestroyOnLoad(this);
        }

        public void OnBeforeSerialize()
        {
            // do nothing
        }
    }
}