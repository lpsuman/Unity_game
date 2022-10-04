using UnityEngine;

public class SpaceshipController : MonoBehaviour
{
    public float forwardThrust = 1000000f;
    public float turnThrust = 500000f;
    Rigidbody rb;
    public KeyCode stopRotationKeyPID = KeyCode.LeftControl;
    public KeyCode stopRotationKeyThrust = KeyCode.LeftAlt;
    private VectorPid angularVelocityController;
    public float startingP = 33.7766f;
    public float startingI = 0.0f;
    public float startingD = 0.2553191f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    void Update()
    {
        Debug.DrawLine(transform.position, transform.position + rb.angularVelocity, Color.cyan);
    }

    private void FixedUpdate()
    {
        Turn();
        Thrust();
    }

    void Turn()
    {
        if (Input.GetKeyDown(stopRotationKeyPID))
        {
            angularVelocityController = new VectorPid(startingP, startingI, startingD);
        }
        else if (Input.GetKey(stopRotationKeyPID))
        {
            Vector3 angularVelocityError = rb.angularVelocity * -1;
            Vector3 angularVelocityCorrection = angularVelocityController.Update(angularVelocityError, Time.fixedDeltaTime);
            rb.AddTorque(angularVelocityCorrection);
        }
        else if (Input.GetKeyDown(stopRotationKeyThrust))
        {
            angularVelocityController = new VectorPid(startingP, startingI, startingD);
        }
        else if (Input.GetKey(stopRotationKeyThrust))
        {
            if (turnThrust * Time.fixedDeltaTime > rb.angularVelocity.magnitude)
            {
                rb.AddTorque(turnThrust * Time.fixedDeltaTime * -rb.angularVelocity.normalized);
            } else if (rb.angularVelocity.magnitude >= 0.001)
            {
                rb.AddTorque(Time.fixedDeltaTime * -rb.angularVelocity);
            }
            
        } else
        {
            rb.AddRelativeTorque(Input.GetAxis("Vertical") * turnThrust * Time.deltaTime * Vector3.left);
            rb.AddRelativeTorque(Input.GetAxis("Horizontal") * turnThrust * Time.deltaTime * Vector3.up);
            rb.AddRelativeTorque(Input.GetAxis("Rotate") * turnThrust * Time.deltaTime * Vector3.forward);
        }
    }

    void Thrust()
    {
        rb.AddRelativeForce(Input.GetAxis("Thrust") * forwardThrust * Time.fixedDeltaTime * Vector3.forward);
    }
}
