using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{

    public GameObject credits;
    public GameObject setting;


    private void Awake()
    {
        this.credits.SetActive(false);
        this.setting.SetActive(false);
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void CreditsButton ()
    {
        this.credits.SetActive(true);

    }

    public void SettingButton()
    {
        this.setting.SetActive(true);

    }

    public void ChangeScene(string sceneName){
        StartCoroutine(ChangeAnimation(sceneName));

    }


    public void QuitGame()
    {

        Debug.Log("Quit Game");
        Application.Quit();

    }
     
    IEnumerator ChangeAnimation(string sceneName){

        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene(sceneName);
    }
}
