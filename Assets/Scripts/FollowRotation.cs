using UnityEngine;

public class FollowRotation : MonoBehaviour
{
    public Transform target;

    void Update()
    {
        transform.rotation = target.rotation;
    }
}
