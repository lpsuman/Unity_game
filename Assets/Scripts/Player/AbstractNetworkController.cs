using UnityEngine;
using Mirror;
using Bluaniman.SpaceGame.Input;
using UnityEngine.InputSystem;
using System;

namespace Bluaniman.SpaceGame.Player
{
	public abstract class AbstractNetworkController : NetworkBehaviour
	{
        public readonly SyncList<float> inputAxii = new();
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
            }
            else
            {
                Controls.Disable();
            }
        }

        protected void BindToInputAction(InputAction inputAction)
        {
            inputAxii.Add(0f);
            int index = inputAxii.Count - 1;
            inputAction.performed += ctx => CmdSetAxisInput(index, ctx.ReadValue<float>());
            inputAction.canceled += ctx => CmdSetAxisInput(index, 0f);
            if (useAuthorityPhysics)
            {
                inputAction.performed += ctx => SetAxisInput(index, ctx.ReadValue<float>());
                inputAction.canceled += ctx => SetAxisInput(index, 0f);
            }
        }

        public void SetAxisInput(int index, float value)
        {
            inputAxii[index] = value;
        }

        [Command]
        public void CmdSetAxisInput(int index, float value)
        {
            SetAxisInput(index, value);
        }
    }
}