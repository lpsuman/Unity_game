using System;
using UnityEngine;

namespace Bluaniman.SpaceGame.General.PID
{
    [Serializable]
    public class PID : BasePID<float>
    {
        public PID(float pFactor, float iFactor, float dFactor, float clampMin = -1.0f, float clampMax = 1.0f)
            : base(pFactor, iFactor, dFactor, clampMin, clampMax) { }

        public PID(PidFactors pidFactors, float clampMin = -1.0f, float clampMax = 1.0f)
            : base(pidFactors, clampMin, clampMax) { }

        public override float Update(float currentError, float timeFrame)
        {
            integral += currentError * timeFrame;
            var deriv = (currentError - lastError) / timeFrame;
            lastError = currentError;
            return Mathf.Clamp(currentError * PidFactors.pFactor + integral * PidFactors.iFactor + deriv * PidFactors.dFactor, clampMin, clampMax);
        }

        public override void Reset()
        {
            integral = 0.0f;
            lastError = 0.0f;
        }
    }
}