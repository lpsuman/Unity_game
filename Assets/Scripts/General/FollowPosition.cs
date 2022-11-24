using UnityEngine;

namespace Bluaniman.SpaceGame.General
{
	public class FollowPosition : MonoBehaviour
	{
		[SerializeField] private Transform target;

        public void Update()
        {
            transform.position = target.position;
        }
    }
}