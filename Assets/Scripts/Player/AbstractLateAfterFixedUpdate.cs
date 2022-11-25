using System.Collections;
using UnityEngine;

namespace Bluaniman.SpaceGame.Player
{
	public class AbstractLateAfterFixedUpdate : MonoBehaviour
	{
        private readonly YieldInstruction waitForEndOfFrame = new WaitForEndOfFrame();
        protected bool wasFixedUpdateCalledThisFrame = false;

        public void FixedUpdate()
        {
            wasFixedUpdateCalledThisFrame = true;
            StartCoroutine(WaitEndOfFrame());
        }

        private IEnumerator WaitEndOfFrame()
        {
            yield return waitForEndOfFrame;
            wasFixedUpdateCalledThisFrame = false;
        }
    }
}