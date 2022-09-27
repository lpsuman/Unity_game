using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Elliptical Orbit Data", menuName = "Data/Elliptical Orbit")]
public class EllipticalOrbitData : ScriptableObject
{
    public double semimajorAxis;
    public double eccentricity;
}
