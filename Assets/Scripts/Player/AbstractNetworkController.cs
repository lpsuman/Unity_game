using UnityEngine;
using Mirror;
using Bluaniman.SpaceGame.Input;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;
using Bluaniman.SpaceGame.Debugging;

namespace Bluaniman.SpaceGame.Player
{
	public abstract class AbstractNetworkController : NetworkBehaviour
	{
        [SerializeField] protected readonly SyncList<float> inputAxii = new();
        private readonly List<InputAction> inputActions = new();
        [SerializeField] protected bool useAuthorityPhysics = true;

        private Controls controls;
        protected Controls Controls
        {
            get
            {
                if (controls == null)
                {
                    controls = new Controls();
                }
                return controls;
            }
        }

        private void Start()
        {
            if (isClient && isOwned)
            {
                Controls.Enable();
                inputAxii.Callback += OnInventoryUpdated;
            }
            else
            {
                Controls.Disable();
            }
        }

        private void OnInventoryUpdated(SyncList<float>.Operation op, int index, float oldItem, float newItem)
        {
            DebugHandler.CheckAndDebugLog(DebugHandler.Input(), $"Input axis at {index} changed to {newItem}.", this);
        }

        protected void BindInputAction(InputAction inputAction)
        {
            inputActions.Add(inputAction);
            DebugHandler.CheckAndDebugLog(DebugHandler.Input(), $"Client added input action at {inputActions.Count - 1}", this);
            CmdAddToInputActionSyncList();
        }

        [Command]
        public void CmdAddToInputActionSyncList(NetworkConnectionToClient sender = null)
        {
            inputAxii.Add(0f);
            int index = inputAxii.Count - 1;
            DebugHandler.CheckAndDebugLog(DebugHandler.Input(), $"Command added axis to list at {index}", this);
            TargetInputBound(sender, index);
        }

        [TargetRpc]
        public void TargetInputBound(NetworkConnection target, int index)
        {
            InputAction inputAction = inputActions[index];
            inputAction.performed += ctx => CmdSetAxisInput(index, ctx.ReadValue<float>());
            inputAction.canceled += ctx => CmdSetAxisInput(index, 0f);
            if (useAuthorityPhysics)
            {
                inputAction.performed += ctx => SetAxisInput(index, ctx.ReadValue<float>());
                inputAction.canceled += ctx => SetAxisInput(index, 0f);
            }
            DebugHandler.CheckAndDebugLog(DebugHandler.Input(), $"Input action RPC done for index {index}", this);
        }

        public void SetAxisInput(int index, float value)
        {
            inputAxii[index] = value;
            DebugHandler.CheckAndDebugLog(DebugHandler.Input(), $"Set axis input {index} to {value}");
        }

        [Command]
        public void CmdSetAxisInput(int index, float value)
        {
            SetAxisInput(index, value);
            DebugHandler.CheckAndDebugLog(DebugHandler.Input(), $"Called command axis input {index} to {value}");
        }
    }
}