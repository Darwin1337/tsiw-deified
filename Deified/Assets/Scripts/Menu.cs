using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] bool hasGameStarted = false;

    public Animator animator;

    private int sceneToLoad;

    public GameObject CreditsPanel;

    public GameObject CreditsPanel2;

    public GameObject MainPanel;

    public AudioSource clickSound;

    public void startGame()
    {
        if (!hasGameStarted)
        {
            FadeToScene(1);
        }
    }

    public void FadeToScene(int sceneIndex)
    {
        sceneToLoad = sceneIndex;
        animator.SetTrigger("FadeOut");
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        Debug.Log("Game Started");
        hasGameStarted = true;
    }

    public void ToggleCredits(){
        clickSound.Play();
        bool isActive = CreditsPanel.activeSelf;
        CreditsPanel.SetActive(!isActive);
        MainPanel.SetActive(isActive);
    }

    public void ToggleCredits2(){
        clickSound.Play();
        bool isActive = CreditsPanel.activeSelf;
        CreditsPanel.SetActive(!isActive);
        CreditsPanel2.SetActive(isActive);
    }

    public void Quit()
    {
        clickSound.Play();
        Debug.Log("Quitting Game");
        Application.Quit();
    }
}
