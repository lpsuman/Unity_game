using System;
using Bluaniman.SpaceGame.Debugging;
using Bluaniman.SpaceGame.Input;
using Bluaniman.SpaceGame.Network;
using Bluaniman.SpaceGame.Player;
using Bluaniman.SpaceGame.Spaceship;
using Cinemachine;
using Mirror;
using twoloop;
using UnityEngine;
using static MovementData;

public class SpaceshipController : MyNetworkBehavior
{
    [SerializeField] private MovementController movementController = null;
    [SerializeField] private NetworkPlayerController networkController = null;
    [SerializeField] private CinemachineVirtualCamera virtualCamera = null;

    public void Start()
    {
        movementController.OnMovementSetupDone += HandleMovementSetupDone;
        networkController.OnControlsEnabled += HandleControlsEnabled;
    }

    public void OnDestroy()
    {
        movementController.OnMovementSetupDone -= HandleMovementSetupDone;
        networkController.OnControlsEnabled -= HandleControlsEnabled;
    }

    private void HandleMovementSetupDone()
    {

    }

    private void HandleControlsEnabled()
    {
        if (IsClientWithOwnership())
        {
            Controls controls = networkController.Controls;
            networkController.BindInputAction(controls.Player.Pitch);
            networkController.BindInputAction(controls.Player.Yaw);
            networkController.BindInputAction(controls.Player.Roll);
            networkController.BindInputAction(controls.Player.ForwardThrust);
            networkController.BindInputAction(controls.Player.HorizontalThrust);
            networkController.BindInputAction(controls.Player.VerticalThrust);
            virtualCamera.gameObject.SetActive(true);
            DebugHandler.NetworkLog("Spaceship bound actions.", this);
        }
        else
        {
            virtualCamera.gameObject.SetActive(false);
        }
    }
}
