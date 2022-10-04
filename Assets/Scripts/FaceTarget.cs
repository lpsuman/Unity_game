using UnityEngine;

public class FaceTarget : MonoBehaviour
{
    public Transform target;

    void Update()
    {
        transform.LookAt(target.position);
    }
}
