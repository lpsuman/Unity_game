using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Bluaniman.SpaceGame.Debugging;
using Bluaniman.SpaceGame.Network;
using UnityEngine.InputSystem;

namespace Bluaniman.SpaceGame.Player
{
    public class InputSyncHandler<T> : AbstractInputSyncHandler, IInputProvider<T> where T : struct, IEquatable<T>
    {
        private readonly SyncList<T> syncList = new();
        private readonly List<T> localList;

        public override bool IsReady { get; set; }
        public override event Action OnReady;

        public InputSyncHandler(string name, MyNetworkBehavior debugNetworkContext)
        {
            debugData = new DebugHandler.DebugNameAndNetContext(debugNetworkContext, name);
            if (debugData.debugNetContext.IsClientWithLocalControls())
            {
                localList = new();
            }

            if (DebugHandler.ShouldDebug(DebugHandler.Input()))
            {
                syncList.Callback += (op, index, oldItem, newItem)
                    => DebugHandler.NetworkLog($"Input {debugData.debugName} at {index} changed to {newItem}.", debugData);
            }
        }

        [Client]
        public int BindInput(InputAction inputAction)
        {
            int currIndex = inputActions.Count;
            inputActions.Add(inputAction);
            //DebugHandler.CheckAndDebugLog(DebugHandler.Input(), $"Client added input action at {inputActions.Count - 1}", debugData);
            CmdAddToInputActionSyncList();
            return currIndex;
        }

        [Client]
        public void FinalizeInputMapping()
        {
            if (IsReady)
            {
                DebugHandler.NetworkLog(InputMappingAlreadyFinalizedExcMsg);
                throw new InvalidOperationException(InputMappingAlreadyFinalizedExcMsg);
            }
            if (syncList.Count == 0)
            {
                DebugHandler.NetworkLog(InputMappingEmptyExcMsg);
                throw new InvalidOperationException(InputMappingEmptyExcMsg);
            }
            IsReady = true;
            OnReady?.Invoke();
            OnReady = null;
            DebugHandler.CheckAndDebugLog(DebugHandler.Input(), $"Input handler for {debugData.debugName} is ready.");
        }

        [Command]
        private void CmdAddToInputActionSyncList(NetworkConnectionToClient sender = null)
        {
            syncList.Add(default);
            int index = syncList.Count - 1;
            //DebugHandler.CheckAndDebugLog(DebugHandler.Input(), $"Command added {debugData.debugName} to list at {index}", debugData);
            TargetInputBound(sender, index);
        }

        [TargetRpc]
        private void TargetInputBound(NetworkConnection target, int index)
        {
            InputAction inputAction = inputActions[index];
            AddInputActionCallback(inputAction, index, CmdSetInput);
            if (debugData.debugNetContext.IsClientWithLocalControls())
            {
                localList.Add(default);
                AddInputActionCallback(inputAction, index, SetInput);
            }
            //DebugHandler.CheckAndDebugLog(DebugHandler.Input(), $"Input action RPC done for index {index}", debugData);
        }

        private void AddInputActionCallback(InputAction inputAction, int index, Action<int, T> setInputFunc)
        {
            if (typeof(T) == typeof(bool))
            {
                // button input is a float and I find that annoying
                inputAction.performed += ctx => setInputFunc.Invoke(index, (T)Convert.ChangeType(ctx.ReadValueAsButton(), typeof(T)));
            }
            else
            {
                inputAction.performed += ctx => setInputFunc.Invoke(index, ctx.ReadValue<T>());
            }
            inputAction.canceled += ctx => setInputFunc.Invoke(index, default);
        }

        public void SetInput(int index, T value)
        {
            if (ShouldUseNonLocal())
            {
                syncList[index] = value;
            }
            else
            {
                localList[index] = value;
            }
            DebugHandler.CheckAndDebugLog(DebugHandler.Input(), $"Set axis input {index} to {value}", debugData);
        }

        [Command]
        private void CmdSetInput(int index, T value)
        {
            SetInput(index, value);
            DebugHandler.CheckAndDebugLog(DebugHandler.Input(), $"Called command axis input {index} to {value}", debugData);
        }

        public T GetInput(int index)
        {
            if (!IsReady)
            {
                DebugHandler.NetworkLog(InputMappingNotFinalizedExcMsg);
                throw new InvalidOperationException(InputMappingNotFinalizedExcMsg);
            }
            return ShouldUseNonLocal() ? syncList[index] : localList[index];
        }

        private bool ShouldUseNonLocal()
        {
            return debugData.debugNetContext.isServer || !debugData.debugNetContext.IsClientWithLocalControls();
        }
    }
}