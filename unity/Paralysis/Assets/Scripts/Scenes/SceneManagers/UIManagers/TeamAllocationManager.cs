using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System;

public class TeamAllocationManager : UIManager {

    private int maxPlayers = 4; //Is 4 because everything is optimized to handle 4 players

    public GameObject playerPrefab;
    public GameObject AIPrefab;
    public Sprite keyboardSprite;
    public Transform neutral;
    public float GapBetweenPlayers = -1.2f;

    /// <summary>
    /// Amount of added players
    /// </summary>
    private int PlayerCount = 0;


    string[] connectedControllers = new string[0];

    /// <summary>
    /// Represantation of rows where a player can move to the lef tor right
    /// </summary>
    private GameObject[] Slots;

	// Use this for initialization
	protected override void Start () {

        base.Start();

        Slots = new GameObject[maxPlayers];
        MyStandaloneInputModule eventSystem = FindObjectOfType<MyStandaloneInputModule>();

        //Switch to controller Controls if a controller is connected
        if (Array.Exists(Input.GetJoystickNames(), x => x == GameConstants.NAME_OF_XBOX360CONTROLLER_IN_ARRAY))
        {
            eventSystem.verticalAxis = "Vertical_XboxPlayer1";
            eventSystem.horizontalAxis = "Horizontal_XboxPlayer1";
            eventSystem.submitButton = "Skill4_XboxPlayer1";
        }
        else
        {
            //To avoid changing focussed Button when using A and D keys to switch teams
            eventSystem.verticalAxis = "Vertical_XboxPlayer1";
            eventSystem.horizontalAxis = "Horizontal_XboxPlayer1";
        }

        //Adding players for every controller connected
        foreach (string controller in connectedControllers.ToList())
        {
            if (!string.IsNullOrEmpty(controller))
            {
                AddNewPlayer(UserControl.InputDevice.XboxController);
            }
        }

        //try to add keyboard player
        AddNewPlayer(UserControl.InputDevice.KeyboardMouse);
	}
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();

        //Reload scene if any controllers are diconnected or connected
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

        connectedControllers = newConnected;
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


    private GameObject AddNewPlayer(UserControl.InputDevice inputDevice)
    {
        if (PlayerCount >= maxPlayers) //Only add when max players not reched yet
            return null;

        GameObject prefab;

        if (inputDevice == UserControl.InputDevice.AI)
            prefab = AIPrefab;
        else
            prefab = playerPrefab;


        GameObject player = Instantiate(prefab, neutral, false);

        //Place new Player in the first free slot
        int index = Array.IndexOf(Slots, Slots.First(x => x == null));
        Slots[index] = player;

        //Position player according to slot used
        player.transform.position = new Vector3(0, player.transform.position.y + index * GapBetweenPlayers);
        if(inputDevice == UserControl.InputDevice.KeyboardMouse)
            player.GetComponent<Image>().sprite = keyboardSprite;

        player.GetComponent<TeamAllocationPlayer>().inputDevice = inputDevice;

        //Setting player number
        player.GetComponent<TeamAllocationPlayer>().playerNumber = (UserControl.PlayerNumbers) Enum.Parse(typeof(UserControl.PlayerNumbers), "Player" + (PlayerCount + 1), true);

        if (inputDevice != UserControl.InputDevice.AI)
            player.GetComponentInChildren<Text>().text = "Player " + (PlayerCount + 1);

        PlayerCount++;
        return player;
    }

    public void AddAiPlayerToTeam(int Team)
    {
        GameObject newAI = AddNewPlayer(UserControl.InputDevice.AI);

        if(newAI != null)
            newAI.GetComponent<TeamAllocationPlayer>().switchToTeam(Team);
    }

    public void RemoveAiFromTeam(int Team)
    {
        TeamAllocationPlayer toRemove = GameObject.FindObjectsOfType<TeamAllocationPlayer>().FirstOrDefault(x => x.inputDevice == UserControl.InputDevice.AI && x.teamNumber == Team);

        if (toRemove != null)
        {
            int index = Array.IndexOf(Slots, toRemove.gameObject);
            Slots[index] = null; //Clear out slot of deleted AI

            Destroy(toRemove.gameObject);
            PlayerCount--;
        }

    }
}
