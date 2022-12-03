using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Bluaniman.SpaceGame.Debugging;
using Bluaniman.SpaceGame.Network;
using twoloop;
using static MovementData;
using Bluaniman.SpaceGame.Player;
using Bluaniman.SpaceGame.General.PID;

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
        [SerializeField] private PidFactors angularPidFactors = new(10f, 0f, 0.1f);
        private bool isStoppingRotation;
        private bool wasRotationAxisInputPresentLastUpdate;
        //showing in editor for testing purposes
        [SerializeField] private float angularVelocityError;
        [SerializeField] private float angularVelocityCorrection;

        [Header("Movement PID")]
        private PID movementPID;
        [SerializeField] private PidFactors movementPidFactors = new(1f, 0f, 0.01f);
        private bool isStoppingMovement;
        //showing in editor for testing purposes
        [SerializeField] private float movementVelocityError;
        [SerializeField] private float movementVelocityCorrection;
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
            angularPID = new PID(angularPidFactors);
            isStoppingRotation = false;
            wasRotationAxisInputPresentLastUpdate = false;
            movementPID = new PID(movementPidFactors);
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
            HandleMotion(0, ActivateThrusters, ref isStoppingRotation, RotationStoppingTrigger,
                angularPID, angularPidFactors, Rbody.angularVelocity, ref angularVelocityError, ref angularVelocityCorrection,
                movementData.GetDeliverableRotationThrust, ApplyAbsoluteTorque);
        }

        private bool RotationStoppingTrigger() => !AreInputAxiiPresent(0, 3);

        private void Thrust()
        {
            HandleMotion(3, ActivateEngines, ref isStoppingMovement, ThrustStoppingTrigger,
                movementPID, movementPidFactors, Rbody.velocity, ref movementVelocityError, ref movementVelocityCorrection,
                movementData.GetDeliverableMovementThrust, ApplyAbsoluteForce);
        }

        private bool ThrustStoppingTrigger() => inputAxisProvider.GetInputAxis(6) != 0f;

        private void HandleMotion(int axiiStartIndex, Action<int> activateAction, ref bool isStopping, Func<bool> stoppingTrigger,
            PID stoppingPID, PidFactors startingPidFactors, Vector3 pidTarget, ref float pidError, ref float pidCorrection,
            Func<Vector3, float> deliverableThrustFunc, Action<Vector3> applyAction)
        {
            bool isAxisInputPresent = AreInputAxiiPresent(axiiStartIndex, 3);
            if (isAxisInputPresent)
            {
                activateAction.Invoke(axiiStartIndex);
                isStopping = false;
            }
            else
            {
                if (stoppingTrigger.Invoke() && !isStopping)
                {
                    stoppingPID.Reset();
                    stoppingPID.PidFactors = startingPidFactors;
                    isStopping = true;
                }
                else if (!stoppingTrigger.Invoke() && isStopping)
                {
                    isStopping = false;
                }
                if (isStopping)
                {
                    pidError = pidTarget.magnitude;
                    if (pidError < 1e-7)
                    {
                        isStopping = false;
                    }
                    else
                    {
                        pidCorrection = stoppingPID.Update(pidError, Time.fixedDeltaTime);
                        float maxAvailableThrust = deliverableThrustFunc.Invoke(-pidTarget.normalized);
                        if (!ApplyMovement(applyAction, pidCorrection, maxAvailableThrust, -pidTarget.normalized))
                        {
                            isStopping = false;
                        }
                    }
                }
            }
        }

        private bool AreInputAxiiPresent(int startIndex, int count)
        {
            for (int i = startIndex; i < startIndex + count; i++)
            {
                if (inputAxisProvider.GetInputAxis(i) != 0f) { return true; }
            }
            return false;
        }

        private void ActivateThrusters(Vector3 rotationAmount)
        {
            ActivateThrusters(rotationAmount.x, rotationAmount.y, rotationAmount.z);
        }

        private void ActivateThrusters(int axiiStartIndex)
        {
            ActivateOnThreeAxii(ActivateThrusters, axiiStartIndex);
        }

        private void ActivateThrusters(float pitchAmount, float yawAmount, float rollAmount)
        {
            ApplyMovement(ApplyTorque, pitchAmount, movementData.pitchThrust, Vector3.right);
            ApplyMovement(ApplyTorque, yawAmount, movementData.yawThrust, Vector3.up);
            ApplyMovement(ApplyTorque, rollAmount, movementData.rollThrust, Vector3.forward);
        }

        private void ActivateEngines(Vector3 movementAmount)
        {
            ActivateEngines(movementAmount.x, movementAmount.y, movementAmount.z);
        }

        private void ActivateEngines(int axiiStartIndex)
        {
            ActivateOnThreeAxii(ActivateEngines, axiiStartIndex);
        }

        private void ActivateEngines(float forwardAmount, float horizontalAmount, float verticalAmount)
        {
            ApplyMovement(ApplyForce, forwardAmount, movementData.forwardThrust, Vector3.forward);
            ApplyMovement(ApplyForce, horizontalAmount, movementData.horizontalThrust, Vector3.right);
            ApplyMovement(ApplyForce, verticalAmount, movementData.verticalThrust, Vector3.up);
        }

        private void ActivateOnThreeAxii(Action<float, float, float> action, int axiiStartIndex)
        {
            action.Invoke(inputAxisProvider.GetInputAxis(axiiStartIndex),
                inputAxisProvider.GetInputAxis(axiiStartIndex + 1),
                inputAxisProvider.GetInputAxis(axiiStartIndex + 2));
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

        private static bool IsVectorNaN(Vector3 vec)
        {
            return float.IsNaN(vec.x) || float.IsNaN(vec.y) || float.IsNaN(vec.z);
        }

        private void ApplyAbsoluteTorque(Vector3 vec) => Rbody.AddTorque(vec);

        private void ApplyTorque(Vector3 vec) => Rbody.AddRelativeTorque(vec);

        private void ApplyAbsoluteForce(Vector3 vec) => Rbody.AddForce(vec);

        private void ApplyForce(Vector3 vec) => Rbody.AddRelativeForce(vec);
    }
}