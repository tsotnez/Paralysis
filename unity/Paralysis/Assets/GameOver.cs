using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour {

    public void Quit()
    {
        //End game
        Application.Quit();
    }

    public void Restart()
    {
        //Restart game
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToChampionSelection()
    {
        //Load selection screen
        SceneManager.LoadScene(0);
    }
}
