using UnityEngine;

public class VectorPid : BasePID<Vector3>
{

    public VectorPid(float pFactor, float iFactor, float dFactor, float clampMin = -1.0f, float clampMax = 1.0f) : base(pFactor, iFactor, dFactor, clampMin, clampMax)
    {
        
    }

    public override Vector3 Update(Vector3 currentError, float timeFrame)
    {
        integral += currentError * timeFrame;
        var deriv = (currentError - lastError) / timeFrame;
        lastError = currentError;
        Vector3 output = currentError * pFactor + integral * iFactor + deriv * dFactor;
        output.x = Mathf.Clamp(output.x, clampMin, clampMax);
        output.y = Mathf.Clamp(output.y, clampMin, clampMax);
        output.z = Mathf.Clamp(output.z, clampMin, clampMax);
        return output;
    }

    public override void Reset()
    {
        integral = Vector3.zero;
        lastError = Vector3.zero;
    }
}
