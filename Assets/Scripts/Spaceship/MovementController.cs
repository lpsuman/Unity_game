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
        [SerializeField] private PidFactors counterRotationPidFactors = new(10f, 0f, 0.1f);
        [SerializeField] private PIDController counterRotationPidController;

        [Header("Movement PID")]
        [SerializeField] private PidFactors counterMovementPidFactors = new(10f, 0f, 0.1f);
        [SerializeField] private PIDController counterMovementPidController;

        #region Setup
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
            counterRotationPidController = new PIDController(counterRotationPidFactors, CounterRotationStartTrigger, CounterRotationStopTrigger,
                CounterRotationErrorFunction, CounterRotationFunction, this, "Rotation PID");
            counterMovementPidController = new PIDController(counterMovementPidFactors, CounterThrustStartTrigger, CounterThrustStopTrigger,
                CounterThrustErrorFunction, CounterThrustFunction, this, "Movement PID");
        }
        #endregion

        private void FixedUpdate()
        {
            //DebugHandler.NetworkLog("Spaceship fixedUpdate start.", this);
            if (isMovementReady && (isServer || IsClientWithLocalControls()))
            {
                HandleMotion(0, ActivateThrusters, counterRotationPidController);
                HandleMotion(3, ActivateEngines, counterMovementPidController);
            }
            //DebugHandler.NetworkLog("Spaceship fixedUpdate end.", this);
        }

        #region Turn & Thrust
        private void HandleMotion(int axiiStartIndex, Action<int> activateAction, PIDController pidController)
        {
            if (inputAxisProvider.AreInputAxiiPresent(axiiStartIndex, 3))
            {
                activateAction.Invoke(axiiStartIndex);
            }
            else
            {
                pidController.PidUpdate(Time.fixedDeltaTime);
            }
        }

        private bool CounterRotationStartTrigger() => !inputAxisProvider.AreInputAxiiPresent(0, 3);

        private bool CounterRotationStopTrigger() => !CounterRotationStartTrigger();

        private float CounterRotationErrorFunction() => Rbody.angularVelocity.magnitude;

        private bool CounterRotationFunction(float pidCorrection) => CounterMovementFunction(pidCorrection,
            movementData.GetDeliverableRotationThrust, ApplyAbsoluteTorque, Rbody.angularVelocity);

        private bool CounterThrustStartTrigger() => inputAxisProvider.GetInputAxis(6) != 0f;

        private bool CounterThrustStopTrigger() => !CounterThrustStartTrigger();

        private float CounterThrustErrorFunction() => Rbody.velocity.magnitude;

        private bool CounterThrustFunction(float pidCorrection) => CounterMovementFunction(pidCorrection,
            movementData.GetDeliverableMovementThrust, ApplyAbsoluteForce, Rbody.velocity);

        private bool CounterMovementFunction(float pidCorrection, Func<Vector3, float> deliverableThrustFunc,
            Action<Vector3> applyAction, Vector3 directionVector)
        {
            float maxAvailableThrust = deliverableThrustFunc.Invoke(-directionVector.normalized);
            return ApplyMovement(applyAction, pidCorrection, maxAvailableThrust, -directionVector.normalized);
        }
        #endregion

        #region Thrusters
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
            ApplyMovement(ApplyRelativeTorque, pitchAmount, movementData.pitchThrust, Vector3.right);
            ApplyMovement(ApplyRelativeTorque, yawAmount, movementData.yawThrust, Vector3.up);
            ApplyMovement(ApplyRelativeTorque, rollAmount, movementData.rollThrust, Vector3.forward);
        }
        #endregion

        #region Engines
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
            ApplyMovement(ApplyRelativeForce, forwardAmount, movementData.forwardThrust, Vector3.forward);
            ApplyMovement(ApplyRelativeForce, horizontalAmount, movementData.horizontalThrust, Vector3.right);
            ApplyMovement(ApplyRelativeForce, verticalAmount, movementData.verticalThrust, Vector3.up);
        }

        private void ActivateOnThreeAxii(Action<float, float, float> action, int axiiStartIndex)
        {
            action.Invoke(inputAxisProvider.GetInputAxis(axiiStartIndex),
                inputAxisProvider.GetInputAxis(axiiStartIndex + 1),
                inputAxisProvider.GetInputAxis(axiiStartIndex + 2));
        }
        #endregion

        #region Apply functions
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

        private void ApplyAbsoluteTorque(Vector3 vec) => Rbody.AddTorque(vec);

        private void ApplyRelativeTorque(Vector3 vec) => Rbody.AddRelativeTorque(vec);

        private void ApplyAbsoluteForce(Vector3 vec) => Rbody.AddForce(vec);

        private void ApplyRelativeForce(Vector3 vec) => Rbody.AddRelativeForce(vec);
        #endregion

        // TODO move to a helper class
        private static bool IsVectorNaN(Vector3 vec)
        {
            return float.IsNaN(vec.x) || float.IsNaN(vec.y) || float.IsNaN(vec.z);
        }
    }
}