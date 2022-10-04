using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Elliptical Orbit Data", menuName = "Data/Elliptical Orbit")]
public class EllipticalOrbitData : ScriptableObject
{
    public Transform orbitingPoint;
    public Vector3 orbitingAxis;
    public float orbitingPlaneRotationAngle;

    public double semimajorAxis;
    public double eccentricity;

    public bool isTidallyLocked;
    public Vector3 rotationAxis;
    public float rotationPeriod;
}
