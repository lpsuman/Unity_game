using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bluaniman.SpaceGame.Network
{
    public class MyNetworkPhysicsBehavior : MyNetworkBehavior
    {
        [SerializeField] protected bool useAuthorityPhysics = true;

        public override bool IsClientWithLocalControls()
        {
            return IsClientWithOwnership() && useAuthorityPhysics;
        }
    }
}
