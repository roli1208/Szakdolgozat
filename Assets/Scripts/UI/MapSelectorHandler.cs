using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapSelectorHandler : MonoBehaviour
{
    public void onMapselectorBackButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void onFirstMapClick()
    {
        SceneManager.LoadScene("Map1");
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
