using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeIn : MonoBehaviour
{
    [SerializeField] private Animator fadeAnim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fadeAnim.SetTrigger("FadeIn");
    }

    public void FadeOut()
    {
        StartCoroutine(LoadGame());
    }

    IEnumerator LoadGame()
    {
        
        fadeAnim.SetTrigger("FadeOut");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Game");
    }

 
}
