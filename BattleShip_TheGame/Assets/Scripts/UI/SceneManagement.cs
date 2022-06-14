using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneManagement : MonoBehaviour
{
    [SerializeField] GameObject transitionGameObjects;
    public Animator transition;
    public float transitionTime = 1f;

    [SerializeField] GameObject pauseMenu;
    GamePhase gamePhase;
    private void Awake()
    {
        transitionGameObjects.SetActive(true);
        gamePhase = FindObjectOfType<GamePhase>();
    }

    public void ResetCurrentScene()
    {
        Time.timeScale = 1f;
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex));
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        StartCoroutine(LoadLevel(0));
    }

    public void PlayGamePressed()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    IEnumerator LoadLevel(int level)
    {
        transition.SetTrigger("Start");


        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(level);
    }

    public void PauseButtonPressed()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;

        gamePhase.DisableGameTile();
        gamePhase.DisablePrepTile();
    }

    public void ResumeButtonPressed()
    {
        Time.timeScale = 1f;

        pauseMenu.SetActive(false);

        gamePhase.DisableGameTile();
        gamePhase.DisablePrepTile();
    }

    public void ExitApplication()
    {
        Application.Quit();
    }
}
