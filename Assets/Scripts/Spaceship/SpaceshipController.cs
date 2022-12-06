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
        virtualCamera.gameObject.SetActive(false);
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
            networkController.InputAxiiHandler.BindInput(controls.Player.Pitch);
            networkController.InputAxiiHandler.BindInput(controls.Player.Yaw);
            networkController.InputAxiiHandler.BindInput(controls.Player.Roll);
            networkController.InputAxiiHandler.BindInput(controls.Player.ForwardThrust);
            networkController.InputAxiiHandler.BindInput(controls.Player.HorizontalThrust);
            networkController.InputAxiiHandler.BindInput(controls.Player.VerticalThrust);
            networkController.InputAxiiHandler.FinalizeInputMapping();
            networkController.InputButtonsHandler.BindInput(controls.Player.Stop);
            networkController.InputButtonsHandler.BindInput(controls.Player.SnapMove);
            networkController.InputButtonsHandler.FinalizeInputMapping();

            virtualCamera.gameObject.SetActive(true);
            DebugHandler.CheckAndDebugLog(DebugHandler.Input(), "Spaceship bound actions.", this);
        }
    }
}
