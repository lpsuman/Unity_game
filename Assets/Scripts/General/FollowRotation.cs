using UnityEngine;

public class FollowRotation : MonoBehaviour
{
    [SerializeField] private Transform target;

    public void Update()
    {
        transform.rotation = target.rotation;
    }
}
