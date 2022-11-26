using UnityEngine;
using Cinemachine;
using twoloop;

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
                    cinemachineBrain.ManualUpdate();
                    //Debug.Log("Brain notified.");
                }
            });
        }

        public void FixedUpdate()
        {
            cinemachineBrain.ManualUpdate();
        }
    }
}