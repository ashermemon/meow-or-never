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
    [SerializeField] private float frontTireGrip = 0.6f;   // og 0.95
    [SerializeField] private float backTireGrip = 0.4f;    // og 0.90
    [SerializeField] private float driftFrontGrip = 0.35f; // og 0.70
    [SerializeField] private float driftBackGrip = 0.05f;  // og 0.15
    [SerializeField] private float tireMass = 40f;  
    
    [SerializeField] private float maxSteerAngle = 35f;
    [SerializeField] private float steerTorqueStrength = 12f;
    [SerializeField] private float steerSpeed = 360f;

    [Header("Input Settings")]
    [SerializeField] private float forwardAccel = 25f;
    [SerializeField] private float reverseAccel = 10f;
    [SerializeField] private float maxSpeed = 50f;

    [Header("Air Gravity / Downforce")]
    [SerializeField] private float airborneGravityMultiplier = 2f;
    [SerializeField] private float downforceValue = 15f;

    public GameObject tireTrail;

    private float speedInput, turnInput, drive;
    private Rigidbody rb;
    private InputSystem_Actions controls;
    private float[] wheelSpinAngles;

    private bool isGrounded;
    private bool isDrifting;

// private bool isSteering;

    public TrailRenderer FRWheel;
    public TrailRenderer FLWheel;
    public TrailRenderer BRWheel;
    public TrailRenderer BLWheel;

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
        rb.centerOfMass = new Vector3(0, -1.5f, 0);

        wheelSpinAngles = new float[wheels.Length];
    }


    void Update()
    {
        drive = controls.Player.Drive.ReadValue<float>();
        isDrifting = controls.Player.DriftHandBrake.ReadValue<float>() > 0.1f;
        turnInput = controls.Player.Steer.ReadValue<float>();

        HandleTrails(isDrifting && isGrounded);

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

    private void HandleTrails(bool emit)
    {
        if (FRWheel != null) {FRWheel.emitting = emit;}
        if (FLWheel != null) {FLWheel.emitting = emit;}
        if (BRWheel != null) {BRWheel.emitting = emit;}
        if (BLWheel != null) {BLWheel.emitting = emit;}
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

            SnapTrailsToHills(wheels[i], i);
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
            Vector3 downforceVector = -transform.up * Mathf.Abs(forwardSpeed) * downforceValue;
            rb.AddForce(downforceVector, ForceMode.Force);
        }
    }

void SnapTrailsToHills(Transform wheel, int index)
{
 
    if (Physics.Raycast(wheel.position + (transform.up * 0.5f), -transform.up, out RaycastHit hit, 2f, groundLayer))
    {
       
        TrailRenderer currentTrail = null;
        if (index == 0) currentTrail = FRWheel;
        else if (index == 1) currentTrail = FLWheel;
        else if (index == 2) currentTrail = BRWheel;
        else if (index == 3) currentTrail = BLWheel;

        if (currentTrail != null)
        {
            
            currentTrail.transform.position = hit.point + (hit.normal * 0.02f);
            

            currentTrail.transform.rotation = Quaternion.LookRotation(transform.forward, hit.normal);
        }
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


            float grip = isDrifting ? isFrontWheel ? driftFrontGrip : driftBackGrip : isFrontWheel ? frontTireGrip : backTireGrip;


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

            Vector3 sidewaysForceDirection = wheelDirection;
            sidewaysForceDirection.y = 0f; 
            sidewaysForceDirection.Normalize();


            rb.AddForceAtPosition(
                sidewaysForceDirection * targetAccel * tireMass,
                wheel.position
            );

            // rb.AddForceAtPosition(
            //     wheelDirection * targetAccel * tireMass,
            //     wheel.position
            // );



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