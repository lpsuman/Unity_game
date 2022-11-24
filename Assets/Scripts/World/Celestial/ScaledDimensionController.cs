using twoloop;
using UnityEngine;

public class ScaledDimensionController : MonoBehaviour
{
    public float scaling;

    public void Update()
    {
        if (transform.hasChanged)
        {
            transform.position = Vector3.zero;
        }
    }
}
