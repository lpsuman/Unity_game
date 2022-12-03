using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Bluaniman.SpaceGame.Debugging;
using Bluaniman.SpaceGame.Network;
using twoloop;
using static MovementData;
using Bluaniman.SpaceGame.Player;

namespace Bluaniman.SpaceGame.Spaceship
{
	public class MovementController : MyNetworkBehavior
	{
        private const float secondsBeforeStarting = 3f;
        public Rigidbody Rbody { get; private set; }

        private bool isMovementReady = false;
        public event Action OnMovementSetupDone;

        private IInputAxisProvider inputAxisProvider = null;
        [SerializeField] private OSNetTransform osNetTransform = null;
        [SerializeField] private OSNetRigidbody osNetRb = null;
        public MovementData movementData;

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
        [SerializeField] private Vector3 torque;

        public void Start()
        {
            DebugHandler.CheckAndDebugLog(DebugHandler.Input(), "Movement controller setup start.", this);
            inputAxisProvider = GetComponent<IInputAxisProvider>();
            Invoke(nameof(DelayedStart), secondsBeforeStarting);
        }

        private void DelayedStart()
        {
            Rbody = GetComponent<Rigidbody>();
            Rbody.mass = movementData.mass;
            Rbody.useGravity = false;
            if (isServer || IsClientWithLocalControls())
            {
                DoSetup();
            }
            osNetTransform.clientAuthority = useAuthorityPhysics;
            osNetRb.clientAuthority = useAuthorityPhysics;
            osNetRb.serverOnlyPhysics = !useAuthorityPhysics;
            DebugHandler.CheckAndDebugLog(DebugHandler.Input(), "Movement controller setup done.", this);
            isMovementReady = true;
            OnMovementSetupDone?.Invoke();
        }

        private void DoSetup()
        {
            Rbody.isKinematic = false;
            angularPID = new PID(angularPIDstartingP, angularPIDstartingI, angularPIDstartingD);
            isStoppingRotation = false;
            wasRotationAxisInputPresentLastUpdate = false;
            movementPID = new PID(movementPIDstartingP, movementPIDstartingI, movementPIDstartingD);
            isStoppingMovement = false;
        }

        private void FixedUpdate()
        {
            //DebugHandler.NetworkLog("Spaceship fixedUpdate start.", this);
            if (isMovementReady && (isServer || IsClientWithLocalControls()))
            {
                Turn();
                Thrust();
            }
            //DebugHandler.NetworkLog("Spaceship fixedUpdate end.", this);
        }

        private void Turn()
        {
            bool isAxisInputPresent = inputAxisProvider.GetInputAxis(0) != 0f || inputAxisProvider.GetInputAxis(1) != 0f || inputAxisProvider.GetInputAxis(2) != 0f;
            if (isAxisInputPresent)
            {
                ActivateThrusters(inputAxisProvider.GetInputAxis(0), inputAxisProvider.GetInputAxis(1), inputAxisProvider.GetInputAxis(2));
            }
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
                angularVelocityError = Rbody.angularVelocity.magnitude;
                if (angularVelocityError < 1e-7)
                {
                    isStoppingRotation = false;
                }
                else
                {
                    angularVelocityCorrection = angularPID.Update(angularVelocityError, Time.fixedDeltaTime);
                    float maxAvailableThrust = movementData.GetDeliverableRotationThrust(-Rbody.angularVelocity.normalized);
                    // NOTE: must be absolute and not relative torque!
                    if (!ApplyMovement(ApplyAbsoluteTorque, angularVelocityCorrection, maxAvailableThrust, -Rbody.angularVelocity.normalized))
                    {
                        isStoppingRotation = false;
                    }
                }
            }
            wasRotationAxisInputPresentLastUpdate = isAxisInputPresent;
        }

        // doesn't seem to work and it's way too many calculations anyways
        //private Vector3 AxisAngleToPitchYawRoll(Vector3 rotationAxis)
        //{
        //    return AxisAngleToPitchYawRoll(rotationAxis.normalized, rotationAxis.magnitude);
        //}

