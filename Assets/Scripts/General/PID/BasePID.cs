using System;
using UnityEngine;

[Serializable]
public abstract class BasePID<T>
{
    public float pFactor, iFactor, dFactor;
    public float clampMin, clampMax;

    protected T integral;
    protected T lastError;

    protected BasePID(float pFactor, float iFactor, float dFactor, float clampMin = -1.0f, float clampMax = 1.0f)
    {
        SetFactors(pFactor, iFactor, dFactor);
        this.clampMin = clampMin;
        this.clampMax = clampMax;
        Reset();
    }

    public void SetFactors(float pFactor, float iFactor, float dFactor)
    {
        this.pFactor = pFactor;
        this.iFactor = iFactor;
        this.dFactor = dFactor;
    }

    public abstract void Reset();

    public abstract T Update(T currentError, float timeFrame);
}