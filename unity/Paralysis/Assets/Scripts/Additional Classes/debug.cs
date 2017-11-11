using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class debug : MonoBehaviour {

    //Used when starting scene from champion selection
    public static GameObject player1 = null;
    public static GameObject player2 = null;
    public static bool startOnMultiplayer = false;

    public Transform spawnPlayer1;
    public Transform spawnPlayer2;

    public Color championSpriteOverlayColor;

    //Used when debugging
    public GameObject[] champions;
    int currentChamp = 0;
    bool multiplayer = false;


    [SerializeField]
    private int startChar = 0;

    private void Start()
    {
        //Delete current player
        GameObject currentPlayer = GameObject.FindGameObjectWithTag("MainPlayer");
        Vector2 startPos = Vector3.zero;
        if (currentPlayer != null) { 
            Destroy(currentPlayer);
        }

        if (startOnMultiplayer && player1 != null && player2 != null)
        {
            //Disable debug buttons
            GameObject.Find("Button_changeCharacter").SetActive(false);
            GameObject.Find("Button_switchMode").SetActive(false);

            //when starting from champion selection, disable debug buttons, instantiate players
            GameObject instPlayer1 = Instantiate(player1, spawnPlayer1.position, Quaternion.identity);
            Camera.main.GetComponent<CameraBehaviour>().changeTarget(instPlayer1.transform);

            GameObject instPlayer2 = Instantiate(player2, spawnPlayer2.position, Quaternion.identity);
            instPlayer2.layer = 12;
            instPlayer2.tag = "MainPlayer2";

            //Change player2 prefab to be an enemy to player 1
            LayerMask whatToHit = new LayerMask();
            whatToHit |= (1 << LayerMask.NameToLayer("Player"));

            instPlayer2.GetComponent<ChampionClassController>().m_whatToHit = whatToHit;
            instPlayer2.GetComponent<UserControl>().inputDevice = UserControl.InputDevice.XboxController;

            Camera.main.GetComponent<CameraBehaviour>().switchToMultiplayer(instPlayer2.GetComponent<Transform>());
            multiplayer = true;
        }
        else
        {
            currentChamp = startChar;

            GameObject newPlayer = Instantiate(champions[currentChamp], startPos, Quaternion.identity);

            Camera.main.GetComponent<CameraBehaviour>().changeTarget(newPlayer.transform);
            newPlayer.transform.Find("graphics").GetComponent<SpriteRenderer>().color = championSpriteOverlayColor;
        }
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
