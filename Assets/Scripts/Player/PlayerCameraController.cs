using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cinemachine;

namespace Bluaniman.SpaceGame.Player
{
    public class PlayerCameraController : AbstractNetworkController
    {
        [Header("Camera")]
        [SerializeField] private Vector2 maxFollowOffset = new(-1f, 6f);
        [SerializeField] private Vector2 cameraVelocity = new(4f, 0.25f);
        [SerializeField] private CinemachineVirtualCamera virtualCamera = null;

        private CinemachineTransposer transposer;

        protected override void OnStartClientWithAuthority()
        {
            if (isOwned)
            {
                //Controls.Player.Look.performed += ctx => Look(ctx.ReadValue<Vector2>());
                transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            }
            virtualCamera.gameObject.SetActive(isOwned);
        }

        private void Look(Vector2 lookAxis)
        {
            transposer.m_FollowOffset.y = Mathf.Clamp(
                transposer.m_FollowOffset.y - (lookAxis.y * cameraVelocity.y * Time.deltaTime),
                maxFollowOffset.x,
                maxFollowOffset.y);
        }
    }
}
