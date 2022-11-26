using UnityEngine;
using Cinemachine;
using twoloop;
using Mirror;

namespace Bluaniman.SpaceGame.Player
{
	public class CinemachineBrainController : AbstractLateAfterFixedUpdate
	{
        [SerializeField] private CinemachineBrain cinemachineBrain = null;

        public override void OnStartClient()
        {
            OriginShift.OnOriginShifted.AddListener((_, _) =>
            {
                if (wasFixedUpdateCalledThisFrame)
                {
                    cinemachineBrain.ManualUpdate();
                    //Debug.Log("Brain notified.");
                }
            });
        }

        [ClientCallback]
        public new void FixedUpdate()
        {
            cinemachineBrain.ManualUpdate();
            base.FixedUpdate();
        }
    }
}