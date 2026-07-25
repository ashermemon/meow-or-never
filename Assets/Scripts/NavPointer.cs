using UnityEngine;

public class NavPointerOrbitFix : MonoBehaviour
{
    [SerializeField] Transform destination;
    [SerializeField] float rotationSpeed;
    [SerializeField] Transform car;

    [SerializeField] private float hoverHeight = 1.2f; 
    [SerializeField] private float zOffset = 3.5f; // Pro-tip: Shrinking this slightly keeps it closer to the car on hills!
    [SerializeField] private LayerMask groundLayer; 

    void Update()
    {
        if (destination != null && car != null)
        {
            Vector3 direction = destination.position - car.position;
            // direction.y = 0;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);   
                
                float modifierX = zOffset * Mathf.Sin(transform.eulerAngles.y * Mathf.Deg2Rad);
                float modifierZ = zOffset * Mathf.Cos(transform.eulerAngles.y * Mathf.Deg2Rad);
                
           
                Vector3 targetPos = new Vector3(car.position.x + modifierX, car.position.y + 50f, car.position.z + modifierZ);

                RaycastHit hit;
         
                if (Physics.Raycast(targetPos, Vector3.down, out hit, 100f, groundLayer))
                {
                    targetPos.y = hit.point.y + hoverHeight;
                }
                else
                {
                    targetPos.y = car.position.y + hoverHeight;
                }

                transform.position = targetPos;
            }
        }
    }
}
