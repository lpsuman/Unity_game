using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Bluaniman.SpaceGame.Debugging;
using Bluaniman.SpaceGame.Network;
using Bluaniman.SpaceGame.Input;
using UnityEngine.InputSystem;
using twoloop;

namespace Bluaniman.SpaceGame.Player
{
    [Serializable]
    public class NetworkPlayerController : MyNetworkBehavior, IInputAxisProvider
    {
        private readonly SyncList<float> inputAxii = new();
        private readonly List<InputAction> inputActions = new();
        private List<float> inputAxiiLocal;

        public event Action OnControlsEnabled;

        private Controls controls;
        public Controls Controls
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
        public void Start()
        {
            DebugHandler.CheckAndDebugLog(DebugHandler.Input(), "Controller setup start.", this);
            if (IsClientWithLocalControls()) { inputAxiiLocal = new(); }
            if (IsClientWithOwnership())
            {
                Controls.Enable();
                if (DebugHandler.ShouldDebug(DebugHandler.Input()))
                {
                    inputAxii.Callback += OnInventoryUpdated;
                }
                DebugHandler.CheckAndDebugLog(DebugHandler.Input(), "Controls enabled.", this);
                OnControlsEnabled?.Invoke();
            }
            else
            {
                Controls.Disable();
            }
            DebugHandler.CheckAndDebugLog(DebugHandler.Input(), "Controller setup done.", this);
        }

        private void OnInventoryUpdated(SyncList<float>.Operation op, int index, float oldItem, float newItem)
        {
            DebugHandler.NetworkLog($"Input axis at {index} changed to {newItem}.", this);
        }

        [Client]
        public void BindInputAction(InputAction inputAction)
        {
            inputActions.Add(inputAction);
            DebugHandler.CheckAndDebugLog(DebugHandler.Input(), $"Client added input action at {inputActions.Count - 1}", this);
            CmdAddToInputActionSyncList();
        }

        [Command]
        private void CmdAddToInputActionSyncList(NetworkConnectionToClient sender = null)
        {
            inputAxii.Add(0f);
            int index = inputAxii.Count - 1;
            DebugHandler.CheckAndDebugLog(DebugHandler.Input(), $"Command added axis to list at {index}", this);
            TargetInputBound(sender, index);
        }

        [TargetRpc]
        private void TargetInputBound(NetworkConnection target, int index)
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
        private void CmdSetAxisInput(int index, float value)
        {
            SetAxisInput(index, value);
            DebugHandler.CheckAndDebugLog(DebugHandler.Input(), $"Called command axis input {index} to {value}", this);
        }

        public float GetInputAxis(int index)
        {
            return isServer || !IsClientWithLocalControls() ? inputAxii[index] : inputAxiiLocal[index];
        }
    }
}