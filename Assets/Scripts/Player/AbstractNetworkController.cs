using System;
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
                if (controls == null)
                {
                    controls = new Controls();
                    Debug.Log("New controls baby");
                }
                return controls;
            }
        }

        protected abstract void OnStartClientWithAuthority();
        public override void OnStartClient()
        {
            OnStartClientWithAuthority();
            if (isOwned)
            {
                Controls.Enable();
            } else
            {
                Controls.Disable();
            }
        }

        protected void BindToInputAction<T>(InputAction inputAction, Action<T> performedAction, Action canceledAction) where T : struct
        {
            inputAction.performed += ctx => performedAction(ctx.ReadValue<T>());
            inputAction.canceled += ctx => canceledAction();
        }
    }
}