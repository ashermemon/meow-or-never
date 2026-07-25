using UnityEngine;

public class Respawn : MonoBehaviour
{
    [SerializeField] private Collider respawnZoneCollider;
    [SerializeField] private Vector3 respawnOffset = new Vector3(0f, 2f, -3f);
    [SerializeField] private bool useRotation = true;
    [SerializeField] private float deathYThreshold = 25f;
    
    [SerializeField] private LayerMask groundLayer; 

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (transform.position.y < deathYThreshold)
        {
            RespawnPlayer();
        }
    }

    private void RespawnPlayer()
    {
        Vector3 spawnPosition;

        if (respawnZoneCollider != null)
        {
          
            Vector3 closestPointInZone = respawnZoneCollider.ClosestPoint(transform.position);
            
   
            Vector3 raycastStart = new Vector3(closestPointInZone.x, 500f, closestPointInZone.z);
            if (Physics.Raycast(raycastStart, Vector3.down, out RaycastHit hit, 600f, groundLayer))
            {
               
                spawnPosition = hit.point + respawnOffset;
            }
            else
            {
               
                spawnPosition = closestPointInZone + respawnOffset;
            }

            if (useRotation) {transform.rotation = Quaternion.identity;}
        }
        else
        {
            spawnPosition = new Vector3(317.290009f, 59.4575691f, -112.980019f) + respawnOffset;
            if (useRotation) {transform.rotation = Quaternion.identity;}
        }

        transform.position = spawnPosition;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
