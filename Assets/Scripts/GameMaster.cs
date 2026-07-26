using UnityEngine;
using TMPro;

public class GameMaster : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdown;
    [SerializeField] private float startingTime = 90.00f;

    [SerializeField] private float regularFontSize = 80f;
    [SerializeField] private float alertFontSize = 100f;
    [SerializeField] private float resizeSpeed = 50f;

    [SerializeField] private Color startColor = Color.blue;
    [SerializeField] private Color endColor = Color.red;

    [SerializeField] private Color startColorGlow = Color.orange;
    [SerializeField] private Color endColorGlow = Color.green;

    [SerializeField] private float speedGlow = 0.5f;

    private Material materialInstance;

    private float timeRemaining;
    private float targetFontSize;

    private float startCountdown = 4f;
    private bool gameStarted = false;

    [SerializeField] Rigidbody rb;

    void Start()
    {

        rb.isKinematic = true; 
        rb.useGravity = false;
        countdown.ForceMeshUpdate();

        materialInstance = countdown.fontMaterial;
        materialInstance.EnableKeyword("GLOW_ON");

        countdown.fontSize = regularFontSize;
        targetFontSize = regularFontSize;

        countdown.text = "Starting in 3";
    }


    void Update()
    {
      
        if (!gameStarted)
        {
            startCountdown -= Time.deltaTime;

            if (startCountdown > 3)
            {
                countdown.text = "Starting in 3";
            }
            else if (startCountdown > 2)
            {
                countdown.text = "Starting in 2";
            }
            else if (startCountdown > 1)
            {
                countdown.text = "Starting in 1";
            }
            else if (startCountdown > 0)
            {   
                rb.isKinematic = false; 
                rb.useGravity = true;
                countdown.text = "GO!";
            }
            else
            {
     
                gameStarted = true;
                timeRemaining = startingTime;
            }

            return;
        }


    
        if (timeRemaining > 0)
        {
            countdown.color = Color.Lerp(startColor, endColor, 
                (startingTime - timeRemaining) / startingTime);

            speedGlow = Mathf.Lerp(0.5f, 2f, 
                (startingTime - timeRemaining) / startingTime);

            float t = Mathf.PingPong(Time.time * speedGlow, 1.0f);
            Color currentGlowColor = Color.Lerp(startColorGlow, endColorGlow, t);
            materialInstance.SetColor(ShaderUtilities.ID_GlowColor, currentGlowColor);

            countdown.UpdateMeshPadding();

            timeRemaining -= Time.deltaTime;
            countdown.text = timeRemaining.ToString("00.00");
        }
        else
        {
            timeRemaining = 0;
            countdown.text = "00.00";
            OnTimerEnd();
        }


        float secondsInt = Mathf.Floor(timeRemaining);

        if (timeRemaining > 0 && secondsInt % 10 == 0)
        {
            targetFontSize = alertFontSize;
        }
        else
        {
            targetFontSize = regularFontSize;
        }

        countdown.fontSize = Mathf.MoveTowards(
            countdown.fontSize,
            targetFontSize,
            resizeSpeed * Time.deltaTime
        );
    }


    void OnTimerEnd()
    {

    }
}