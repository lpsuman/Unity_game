using System;
using Bluaniman.SpaceGame.Debugging;
using Bluaniman.SpaceGame.Network;
using Mirror;

namespace Bluaniman.SpaceGame.General.PID
{
    [Serializable]
	public class PIDController
	{
        public class PIDControllerFunctionSet
        {
            public readonly Func<bool> startingTrigger;
            public readonly Func<bool> stoppingTrigger;
            public readonly Func<float> errorFunction;
            public readonly Func<float, bool> correctionFunction;

            public PIDControllerFunctionSet(Func<bool> startingTrigger, Func<bool> stoppingTrigger, Func<float> errorFunction, Func<float, bool> correctionFunction)
            {
                this.startingTrigger = startingTrigger;
                this.stoppingTrigger = stoppingTrigger;
                this.errorFunction = errorFunction;
                this.correctionFunction = correctionFunction;
            }
        }
        public const float MinPidError = 1e-6f;

        private readonly PID pid;
        private readonly PidFactors startingPidFactors;
        public float PidError { get; private set; }
        public float PidCorrection { get; private set; }
        public bool IsRunning { get; set; }
        private readonly PIDControllerFunctionSet funcSet;
        private readonly DebugHandler.DebugNameAndNetContext debugData;

        public PIDController(PidFactors startingPidFactors, PIDControllerFunctionSet funcSet, MyNetworkBehavior debugNetworkContext = null, string debugName = "PID Controller")
        {
            this.startingPidFactors = startingPidFactors;
            this.funcSet = funcSet;
            debugData = new DebugHandler.DebugNameAndNetContext(debugNetworkContext, debugName);
            pid = new PID(startingPidFactors);
            IsRunning = false;
        }

        public void PidUpdate(float deltaTime)
        {
            if (!IsRunning && funcSet.startingTrigger.Invoke() && !IsCurrentErrorNegligible())
            {
                pid.Reset();
                pid.PidFactors = startingPidFactors;
                IsRunning = true;
                DebugHandler.CheckAndDebugLog(DebugHandler.Movement(), $"{debugData.debugName} start triggered.", debugData);
            }
            else if (IsRunning && funcSet.stoppingTrigger.Invoke())
            {
                IsRunning = false;
                DebugHandler.CheckAndDebugLog(DebugHandler.Movement(), $"{debugData.debugName} stop triggered.", debugData);
            }
            if (IsRunning)
            {
                PidError = funcSet.errorFunction.Invoke();
                if (PidError < MinPidError)
                {
                    IsRunning = false;
                    DebugHandler.CheckAndDebugLog(DebugHandler.Movement(), $"{debugData.debugName} minimum error reached.", debugData);
                }
                else
                {
                    PidCorrection = pid.Update(PidError, deltaTime);
                    if (!funcSet.correctionFunction.Invoke(PidCorrection))
                    {
                        IsRunning = false;
                        DebugHandler.CheckAndDebugLog(DebugHandler.Movement(), $"{debugData.debugName} stopped from correction function.", debugData);
                    }
                }
            }
        }

        public bool IsCurrentErrorNegligible()
        {
            return funcSet.errorFunction.Invoke() < MinPidError;
        }
    }
}