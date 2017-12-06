using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour {

    public CanvasGroup mainCanvas;
    public CanvasGroup startPage;
    private CanvasGroup currentPage; //Page which is shown currently

    public CursorLockMode CursorLockMode { get; private set; }

    // Use this for initialization
    void Start () {

        //Switch to controller Controls if a controller is connected
        if (Array.Exists(Input.GetJoystickNames(), x => x == "Controller (XBOX 360 For Windows)"))
        {
            StandaloneInputModule eventSystem = FindObjectOfType<StandaloneInputModule>();
            eventSystem.verticalAxis = "Vertical_XboxPlayer1";
            eventSystem.horizontalAxis = "Horizontal_XboxPlayer1";
            eventSystem.submitButton = "Skill4_XboxPlayer1";
        }


        //Disable all pages and enable starting page
        foreach (CanvasGroup item in FindObjectsOfType<CanvasGroup>())
        {
            if (item == mainCanvas)
                continue;
            disablePage(item);
        }

        enablePage(startPage);
        currentPage = startPage;
    }
	
	// Update is called once per frame
	void Update () {
	}

    /// <summary>
    /// Switch to a passed page, disabeling the current page
    /// </summary>
    /// <param name="nextPage"></param>
    public void switchToPage(CanvasGroup nextPage)
    {
        disablePage(currentPage);
        enablePage(nextPage);

        currentPage = nextPage;
        FindObjectOfType<EventSystem>().SetSelectedGameObject(currentPage.gameObject.GetComponentInChildren<Selectable>().gameObject);
    }

    private void disablePage(CanvasGroup toDisable)
    {
        toDisable.alpha = 0;
        toDisable.blocksRaycasts = false;
        toDisable.interactable = false;
    }

    private void enablePage(CanvasGroup toEnable)
    {
        toEnable.alpha = 1;
        toEnable.blocksRaycasts = true;
        toEnable.interactable = true;
    }
}
