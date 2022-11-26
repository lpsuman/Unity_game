using System.Collections;
using UnityEngine;

namespace Bluaniman.SpaceGame.Player
{
	public class AfterFixedUpdate : MonoBehaviour
    {
        public static bool wasFixedUpdateCalledThisFrame = false;

        private IEnumerator WaitEndOfFrame()
        {
            yield return waitForEndOfFrame;
            wasFixedUpdateCalledThisFrame = false;
        }

        private readonly YieldInstruction waitForEndOfFrame = new WaitForEndOfFrame();

        public void FixedUpdate()
        {
            wasFixedUpdateCalledThisFrame = true;
            StartCoroutine(WaitEndOfFrame());
        }
    }
}