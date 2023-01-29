using Firebase.Auth;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonClickHandler : MonoBehaviour
{
    public GameObject optionPanel;
    public Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public Dropdown qualityDropdown;
    Resolution[] resolutions;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
        optionPanel.SetActive(false);

        int currentResIndex = 0;
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int i = 0;
        foreach (Resolution resolution in resolutions)
        {
            string option = resolution.width + " x " + resolution.height + " @ " + resolution.refreshRate + "hz";
            options.Add(option);
            i++;
            if (resolution.width == Screen.currentResolution.width && resolution.height == Screen.currentResolution.height)
            {
                currentResIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResIndex;
        resolutionDropdown.RefreshShownValue();

        QualitySettings.SetQualityLevel(3);
    }
    public void onLogoutClick()
    {
        FirebaseAuth.DefaultInstance.SignOut();
        Debug.Log("Logged out");
        SceneManager.LoadScene("Authentication");
    }
    public void onExitClick()
    {
        Application.Quit();
    }
    public void onOptionsClick()
    {
        optionPanel.SetActive(true);
    }

    public void setResolution()
    {
        int resIndex = resolutionDropdown.value;
        Screen.SetResolution(resolutions[resIndex].width, resolutions[resIndex].height, Screen.fullScreen);
    }

    public void setFullScreen()
    {
        Screen.fullScreen = fullscreenToggle.isOn;
    }
    public void showOptionsPanel()
    {
        optionPanel.gameObject.SetActive(true);
    }
    public void onBackButtonClick()
    {
        optionPanel.gameObject.SetActive(false);
    }
    public void setQuality()
    {
        QualitySettings.SetQualityLevel(qualityDropdown.value);
    }
    public void onClickSnigleplayer()
    {
        SceneManager.LoadScene("MapSelector");
    }
}
