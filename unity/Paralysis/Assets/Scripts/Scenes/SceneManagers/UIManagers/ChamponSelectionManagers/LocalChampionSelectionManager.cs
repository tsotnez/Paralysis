using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;

public class LocalChampionSelectionManager : ChampionSelectionManager
{
    public Transform EventSystemsParent;

    [Header("Pages")]
    public GameObject ChampionSelect;
    public GameObject TrinketSelect;
    public GameObject SkinSelect;
    public GameObject MapSelect;

    public static Player[] team2;
    public static Player[] team1;

    EventSystem[] eventSystemInstances = new EventSystem[4];
    /// <summary>
    /// All players
    /// </summary>
    private Player[] players;
    private int controllersConnected = 0;

    public GameObject[] eventSystemPrefabs;
    public GameObject[] firstSelecteds;

    public GameObject[] Areas;

    /// <summary>
    /// Contains PopUp Tranforms
    /// </summary>
    public GameObject[] popUps;

    /// <summary>
    /// Contains whether skill pop ups should be shown for certain player
    /// </summary>
    [HideInInspector]
    public Dictionary<UserControl.PlayerNumbers, bool> showSkillPopUpsForPlayer = new Dictionary<UserControl.PlayerNumbers, bool>()
    {
        {UserControl.PlayerNumbers.Player1, false},
        {UserControl.PlayerNumbers.Player2, false},
        {UserControl.PlayerNumbers.Player3, false},
        {UserControl.PlayerNumbers.Player4, false},
    };

    protected override void Awake()
    {
        base.Awake();
        PhotonNetwork.offlineMode = true;
    }

    // Use this for initialization
    protected override void Start()
    {
        controllersConnected = Input.GetJoystickNames().Count(x => x == GameConstants.NAME_OF_XBOX360CONTROLLER_IN_ARRAY);

        //Set default Values for debugging
        if (team1 == null && team2 == null)
        {
            team1 = new Player[1];
            team2 = new Player[1];

            team1[0] = new Player(UserControl.PlayerNumbers.Player2, UserControl.InputDevice.KeyboardMouse, 1);
            team2[0] = new Player(UserControl.PlayerNumbers.Player1, UserControl.InputDevice.XboxController, 2);
        }

        //All players are the sum of both teams
        players = team1.Concat(team2).ToArray();

        //Show Team signs
        foreach (Player item in players)
        {
            Areas[(int)item.playerNumber - 1].transform.Find("team" + item.TeamNumber + "Symbol").gameObject.SetActive(true);
        }

        //Hide all unused areas
        for (int i = players.Length; i < Areas.Length; i++)
        {
            Areas[i].SetActive(false);
        }

        //Move skill popups down when there are only 2 players
        if (players.Length <= 2)
        {
            LocalChampionSelectionButtonChampion[] temp = GameObject.FindObjectsOfType<LocalChampionSelectionButtonChampion>().Where(x => x.TargetPlayerNumber == UserControl.PlayerNumbers.Player1).ToArray();
            foreach (LocalChampionSelectionButtonChampion item in temp)
                item.skills = popUps[2].transform;

            temp = GameObject.FindObjectsOfType<LocalChampionSelectionButtonChampion>().Where(x => x.TargetPlayerNumber == UserControl.PlayerNumbers.Player2).ToArray();
            foreach (LocalChampionSelectionButtonChampion item in temp)
                item.skills = popUps[3].transform;
        }

        setUpEventSystems();
    }

