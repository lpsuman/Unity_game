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
using static Bluaniman.SpaceGame.General.PID.PIDController;

namespace Bluaniman.SpaceGame.Spaceship
{
	public class MovementController : MyNetworkBehavior, IMovementController
	{
        private const float secondsBeforeStarting = 3f;
        public Rigidbody Rbody { get; private set; }

        public bool IsReady { get; set; }
        public event Action OnMovementSetupDone;
        public event Action OnReady;

        private IInputController inputController;

        public IInputProvider<float> InputAxiiProvider { get; set; }
        public IInputProvider<bool> InputButtonsProvider { get; set; }
        [SerializeField] private OSNetTransform osNetTransform = null;
        [SerializeField] private OSNetRigidbody osNetRb = null;

        [SerializeField] private MovementData movementData;
        public MovementData MovementData { get
            {
                return movementData;
            }
        }

        [Header("Manual PIDs")]
        [SerializeField] private PidFactors counterRotationPidFactors = new(10f, 0f, 0.1f);
        [SerializeField] private PIDController counterRotationPidController;
        [SerializeField] private PidFactors counterMovementPidFactors = new(0.1f, 0f, 0.001f);
        [SerializeField] private PIDController counterMovementPidController;

        [Header("Automatic PIDs")]
        [SerializeField] private PidFactors adjustHeadingPidFactors = new(10f, 0f, 0.1f);
        [SerializeField] private PIDController adjustHeadingPidController;
        [SerializeField] private PidFactors redirectThrustPidFactors = new(10f, 0f, 0.1f);
        [SerializeField] private PIDController redirectThrustPidController;
        [SerializeField] public Vector3 AutomaticHeading { get; set; }

        private Vector3 autoHeadingDeltaVelocity;
        private Vector3 autoHeadingCrossProduct;

        #region Setup
        public void Start()
        {
            if (isServer || IsClientWithLocalControls())
            {
                inputController = GetComponent<IInputController>();
                inputController.OnControlsSetupDone += StartAfterInputController;
                return;
            }
            CommonStart();
        }

        private void StartAfterInputController()
        {
            InputAxiiProvider = inputController.GetInputAxisProvider();
            InputButtonsProvider = inputController.GetInputButtonsProvider();
            CommonStart();
        }

        private void CommonStart()
        {
            DebugHandler.CheckAndDebugLog(DebugHandler.Input(), "Movement controller setup start.", this);
            Invoke(nameof(DelayedStart), secondsBeforeStarting);
        }

        private void OnDestroy()
        {
            if (isServer || IsClientWithLocalControls())
            {
                inputController.OnControlsSetupDone -= StartAfterInputController;
            }
        }

        private void DelayedStart()
        {
            Rbody = GetComponent<Rigidbody>();
            Rbody.mass = MovementData.mass;
            Rbody.useGravity = false;
            if (isServer || IsClientWithLocalControls())
            {
                DoSetup();
            }
            osNetTransform.clientAuthority = useAuthorityPhysics;
            osNetRb.clientAuthority = useAuthorityPhysics;
            osNetRb.serverOnlyPhysics = !useAuthorityPhysics;
            if (isServer || IsClientWithLocalControls())
            {
                inputController.DoWhenReady(HandleControlsFinalized);
            }
        }

