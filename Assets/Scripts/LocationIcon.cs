using UnityEngine;

public class LocationIcon : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 50f;

    [Header("Hover Settings")]
    [SerializeField] private float hoverSpeed = 2f;   
    [SerializeField] private float hoverAmount = 0.2f; 

    private Vector3 startPosition;

    void Start()
    {
   
        startPosition = transform.position;
    }

    void Update()
    {
  
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime, Space.Self);

        float newY = startPosition.y + Mathf.Sin(Time.time * hoverSpeed) * hoverAmount;
        
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}
