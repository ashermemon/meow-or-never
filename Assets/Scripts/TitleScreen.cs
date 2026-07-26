using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] private Animator fadeAnim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(LoadHome());
    }

    // Update is called once per frame
    IEnumerator LoadHome()
    {
        yield return new WaitForSeconds(4f);

        fadeAnim.SetTrigger("FadeOut");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Home");
    }
}
