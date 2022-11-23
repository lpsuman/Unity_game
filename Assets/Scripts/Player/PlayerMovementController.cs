using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Bluaniman.SpaceGame.Input;

namespace Bluaniman.SpaceGame.Player
{
	public class PlayerMovementController : AbstractNetworkController
    {
		[SerializeField] private float movementSpeed = 5f;
		[SerializeField] private CharacterController characterController = null;

		private Vector2 previousInput;

        protected override void OnStartClientWithAuthority()
        {
            if (hasAuthority)
            {
                Controls.Player.Move.performed += ctx => SetMovement(ctx.ReadValue<Vector2>());
                Controls.Player.Move.canceled += ctx => ResetMovement();
            }
        }

        [Client]
        private void SetMovement(Vector2 movement) => previousInput = movement;

        [Client]
        private void ResetMovement() => previousInput = Vector2.zero;

        [ClientCallback]
        private void Update() => Move();

        [Client]
        private void Move()
        {
            Vector3 forward = characterController.transform.forward;
            Vector3 right = characterController.transform.right;
            forward.y = 0f;
            right.y = 0f;
            Vector3 movement = forward.normalized * previousInput.y + right.normalized * previousInput.x;
            characterController.Move(movement * movementSpeed * Time.deltaTime);
        }
    }
}