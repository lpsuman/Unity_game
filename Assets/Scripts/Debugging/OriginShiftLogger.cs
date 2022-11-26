using UnityEngine;
using twoloop;
using Mirror;

namespace Bluaniman.SpaceGame.Debugging
{
	public class OriginShiftLogger : NetworkBehaviour
	{
        public override void OnStartServer()
        {
            OriginShift.OnOriginShifted.AddListener((_, shiftVector) =>
            {
                Debug.Log($"Origin shifted by {shiftVector}");
            });
        }
    }
}