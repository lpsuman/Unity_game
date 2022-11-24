using System;
using twoloop;
using UnityEngine;
using static twoloop.OriginShift;

namespace Bluaniman.SpaceGame.World.Celestial
{
	public class CelestialCameraController : MonoBehaviour
	{
		[SerializeField] private Camera mainCamera;
        private float scaling = 0f;

        public void Start()
        {
            scaling = transform.parent.GetComponent<ScaledDimensionController>().scaling;
        }

        public void Update()
        {
            Offset localOffset = OriginShift.LocalOffset;
            decimal localOffsetX, localOffsetY, localOffsetZ; 
            switch (OriginShift.singleton.precisionMode)
            {
                case OffsetPrecisionMode.Float:
                    localOffsetX = (decimal)localOffset.vector.x;
                    localOffsetY = (decimal)localOffset.vector.y;
                    localOffsetZ = (decimal)localOffset.vector.z;
                    break;
                case OffsetPrecisionMode.Double:
                    localOffsetX = (decimal)localOffset.xDouble;
                    localOffsetY = (decimal)localOffset.yDouble;
                    localOffsetZ = (decimal)localOffset.zDouble;
                    break;
                case OffsetPrecisionMode.Decimal:
                    localOffsetX = localOffset.xDecimal;
                    localOffsetY = localOffset.yDecimal;
                    localOffsetZ = localOffset.zDecimal;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            Vector3 mainCameraPos = mainCamera.transform.position;
            transform.SetPositionAndRotation(new Vector3(
                (float)((localOffsetX + (decimal)mainCameraPos.x) * (decimal)scaling),
                (float)((localOffsetY + (decimal)mainCameraPos.y) * (decimal)scaling),
                (float)((localOffsetZ + (decimal)mainCameraPos.z) * (decimal)scaling)),
                mainCamera.transform.rotation);
        }
    }
}