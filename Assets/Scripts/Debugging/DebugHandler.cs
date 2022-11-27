using UnityEngine;

namespace Bluaniman.SpaceGame.Debugging
{
	public class DebugHandler : MonoBehaviour, ISerializationCallbackReceiver
	{
		// ~SH fields (serialization helper) are used to show static fields in Unity's inspector
		[Header("Debug")]
		[SerializeField] private bool isDebugEnabledSH;
		public static bool isDebugEnabled = false;
		[SerializeField] private bool updateOrderSH;
		public static bool updateOrder = false;
		[SerializeField] private bool originShiftSH;
		public static bool originShift = false;
		[SerializeField] private bool cinemachineBrainUpdatingSH;
		public static bool cinemachineBrainUpdating = false;

		[Header("Lobby")]
		[SerializeField] private bool autoHostSH;
		public static bool autoHost = false;
		[SerializeField] private bool autoReadySH;
		public static bool autoReady = false;
		[SerializeField] private bool autoStartSH;
		public static bool autoStart = false;
		[SerializeField] private bool autoStartNotAloneSH;
		public static bool autoStartNotAlone = false;


		public void OnAfterDeserialize()
		{
			isDebugEnabled = isDebugEnabledSH;
			updateOrder = updateOrderSH;
			originShift = originShiftSH;
			cinemachineBrainUpdating = cinemachineBrainUpdatingSH;

			autoHost = autoHostSH;
			autoReady = autoReadySH;
			autoStart = autoStartSH;
			autoStartNotAlone = autoStartNotAloneSH;
		}

		public static bool ShouldDebug(bool additionalCondition)
        {
			return isDebugEnabled && Application.isEditor && additionalCondition;
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