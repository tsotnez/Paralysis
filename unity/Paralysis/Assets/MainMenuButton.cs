using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        TeamAllocationManager.maxPlayers = 2;
        SceneManager.LoadScene("Scenes/TeamAllocation");
    }

    public void quit()
    {
        Application.Quit();
    }

    public void loadMainMenu()
    {
        SceneManager.LoadScene("Scenes/MainMenu");
    }
}
