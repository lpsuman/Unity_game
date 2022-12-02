using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Bluaniman.SpaceGame.Debugging;
using Bluaniman.SpaceGame.Network;
using Bluaniman.SpaceGame.Input;
using UnityEngine.InputSystem;

namespace Bluaniman.SpaceGame.Player
{
	public abstract class AbstractNetworkController : MyNetworkBehavior
	{
        private readonly SyncList<float> inputAxii = new();
        private readonly List<InputAction> inputActions = new();
        private List<float> inputAxiiLocal;
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
            if (IsClientWithOwnership())
            {
                Controls.Enable();
                if (DebugHandler.ShouldDebug(DebugHandler.Input())) {
                    inputAxii.Callback += OnInventoryUpdated;
                    DebugHandler.NetworkLog("Controls enabled.", this);
                }
            }
            else
            {
                Controls.Disable();
            }
            if (IsClientWithLocalControls()) { inputAxiiLocal = new(); }
        }

        protected bool IsClientWithLocalControls()
        {
            return IsClientWithOwnership() && useAuthorityPhysics;
        }

        private void OnInventoryUpdated(SyncList<float>.Operation op, int index, float oldItem, float newItem)
        {
            DebugHandler.NetworkLog($"Input axis at {index} changed to {newItem}.", this);
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
            if (IsClientWithLocalControls())
            {
                inputAxiiLocal.Add(0f);
                inputAction.performed += ctx => SetAxisInput(index, ctx.ReadValue<float>());
                inputAction.canceled += ctx => SetAxisInput(index, 0f);
            }
            DebugHandler.CheckAndDebugLog(DebugHandler.Input(), $"Input action RPC done for index {index}", this);
        }

        public void SetAxisInput(int index, float value)
        {
            if (isServer || !IsClientWithLocalControls())
            {
                inputAxii[index] = value;
            }
            else
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
            return isServer || !IsClientWithLocalControls() ? inputAxii[index] : inputAxiiLocal[index];
        }
    }
}