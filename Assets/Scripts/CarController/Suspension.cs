
using UnityEngine;

public class Suspension : MonoBehaviour
{
    [SerializeField] private Transform[] wheels;
    [SerializeField] private Transform[] visualWheels;
    [SerializeField] private LayerMask groundLayer;

    [Header("Suspension Settings")]
    [SerializeField] private float restLength = 1.0f;     // Target length of the spring
    [SerializeField] private float springTravel = 0.5f;   // Maximum compression travel
    [SerializeField] private float springStiffness = 30000f; 
    [SerializeField] private float damperStiffness = 2000f;
    [SerializeField] private float wheelRadius = 0.3490233f;

    private Rigidbody rb;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0); 
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void FixedUpdate() {
        
     for(int i=0; i<  wheels.Length; i++)
        {
            
            SuspensionForces(wheels[i],visualWheels[i]);
        }
    }

    void SuspensionForces(Transform wheel, Transform visual)
    {
        float maxLength = restLength + springTravel;
        float rayLength = maxLength + wheelRadius;

        if(Physics.Raycast(wheel.position, -wheel.up, out RaycastHit hit, rayLength, groundLayer))
        {
            // 1. Calculate the current length of the spring   
            float springLength = hit.distance -wheelRadius;

            float compression = restLength - springLength; 

            

            Vector3 vehicleVelocity = rb.GetPointVelocity(wheel.position);
            float springVelocity = Vector3.Dot(wheel.up, vehicleVelocity);

            float springForce = compression * springStiffness;
            float dampingForce = springVelocity * damperStiffness;

            float totalForce = springForce - dampingForce;
            
            if (totalForce > 0)
            {
                rb.AddForceAtPosition(wheel.up * totalForce, wheel.position);
            }

        Vector3 targetWorldPos = hit.point + (wheel.up * wheelRadius);
        visual.position = targetWorldPos;

      
        }
    }
 
}
