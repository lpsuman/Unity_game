using UnityEngine;

public class ScaledDimensionController : MonoBehaviour
{
    public float scaling;
    void Start()
    {
        twoloop.FloatingOrigin.singleton.layerIDToScalingDict[gameObject.layer] = scaling;
    }
}
