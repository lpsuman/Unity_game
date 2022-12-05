using System;
using Bluaniman.SpaceGame.Debugging;
using Mirror;

namespace Bluaniman.SpaceGame.General.PID
{
    [Serializable]
	public class PIDController
	{
        public const float MinPidError = 1e-6f;

        private readonly PID pid;
        private readonly PidFactors startingPidFactors;
        public float PidError { get; private set; }
        public float PidCorrection { get; private set; }
        public bool IsRunning { get; set; }
        private readonly Func<bool> startingTrigger;
        private readonly Func<bool> stoppingTrigger;
        private readonly Func<float> errorFunction;
        private readonly Func<float, bool> correctionFunction;
        private readonly NetworkBehaviour debugNetworkContext;
        private readonly string debugName;

        public PIDController(PidFactors startingPidFactors, Func<bool> startingTrigger, Func<bool> stoppingTrigger,
            Func<float> errorFunction, Func<float, bool> correctionFunction, NetworkBehaviour debugNetworkContext = null, string debugName = "PID Controller")
        {
            this.startingPidFactors = startingPidFactors;
            this.startingTrigger = startingTrigger;
            this.stoppingTrigger = stoppingTrigger;
            this.errorFunction = errorFunction;
            this.correctionFunction = correctionFunction;
            this.debugNetworkContext = debugNetworkContext;
            this.debugName = debugName;
            pid = new PID(startingPidFactors);
            IsRunning = false;
        }

        public void PidUpdate(float deltaTime)
        {
            if (!IsRunning && startingTrigger.Invoke() && !IsCurrentErrorNegligible())
            {
                pid.Reset();
                pid.PidFactors = startingPidFactors;
                IsRunning = true;
                DebugHandler.CheckAndDebugLog(DebugHandler.Movement(), $"{debugName} start triggered.", debugNetworkContext);
            }
            else if (IsRunning && stoppingTrigger.Invoke())
            {
                IsRunning = false;
                DebugHandler.CheckAndDebugLog(DebugHandler.Movement(), $"{debugName} stop triggered.", debugNetworkContext);
            }
            if (IsRunning)
            {
                PidError = errorFunction.Invoke();
                if (PidError < MinPidError)
                {
                    IsRunning = false;
                    DebugHandler.CheckAndDebugLog(DebugHandler.Movement(), $"{debugName} minimum error reached.", debugNetworkContext);
                }
                else
                {
                    PidCorrection = pid.Update(PidError, deltaTime);
                    if (!correctionFunction.Invoke(PidCorrection))
                    {
                        IsRunning = false;
                        DebugHandler.CheckAndDebugLog(DebugHandler.Movement(), $"{debugName} stopped from correction function.", debugNetworkContext);
                    }
                }
            }
        }

        public bool IsCurrentErrorNegligible()
        {
            return errorFunction.Invoke() < MinPidError;
        }
    }
}