using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Spaceship Data", menuName = "Data/Spaceship")]
public class MovementData : ScriptableObject
{
    [Serializable]
    public class RatioVector3
    {
        private Vector2 resultVector = new();
        private Vector2 resultRatio = new();

        public ref Vector2 GetResultVector()
        {
            return ref resultVector;
        }

        [SerializeField] private float ratio;
        public float Ratio
        {
            get { return ratio; }
            set
            {
                float changeRatio = value / ratio;
                resultRatio *= changeRatio;
                resultVector *= changeRatio;
                ratio = value;
            }
        }

        [SerializeField] private float ratioPositive;
        public float RatioPositive
        {
            get { return ratioPositive; }
            set
            {
                float changeRatio = value / ratioPositive;
                resultRatio.x *= changeRatio;
                resultVector.x *= changeRatio;
                ratioPositive = value;
            }
        }

        [SerializeField] private float ratioNegative;
        public float RatioNegative
        {
            get { return ratioNegative; }
            set
            {
                float changeRatio = value / ratioNegative;
                resultRatio.x *= changeRatio;
                resultVector.x *= changeRatio;
                ratioNegative = value;
            }
        }

        public float GetRatioForDirection(float direction)
        {
            return direction > 0 ? resultRatio.x : resultRatio.y;
        }

        public float GetThrustForDirection(float direction)
        {
            return direction > 0 ? resultVector.x : resultVector.y;
        }

        public void ManualUpdate(float baseValue)
        {
            resultRatio.x = Ratio * RatioPositive;
            resultRatio.y = Ratio * RatioNegative;
            resultVector.x = baseValue * resultRatio.x;
            resultVector.y = baseValue * resultRatio.y;
        }

        public static void ManualUpdateRatioVectors(float baseValue, params RatioVector3[] ratioVectors)
        {
            Array.ForEach(ratioVectors, rv => rv.ManualUpdate(baseValue));
        }

        public static void UpdateRatioVectors(float changeRatio, params RatioVector3[] ratioVectors)
        {
            Array.ForEach(ratioVectors, rv => rv.GetResultVector() *= changeRatio);
        }
    }

    public float mass;

    [SerializeField] private float totalThrust;
    public float TotalThrust
    {
        get { return totalThrust; }
        set
        {
            float changeRatio = value / totalThrust;
            UpdateMovementRatioVectors(changeRatio);
            UpdateRotationRatioVectors(changeRatio);
            totalThrust = value;
        }
    }

    [Header("Movement")]
    [SerializeField] private float movementThrustRatio;
    public float MovementThrustRatio
    {
        get { return movementThrustRatio; }
        set
        {
            float changeRatio = value / movementThrustRatio;
            UpdateMovementRatioVectors(changeRatio);
            TotalMovementThrust *= changeRatio;
            movementThrustRatio = value;
        }
    }
    public float TotalMovementThrust { get; private set; }
    public RatioVector3 forwardThrust;
    public RatioVector3 horizontalThrust;
    public RatioVector3 verticalThrust;
    private void UpdateMovementRatioVectors(float changeRatio)
    {
        RatioVector3.UpdateRatioVectors(changeRatio, forwardThrust, horizontalThrust, verticalThrust);
    }

    [Header("Rotation")]
    [SerializeField] private float rotationThrustRatio;
    public float RotationThrustRatio
    {
        get { return rotationThrustRatio; }
        set
        {
            float changeRatio = value / rotationThrustRatio;
            UpdateRotationRatioVectors(changeRatio);
            TotalRotationThrust *= changeRatio;
            rotationThrustRatio = value;
        }
    }
    public float TotalRotationThrust { get; private set; }
    public RatioVector3 pitchThrust;
    public RatioVector3 yawThrust;
    public RatioVector3 rollThrust;

    private void UpdateRotationRatioVectors(float changeRatio)
    {
        RatioVector3.UpdateRatioVectors(changeRatio, pitchThrust, yawThrust, rollThrust);
    }

    public void ManualUpdate()
    {
        RatioVector3.ManualUpdateRatioVectors(TotalThrust * MovementThrustRatio, forwardThrust, horizontalThrust, verticalThrust);
        RatioVector3.ManualUpdateRatioVectors(TotalThrust * RotationThrustRatio, pitchThrust, yawThrust, rollThrust);
        TotalMovementThrust = TotalThrust * MovementThrustRatio;
        TotalRotationThrust = TotalThrust * RotationThrustRatio;
    }

    public void OnEnable()
    {
        ManualUpdate();
    }

    public float GetDeliverableMovementThrust(Vector3 rotationAxis)
    {
        return TotalMovementThrust * CalcDeliverableThrust(forwardThrust, horizontalThrust, verticalThrust, rotationAxis);
    }

    public float GetDeliverableRotationThrust(Vector3 rotationAxis)
    {
        return TotalRotationThrust * CalcDeliverableThrust(pitchThrust, yawThrust, rollThrust, rotationAxis);
    }

    public static float CalcDeliverableThrust(RatioVector3 thrustX, RatioVector3 thrustY, RatioVector3 thrustZ, Vector3 rotationAxis)
    {
        Vector3 rotationAxisPercentXYZ = VectorAsPercentXYZ(rotationAxis);
        float availableRatioX = rotationAxisPercentXYZ.x * thrustX.GetRatioForDirection(rotationAxisPercentXYZ.x);
        float availableRatioY = rotationAxisPercentXYZ.y * thrustX.GetRatioForDirection(rotationAxisPercentXYZ.y);
        float availableRatioZ = rotationAxisPercentXYZ.z * thrustX.GetRatioForDirection(rotationAxisPercentXYZ.z);
        return Math.Min(Vector3.Magnitude(new Vector3(availableRatioX, availableRatioY, availableRatioZ)), 1f);
    }


    public static Vector3 VectorAsPercentXYZ(Vector3 input)
    {
        float sum = input.x + input.y + input.z;
        return new Vector3(input.x / sum, input.y / sum, input.z / sum);
    }
}