        //private Vector3 AxisAngleToPitchYawRoll(Vector3 rotationAxisNormalized, float angle)
        //{
        //    Quaternion q = Quaternion.AngleAxis(angle, rotationAxisNormalized);
        //    float yaw = Mathf.Atan2(2f * (q.y * q.z + q.w * q.x), q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z);
        //    float pitch = Mathf.Asin(-2f * (q.x * q.z - q.w * q.y));
        //    float roll = Mathf.Atan2(2f * (q.x * q.y + q.w * q.z), q.w * q.w + q.x * q.x - q.y * q.y - q.z * q.z);
        //    //float pitch = Mathf.Rad2Deg * Mathf.Atan2(2 * q.x * q.w - 2 * q.y * q.z, 1 - 2 * q.x * q.x - 2 * q.z * q.z);
        //    //float yaw   = Mathf.Rad2Deg * Mathf.Atan2(2 * q.y * q.w - 2 * q.x * q.z, 1 - 2 * q.y * q.y - 2 * q.z * q.z);
        //    //float roll  = Mathf.Rad2Deg * Mathf.Asin (2 * q.x * q.y + 2 * q.z * q.w);
        //    return new Vector3(pitch, yaw, roll);
        //}

        private void ActivateThrusters(Vector3 rotationAmount)
        {
            ActivateThrusters(rotationAmount.x, rotationAmount.y, rotationAmount.z);
        }

        private void ActivateThrusters(float pitchAmount, float yawAmount, float rollAmount)
        {
            ApplyMovement(ApplyTorque, pitchAmount, movementData.pitchThrust, Vector3.right);
            ApplyMovement(ApplyTorque, yawAmount, movementData.yawThrust, Vector3.up);
            ApplyMovement(ApplyTorque, rollAmount, movementData.rollThrust, Vector3.forward);
        }

        private void Thrust()
        {
            ApplyMovement(ApplyForce, inputAxisProvider.GetInputAxis(3), movementData.forwardThrust, Vector3.forward);
            ApplyMovement(ApplyForce, inputAxisProvider.GetInputAxis(4), movementData.horizontalThrust, Vector3.right);
            ApplyMovement(ApplyForce, inputAxisProvider.GetInputAxis(5), movementData.verticalThrust, Vector3.up);
            //float forward = GetInputAxis(3);
            //if (forward < 0f)
            //{
            //    if (!isStoppingMovement)
            //    {
            //        movementPID.Reset();
            //        movementPID.SetFactors(movementPIDstartingP, movementPIDstartingI, movementPIDstartingD);
            //        isStoppingMovement = true;
            //    }
            //}
            //else
            //{
            //    isStoppingMovement = false;
            //}
            //if (isStoppingMovement)
            //{
            //    movementError = rb.velocity.magnitude;
            //    movementCorrection = movementPID.Update(movementError, Time.fixedDeltaTime);
            //    rb.AddForce(movementCorrection * deltaTimeForwardThrust * -rb.velocity.normalized);
            //}
            //else
            //{
            //    if (forward != 0.0f)
            //    {
            //        rb.AddRelativeForce(forward * deltaTimeForwardThrust * Vector3.forward);
            //    }
            //}
        }

        private bool ApplyMovement(Action<Vector3> thrustAction, float axisInput, RatioVector3 thrustRatio, Vector3 direction)
        {
            return ApplyMovement(thrustAction, axisInput, thrustRatio.GetThrustForDirection(axisInput), direction);
        }

        private bool ApplyMovement(Action<Vector3> thrustAction, float axisInput, float thrust, Vector3 direction)
        {
            if (axisInput == 0f || thrust == 0f) { return false; }
            Vector3 temp = axisInput * thrust * Time.fixedDeltaTime * direction;
            if (IsVectorNaN(temp)) { return false; }
            thrustAction.Invoke(temp);
            return true;
        }

        private bool IsVectorNaN(Vector3 vec)
        {
            return float.IsNaN(vec.x) || float.IsNaN(vec.y) || float.IsNaN(vec.z);
        }

        private void ApplyAbsoluteTorque(Vector3 vec) => Rbody.AddTorque(vec);

        private void ApplyTorque(Vector3 vec) => Rbody.AddRelativeTorque(vec);

        private void ApplyForce(Vector3 vec) => Rbody.AddRelativeForce(vec);
    }
}