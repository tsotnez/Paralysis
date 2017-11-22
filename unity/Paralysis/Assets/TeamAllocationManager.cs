using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class TeamAllocationManager : MonoBehaviour {

    public static int maxPlayers = 2;

    public GameObject playerPrefab;
    public Sprite xBox;
    public Sprite keyboard;
    public Transform neutral;

    string[] connectedControllers;

	// Use this for initialization
	void Start () {

        connectedControllers = Input.GetJoystickNames();

        float yOffset = 0;
        int playerNumber = 1;

        //Adding players for every controller connected
        foreach(string controller in connectedControllers.ToList())
        {
            if (!string.IsNullOrEmpty(controller) && playerNumber <= maxPlayers)
            {
                GameObject player = Instantiate(playerPrefab, neutral, false);
                player.transform.position = new Vector3(0, player.transform.position.y + yOffset);

                //Setting player number
                switch (playerNumber)
                {
                    case 1:
                        player.GetComponent<TeamAllocationPlayer>().playerNumber = UserControl.PlayerNumbers.Player1;
                        break;
                    case 2:
                        player.GetComponent<TeamAllocationPlayer>().playerNumber = UserControl.PlayerNumbers.Player2;
                        break;
                    case 3:
                        player.GetComponent<TeamAllocationPlayer>().playerNumber = UserControl.PlayerNumbers.Player3;
                        break;
                    case 4:
                        player.GetComponent<TeamAllocationPlayer>().playerNumber = UserControl.PlayerNumbers.Player4;
                        break;
                }

                player.GetComponentInChildren<Text>().text = "Player " + playerNumber.ToString();

                yOffset -= 1.2f;
                playerNumber++;
            }
        }

        if (playerNumber <= maxPlayers)
        {
            //Adding a keyboard player, if running on pc and below 4 players
            if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
            {
                GameObject player = Instantiate(playerPrefab, neutral, false);
                player.transform.position = new Vector3(0, player.transform.position.y + yOffset, 0);
                player.GetComponent<Image>().sprite = keyboard;

                player.GetComponent<TeamAllocationPlayer>().inputDevice = UserControl.InputDevice.KeyboardMouse;

                switch (playerNumber)
                {
                    case 1:
                        player.GetComponent<TeamAllocationPlayer>().playerNumber = UserControl.PlayerNumbers.Player1;
                        break;
                    case 2:
                        player.GetComponent<TeamAllocationPlayer>().playerNumber = UserControl.PlayerNumbers.Player2;
                        break;
                    case 3:
                        player.GetComponent<TeamAllocationPlayer>().playerNumber = UserControl.PlayerNumbers.Player3;
                        break;
                }
                player.GetComponentInChildren<Text>().text = "Player " + playerNumber.ToString();

            }
        }
	}
	
	// Update is called once per frame
	void Update () {

        //Reload scene if any controller are diconnected or connected
        string[] newConnected = Input.GetJoystickNames();

        if (newConnected.Length != connectedControllers.Length)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        else
        {
            for (int i = 0; i < connectedControllers.Length; i++)
            {
                if(connectedControllers[i] != newConnected[i])
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

    }

    public void finished()
    {
        //Get players in teams
        TeamAllocationPlayer[] activePlayers = GameObject.FindObjectsOfType<TeamAllocationPlayer>().Where(x => x.teamNumber != 0).ToArray();
        TeamAllocationPlayer[] team1 = activePlayers.Where(x => x.teamNumber == -1).ToArray();
        TeamAllocationPlayer[] team2 = activePlayers.Where(x => x.teamNumber == 1).ToArray();

        if (team1.Length == maxPlayers / 2 && team2.Length == maxPlayers / 2) //Only start if both teams are full
        {
            //Load champion selection depending on max number of players
            switch (maxPlayers)
            {
                case 2:
                    ChampionSelectionManagerbehaviour.inputPlayer1 = activePlayers.First(x => x.playerNumber == UserControl.PlayerNumbers.Player1).inputDevice;
                    ChampionSelectionManagerbehaviour.inputPlayer2 = activePlayers.First(x => x.playerNumber == UserControl.PlayerNumbers.Player2).inputDevice;
                    ChampionSelectionManagerbehaviour.team1 = team1.Select(x => x.playerNumber).ToArray();
                    ChampionSelectionManagerbehaviour.team2 = team2.Select(x => x.playerNumber).ToArray();
                    SceneManager.LoadScene("ChampionSelect1v1");
                    break;
                case 4:
                    SceneManager.LoadScene("ChampionSelect2v2");
                    break;
            }
        }
    }
}
