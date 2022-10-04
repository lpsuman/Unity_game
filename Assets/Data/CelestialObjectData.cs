using UnityEngine;

[CreateAssetMenu(fileName = "Celestial Object Data", menuName = "Data/Celestial Object")]
public class CelestialObjectData : ScriptableObject
{
    public enum CelestialObjectType
    {
        PlanetRocky,
        PlanetGasGiant,
        Star
    }

    public CelestialObjectType type;
    public float mass;
    public float radius;
}
