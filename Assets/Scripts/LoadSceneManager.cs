using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading;
using UnityEngine.EventSystems;

public class LoadSceneManager : MonoBehaviour
{
    public GameObject LoadingScreen;
    public GameObject MapselectorPanel;
    public Image LoadingBarFill;
    static public string mapName;

    public void LoadScene(string name)
    {
        StartCoroutine(LoadSceneAsync(name));
        mapName = EventSystem.current.currentSelectedGameObject.name;
    }

    IEnumerator LoadSceneAsync(string name)
    {
        AsyncOperation sceneOperation = SceneManager.LoadSceneAsync(name);

        MapselectorPanel.SetActive(false);

        LoadingScreen.SetActive(true);

        Thread.Sleep(500);

        while (!sceneOperation.isDone)
        {
            float progress = Mathf.Clamp01(sceneOperation.progress / 0.9f);
            LoadingBarFill.fillAmount = progress;

            yield return null;
        }
    }
}
