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

			autoHost = autoHostSH;
			autoJoin = autoJoinSH;
			autoReady = autoReadySH;
			autoStart = autoStartSH;
			autoStartNotAlone = autoStartNotAloneSH;
		}

		public static bool ShouldDebug(bool additionalCondition)
        {
			return isDebugEnabled && Application.isEditor && additionalCondition;
		}

		public static bool ShouldAutoLobbyAction(AutoLobbyAction lobbyAction)
		{
			if (!isDebugEnabled)
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
			else return Application.isEditor ^ lobbyAction == AutoLobbyAction.NonEditor;
		}

		public static void CheckAndDebugLog(bool additionalCondition, string debugMsg)
        {
			if (ShouldDebug(additionalCondition))
            {
				Debug.Log(debugMsg);
            }
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