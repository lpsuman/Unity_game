using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Bluaniman.SpaceGame.Input;
using UnityEngine.InputSystem;

namespace Bluaniman.SpaceGame.Player
{
	public abstract class AbstractNetworkController : NetworkBehaviour
	{
        private Controls controls;
        protected Controls Controls
        {
            get
            {
                return controls ??= new Controls();
            }
        }

        protected abstract void OnStartClientWithAuthority();
        public override void OnStartClient()
        {
            OnStartClientWithAuthority();
            enabled = hasAuthority;
        }

        protected void BindToInputAction<T>(InputAction inputAction, Action<T> performedAction, Action canceledAction) where T : struct
        {
            inputAction.performed += ctx => performedAction(ctx.ReadValue<T>());
            inputAction.canceled += ctx => canceledAction();
        }

        [ClientCallback]
        private void OnEnable() => Controls.Enable();

        [ClientCallback]
        private void OnDisable() => Controls.Disable();
    }
}