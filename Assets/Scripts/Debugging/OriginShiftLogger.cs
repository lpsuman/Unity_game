using UnityEngine;
using twoloop;
using Mirror;

namespace Bluaniman.SpaceGame.Debugging
{
	public class OriginShiftLogger : NetworkBehaviour
	{
        private void Start()
        {
            if (DebugHandler.ShouldDebug(DebugHandler.originShift))
            {
                OriginShift.OnOriginShifted.AddListener((_, shiftVector) =>
                {
                    Debug.Log($"Origin shifted by {shiftVector}");
                });
            }
        }
    }
}