using System;

namespace Bluaniman.SpaceGame.General.PID
{
    [Serializable]
    public abstract class BasePID<T>
    {
        public PidFactors PidFactors { get; set; }
        public float clampMin, clampMax;

        protected T integral;
        protected T lastError;

        protected BasePID(float pFactor, float iFactor, float dFactor, float clampMin = -1.0f, float clampMax = 1.0f)
            : this(new PidFactors(pFactor, iFactor, dFactor), clampMin, clampMax) { }

        protected BasePID(PidFactors pidFactors, float clampMin = -1.0f, float clampMax = 1.0f)
        {
            PidFactors = pidFactors;
            this.clampMin = clampMin;
            this.clampMax = clampMax;
            Reset();
        }

        public void SetFactors(float pFactor, float iFactor, float dFactor)
        {
            PidFactors.pFactor = pFactor;
            PidFactors.iFactor = iFactor;
            PidFactors.dFactor = dFactor;
        }

        public abstract void Reset();

        public abstract T Update(T currentError, float timeFrame);
    }
}