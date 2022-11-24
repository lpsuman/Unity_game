using System;
using Bluaniman.SpaceGame.Player;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpaceshipController : AbstractNetworkController
{
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

    [SyncVar]
    private Vector3 rotationInput = Vector3.zero;
    [SyncVar]
    private Vector3 thrustInput = Vector3.zero;

    protected override void OnStartClientWithAuthority()
    {
        if (isOwned)
        {
            BindToInputAction<float>(Controls.Player.Pitch, SetPitchAxisInput, ResetPitchAxisInput);
            BindToInputAction<float>(Controls.Player.Yaw, SetYawAxisInput, ResetYawAxisInput);
            BindToInputAction<float>(Controls.Player.Roll, SetRollAxisInput, ResetRollAxisInput);
            BindToInputAction<float>(Controls.Player.Thrust, SetForwardThrustInput, ResetForwardThrustInput);
        }
    }

    private void SetPitchAxisInput(float pitch) => rotationInput.x = pitch;

    private void ResetPitchAxisInput() => rotationInput.x = 0f;

    private void SetYawAxisInput(float pitch) => rotationInput.y = pitch;

    private void ResetYawAxisInput() => rotationInput.y = 0f;

    private void SetRollAxisInput(float pitch) => rotationInput.z = pitch;

    private void ResetRollAxisInput() => rotationInput.z = 0f;

    private void SetForwardThrustInput(float pitch) => thrustInput.z = pitch;

    private void ResetForwardThrustInput() => thrustInput.z = 0f;

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
        if (rotationInput.x != 0.0f)
        {
            rb.AddRelativeTorque(rotationInput.x * deltaTimeRotationThrust * Vector3.left);
        }
        if (rotationInput.y != 0.0f)
        {
            rb.AddRelativeTorque(rotationInput.y * deltaTimeRotationThrust * Vector3.up);
        }
        if (rotationInput.z != 0.0f)
        {
            rb.AddRelativeTorque(rotationInput.z * deltaTimeRotationThrust * Vector3.forward);
        }

        bool isAxisInputPresent = rotationInput.magnitude != 0f;
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
