using UnityEngine;

namespace Bluaniman.SpaceGame.World.Celestial
{
	public class CelestialCameraController : MonoBehaviour
	{
		[SerializeField] private ScaledDimensionController scaledDimensionController = null;
		[SerializeField] private Camera mainCamera = null;

        public void LateUpdate()
        {
            transform.SetPositionAndRotation(mainCamera.transform.position * scaledDimensionController.scaling, mainCamera.transform.rotation);
        }
    }
}