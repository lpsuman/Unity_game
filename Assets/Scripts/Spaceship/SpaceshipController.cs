using System;
using Bluaniman.SpaceGame.Player;
using Cinemachine;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpaceshipController : AbstractNetworkController
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera = null;

    public SpaceshipData spaceshipData;
    private Rigidbody rb;

    private PID angularPID;
    [SerializeField] private float angularPIDstartingP = 10.0f;
    [SerializeField] private float angularPIDstartingI = 0.0f;
    [SerializeField] private float angularPIDstartingD = 0.1f;
    private bool isStoppingRotation;
    private bool wasRotationAxisInputPresentLastUpdate;
    //showing in editor for testing purposes
    [SerializeField] private float angularVelocityError;
    [SerializeField] private float angularVelocityCorrection;

    private PID movementPID;
    [SerializeField] private float movementPIDstartingP = 1.0f;
    [SerializeField] private float movementPIDstartingI = 0.0f;
    [SerializeField] private float movementPIDstartingD = 0.01f;
    private bool isStoppingMovement;
    //showing in editor for testing purposes
    [SerializeField] private float movementError;
    [SerializeField] private float movementCorrection;

    [Header("Input debug")]
    [SyncVar]
    [SerializeField] private float pitchInput = 0f;
    [SyncVar]
    [SerializeField] private float yawInput = 0f;
    [SyncVar]
    [SerializeField] private float rollInput = 0f;
    [SyncVar]
    [SerializeField] private Vector3 thrustInput = Vector3.zero;

    protected override void OnStartClientWithAuthority()
    {
        if (isOwned)
        {
            Debug.Log("Bind them");
            BindToInputAction<float>(Controls.Player.Pitch, CmdSetPitchAxisInput, CmdResetPitchAxisInput);
            BindToInputAction<float>(Controls.Player.Yaw, CmdSetYawAxisInput, CmdResetYawAxisInput);
            BindToInputAction<float>(Controls.Player.Roll, CmdSetRollAxisInput, CmdResetRollAxisInput);
            BindToInputAction<float>(Controls.Player.Thrust, CmdSetForwardThrustInput, CmdResetForwardThrustInput);
        }
        virtualCamera.gameObject.SetActive(isOwned);
    }

    [Command]
    private void CmdSetPitchAxisInput(float pitch) => pitchInput = pitch;

    [Command]
    private void CmdResetPitchAxisInput() => pitchInput = 0f;

    [Command]
    private void CmdSetYawAxisInput(float pitch) => yawInput = pitch;

    [Command]
    private void CmdResetYawAxisInput() => yawInput = 0f;

    [Command]
    private void CmdSetRollAxisInput(float pitch) => rollInput = pitch;

    [Command]
    private void CmdResetRollAxisInput() => rollInput = 0f;

    [Command]
    private void CmdSetForwardThrustInput(float pitch) => thrustInput.z = pitch;

    [Command]
    private void CmdResetForwardThrustInput() => thrustInput.z = 0f;

    public override void OnStartServer()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = spaceshipData.mass;
        rb.useGravity = false;
        angularPID = new PID(angularPIDstartingP, angularPIDstartingI, angularPIDstartingD);
        isStoppingRotation = false;
        wasRotationAxisInputPresentLastUpdate = false;
        movementPID = new PID(movementPIDstartingP, movementPIDstartingI, movementPIDstartingD);
        isStoppingMovement = false;
    }

    [ServerCallback]
    private void FixedUpdate()
    {
        Turn();
        Strafe();
        Thrust();
    }

    [Server]
    private void Turn()
    {
        float deltaTimeRotationThrust = spaceshipData.rotationThrust * Time.fixedDeltaTime;
        if (pitchInput != 0.0f)
        {
            rb.AddRelativeTorque(pitchInput * deltaTimeRotationThrust * Vector3.right);
        }
        if (yawInput != 0.0f)
        {
            rb.AddRelativeTorque(yawInput * deltaTimeRotationThrust * Vector3.up);
        }
        if (rollInput != 0.0f)
        {
            rb.AddRelativeTorque(rollInput * deltaTimeRotationThrust * Vector3.forward);
        }

        bool isAxisInputPresent = pitchInput != 0f || yawInput != 0f || rollInput != 0f;
        if (wasRotationAxisInputPresentLastUpdate && !isAxisInputPresent)
        {
            angularPID.Reset();
            angularPID.SetFactors(angularPIDstartingP, angularPIDstartingI, angularPIDstartingD);
            isStoppingRotation = true;
        }
        else if (!wasRotationAxisInputPresentLastUpdate && isAxisInputPresent)
        {
            isStoppingRotation = false;
        }
        if (isStoppingRotation)
        {
            angularVelocityError = rb.angularVelocity.magnitude;
            angularVelocityCorrection = angularPID.Update(angularVelocityError, Time.fixedDeltaTime);
            rb.AddTorque(angularVelocityCorrection * deltaTimeRotationThrust * -rb.angularVelocity.normalized);
        }
        wasRotationAxisInputPresentLastUpdate = isAxisInputPresent;
    }

    [Server]
    private void Strafe()
    {
        // TODO
    }

    [Server]
    private void Thrust()
    {
        if (thrustInput.z < 0f)
        {
            if (!isStoppingMovement)
            {
                movementPID.Reset();
                movementPID.SetFactors(movementPIDstartingP, movementPIDstartingI, movementPIDstartingD);
                isStoppingMovement = true;
            }
        }
        else
        {
            isStoppingMovement = false;
        }

        float deltaTimeForwardThrust = spaceshipData.forwardThrust * Time.fixedDeltaTime;
        if (isStoppingMovement)
        {
            movementError = rb.velocity.magnitude;
            movementCorrection = movementPID.Update(movementError, Time.fixedDeltaTime);
            rb.AddForce(movementCorrection * deltaTimeForwardThrust * -rb.velocity.normalized);
        }
        else
        {
            //if (Input.GetKey(toggleStrafeKeyID))
            //{
            //    float deltaTimeStrafeThrust = spaceshipData.strafeThrust * Time.fixedDeltaTime;
            //    if (inputAxisVertical != 0.0f)
            //    {
            //        rb.AddRelativeForce(inputAxisVertical * deltaTimeStrafeThrust * Vector3.right);
            //    }
            //    if (inputAxisHorizontal != 0.0f)
            //    {
            //        rb.AddRelativeForce(inputAxisHorizontal * deltaTimeStrafeThrust * Vector3.up);
            //    }
            //}
            if (thrustInput.z != 0.0f)
            {
                rb.AddRelativeForce(thrustInput.z * deltaTimeForwardThrust * Vector3.forward);
            }
        }
        
    }
}
