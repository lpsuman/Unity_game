using System;
using UnityEngine;
using System.Collections;
using Bluaniman.SpaceGame.Debugging;

namespace Bluaniman.SpaceGame.General
{
	public class CoroutineHelper : MonoBehaviour
	{
		public static IEnumerator CheckDoWait(float waitTime, Func<bool> condition, Action action)
        {
			YieldInstruction waitForTick = new WaitForSeconds(waitTime);
			DebugHandler.CheckAndDebugLog(DebugHandler.Coroutine(), "CheckDoWait has started.");
			while (condition.Invoke())
            {
				action.Invoke();
				yield return waitForTick;
				DebugHandler.CheckAndDebugLog(DebugHandler.Coroutine(), "CheckDoWait has waited.");
			}
			DebugHandler.CheckAndDebugLog(DebugHandler.Coroutine(), "CheckDoWait is done.");
        }
	}
}