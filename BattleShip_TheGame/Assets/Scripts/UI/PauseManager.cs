using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Button pauseButton;
    [SerializeField] GameObject pausePanel;

    float defaulTimeScale;

    private void Awake()
    {
        pausePanel.SetActive(false);
        defaulTimeScale = Time.timeScale;
        Debug.Log(defaulTimeScale);
    }
    
    public void EnablePausePanel()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void ResumeGamePressed()
    {
        pausePanel.SetActive(false);
        Time.timeScale = defaulTimeScale;
    }

    public void RestartGamePressed()
    {
        Time.timeScale = defaulTimeScale;
    }
}
