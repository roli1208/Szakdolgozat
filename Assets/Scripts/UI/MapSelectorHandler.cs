using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapSelectorHandler : MonoBehaviour
{
    [SerializeField] GridLayoutGroup gridLayoutGroup;
    [SerializeField] GameObject mapButton;
    static string mapName;
    public void onMapselectorBackButton()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void startMap()
    {
        SceneManager.LoadSceneAsync("PlayMap");
    }
    // Start is called before the first frame update
    void Start()
    {
        string filePath = Application.persistentDataPath + "/maps/";
        if (!Directory.Exists(filePath))
           Directory.CreateDirectory(filePath);
        List<string> mapNames = FileHandler.getMapNames(filePath);
        GameObject btn;
        foreach (string name in mapNames)
        {
            Debug.Log("name: " + name);
            if (!name.Contains("waypoint"))
            {
                btn = Instantiate(mapButton);
                btn.name = name.Split('.')[0];
                btn.GetComponentInChildren<TextMeshProUGUI>().text = name.Split('.')[0];
                btn.SetActive(true);
                btn.transform.SetParent(gridLayoutGroup.transform);
                btn.transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
