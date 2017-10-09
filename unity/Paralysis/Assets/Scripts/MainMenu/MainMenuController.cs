using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void ChangeScene(string sceneName){
        StartCoroutine(ChangeAnimation(sceneName));

    }
     
    IEnumerator ChangeAnimation(string sceneName){

        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene(sceneName);
    }
}
