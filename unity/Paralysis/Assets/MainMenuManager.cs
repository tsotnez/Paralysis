using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class MainMenuManager : MonoBehaviour {

    public CanvasGroup startPage;
    private CanvasGroup currentPage; //Page which is shown currently

	// Use this for initialization
	void Start () {

        //Switch to controller Controls if a controller is connected
        if (Array.Exists(Input.GetJoystickNames(), x => x == "Controller (XBOX 360 For Windows)"))
        {
            StandaloneInputModule eventSystem = FindObjectOfType<StandaloneInputModule>();
            eventSystem.verticalAxis = "Vertical_XboxPlayer1";
            eventSystem.horizontalAxis = "Horizontal_XboxPlayer1";
        }


        //Disable all pages and enable starting page
        foreach (CanvasGroup item in FindObjectsOfType<CanvasGroup>())
        {
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
        enablePage(nextPage);
        disablePage(currentPage);

        currentPage = nextPage;
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
