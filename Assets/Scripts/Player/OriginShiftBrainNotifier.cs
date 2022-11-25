using UnityEngine;
using Cinemachine;
using twoloop;

namespace Bluaniman.SpaceGame.Player
{
	public class OriginShiftBrainNotifier : AbstractLateAfterFixedUpdate
	{
        [SerializeField] private CinemachineBrain cinemachineBrain = null;

        public void Start()
        {
            OriginShift.OnOriginShifted.AddListener((_, _) =>
            {
                if (wasFixedUpdateCalledThisFrame)
                {
                    cinemachineBrain.ManualUpdate();
                    Debug.Log("Brain notified.");
                }
            });
        }
    }
}