    protected override void Update()
    {
        //Checking for disconnecting/connecting controllers
        int newCount = Input.GetJoystickNames().Count(x => x == GameConstants.NAME_OF_XBOX360CONTROLLER_IN_ARRAY);
        if (newCount < controllersConnected)
            StartCoroutine(UIManager.showMessageBox(GameObject.FindObjectOfType<Canvas>(), "Lost connection to controller"));
        else if (newCount > controllersConnected)
            StartCoroutine(UIManager.showMessageBox(GameObject.FindObjectOfType<Canvas>(), "Controller connected"));

        controllersConnected = newCount;

        //Check if players show/hide skill popups
        if(Input.GetButtonDown("ShowSkillPopUp"))
        {
            Player target = players.FirstOrDefault(x => x.inputDevice == UserControl.InputDevice.KeyboardMouse);
            if (target != null)
                showSkillPopUpsForPlayer[target.playerNumber] = !showSkillPopUpsForPlayer[target.playerNumber]; //Keyboard input can be any player number...
            togglePopUpOnPlayerNumber(target.playerNumber);
        }
        if (Input.GetButtonDown("ShowSkillPopUp_XboxPlayer1"))
        {
            showSkillPopUpsForPlayer[UserControl.PlayerNumbers.Player1] = !showSkillPopUpsForPlayer[UserControl.PlayerNumbers.Player1];
            togglePopUpOnPlayerNumber(UserControl.PlayerNumbers.Player1);
        }
        if (Input.GetButtonDown("ShowSkillPopUp_XboxPlayer2"))
        {
            togglePopUpOnPlayerNumber(UserControl.PlayerNumbers.Player2);
            showSkillPopUpsForPlayer[UserControl.PlayerNumbers.Player2] = !showSkillPopUpsForPlayer[UserControl.PlayerNumbers.Player2];
        }
        if (Input.GetButtonDown("ShowSkillPopUp_XboxPlayer3"))
        {
            togglePopUpOnPlayerNumber(UserControl.PlayerNumbers.Player3);
            showSkillPopUpsForPlayer[UserControl.PlayerNumbers.Player3] = !showSkillPopUpsForPlayer[UserControl.PlayerNumbers.Player3];
        }
        if (Input.GetButtonDown("ShowSkillPopUp_XboxPlayer4"))
        {
            togglePopUpOnPlayerNumber(UserControl.PlayerNumbers.Player4);
            showSkillPopUpsForPlayer[UserControl.PlayerNumbers.Player4] = !showSkillPopUpsForPlayer[UserControl.PlayerNumbers.Player4];
        }
    }

    /// <summary>
    /// Gets the currently highlighted button and toggles its popUps
    /// </summary>
    /// <param name=""></param>
    private void togglePopUpOnPlayerNumber(UserControl.PlayerNumbers targetNumber)
    {
        LocalChampionSelectionButtonChampion currentlyHighlighted = GameObject.FindObjectsOfType<LocalChampionSelectionButtonChampion>().FirstOrDefault(x => x.TargetPlayerNumber == targetNumber && x.highlighted);
        if (currentlyHighlighted != null)
            currentlyHighlighted.toggleSkillPopUp();
    }

    /// <summary>
    /// Sets the champion value for player object and shows a preview for the champion
    /// </summary>
    /// <param name="targetPlayer"></param>
    /// <param name="Champion"></param>
    public override void setChampion(UserControl.PlayerNumbers targetPlayer, GameObject Champion)
    {
        Player target = players.First(x => x.playerNumber == targetPlayer); //Get player object out of array
        target.ChampionPrefab = Champion;

        if (Champion == null)
            Areas[(int)targetPlayer - 1].transform.Find("Ready").gameObject.SetActive(false);
        else
        {
            Areas[(int)targetPlayer - 1].transform.Find("Ready").gameObject.SetActive(true);
            showChampion(targetPlayer, Champion, true);
        }
    }

    /// <summary>
    /// Showeing preview of champion
    /// </summary>
    /// <param name="targetPlayer"></param>
    /// <param name="Champion"></param>
    public void showChampion(UserControl.PlayerNumbers targetPlayer, GameObject Champion, bool playAnim)
    {
        bool flip;

        if ((int)targetPlayer % 2 == 0)
            flip = true;
        else
            flip = false;

        Transform platform = Areas[(int)targetPlayer - 1].transform.Find("Platform");

        DestroyExistingPreview(platform);
        ShowPrefab(Champion, platform, flip, playAnim);
    }

    public override void setTrinket1(UserControl.PlayerNumbers targetPlayer, Trinket.Trinkets trinketName)
    {
        players.First(x => x.playerNumber == targetPlayer).trinket1 = trinketName;
    }

    public override void setTrinket2(UserControl.PlayerNumbers targetPlayer, Trinket.Trinkets trinketName)
    {
        players.First(x => x.playerNumber == targetPlayer).trinket2 = trinketName;
    }

    public override void setTrinket(UserControl.PlayerNumbers targetPlayer, Trinket.Trinkets trinketName, Trinket.Trinkets toOverwrite)
    {
        Player target = players.First(x => x.playerNumber == targetPlayer);

        if (target.trinket1 == toOverwrite)
            target.trinket1 = trinketName;
        else
            target.trinket2 = trinketName;
    }

