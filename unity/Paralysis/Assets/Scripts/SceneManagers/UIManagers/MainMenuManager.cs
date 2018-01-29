using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuManager : UIManager {

    public CanvasGroup mainCanvas;
    public CanvasGroup startPage;
    private CanvasGroup currentPage; //Page which is shown currently
    public CursorLockMode CursorLockMode { get; private set; }

    public CanvasGroup[] MenuPages;
    public GameObject[] firstSelectedObjects;
    public EventSystem es;

    //Set from outside to change to a certain menu page immediatly after loading the scene
    public static int DefaultPageIndex = -1;

    // Use this for initialization
    protected override void Start () {
        base.Start();

        if (DefaultPageIndex != -1)
        {
            startPage = MenuPages[DefaultPageIndex];
            es.SetSelectedGameObject(firstSelectedObjects[DefaultPageIndex]);
        }

        DefaultPageIndex = -1;

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
