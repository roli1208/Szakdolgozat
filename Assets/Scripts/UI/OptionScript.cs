using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionScript : MonoBehaviour
{
    Resolution[] resolutions;

    public Dropdown dropdown;

    public GameObject optionPanel;
    // Start is called before the first frame update
    void Start()
    {
        int currentResIndex = 0;
        resolutions = Screen.resolutions;

        dropdown.ClearOptions();
        
        List<string> options = new List<string>();
        int i = 0;
        foreach (Resolution resolution in resolutions)
        {
            string option = resolution.width + " x " + resolution.height + " @ " + resolution.refreshRate + "hz"; 
            options.Add(option);
            i++;
            if(resolution.width == Screen.currentResolution.width && resolution.height == Screen.currentResolution.height)
            {
                currentResIndex = i;
            }
        }
        dropdown.AddOptions(options);
        dropdown.value = currentResIndex;
        dropdown.RefreshShownValue();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void setResolution(int resIndex)
    {
        Screen.SetResolution(resolutions[resIndex].width, resolutions[resIndex].height,Screen.fullScreen);
    }

    public void setFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    public void onBackButtobClick()
    {
        optionPanel.gameObject.SetActive(false);
    }
}
