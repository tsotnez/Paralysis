using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class debug : MonoBehaviour {
    public GameObject[] champions;
    public GameObject[] championsPlayer2;
    int currentChamp = 0;
    bool multiplayer = false;

    [SerializeField]
    private int startChar = 0;

    private void Start()
    {
        currentChamp = startChar;

        GameObject currentPlayer = GameObject.FindGameObjectWithTag("MainPlayer");
        Vector2 startPos = Vector3.zero;
        if (currentPlayer != null)
        {
            Debug.Log(currentPlayer.name);
            Destroy(currentPlayer);
        }
        GameObject newPlayer = Instantiate(champions[currentChamp], startPos, Quaternion.identity);

        Camera.main.GetComponent<CameraBehaviour>().changeTarget(newPlayer.transform);
    }

    //Switches the champion by changing the prefab
    public void changeChampion()
    {
        increaseIndex();

        GameObject currentPlayer = GameObject.FindGameObjectWithTag("MainPlayer");

        Debug.Log(currentPlayer.name);
        GameObject newPlayer = Instantiate(champions[currentChamp], currentPlayer.transform.position, Quaternion.identity);

        Camera.main.GetComponent<CameraBehaviour>().changeTarget(newPlayer.transform);
        Destroy(currentPlayer);
    }

    /// <summary>
    /// Switch from multiplayer mode to singleplayer mode and vice versa
    /// </summary>
    public void switchPlayingMode()
    {
        if (multiplayer)
        {
            Camera.main.GetComponent<CameraBehaviour>().switchToSingleplayer();
            Destroy(GameObject.FindGameObjectWithTag("MainPlayer2"));
            multiplayer = false;
        }
        else
        {
            //Instantiate player2 prefab and switch camera to multiplayer-mode
            GameObject player2 = Instantiate(champions[0], Vector3.zero, Quaternion.identity);
            player2.layer = 12;
            player2.tag = "MainPlayer2";

            LayerMask whatToHit = new LayerMask();
            whatToHit |= (1 << LayerMask.NameToLayer("Player"));

            player2.GetComponent<ChampionClassController>().m_whatToHit = whatToHit;
            player2.GetComponent<UserControl>().inputDevice = UserControl.InputDevice.XboxController;

            Camera.main.GetComponent<CameraBehaviour>().switchToMultiplayer(player2.GetComponent<Transform>());
            multiplayer = true;
        }
    }

    private void increaseIndex()
    {
        if ((currentChamp + 1) <= (champions.Length - 1)) currentChamp++;
        else currentChamp = 0;
    }
}
