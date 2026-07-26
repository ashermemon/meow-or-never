using UnityEngine;

public class MenuCarBounce : MonoBehaviour
{

    [SerializeField] private float bounceHeight = 0.1f;
    [SerializeField] private float bounceSpeed = 2f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    private Vector3 startPos;
    void Start()
    {
        startPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = startPos;
        pos.y += Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;

        transform.localPosition = pos;
    }
}
