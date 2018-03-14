using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class MenuPagesManager : UIManager {

    public CanvasGroup startPage;
    protected CanvasGroup currentPage; //Page which is shown currently

    public CanvasGroup settings;
    public CanvasGroup[] MenuPages;
    public GameObject[] firstSelectedObjects;
    public Image fadeOverlay;
    [SerializeField]
    private float fadeDuration = .25f;

    //Set from outside to change to a certain menu page immediatly after loading the scene
    protected int DefaultPageIndex = -1;

    // Use this for initialization
    protected override void Start () {
        base.Start();

        fadeOverlay.canvasRenderer.SetAlpha(0);
        fadeOverlay.gameObject.SetActive(true);

        Init();
    }

    protected void Init()
    {
        if (DefaultPageIndex != -1)
        {
            startPage = MenuPages[DefaultPageIndex];
            EventSystem.current.SetSelectedGameObject(firstSelectedObjects[DefaultPageIndex]);
        }

        DefaultPageIndex = -1;

        //Disable all pages and enable starting page
        foreach (CanvasGroup item in MenuPages)
        {
            disablePage(item);
        }

        enablePage(startPage);
        currentPage = startPage;
    }

    protected override void gotoController()
    {
        module.SetControllingPlayerInputDevice(UserControl.InputDevice.XboxController);
        int currentPageIndex = Array.IndexOf(MenuPages, currentPage);

        if (currentPage != settings)
            EventSystem.current.SetSelectedGameObject(firstSelectedObjects[currentPageIndex]);
        else
            FindObjectOfType<SettingsManager>().setSelectedGoAccordingly();

    }

    protected override void gotoKeyboard()
    {
        base.gotoKeyboard();
    }

    /// <summary>
    /// Switch to a passed page, disabeling the current page
    /// </summary>
    public virtual void switchToPage(CanvasGroup nextPage)
    {
        EventSystem.current.SetSelectedGameObject(null);
        //Fade in
        if (fadeOverlay != null)
        {
            fadeOverlay.canvasRenderer.SetAlpha(1); //Workaround for unity bug, its weird but it works
            fadeOverlay.CrossFadeAlpha(1, fadeDuration, true);
        }

        disablePage(currentPage);
        enablePage(nextPage);

        currentPage = nextPage;

        //Only preselect a GO if using controller
        if(MyStandaloneInputModule.ControllingPlayerInputDevice == UserControl.InputDevice.XboxController)
            EventSystem.current.SetSelectedGameObject(currentPage.gameObject.GetComponentInChildren<Selectable>().gameObject);

        //Fade out
        if(fadeOverlay != null)
            fadeOverlay.CrossFadeAlpha(0, fadeDuration, true);
    }

    protected void disablePage(CanvasGroup toDisable)
    {
        toDisable.alpha = 0;
        toDisable.blocksRaycasts = false;
        toDisable.interactable = false;
    }

    protected virtual void enablePage(CanvasGroup toEnable)
    {
        toEnable.alpha = 1;
        toEnable.blocksRaycasts = true;
        toEnable.interactable = true;
    }
}
