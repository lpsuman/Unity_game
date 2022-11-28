using System;
using Bluaniman.SpaceGame.Player;
using Cinemachine;
using Mirror;
using twoloop;
using UnityEngine;

public class SpaceshipController : AbstractNetworkController
{
    [SerializeField] private OSNetTransform osNetTransform = null;
    [SerializeField] private OSNetRigidbody osNetRb = null;
    [SerializeField] private CinemachineVirtualCamera virtualCamera = null;
    public SpaceshipData spaceshipData;
    private Rigidbody rb;

    [Header("Rotation PID")]
    private PID angularPID;
    [SerializeField] private float angularPIDstartingP = 10.0f;
    [SerializeField] private float angularPIDstartingI = 0.0f;
    [SerializeField] private float angularPIDstartingD = 0.1f;
    private bool isStoppingRotation;
    private bool wasRotationAxisInputPresentLastUpdate;
    //showing in editor for testing purposes
    [SerializeField] private float angularVelocityError;
    [SerializeField] private float angularVelocityCorrection;

    [Header("Movement PID")]
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
    [SerializeField] private float thrustInput = 0f;

    protected override void OnStartClientWithAuthority()
    {
        if (isOwned)
        {
            BindToInputAction<float>(Controls.Player.Pitch, SetPitchInput, CmdSetPitchAxisInput, ResetPitchAxisInput, CmdResetPitchAxisInput);
            BindToInputAction<float>(Controls.Player.Yaw, SetYawAxisInput, CmdSetYawAxisInput, ResetYawAxisInput, CmdResetYawAxisInput);
            BindToInputAction<float>(Controls.Player.Roll, SetRollAxisInput, CmdSetRollAxisInput, ResetRollAxisInput, CmdResetRollAxisInput);
            BindToInputAction<float>(Controls.Player.Thrust, SetForwardThrustInput, CmdSetForwardThrustInput, ResetForwardThrustInput, CmdResetForwardThrustInput);
        }
        virtualCamera.gameObject.SetActive(isOwned);
    }

    private void SetPitchInput(float pitch) => pitchInput = pitch;
    [Command]
    private void CmdSetPitchAxisInput(float pitch) => SetPitchInput(pitch);

    private void ResetPitchAxisInput() => pitchInput = 0f;
    [Command]
    private void CmdResetPitchAxisInput() => ResetPitchAxisInput();

    private void SetYawAxisInput(float pitch) => yawInput = pitch;
    [Command]
    private void CmdSetYawAxisInput(float pitch) => SetYawAxisInput(pitch);

    private void ResetYawAxisInput() => yawInput = 0f;
    [Command]
    private void CmdResetYawAxisInput() => ResetYawAxisInput();

    private void SetRollAxisInput(float pitch) => rollInput = pitch;
    [Command]
    private void CmdSetRollAxisInput(float pitch) => SetRollAxisInput(pitch);

    private void ResetRollAxisInput() => rollInput = 0f;
    [Command]
    private void CmdResetRollAxisInput() => ResetRollAxisInput();

    private void SetForwardThrustInput(float pitch) => thrustInput = pitch;
    [Command]
    private void CmdSetForwardThrustInput(float pitch) => SetForwardThrustInput(pitch);

    private void ResetForwardThrustInput() => thrustInput = 0f;
    [Command]
    private void CmdResetForwardThrustInput() => ResetForwardThrustInput();

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = spaceshipData.mass;
        rb.useGravity = false;
        if (isServer || (isClientOnly && useAuthorityPhysics))
        {
            DoSetup();
        }
        osNetTransform.clientAuthority = useAuthorityPhysics;
        osNetRb.clientAuthority = useAuthorityPhysics;
        osNetRb.serverOnlyPhysics = !useAuthorityPhysics;
    }

    private void DoSetup()
    {
        rb.isKinematic = false;
        angularPID = new PID(angularPIDstartingP, angularPIDstartingI, angularPIDstartingD);
        isStoppingRotation = false;
        wasRotationAxisInputPresentLastUpdate = false;
        movementPID = new PID(movementPIDstartingP, movementPIDstartingI, movementPIDstartingD);
        isStoppingMovement = false;
    }

    private void FixedUpdate()
    {
        if (isClientOnly && (!useAuthorityPhysics || !isOwned)) { return; }
        Turn();
        Strafe();
        Thrust();
    }

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

    private void Strafe()
    {
        // TODO
    }

    private void Thrust()
    {
        if (thrustInput < 0f)
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
            if (thrustInput != 0.0f)
            {
                rb.AddRelativeForce(thrustInput * deltaTimeForwardThrust * Vector3.forward);
            }
        }
        
    }
}
