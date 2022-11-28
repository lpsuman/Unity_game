using UnityEngine;
using twoloop;
using Mirror;
using System.ComponentModel;

namespace Bluaniman.SpaceGame.Debugging
{
	public class OriginShiftLogger : NetworkBehaviour
	{
        private void Start()
        {
            switch (DebugHandler.originShift)
            {
                case DebugHandler.OriginShiftLoggingMode.Disabled:
                    break;
                case DebugHandler.OriginShiftLoggingMode.OnlyOnChange:
                    OriginShift.OnOriginShifted.AddListener((_, shiftVector) =>
                    {
                        Debug.Log($"Origin changed and shifted by {shiftVector}");
                    });
                    break;
                case DebugHandler.OriginShiftLoggingMode.Always:
                    OriginShift.OnOriginShifted.AddListener((_, shiftVector) =>
                    {
                        Debug.Log($"Origin shifted by {shiftVector}");
                    });
                    break;
                default:
                    throw new InvalidEnumArgumentException();
            }
        }
    }
}