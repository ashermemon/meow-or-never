using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinCondition : MonoBehaviour
{
     [SerializeField] private Animator fadeAnim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Finish")){
            FadeOut();  
        }
         
    }
    void FadeOut()
    {
        StartCoroutine(LoadGame("Win"));
    }

    IEnumerator LoadGame(string scene)
    {
        
        fadeAnim.SetTrigger("FadeOut");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(scene);
    }

}
