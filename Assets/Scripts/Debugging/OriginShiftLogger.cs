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
            switch (DebugHandler.OriginShift())
            {
                case DebugHandler.OriginShiftLoggingMode.Disabled:
                    break;
                case DebugHandler.OriginShiftLoggingMode.OnlyOnChange:
                    OriginShift.OnOriginShifted.AddListener((_, shiftVector) =>
                    {
                        if (shiftVector.magnitude > 0f)
                        {
                            DebugHandler.NetworkLog($"Origin changed and shifted by {shiftVector}", this);
                        }
                    });
                    break;
                case DebugHandler.OriginShiftLoggingMode.Always:
                    OriginShift.OnOriginShifted.AddListener((_, shiftVector) =>
                    {
                        DebugHandler.NetworkLog($"Origin shifted by {shiftVector}", this);
                    });
                    break;
                default:
                    throw new InvalidEnumArgumentException();
            }
        }
    }
}