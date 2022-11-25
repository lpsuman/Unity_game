using twoloop;
using UnityEngine;

public class ScaledDimensionController : MonoBehaviour
{
    public float scaling;

    public void Start()
    {
        OriginShift.OnOriginShifted.AddListener((localOffset, _) => transform.position = -localOffset * scaling);
    }
}
