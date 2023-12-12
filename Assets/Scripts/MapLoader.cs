using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapLoader : MonoBehaviour
{
    [SerializeField] SaveHandler handler;
    private void Awake()
    {
        Invoke("loadMap",0.05f);
    }
    public void loadMap()
    {
        LoadSceneManager loadSceneManager = new LoadSceneManager();
        string name = LoadSceneManager.mapName;
        handler.onLoad(name + ".json");

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}