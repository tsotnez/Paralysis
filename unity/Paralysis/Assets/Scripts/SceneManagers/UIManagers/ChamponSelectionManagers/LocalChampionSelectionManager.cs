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
    public GameObject MapSelect;

    public static Player[] team2;
    public static Player[] team1;

    private bool everythingSelected = false;
    EventSystem[] eventSystemInstances = new EventSystem[4];
    /// <summary>
    /// All players
    /// </summary>
    private Player[] players;

    public GameObject[] eventSystemPrefabs;
    public GameObject[] firstSelecteds;

    public GameObject[] platformsTeam1;
    public GameObject[] platformsTeam2;

    void Awake()
    {
        PhotonNetwork.offlineMode = true;
    }

    // Use this for initialization
    protected override void Start()
    {
        //Set default Values for debugging
        if (team1 == null && team2 == null)
        {
            team1 = new Player[1];
            team2 = new Player[1];

            team1[0] = new Player(UserControl.PlayerNumbers.Player2, UserControl.InputDevice.KeyboardMouse, 1);
            team2[0] = new Player(UserControl.PlayerNumbers.Player1, UserControl.InputDevice.XboxController, 2);
        }

        //Show additionjal player Platforms if 2v2
        if (team1.Length == 2 && team2.Length == 2)
        {
            additionalPlatforms2v2.SetActive(true);
        }

        //All players are the sum of both teams
        players = team1.Concat(team2).ToArray();
        setUpEventSystems();
    }

    protected override void Update()
    {
        //Only allow to start if selections are complete
        bool allSelected = true;
        //Check if every player has selected a champion and 2 different trinkets
        foreach (Player player in players)
            if (player.ChampionPrefab == null || player.trinket1 == player.trinket2)
                allSelected = false;

        everythingSelected = allSelected;

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

        GameObject[] platformGroup;
        bool flip;

        if (target.TeamNumber == 1)
        {
            platformGroup = platformsTeam1;
            flip = false;
        }
        else //If player is in team 2, use platforms of team 2 and flip the sprite
        {
            platformGroup = platformsTeam2;
            flip = true;
        }

        if (team1.Length == 2 && team2.Length == 2)//if 2v2
        {
            if (team1.First(x => x != target).playerNumber > target.playerNumber) //If target has the highest playerNumber in team, use second platform
            {
                DestroyExistingPreview(platformGroup[1].transform);
                ShowPrefab(Champion, platformGroup[1].transform, flip);
            }
            else
            {
                //Use first platform
                DestroyExistingPreview(platformGroup[0].transform);
                ShowPrefab(Champion, platformGroup[0].transform, flip);
            }
        }
        else //Just use first platform
        {
            DestroyExistingPreview(platformGroup[0].transform);
            ShowPrefab(Champion, platformGroup[0].transform, flip);
        }
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
        if (everythingSelected)
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
        if (everythingSelected)
        {
            AdvancedSceneManager.LoadSceneWithLoadingScreen(mapName, "MultiplayerLoadingScreen");
        }
        else
        {
            StartCoroutine(UIManager.showMessageBox(GameObject.FindObjectOfType<Canvas>(), "Select a champion and two different trinkets for each player"));
        }
    }
}
