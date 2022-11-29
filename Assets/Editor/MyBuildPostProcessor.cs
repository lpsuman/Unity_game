using UnityEngine;
using Mirror;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Threading.Tasks;

namespace Bluaniman.SpaceGame.Editor
{
	public class MyBuildPostProcessor : NetworkBehaviour
	{
		[PostProcessBuild(1)]
		public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
		{
			#if UNITY_EDITOR
			DelayUseAsync(5000);
            EditorApplication.isPlaying = true;
			#endif
        }

		async static void DelayUseAsync(int delayMilis)
		{
			await Task.Delay(delayMilis);
		}
	}
}