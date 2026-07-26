using UnityEngine;

public class LocationIcon : MonoBehaviour
{

    [SerializeField] private float rotationSpeed = 50f;


    [SerializeField] private float hoverSpeed = 2f;   
    [SerializeField] private float hoverAmount = 0.2f; 

    private Vector3 startPosition;

    void Start()
    {
   
        startPosition = transform.position;
    }

    void Update()
    {
  
        transform.Rotate(rotationSpeed * Time.deltaTime, 0f, 0f, Space.Self);

        float newY = startPosition.y + Mathf.Sin(Time.time * hoverSpeed) * hoverAmount;
        
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}
