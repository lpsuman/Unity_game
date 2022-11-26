using UnityEngine;
using Cinemachine;
using twoloop;

namespace Bluaniman.SpaceGame.Player
{
	public class OriginShiftCameraNotifier : AbstractLateAfterFixedUpdate
    {
		[SerializeField] private CinemachineVirtualCamera virtualCamera = null;

        public override void OnStartClient()
        {
            OriginShift.OnOriginShifted.AddListener((_, shiftVector) =>
            {
                if (wasFixedUpdateCalledThisFrame)
                {
                    virtualCamera.OnTargetObjectWarped(transform, Vector3.zero);
                    //Debug.Log("Camera notified.");
                }
            });
        }
    }
}