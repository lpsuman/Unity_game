using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitController : MonoBehaviour
{
    public EllipticalOrbitData ellipticalOrbitData;
    public Transform orbitingTarget;
    private float period;
    private float meanMotion;

    private void Start()
    {
        
    }
}
