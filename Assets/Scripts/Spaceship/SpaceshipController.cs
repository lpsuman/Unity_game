using System;
using Bluaniman.SpaceGame.Player;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpaceshipController : AbstractNetworkController
{
    public SpaceshipData spaceshipData;
    private Rigidbody rb;
    [SerializeField] private KeyCode toggleStrafeKeyID = KeyCode.LeftControl;
    [SerializeField] private KeyCode stopMovementKeyID = KeyCode.LeftAlt;

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

    private float pitchAxisInput;
    private float yawAxisInput;
    private float rollAxisInput;
    private float thrustAxisInput;

    protected override void OnStartClientWithAuthority()
    {
        if (hasAuthority)
        {
            BindToInputAction<float>(Controls.Player.Pitch, CmdSetPitchAxisInput, CmdResetPitchAxisInput);
        }
    }

    [Command]
    private void CmdSetPitchAxisInput(float pitch) => pitchAxisInput = pitch;

    [Command]
    private void CmdResetPitchAxisInput() => pitchAxisInput = 0f;

    private void Start()
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

    private void Update()
    {
        Debug.DrawLine(transform.position, transform.position + rb.angularVelocity, Color.cyan);
    }

    private void FixedUpdate()
    {
        if (!useNewInput)
        {
            readInputAxii();
        }
        Turn();
        Strafe();
        Thrust();
    }

    private void readInputAxii()
    {
        inputAxisVertical = Input.GetAxis("Vertical");
        inputAxisHorizontal = Input.GetAxis("Horizontal");
        inputAxisRotate = Input.GetAxis("Rotate");
        inputAxisThrust = Input.GetAxis("Thrust");
    }

    private void Turn()
    {
        float deltaTimeRotationThrust = spaceshipData.rotationThrust * Time.fixedDeltaTime;
        bool isStrafingEnabled = Input.GetKey(toggleStrafeKeyID);
        if (!isStrafingEnabled)
        {
            if (inputAxisVertical != 0.0f)
            {
                rb.AddRelativeTorque(inputAxisVertical * deltaTimeRotationThrust * Vector3.left);
            }
            if (inputAxisHorizontal != 0.0f)
            {
                rb.AddRelativeTorque(inputAxisHorizontal * deltaTimeRotationThrust * Vector3.up);
            }
        }
        if (inputAxisRotate != 0.0f)
        {
            rb.AddRelativeTorque(inputAxisRotate * deltaTimeRotationThrust * Vector3.forward);
        }

        bool isAxisInputPresent = (!isStrafingEnabled && (inputAxisVertical != 0.0f || inputAxisHorizontal != 0.0f)) || inputAxisRotate != 0.0f;
        if (wasRotationAxisInputPresentLastUpdate && !isAxisInputPresent)
        {
            angularPID.Reset();
            angularPID.SetFactors(angularPIDstartingP, angularPIDstartingI, angularPIDstartingD);
            //Debug.Log(string.Format("new pid with: {0} {1} {2}", startingP, startingI, startingD));
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
        if (Input.GetKey(stopMovementKeyID))
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
            if (Input.GetKey(toggleStrafeKeyID))
            {
                float deltaTimeStrafeThrust = spaceshipData.strafeThrust * Time.fixedDeltaTime;
                if (inputAxisVertical != 0.0f)
                {
                    rb.AddRelativeForce(inputAxisVertical * deltaTimeStrafeThrust * Vector3.right);
                }
                if (inputAxisHorizontal != 0.0f)
                {
                    rb.AddRelativeForce(inputAxisHorizontal * deltaTimeStrafeThrust * Vector3.up);
                }
            }
            if (inputAxisThrust != 0.0f)
            {
                rb.AddRelativeForce(inputAxisThrust * deltaTimeForwardThrust * Vector3.forward);
            }
        }
        
    }
}
