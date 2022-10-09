using UnityEngine;

[CreateAssetMenu(fileName = "Spaceship Data", menuName = "Data/Spaceship")]
public class SpaceshipData : ScriptableObject
{
    public float mass;
    public float forwardThrust;
    public float rotationThrust;
    public float strafeThrust;
}
