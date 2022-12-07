using System;
using Bluaniman.SpaceGame.Debugging;
using Bluaniman.SpaceGame.Input;
using Bluaniman.SpaceGame.Network;
using Bluaniman.SpaceGame.Player;
using Cinemachine;
using Mirror;
using twoloop;
using UnityEngine;
using static Bluaniman.SpaceGame.Player.IMovementController;

public class SpaceshipController : MyNetworkBehavior
{
    private IMovementController movementController = null;
    private IInputController networkController = null;
    [SerializeField] private CinemachineVirtualCamera virtualCamera = null;

    public void Start()
    {
        movementController = GetComponent<IMovementController>();
        movementController.DoWhenReady(HandleMovementSetupDone);
        networkController = GetComponent<IInputController>();
        networkController.OnControlsEnabled += HandleControlsEnabled;
        virtualCamera.gameObject.SetActive(false);
    }

    public void OnDestroy()
    {
        networkController.OnControlsEnabled -= HandleControlsEnabled;
    }

    private void HandleMovementSetupDone()
    {

    }

    private void HandleControlsEnabled()
    {
        networkController.OnControlsEnabled -= HandleControlsEnabled;
        if (IsClientWithOwnership())
        {
            Controls controls = networkController.Controls;
            IInputProvider<float> inputAxiiHandler = networkController.GetInputAxisProvider();
            movementController.SetBindingIndex(MovementControllerInputID.Pitch, inputAxiiHandler.BindInput(controls.Player.Pitch));
            movementController.SetBindingIndex(MovementControllerInputID.Yaw, inputAxiiHandler.BindInput(controls.Player.Yaw));
            movementController.SetBindingIndex(MovementControllerInputID.Roll, inputAxiiHandler.BindInput(controls.Player.Roll));
            movementController.SetBindingIndex(MovementControllerInputID.ForwardThrust, inputAxiiHandler.BindInput(controls.Player.ForwardThrust));
            movementController.SetBindingIndex(MovementControllerInputID.HorizontalThrust, inputAxiiHandler.BindInput(controls.Player.HorizontalThrust));
            movementController.SetBindingIndex(MovementControllerInputID.VerticalThrust, inputAxiiHandler.BindInput(controls.Player.VerticalThrust));
            inputAxiiHandler.BindInput(controls.Player.LookX);
            inputAxiiHandler.BindInput(controls.Player.LookY);
            inputAxiiHandler.FinalizeInputMapping();

            IInputProvider<bool> inputButtonsHandler = networkController.GetInputButtonsProvider();
            movementController.SetBindingIndex(MovementControllerInputID.Stop, inputButtonsHandler.BindInput(controls.Player.Stop));
            movementController.SetBindingIndex(MovementControllerInputID.SnapMove, inputButtonsHandler.BindInput(controls.Player.SnapMove));
            inputButtonsHandler.BindInput(controls.Player.FreeCamera);
            inputButtonsHandler.FinalizeInputMapping();

            virtualCamera.gameObject.SetActive(true);
            DebugHandler.CheckAndDebugLog(DebugHandler.Input(), "Spaceship bound actions.", this);
        }
    }
}
