using UnityEngine;
using Cinemachine;
using twoloop;
using Mirror;

namespace Bluaniman.SpaceGame.Player
{
	public class OriginShiftCameraNotifier : NetworkBehaviour
    {
		[SerializeField] private CinemachineVirtualCamera virtualCamera = null;

        public override void OnStartAuthority()
        {
            OriginShift.OnOriginShifted.AddListener((_, shiftVector) =>
            {
                if (AfterFixedUpdate.wasFixedUpdateCalledThisFrame)
                {
                    virtualCamera.OnTargetObjectWarped(transform, Vector3.zero);
                    //Debug.Log("Camera notified.");
                }
            });
        }
    }
}