        private void DoSetup()
        {
            Rbody.isKinematic = false;
            AutomaticHeading = Vector3.zero;

            PIDControllerFunctionSet counterRotationPidFuncSet = new(
                () => !InputAxiiProvider.AreInputsPresent(0, 3),
                () => InputAxiiProvider.AreInputsPresent(0, 3),
                () => Rbody.angularVelocity.magnitude,
                (float pidCorrection) => CounterMovementFunction(pidCorrection,
                    MovementData.GetDeliverableRotationThrust, ApplyAbsoluteTorque, Rbody.angularVelocity)
                );
            counterRotationPidController = new PIDController(counterRotationPidFactors, counterRotationPidFuncSet, this, "Rotation PID");

            PIDControllerFunctionSet counterMovementPidFuncSet = new(
                () => InputButtonsProvider.GetInput(0),
                () => !InputButtonsProvider.GetInput(0),
                () => Rbody.velocity.magnitude,
                (float pidCorrection) => CounterMovementFunction(pidCorrection,
                    MovementData.GetDeliverableMovementThrust, ApplyAbsoluteForce, Rbody.velocity)
                );
            counterMovementPidController = new PIDController(counterMovementPidFactors, counterMovementPidFuncSet, this, "Movement PID");

            // TODO adjust heading PID

            PIDControllerFunctionSet redirectThrustPidFuncSet = new(
                () => InputButtonsProvider.GetInput(1),
                () => !InputButtonsProvider.GetInput(1),
                () => autoHeadingDeltaVelocity.magnitude,
                (float pidCorrection) => CounterMovementFunction(pidCorrection,
                    MovementData.GetDeliverableMovementThrust, ApplyAbsoluteForce, autoHeadingDeltaVelocity)
                );
            redirectThrustPidController = new PIDController(redirectThrustPidFactors, redirectThrustPidFuncSet, this, "Redirect thrust PID");
        }

        private void HandleControlsFinalized()
        {
            IsReady = true;
            OnReady?.Invoke();
            OnReady = null;
            DebugHandler.CheckAndDebugLog(DebugHandler.Input(), "Movement controller setup done.", this);
        }
        #endregion

        private void FixedUpdate()
        {
            //DebugHandler.NetworkLog("Spaceship fixedUpdate start.", this);
            if (IsReady && (isServer || IsClientWithLocalControls()))
            {
                if (!AutomaticHeadingUpdate())
                {
                    HandleMotion(0, ActivateThrusters, counterRotationPidController);
                    HandleMotion(3, ActivateEngines, counterMovementPidController);
                }
            }
            //DebugHandler.NetworkLog("Spaceship fixedUpdate end.", this);
        }

        private bool AutomaticHeadingUpdate()
        {
            if (AutomaticHeading != Vector3.zero)
            {
                autoHeadingDeltaVelocity = AutomaticHeading - Rbody.velocity;
                autoHeadingCrossProduct = Vector3.Cross(Rbody.velocity, AutomaticHeading);
                redirectThrustPidController.PidUpdate(Time.fixedDeltaTime);
                return redirectThrustPidController.IsRunning;
            }
            else
            {
                return false;
            }
        }

        private void HandleMotion(int axiiStartIndex, Action<int> activateAction, PIDController pidController)
        {
            if (InputAxiiProvider.AreInputsPresent(axiiStartIndex, 3))
            {
                activateAction.Invoke(axiiStartIndex);
            }
            else
            {
                pidController.PidUpdate(Time.fixedDeltaTime);
            }
        }

        private bool CounterMovementFunction(float pidCorrection, Func<Vector3, float> deliverableThrustFunc,
            Action<Vector3> applyAction, Vector3 directionVector)
        {
            float maxAvailableThrust = deliverableThrustFunc.Invoke(-directionVector.normalized);
            return ApplyMovement(applyAction, pidCorrection, maxAvailableThrust, -directionVector.normalized);
        }

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
            ApplyMovement(ApplyRelativeTorque, pitchAmount, MovementData.pitchThrust, Vector3.right);
            ApplyMovement(ApplyRelativeTorque, yawAmount, MovementData.yawThrust, Vector3.up);
            ApplyMovement(ApplyRelativeTorque, rollAmount, MovementData.rollThrust, Vector3.forward);
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
            ApplyMovement(ApplyRelativeForce, forwardAmount, MovementData.forwardThrust, Vector3.forward);
            ApplyMovement(ApplyRelativeForce, horizontalAmount, MovementData.horizontalThrust, Vector3.right);
            ApplyMovement(ApplyRelativeForce, verticalAmount, MovementData.verticalThrust, Vector3.up);
        }

        private void ActivateOnThreeAxii(Action<float, float, float> action, int axiiStartIndex)
        {
            action.Invoke(InputAxiiProvider.GetInput(axiiStartIndex),
                InputAxiiProvider.GetInput(axiiStartIndex + 1),
                InputAxiiProvider.GetInput(axiiStartIndex + 2));
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