using System;

namespace Bluaniman.SpaceGame.General.PID
{
    [Serializable]
    public class PidFactors
    {
        public float pFactor;
        public float iFactor;
        public float dFactor;

        public PidFactors(float pFactor, float iFactor, float dFactor)
        {
            this.pFactor = pFactor;
            this.iFactor = iFactor;
            this.dFactor = dFactor;
        }
    }
}