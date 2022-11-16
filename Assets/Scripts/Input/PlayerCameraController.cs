using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cinemachine;

namespace Bluaniman.SpaceGame.Input
{
    public class PlayerCameraController : NetworkBehaviour
    {
        [Header("Camera")]
        [SerializeField] private Vector2 maxFollowOffset = new(-1f, 6f);
        [SerializeField] private Vector2 cameraVelocity = new(4f, 0.25f);
        [SerializeField] private Transform playerTransform = null;
        [SerializeField] private CinemachineVirtualCamera virtualCamera = null;

        private Controls controls;
        private Controls Controls
        {
            get
            {
                return controls ??= new Controls();
            }
        }
        private CinemachineTransposer transposer;

        public override void OnStartClient()
        {
            if (hasAuthority)
            {
                Controls.Player.Look.performed += ctx => Look(ctx.ReadValue<Vector2>());
                transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            }
            
            virtualCamera.gameObject.SetActive(hasAuthority);
            enabled = hasAuthority;
        }

        [ClientCallback]
        private void OnEnable() => Controls.Enable();

        [ClientCallback]
        private void OnDisable() => Controls.Disable();

        private void Look(Vector2 lookAxis)
        {
            Debug.Log("Looking");
            transposer.m_FollowOffset.y = Mathf.Clamp(
                transposer.m_FollowOffset.y - (lookAxis.y * cameraVelocity.y * Time.deltaTime),
                maxFollowOffset.x,
                maxFollowOffset.y);

            playerTransform.Rotate(0f, lookAxis.x * cameraVelocity.x * Time.deltaTime, 0f);
        }
    }
}
