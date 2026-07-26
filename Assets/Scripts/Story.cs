using UnityEngine;
using TMPro;

public class Story : MonoBehaviour
{
    public GameObject storyText;
    public GameObject storyTitle;
    public GameObject storyBtn;

    public GameObject[] toHide;
    public Camera camera;

    private bool tiltCamera = false;
    private Quaternion targetRotation;

    [SerializeField] private float tiltSpeed = 2f;

    void Update()
    {
        if (tiltCamera)
        {
            camera.transform.rotation = Quaternion.Lerp(
                camera.transform.rotation,
                targetRotation,
                Time.deltaTime * tiltSpeed
            );

            
            if (Quaternion.Angle(camera.transform.rotation, targetRotation) < 0.1f)
            {
                camera.transform.rotation = targetRotation;
                tiltCamera = false;
            }
        }
    }

    public void showStory()
    {
        for(int i = 0; i < toHide.Length; i++)
        {
            toHide[i].gameObject.SetActive(false);
        }

        storyText.gameObject.SetActive(true);
        storyTitle.gameObject.SetActive(true);
        storyBtn.gameObject.SetActive(true);

        Destroy(camera.GetComponent<CameraDrift>());

        targetRotation = Quaternion.Euler(
            camera.transform.eulerAngles.x - 40f,
            camera.transform.eulerAngles.y,
            camera.transform.eulerAngles.z
        );

        tiltCamera = true;
    }
}