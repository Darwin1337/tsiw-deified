using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameOver : MonoBehaviour
{
    public static bool gameover = false;
    public static float metros;
    [SerializeField] Text mostradorBest;
    [SerializeField] Text mostradorScore;
    [SerializeField] GameObject painel;
    [SerializeField] GameObject gameController;
    public AudioSource gameOverSound;
    public GameObject hudElements;
    //void Start()
    //{
    //    Debug.Log(PlayerPrefs.GetFloat("Score"));
    //}
    void Update()
    {
        if (gameover)
        {

            painel.SetActive(true);
            hudElements.SetActive(false);
            gameOverSound.Play();
            Time.timeScale = 0f;
            mostradorBest.text = "BEST SCORE: " + PlayerPrefs.GetFloat("Score").ToString();
            mostradorScore.text = "SCORE: "+ metros;
            gameover = false;
        }
    }
    public void ReiniciarJogo()
    {
        gameController.GetComponent<GameController>().shouldSpawnFirstTwo = false;
        gameController.GetComponent<GameController>().spawnListObstacles.Clear();
        gameController.GetComponent<GameController>().spawnedPrefabList.Clear();
        Time.timeScale = 1f;
        painel.SetActive(false);
        gameover = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void GoToMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
