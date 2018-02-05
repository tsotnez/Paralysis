using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuButton : MonoBehaviour {

    public CanvasGroup nextPage;

	public void MouseEnter()
    {
        transform.Find("Hover").gameObject.SetActive(true);
    }

    public void activateAnimator()
    {
        GetComponent<Animator>().SetBool("spin", true);
    }

    public void deActivateAnimator()
    {
        GetComponent<Animator>().SetBool("spin", false);
    }

    public void MouseExit()
    {
        transform.Find("Hover").gameObject.SetActive(false);
    }

    public void changePage()
    {
        if(nextPage != null)
            FindObjectOfType<MainMenuManager>().switchToPage(nextPage);
    }

    public void local1v1()
    {
        TeamAllocationManager.maxPlayers = 2;
        SceneManager.LoadScene("Scenes/TeamAllocation");
    }

    public void local2v2()
    {
        TeamAllocationManager.maxPlayers = 4;
        SceneManager.LoadScene("Scenes/TeamAllocation");
    }

    public void loadMainMenuPageFromExternal(int index)
    {
        MainMenuManager.DefaultPageIndex = index;
        SceneManager.LoadScene("Scenes/MainMenu");
    }

    public void quit()
    {
        Application.Quit();
    }

    public void loadScene(string toLoad)
    {
        SceneManager.LoadScene("Scenes/" + toLoad);
    }


    //Spinning into ModeSelection
    public void switchToModeSelection()
    {
        StartCoroutine(setModeSelectionActive());
    }

    public void leaveModeSelection()
    {
        StartCoroutine(setModeSelectionInActive());
    }

    public void AAAAAA()
    {
        AdvancedSceneManager.LoadSceneWithLoadingScreen("MainMenu", "MultiplayerLoadingScreen");
        //SceneManager.LoadScene("Scenes/test");
    }

    private IEnumerator setModeSelectionInActive()
    {
        GameObject target = transform.Find("ModeSelection").gameObject;
        
        GetComponent<EventTrigger>().enabled = true;
        GetComponent<Animator>().SetTrigger("Spin");
        GetComponent<Button>().interactable = true;

        yield return new WaitForSeconds(.33f);
        target.SetActive(false);
        transform.Find("Symbol").gameObject.SetActive(true);

        if (MyStandaloneInputModule.ControllingPlayerInputDevice == UserControl.InputDevice.XboxController)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }

    private IEnumerator setModeSelectionActive()
    {
        GameObject target = transform.Find("ModeSelection").gameObject;
        MouseExit();
        GetComponent<EventTrigger>().enabled = false;
        GetComponent<Animator>().SetTrigger("Spin");
        GetComponent<Button>().interactable = false;

        yield return new WaitForSeconds(.33f);
        transform.Find("Symbol").gameObject.SetActive(false);
        target.SetActive(true);

        if (MyStandaloneInputModule.ControllingPlayerInputDevice == UserControl.InputDevice.XboxController)
        {
            EventSystem.current.SetSelectedGameObject(target.transform.GetChild(0).gameObject);
        }
    }
}
