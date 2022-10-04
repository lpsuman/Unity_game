using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 relativePosition;
    public float lookUpAngle;
    void Start()
    {
        twoloop.FloatingOrigin.OnOriginShifted.AddListener((a, b) =>
        {
            updateTransform();
        });
    }

    void LateUpdate()
    {
        updateTransform();
    }

    void updateTransform()
    {
        transform.position = target.position + target.forward * relativePosition.x + target.up * relativePosition.y;
        transform.rotation = target.rotation;
        //transform.LookAt(target);
        transform.Rotate(-lookUpAngle, 0, 0);
    }
}
