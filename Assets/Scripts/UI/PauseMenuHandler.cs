using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuHandler : MonoBehaviour
{
    public GameObject pausePanel;
    // Start is called before the first frame update
    void Start()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pausePanel.gameObject.SetActive(true);
            Time.timeScale = 0f;
        }
    }
    public void onQuitClick()
    {
        Application.Quit();
    }
    public void onMainMenuClick()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void onResumeClick()
    {
        pausePanel.gameObject.SetActive(false);
        Time.timeScale = 1.0f;
    }
}
