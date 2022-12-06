using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Bluaniman.SpaceGame.Debugging;
using Bluaniman.SpaceGame.Network;
using Bluaniman.SpaceGame.Input;
using Bluaniman.SpaceGame.General;

namespace Bluaniman.SpaceGame.Player
{
    [Serializable]
    public class NetworkPlayerController : MyNetworkBehavior, IInputController
    {
        public InputSyncHandler<float> InputAxiiHandler { get; private set; }
        public InputSyncHandler<bool> InputButtonsHandler { get; private set; }

        public event Action OnControlsEnabled;
        public event Action OnControlsSetupDone;
        public bool IsReady { get; set; }
        public event Action OnReady;

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


        #region Setup
        public void Start()
        {
            DebugHandler.CheckAndDebugLog(DebugHandler.Input(), "Controller setup start.", this);
            if (isServer || IsClientWithLocalControls()) {
                InputAxiiHandler = new InputSyncHandler<float>("axis", this);
                InputButtonsHandler = new InputSyncHandler<bool>("button", this);
                ((IReadiable)InputAxiiHandler).DoWhenReady(() => HandleInputHandlerFinalized(InputButtonsHandler));
                ((IReadiable)InputButtonsHandler).DoWhenReady(() => HandleInputHandlerFinalized(InputAxiiHandler));
            }
            if (IsClientWithOwnership())
            {
                Controls.Enable();
                DebugHandler.CheckAndDebugLog(DebugHandler.Input(), "Controls enabled.", this);
                OnControlsEnabled?.Invoke();
            }
            else
            {
                Controls.Disable();
            }
            DebugHandler.CheckAndDebugLog(DebugHandler.Input(), "Controller setup done.", this);
            OnControlsSetupDone?.Invoke();
        }

        private void HandleInputHandlerFinalized(AbstractInputSyncHandler other)
        {
            if (other.IsReady)
            {
                IsReady = true;
                OnReady?.Invoke();
                OnReady = null;
            }
            else
            {
                DebugHandler.CheckAndDebugLog(DebugHandler.Input(), "Other input handler is not ready.");
            }
        }

        public IInputProvider<float> GetInputAxisProvider() => InputAxiiHandler;

        public IInputProvider<bool> GetInputButtonsProvider() => InputButtonsHandler;
        #endregion
    }
}