using System.Collections;
using UnityEngine;
using Mirror;

namespace Bluaniman.SpaceGame.Player
{
	public class AbstractLateAfterFixedUpdate : NetworkBehaviour
    {
        private IEnumerator WaitEndOfFrame()
        {
            yield return waitForEndOfFrame;
            wasFixedUpdateCalledThisFrame = false;
        }

        private readonly YieldInstruction waitForEndOfFrame = new WaitForEndOfFrame();
        protected bool wasFixedUpdateCalledThisFrame = false;

        [ClientCallback]
        public void FixedUpdate()
        {
            wasFixedUpdateCalledThisFrame = true;
            StartCoroutine(WaitEndOfFrame());
        }
    }
}