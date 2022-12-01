using System;
using Bluaniman.SpaceGame.Debugging;
using Bluaniman.SpaceGame.Player;
using Cinemachine;
using Mirror;
using twoloop;
using UnityEngine;

public class SpaceshipController : AbstractNetworkController
{
    private const float secondsBeforeStarting = 3f;
    private bool isSetupDone = false;

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

    public void Start()
    {
        DebugHandler.NetworkLog("Spaceship start.", this);
        if (isClient && isOwned)
        {
            BindInputAction(Controls.Player.Pitch);
            BindInputAction(Controls.Player.Yaw);
            BindInputAction(Controls.Player.Roll);
            BindInputAction(Controls.Player.ForwardThrust);
            BindInputAction(Controls.Player.HorizontalThrust);
            BindInputAction(Controls.Player.VerticalThrust);
            virtualCamera.gameObject.SetActive(true);
            DebugHandler.NetworkLog("Spaceship bound actions.", this);
        } else
        {
            virtualCamera.gameObject.SetActive(false);
        }
        Invoke(nameof(DelayedStart), secondsBeforeStarting);
    }

    private void DelayedStart()
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

        //osNetTransform.clientAuthority = false;
        //osNetRb.clientAuthority = false;
        //osNetRb.serverOnlyPhysics = true;
        //rb.isKinematic = !useAuthorityPhysics;

        isSetupDone = true;
        DebugHandler.NetworkLog("Spaceship setup done.", this);
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
        if (!isSetupDone || isClientOnly && (!useAuthorityPhysics || !isOwned)) { return; }
        float deltaTimeTotalThrust = spaceshipData.totalThrust * Time.fixedDeltaTime;
        Turn(deltaTimeTotalThrust);
        Strafe(deltaTimeTotalThrust);
        Thrust(deltaTimeTotalThrust);
    }

    private void Turn(float deltaTimeTotalThrust)
    {
        float deltaTimeRotationThrust = deltaTimeTotalThrust * spaceshipData.rotationThrustRatio;
        ApplyMovement(ApplyTorque, inputAxii[0], spaceshipData.pitchThrust, Vector3.right, deltaTimeRotationThrust);
        ApplyMovement(ApplyTorque, inputAxii[1], spaceshipData.yawThrust, Vector3.up, deltaTimeRotationThrust);
        ApplyMovement(ApplyTorque, inputAxii[2], spaceshipData.rollThrust, Vector3.forward, deltaTimeRotationThrust);

        bool isAxisInputPresent = inputAxii[0] != 0f || inputAxii[1] != 0f || inputAxii[2] != 0f;
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

    private void Strafe(float deltaTimeTotalThrust)
    {
        float deltaTimeStrafeThrust = deltaTimeTotalThrust * spaceshipData.movementThrustRatio;
        ApplyMovement(ApplyForce, inputAxii[4], spaceshipData.horizontalThrust, Vector3.right, deltaTimeStrafeThrust);
        ApplyMovement(ApplyForce, inputAxii[5], spaceshipData.verticalThrust, Vector3.up, deltaTimeStrafeThrust);
    }

    private void Thrust(float deltaTimeTotalThrust)
    {
        float forward = inputAxii[3];
        if (forward < 0f)
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

        float deltaTimeForwardThrust = deltaTimeTotalThrust * spaceshipData.movementThrustRatio;
        if (isStoppingMovement)
        {
            movementError = rb.velocity.magnitude;
            movementCorrection = movementPID.Update(movementError, Time.fixedDeltaTime);
            rb.AddForce(movementCorrection * deltaTimeForwardThrust * -rb.velocity.normalized);
        }
        else
        {
            if (forward != 0.0f)
            {
                rb.AddRelativeForce(forward * deltaTimeForwardThrust * Vector3.forward);
            }
        }
    }
    private void ApplyMovement(Action<Vector3> thrustAction, float axisInput, Vector3 thrustRatio, Vector3 direction, float deltaTimeThrust)
    {
        if (axisInput != 0f)
        {
            float totalThrust = deltaTimeThrust * thrustRatio.x * (axisInput > 0f ? thrustRatio.y : thrustRatio.z);
            thrustAction.Invoke(axisInput * totalThrust * direction);
        }
    }

    private void ApplyTorque(Vector3 vec) => rb.AddRelativeTorque(vec);

    private void ApplyForce(Vector3 vec) => rb.AddRelativeForce(vec);
}
