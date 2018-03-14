using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuButton : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler {

    public CanvasGroup nextPage;
    GameObject hover = null;

    void Start()
    {
        Transform t = transform.Find("Hover");
        if(t != null)
            hover = t.gameObject;
    }


	public void MouseEnter()
    {
        if(hover != null)
            hover.SetActive(true);
    }

    public void MouseExit()
    {
        if(hover != null)
            hover.SetActive(false);
    }

    public void activateAnimator()
    {
        GetComponent<Animator>().SetBool("spin", true);
    }

    public void deActivateAnimator()
    {
        GetComponent<Animator>().SetBool("spin", false);
    }

    public void changePage()
    {
        if(nextPage != null)
           GameObject.Find("manager").GetComponent<MainMenuManager>().switchToPage(nextPage);
    }

    public void loadMainMenuPageFromExternal(int index)
    {
        MainMenuManager.MainMenuDefaultPageIndex = index;
        SceneManager.LoadScene(GameConstants.MAIN_MENU_SCENE);
    }

    public void quit()
    {
        Application.Quit();
    }

    public void loadHostPrivateGame()
    {
        SceneManager.LoadScene(GameConstants.NETWORK_HOST_PRIVATE_ROOM);
    }

    public void loadScene(string toLoad)
    {
        SceneManager.LoadScene(toLoad);
    }

    //Spinning into ModeSelection
    public void switchToModeSelection(GameObject nextSelected)
    {
        StartCoroutine(setModeSelectionActive(nextSelected));
    }

    public void leaveModeSelection()
    {
        StartCoroutine(setModeSelectionInActive());
    }

    public void LoadSceneWithVersusLoadingScreen(string Scene)
    {
        AdvancedSceneManager.LoadSceneWithLoadingScreen(Scene, "MultiplayerLoadingScreen");
        //SceneManager.LoadScene("Scenes/test");
    }

    private IEnumerator setModeSelectionInActive()
    {
        GameObject target = transform.Find("ModeSelection").gameObject;
        
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

    private IEnumerator setModeSelectionActive(GameObject nextSelected)
    {
        GameObject target = transform.Find("ModeSelection").gameObject;
        MouseExit();
        GetComponent<Animator>().SetTrigger("Spin");
        GetComponent<Button>().interactable = false;

        yield return new WaitForSeconds(.33f);
        transform.Find("Symbol").gameObject.SetActive(false);
        target.SetActive(true);

        if (MyStandaloneInputModule.ControllingPlayerInputDevice == UserControl.InputDevice.XboxController)
        {
            EventSystem.current.SetSelectedGameObject(nextSelected);
        }
    }


    #region Interface Implementations
    public void OnSelect(BaseEventData eventData)
    {
        MouseEnter();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        MouseExit();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        MouseEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MouseExit();
    }
    #endregion
}
