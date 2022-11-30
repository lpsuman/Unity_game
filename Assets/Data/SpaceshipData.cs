using UnityEngine;

[CreateAssetMenu(fileName = "Spaceship Data", menuName = "Data/Spaceship")]
public class SpaceshipData : ScriptableObject
{
    public float mass;
    public float totalThrust;

    public float movementThrustRatio;
    public Vector3 forwardThrust;
    public Vector3 horizontalThrust;
    public Vector3 verticalThrust;

    public float rotationThrustRatio;
    public Vector3 pitchThrust;
    public Vector3 yawThrust;
    public Vector3 rollThrust;
}
