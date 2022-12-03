using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Bluaniman.SpaceGame.Network
{
	public class MyNetworkBehavior : NetworkBehaviour
    {
        [SerializeField] protected bool useAuthorityPhysics = true;
        protected bool IsClientWithOwnership()
        {
            return isClient && isOwned;
        }

        protected bool IsClientWithLocalControls()
        {
            return IsClientWithOwnership() && useAuthorityPhysics;
        }
    }
}