using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonPressHandler : MonoBehaviour
{
    public Button yourButton;
    // Start is called before the first frame update
    void Start()
    {
        yourButton = GetComponent<Button>();
        yourButton.onClick.AddListener(clickedStart);
    }

    private void clickedStart()
    {
        SceneManager.LoadScene("SampleScene");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
