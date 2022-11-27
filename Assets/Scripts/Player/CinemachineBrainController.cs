using UnityEngine;
using Cinemachine;
using twoloop;
using Bluaniman.SpaceGame.Debugging;

namespace Bluaniman.SpaceGame.Player
{
	public class CinemachineBrainController : MonoBehaviour
	{
        [SerializeField] private CinemachineBrain cinemachineBrain = null;

        public void Start()
        {
            OriginShift.OnOriginShifted.AddListener((_, _) =>
            {
                if (AfterFixedUpdate.wasFixedUpdateCalledThisFrame)
                {
                    ICinemachineCamera vcam = cinemachineBrain.ActiveVirtualCamera;
                    vcam.OnTargetObjectWarped(vcam.Follow, Vector3.zero);
                    cinemachineBrain.ManualUpdate();
                    if (DebugHandler.ShouldDebug(DebugHandler.cinemachineBrainUpdating)) {
                        Debug.Log("Brain notified.");
                    }
                }
            });
        }

        public void FixedUpdate()
        {
            cinemachineBrain.ManualUpdate();
            if (DebugHandler.ShouldDebug(DebugHandler.cinemachineBrainUpdating))
            {
                Debug.Log("Brain fixed update.");
            }
        }
    }
}