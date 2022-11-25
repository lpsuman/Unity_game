using UnityEngine;

public class CelestialObjectController : MonoBehaviour
{
    public CelestialObjectData celestialObjectData;
    void Start()
    {
        float scaling = transform.parent.GetComponentInParent<ScaledDimensionController>().scaling;
        float scaledRadius = celestialObjectData.radius * scaling;
        transform.localScale = new Vector3(scaledRadius, scaledRadius, scaledRadius);
    }
}
