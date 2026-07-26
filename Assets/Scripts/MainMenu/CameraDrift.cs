using UnityEngine;
using UnityEngine.SceneManagement;
public class CameraDrift : MonoBehaviour
{

    

    [SerializeField] private float rotateSpeed = 2f;   
    [SerializeField] private float rotateAmount = 4f; 
    private Vector3 startEuler;


    public void playGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void exit()
    {
        Application.Quit();
    }


    void Start()
    {
        startEuler = transform.localEulerAngles;
    }

    void Update()
    {
        float newX = startEuler.x + Mathf.Sin(Time.time * rotateSpeed) * rotateAmount;

        transform.localRotation = Quaternion.Euler(
            newX,
            startEuler.y,
            startEuler.z
        );
    }
}
