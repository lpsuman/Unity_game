using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Bluaniman.SpaceGame.Debugging;
using Bluaniman.SpaceGame.Network;
using UnityEngine.InputSystem;
using Bluaniman.SpaceGame.General;

namespace Bluaniman.SpaceGame.Player
{
	public abstract class AbstractInputSyncHandler : IReadiable
	{
		protected const string InputMappingNotFinalizedExcMsg = "ERROR! Input mapping is not finalized!";
		protected const string InputMappingAlreadyFinalizedExcMsg = "ERROR! Input mapping is already finalized!";
		protected const string InputMappingEmptyExcMsg = "ERROR! Can't finalize an empty input mapping!";

		protected readonly List<InputAction> inputActions = new();
		protected DebugHandler.DebugNameAndNetContext debugData;

        public abstract bool IsReady { get; set; }

        public abstract event Action OnReady;
    }
}