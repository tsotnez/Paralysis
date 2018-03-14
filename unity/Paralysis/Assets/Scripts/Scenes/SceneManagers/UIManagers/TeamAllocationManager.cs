using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System;

public class TeamAllocationManager : UIManager {

    private int maxPlayers = 4; //Is 4 because everything is optimized to handle 4 players
    
    public GameObject playerPrefab;
    public Sprite keyboard;
    public Transform neutral;

    string[] connectedControllers;


	// Use this for initialization
	protected override void Start () {
        base.Start();

        connectedControllers = Input.GetJoystickNames();

        float yOffset = 0;
        int playerNumber = 1;

        //Switch to controller Controls if a controller is connected
        if (Array.Exists(Input.GetJoystickNames(), x => x == GameConstants.NAME_OF_XBOX360CONTROLLER_IN_ARRAY))
        {
            MyStandaloneInputModule eventSystem = FindObjectOfType<MyStandaloneInputModule>();
            eventSystem.verticalAxis = "Vertical_XboxPlayer1";
            eventSystem.horizontalAxis = "Horizontal_XboxPlayer1";
            eventSystem.submitButton = "Skill4_XboxPlayer1";
        }

        //Adding players for every controller connected
        foreach (string controller in connectedControllers.ToList())
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
	protected override void Update () {
        base.Update();

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

        if (team1.Length > 0 || team2.Length > 0) //Only start if there is at least one player allocated
        {
            LocalChampionSelectionManager.team1 = team1.Select(x => new Player(x.playerNumber, x.inputDevice, 1)).ToArray();
            LocalChampionSelectionManager.team2 = team2.Select(x => new Player(x.playerNumber, x.inputDevice, 2)).ToArray();
            SceneManager.LoadScene("Scenes/LocalChampionSelection");
        }
        else
        {
            StartCoroutine(UIManager.showMessageBox(GameObject.FindObjectOfType<Canvas>(), "Allocate at least one player"));
        }
    }
}
