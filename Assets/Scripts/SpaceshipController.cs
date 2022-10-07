using UnityEngine;

public class SpaceshipController : MonoBehaviour
{
    public float forwardThrust = 1000000f;
    public float turnThrust = 500000f;
    Rigidbody rb;
    public KeyCode stopRotationKeyPID = KeyCode.LeftControl;
    public KeyCode stopRotationKeyThrust = KeyCode.LeftAlt;
    private PID angularVelocityController;
    public float startingP = 33.7766f;
    public float startingI = 0.0f;
    public float startingD = 0.2553191f;

    public float angularVelocityError;
    public float angularVelocityCorrection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        angularVelocityController = new PID(startingP, startingI, startingD);
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
            angularVelocityController.Reset();
            angularVelocityController.SetFactors(startingP, startingI, startingD);
            //Debug.Log(string.Format("new pid with: {0} {1} {2}", startingP, startingI, startingD));
        }
        else if (Input.GetKey(stopRotationKeyPID))
        {
            angularVelocityError = rb.angularVelocity.magnitude;
            angularVelocityCorrection = angularVelocityController.Update(angularVelocityError, Time.fixedDeltaTime);
            rb.AddTorque(turnThrust * -rb.angularVelocity.normalized * angularVelocityCorrection * Time.fixedDeltaTime);
        }
        else
        {
            rb.AddRelativeTorque(Input.GetAxis("Vertical") * turnThrust * Time.fixedDeltaTime * Vector3.left);
            rb.AddRelativeTorque(Input.GetAxis("Horizontal") * turnThrust * Time.fixedDeltaTime * Vector3.up);
            rb.AddRelativeTorque(Input.GetAxis("Rotate") * turnThrust * Time.fixedDeltaTime * Vector3.forward);
        }
    }

    void Thrust()
    {
        rb.AddRelativeForce(Input.GetAxis("Thrust") * forwardThrust * Time.fixedDeltaTime * Vector3.forward);
    }
}