    /// <summary>
    /// Generates Eventsystems based on how many Input devices are connected
    /// </summary>
    private void setUpEventSystems()
    {
        foreach (Player player in players)
        {
            switch (player.playerNumber)
            {
                case UserControl.PlayerNumbers.Player1:
                    eventSystemInstances[0] = instantiateEventSystem(player, 1).GetComponent<EventSystem>();

                    //Set player1 EventsSystem as target ES for every HorizontalAutomaticScrollScript (just the one for map select really)
                    foreach (HorizontalAutomaticScroll item in MapSelect.GetComponentsInChildren<HorizontalAutomaticScroll>())
                    {
                        item.es = eventSystemInstances[0];
                    }
                    break;
                case UserControl.PlayerNumbers.Player2:
                    eventSystemInstances[1] = instantiateEventSystem(player, 2).GetComponent<EventSystem>(); ;
                    break;
                case UserControl.PlayerNumbers.Player3:
                    eventSystemInstances[2] = instantiateEventSystem(player, 3).GetComponent<EventSystem>(); ;
                    break;
                case UserControl.PlayerNumbers.Player4:
                    eventSystemInstances[3] = instantiateEventSystem(player, 4).GetComponent<EventSystem>(); ;
                    break;
            }
        }
    }

    /// <summary>
    /// Instantiates EventSystem and sets its variables
    /// </summary>
    /// <param name="player"></param>
    /// <param name="ID"></param>
    private GameObject instantiateEventSystem(Player player, int ID)
    {
        Transform parent = EventSystemsParent;

        GameObject newSystem = Instantiate(eventSystemPrefabs[ID - 1], parent, false);
        MyStandaloneInputModule inputModule = newSystem.GetComponent<MyStandaloneInputModule>();

        //Set first selected by getting Array Value
        newSystem.GetComponent<EventSystem>().firstSelectedGameObject = firstSelecteds[ID - 1];

        if (player.inputDevice == UserControl.InputDevice.KeyboardMouse)
        {
            //Set axis for Keyboard
            inputModule.horizontalAxis = "Horizontal";
            inputModule.verticalAxis = "Vertical";
            inputModule.submitButton = "Submit";
        }
        else if (player.inputDevice == UserControl.InputDevice.XboxController)
        {
            //Set axis for xbox
            inputModule.horizontalAxis = "Horizontal_XboxPlayer" + ID.ToString();
            inputModule.verticalAxis = "Vertical_XboxPlayer" + ID.ToString();
            inputModule.submitButton = "Submit_XboxPlayer" + ID.ToString();
        }
        inputModule.processMouseEvents = false;
        return newSystem;
    }

    public void GoToChampionSelection()
    {
        deselectAll();
        MapSelect.SetActive(false);
        ChampionSelect.SetActive(true);

        //Select gameObjects
        for (int i = 0; i < eventSystemInstances.Length; i++)
        {
            if(eventSystemInstances[i] != null)
                eventSystemInstances[i].SetSelectedGameObject(firstSelecteds[i]);
        }
    }

    public void GoToMapSelection()
    {
        if (everythingSelected())
        {
            deselectAll();

            LocalMultiplayerManager.Teams = new Dictionary<int, Player[]>();
            LocalMultiplayerManager.Teams.Add(0, team1);
            LocalMultiplayerManager.Teams.Add(1, team2);

            ChampionSelect.SetActive(false);
            MapSelect.SetActive(true);

            GameObject mapSlideshow = MapSelect.transform.Find("Maps").GetComponent<HorizontalAutomaticScroll>().contentTrans.GetChild(0).gameObject;
           eventSystemInstances[0].SetSelectedGameObject(mapSlideshow);

        }
        else
        {
            StartCoroutine(UIManager.showMessageBox(GameObject.FindObjectOfType<Canvas>(), "Select a champion and two different trinkets for each player"));
        }
    }

    private void deselectAll() {
        foreach (EventSystem item in eventSystemInstances)
        {
            if(item != null)
                item.SetSelectedGameObject(null);
        }
    }

    public override void startGame(string mapName)
    {
        //Check if every player has selected a champion and 2 different trinkets
        if (everythingSelected())
        {
            AdvancedSceneManager.LoadSceneWithLoadingScreen(mapName, "MultiplayerLoadingScreen");
        }
        else
        {
            StartCoroutine(UIManager.showMessageBox(GameObject.FindObjectOfType<Canvas>(), "Select a champion and two different trinkets for each player"));
        }
    }

    private bool everythingSelected()
    {
        //Only allow to start if selections are complete
        bool allSelected = true;
        //Check if every player has selected a champion and 2 different trinkets
        foreach (Player player in players)
            if (player.ChampionPrefab == null || player.trinket1 == player.trinket2)
                allSelected = false;

        return allSelected;
    }
}
