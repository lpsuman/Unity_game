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
        private readonly SyncList<float> inputAxii = new();
        private List<float> inputAxiiLocal;
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
                    DebugHandler.CheckAndDebugLog(DebugHandler.Input(), "New controls.", this);
                }
                return controls;
            }
        }

        public virtual void Start()
        {
            if (useAuthorityPhysics) { inputAxiiLocal = new(); }
            if (isClient && isOwned)
            {
                Controls.Enable();
                inputAxii.Callback += OnInventoryUpdated;
                DebugHandler.CheckAndDebugLog(DebugHandler.Input(), "Controls enabled.", this);
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

        [Client]
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
                inputAxiiLocal.Add(0f);
                inputAction.performed += ctx => SetAxisInput(index, ctx.ReadValue<float>());
                inputAction.canceled += ctx => SetAxisInput(index, 0f);
            }
            DebugHandler.CheckAndDebugLog(DebugHandler.Input(), $"Input action RPC done for index {index}", this);
        }

        public void SetAxisInput(int index, float value)
        {
            if (isServer)
            {
                inputAxii[index] = value;
            }
            if (isClient && useAuthorityPhysics)
            {
                inputAxiiLocal[index] = value;
            }
            DebugHandler.CheckAndDebugLog(DebugHandler.Input(), $"Set axis input {index} to {value}", this);
        }

        [Command]
        public void CmdSetAxisInput(int index, float value)
        {
            SetAxisInput(index, value);
            DebugHandler.CheckAndDebugLog(DebugHandler.Input(), $"Called command axis input {index} to {value}", this);
        }

        protected float GetInputAxis(int index)
        {
            return isServer || !useAuthorityPhysics ? inputAxii[index] : inputAxiiLocal[index];
        }
    }
}