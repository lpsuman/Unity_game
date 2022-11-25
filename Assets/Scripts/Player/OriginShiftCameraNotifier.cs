using UnityEngine;
using Cinemachine;
using twoloop;

namespace Bluaniman.SpaceGame.Player
{
	public class OriginShiftCameraNotifier : AbstractLateAfterFixedUpdate
    {
		[SerializeField] private CinemachineVirtualCamera virtualCamera = null;

        public void Start()
        {
            OriginShift.OnOriginShifted.AddListener((_, shiftVector) =>
            {
                if (wasFixedUpdateCalledThisFrame)
                {
                    //virtualCamera.PreviousStateIsValid = false;
                    virtualCamera.OnTargetObjectWarped(transform, shiftVector);
                    Debug.Log("Camera notified.");

                    //int numVcams = CinemachineCore.Instance.VirtualCameraCount;
                    //for (int i = 0; i < numVcams; ++i)
                    //    CinemachineCore.Instance.GetVirtualCamera(i).OnTargetObjectWarped(
                    //        transform, shiftVector);
                }
            });
        }
    }
}