using UnityEngine;
using twoloop;

namespace Bluaniman.SpaceGame.Debugging
{
	public class OriginShiftLogger : MonoBehaviour
	{
        public void Start()
        {
            OriginShift.OnOriginShifted.AddListener((_, shiftVector) =>
            {
                Debug.Log($"Origin shifted by {shiftVector}");
            });
        }
    }
}