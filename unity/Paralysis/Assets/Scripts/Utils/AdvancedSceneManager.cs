using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AdvancedSceneManager : MonoBehaviour {

    static string toLoad;
    static string loadingScreenScene;

    AsyncOperation operation;

    public static void LoadSceneWithLoadingScreen(string Scene, string pLoadingScreenScene)
    {
        Application.backgroundLoadingPriority = ThreadPriority.High;
        toLoad = "Scenes/" + Scene;
        loadingScreenScene = "Scenes/" + pLoadingScreenScene;
        SceneManager.LoadScene(pLoadingScreenScene);
    }

    void Start()
    {
        if (toLoad == null || loadingScreenScene == null)
            return;

        StartCoroutine(LoadSceneAsync());
    }


    public IEnumerator LoadSceneAsync()
    {
        Debug.Log("loading scene " + toLoad);
        yield return null;

        operation = SceneManager.LoadSceneAsync(toLoad, LoadSceneMode.Single);
        operation.allowSceneActivation = true;

        while (operation.progress < 0.9f)
        {
            Debug.Log(operation.progress);
            yield return null;
        }

    }
}
