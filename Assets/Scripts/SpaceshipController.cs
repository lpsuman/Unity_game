using UnityEngine;

public class SpaceshipController : MonoBehaviour
{
    public SpaceshipData spaceshipData;
    Rigidbody rb;
    public KeyCode toggleStrafeKeyID = KeyCode.LeftControl;
    public KeyCode stopMovementKeyID = KeyCode.LeftAlt;

    private PID angularPID;
    public float angularPIDstartingP = 10.0f;
    public float angularPIDstartingI = 0.0f;
    public float angularPIDstartingD = 0.1f;
    private bool isStoppingRotation;
    private bool wasRotationAxisInputPresentLastUpdate;
    //public for testing purposes
    public float angularVelocityError;
    public float angularVelocityCorrection;

    private PID movementPID;
    public float movementPIDstartingP = 1.0f;
    public float movementPIDstartingI = 0.0f;
    public float movementPIDstartingD = 0.01f;
    private bool isStoppingMovement;
    //public for testing purposes
    public float movementError;
    public float movementCorrection;

    private float inputAxisVertical;
    private float inputAxisHorizontal;
    private float inputAxisRotate;
    private float inputAxisThrust;

    

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = spaceshipData.mass;
        rb.useGravity = false;
        angularPID = new PID(angularPIDstartingP, angularPIDstartingI, angularPIDstartingD);
        isStoppingRotation = false;
        wasRotationAxisInputPresentLastUpdate = false;
        movementPID = new PID(movementPIDstartingP, movementPIDstartingI, movementPIDstartingD);
        isStoppingMovement = false;
    }

    void Update()
    {
        Debug.DrawLine(transform.position, transform.position + rb.angularVelocity, Color.cyan);
    }

    private void FixedUpdate()
    {
        readInputAxii();
        Turn();
        Thrust();
    }

    private void readInputAxii()
    {
        inputAxisVertical = Input.GetAxis("Vertical");
        inputAxisHorizontal = Input.GetAxis("Horizontal");
        inputAxisRotate = Input.GetAxis("Rotate");
        inputAxisThrust = Input.GetAxis("Thrust");
    }

    void Turn()
    {
        float deltaTimeRotationThrust = spaceshipData.rotationThrust * Time.fixedDeltaTime;
        bool isStrafingEnabled = Input.GetKey(toggleStrafeKeyID);
        if (!isStrafingEnabled)
        {
            if (inputAxisVertical != 0.0f)
            {
                rb.AddRelativeTorque(inputAxisVertical * deltaTimeRotationThrust * Vector3.left);
            }
            if (inputAxisHorizontal != 0.0f)
            {
                rb.AddRelativeTorque(inputAxisHorizontal * deltaTimeRotationThrust * Vector3.up);
            }
        }
        if (inputAxisRotate != 0.0f)
        {
            rb.AddRelativeTorque(inputAxisRotate * deltaTimeRotationThrust * Vector3.forward);
        }

        bool isAxisInputPresent = (!isStrafingEnabled && (inputAxisVertical != 0.0f || inputAxisHorizontal != 0.0f)) || inputAxisRotate != 0.0f;
        if (wasRotationAxisInputPresentLastUpdate && !isAxisInputPresent)
        {
            angularPID.Reset();
            angularPID.SetFactors(angularPIDstartingP, angularPIDstartingI, angularPIDstartingD);
            //Debug.Log(string.Format("new pid with: {0} {1} {2}", startingP, startingI, startingD));
            isStoppingRotation = true;
        }
        else if (!wasRotationAxisInputPresentLastUpdate && isAxisInputPresent)
        {
            isStoppingRotation = false;
        }
        if (isStoppingRotation)
        {
            angularVelocityError = rb.angularVelocity.magnitude;
            angularVelocityCorrection = angularPID.Update(angularVelocityError, Time.fixedDeltaTime);
            rb.AddTorque(angularVelocityCorrection * deltaTimeRotationThrust * -rb.angularVelocity.normalized);
        }
        wasRotationAxisInputPresentLastUpdate = isAxisInputPresent;
    }

    void Thrust()
    {
        if (Input.GetKey(stopMovementKeyID))
        {
            if (!isStoppingMovement)
            {
                movementPID.Reset();
                movementPID.SetFactors(movementPIDstartingP, movementPIDstartingI, movementPIDstartingD);
                isStoppingMovement = true;
            }
        }
        else
        {
            isStoppingMovement = false;
        }

        float deltaTimeForwardThrust = spaceshipData.forwardThrust * Time.fixedDeltaTime;
        if (isStoppingMovement)
        {
            movementError = rb.velocity.magnitude;
            movementCorrection = movementPID.Update(movementError, Time.fixedDeltaTime);
            rb.AddForce(movementCorrection * deltaTimeForwardThrust * -rb.velocity.normalized);
        }
        else
        {
            if (Input.GetKey(toggleStrafeKeyID))
            {
                float deltaTimeStrafeThrust = spaceshipData.strafeThrust * Time.fixedDeltaTime;
                if (inputAxisVertical != 0.0f)
                {
                    rb.AddRelativeForce(inputAxisVertical * deltaTimeStrafeThrust * Vector3.right);
                }
                if (inputAxisHorizontal != 0.0f)
                {
                    rb.AddRelativeForce(inputAxisHorizontal * deltaTimeStrafeThrust * Vector3.up);
                }
            }
            if (inputAxisThrust != 0.0f)
            {
                rb.AddRelativeForce(inputAxisThrust * deltaTimeForwardThrust * Vector3.forward);
            }
        }
        
    }
}
