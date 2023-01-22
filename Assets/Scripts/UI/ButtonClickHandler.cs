using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonClickHandler : MonoBehaviour
{
    Button yourButton;
    public GameObject optionPanel;
    // Start is called before the first frame update
    void Start()
    {
        yourButton = GetComponent<Button>();
        yourButton.onClick.AddListener(clickedOnButton);
        optionPanel.gameObject.SetActive(false);
    }

    private void clickedOnButton()
    {
        switch (yourButton.name)
        {
            case "SingleplayerButton":
                SceneManager.LoadScene("SampleScene");
                break;
            case "ExitButton":
                Application.Quit();
                break;
            case "OptionsButton":
                optionPanel.gameObject.SetActive(true);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
