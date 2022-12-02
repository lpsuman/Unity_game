using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Bluaniman.SpaceGame.Network
{
	public class MyNetworkBehavior : NetworkBehaviour
	{
        protected bool IsClientWithOwnership()
        {
            return isClient && isOwned;
        }
    }
}