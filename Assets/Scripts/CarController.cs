using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    public Rigidbody rb;
    private InputSystem_Actions controls;

    [SerializeField] private float forwardAccel =8f, reverseAccel=4f, maxSpeed=50f, turnStrength=180f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private float speedInput, turnInput;
   
    void Awake(){
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
        
        rb.transform.parent = null;
        
    }

    // Update is called once per frame
    void Update()
    {
        float drive = controls.Player.Drive.ReadValue<float>();

        speedInput = 0f;

        if (drive > 0)
        {
            speedInput = drive * forwardAccel * 1000f;
        }
        else if (drive < 0)
        {
            speedInput = drive * reverseAccel * 1000f;
        }

        turnInput = controls.Player.Steer.ReadValue<float>();
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput *turnStrength * Time.deltaTime *drive, 0f));
       transform.position = rb.transform.position;
        
    }
    void FixedUpdate() {
        if(Mathf.Abs(speedInput)>0){
            rb.AddForce(transform.forward * speedInput);
        }
    }
}
