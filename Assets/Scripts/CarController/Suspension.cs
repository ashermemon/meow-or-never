using UnityEngine;

public class Suspension : MonoBehaviour
{
    [SerializeField] private Transform[] wheels;
    [SerializeField] private Transform[] visualWheels;
    [SerializeField] private LayerMask groundLayer;

    [Header("Suspension Settings")]
    [SerializeField] private float restLength = 0.5f;
    [SerializeField] private float springTravel = 0.3f;
    [SerializeField] private float springStiffness = 45000f;
    [SerializeField] private float damperStiffness = 3500f;
    [SerializeField] private float wheelRadius = 0.35f;

    [Header("Steering Settings")]
    [SerializeField] private float frontTireGrip = 0.8f;
    [SerializeField] private float backTireGrip = 0.4f;
    [SerializeField] private float tireMass = 20f;
    [SerializeField] private float maxSteerAngle = 35f;
    [SerializeField] private float steerTorqueStrength = 12f;
    [SerializeField] private float steerSpeed = 360f;

    [Header("Input Settings")]
    [SerializeField] private float forwardAccel = 25f;
    [SerializeField] private float reverseAccel = 10f;
    [SerializeField] private float maxSpeed = 50f;

    [Header("Air Gravity")]
    [SerializeField] private float airborneGravityMultiplier = 2f;

    private float speedInput, turnInput, drive;
    private Rigidbody rb;
    private InputSystem_Actions controls;
    private float[] wheelSpinAngles;

    private bool isGrounded;



    void Awake()
    {
        controls = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.6f, 0);

        wheelSpinAngles = new float[wheels.Length];
    }


    void Update()
    {
        drive = controls.Player.Drive.ReadValue<float>();
        turnInput = controls.Player.Steer.ReadValue<float>();

        if (drive > 0f)
        {
            speedInput = drive * forwardAccel;
        }
        else if (drive < 0f)
        {
            speedInput = drive * reverseAccel;
        }
        else
        {
            speedInput = 0f;
        }
    }


    void FixedUpdate()
    {
        int groundedWheels = 0;

        float targetSteerAngle = turnInput * maxSteerAngle;
        float forwardSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);


     
        isGrounded = false;


        for (int i = 0; i < wheels.Length; i++)
        {
            bool isFrontWheel = i < 2;

            if (SuspensionForces(wheels[i], visualWheels[i], isFrontWheel, i))
            {
                groundedWheels++;
            }


            if (isFrontWheel)
            {
                wheels[i].localRotation = Quaternion.RotateTowards(
                    wheels[i].localRotation,
                    Quaternion.Euler(0f, targetSteerAngle, 0f),
                    steerSpeed * Time.fixedDeltaTime
                );
            }
            else
            {
                wheels[i].localRotation = Quaternion.RotateTowards(
                    wheels[i].localRotation,
                    Quaternion.identity,
                    steerSpeed * Time.fixedDeltaTime
                );
            }


            float spinDelta = (forwardSpeed / wheelRadius) * Time.fixedDeltaTime * Mathf.Rad2Deg;
            wheelSpinAngles[i] += spinDelta;
        }



        if (groundedWheels > 0)
        {
            isGrounded = true;
        }

        if (!isGrounded)
        {
            rb.AddForce(Physics.gravity * (airborneGravityMultiplier - 1f), ForceMode.Acceleration);
        }

   

        if (isGrounded)
        {
            if (speedInput > 0f && forwardSpeed < maxSpeed)
            {
                rb.AddForce(transform.forward * speedInput, ForceMode.Acceleration);
            }
            else if (speedInput < 0f && forwardSpeed > -maxSpeed)
            {
                rb.AddForce(transform.forward * speedInput, ForceMode.Acceleration);
            }


            float steeringFactor = Mathf.Clamp01(0.25f + Mathf.Abs(forwardSpeed) / maxSpeed);

            float steerTorque = turnInput * steeringFactor * steerTorqueStrength;

            rb.AddRelativeTorque(
                Vector3.up * steerTorque,
                ForceMode.Acceleration
            );
        }
    }



    bool SuspensionForces(Transform wheel, Transform visual, bool isFrontWheel, int wheelIndex)
    {
        float maxLength = restLength + springTravel;
        float rayLength = maxLength + wheelRadius;

        Vector3 suspensionDown = -transform.up;


        Quaternion spinRotation = Quaternion.AngleAxis(
            wheelSpinAngles[wheelIndex],
            Vector3.right
        );

        visual.localRotation = wheel.localRotation * spinRotation;



        if (Physics.Raycast(
            wheel.position,
            suspensionDown,
            out RaycastHit hit,
            rayLength,
            groundLayer))
        {

            float springLength = Mathf.Max(0f, hit.distance - wheelRadius);
             

            float compression = Mathf.Clamp(
                restLength - springLength,
                0f,
                springTravel
            );


            float grip = isFrontWheel ? frontTireGrip : backTireGrip;


            Vector3 vehicleVelocity = rb.GetPointVelocity(wheel.position);

            float springVelocity = Vector3.Dot(
                transform.up,
                vehicleVelocity
            );


            float springForce = compression * springStiffness;

            float dampingForce = springVelocity * damperStiffness;


            float totalForce = Mathf.Max(
                0f,
                springForce - dampingForce
            );


            rb.AddForceAtPosition(
                transform.up * totalForce,
                wheel.position,
                ForceMode.Force
            );



            Vector3 wheelDirection = wheel.right;

            Vector3 tireVelocity = rb.GetPointVelocity(wheel.position);

            float slideVelocity = Vector3.Dot(
                wheelDirection,
                tireVelocity
            );


            float velocityChange = -slideVelocity * grip;

            float targetAccel = velocityChange / Time.fixedDeltaTime;


            rb.AddForceAtPosition(
                wheelDirection * targetAccel * tireMass,
                wheel.position
            );

            visual.position = wheel.position + suspensionDown * springLength;
            return true;
        }
        else
        {
            visual.position = wheel.position + suspensionDown * maxLength;
        }


        return false;
    }
}