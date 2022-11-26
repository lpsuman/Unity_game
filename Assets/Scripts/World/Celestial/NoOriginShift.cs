using twoloop;
using UnityEngine;

namespace Bluaniman.SpaceGame.World.Celestial
{
	public class NoOriginShift : MonoBehaviour
	{
		public void Start()
		{
			OriginShift.OnOriginShifted.AddListener((_, _) => transform.position = Vector3.zero);
		}
	}
}