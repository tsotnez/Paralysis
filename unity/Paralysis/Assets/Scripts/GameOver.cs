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
        if (PhotonNetwork.offlineMode)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        else
        {
            GameObject.Find("manager").GetComponent<GameplayManager>().restart();
        }
    }

    public void BackToChampionSelection()
    {
        //Load selection screen
        SceneManager.LoadScene(0);
    }
}
