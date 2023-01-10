using Bluaniman.SpaceGame.Debugging;
using Bluaniman.SpaceGame.Networking;
using UnityEngine;

namespace Bluaniman.SpaceGame.General
{
	public class DDOL : MonoBehaviour
	{
        public void Awake()
        {
            DontDestroyOnLoad(gameObject);

            Globals.networkManager = GetComponentInChildren<MyNetworkManager>();
            Globals.debugHandler = GetComponentInChildren<DebugHandler>();
        }
	}
}