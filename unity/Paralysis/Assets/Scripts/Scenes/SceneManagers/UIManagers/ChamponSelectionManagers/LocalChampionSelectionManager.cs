using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

public class LocalChampionSelectionManager : ChampionSelectionManager
{
    public static Player[] team2;
    public static Player[] team1;

    public Transform EventSystemsParent;
    public GameObject[] eventSystemPrefabs;

    [Header("Pages")]
    public GameObject ChampionSelect;
    public GameObject TrinketSelect;
    public GameObject SkinSelect;
    public GameObject MapSelect;

    [Header("ChampionSelection")]
    public GameObject[] ChampionSelectionFirstSelecteds;
    public Text CountDownChampions;
    public int countDownChampionsSeconds = 4;
    private IEnumerator ChampionsCountdownRoutine = null;
    /// <summary>
    /// Contains PopUp Tranforms
    /// </summary>
    public GameObject[] popUps;
    public GameObject[] Areas;

    [Header("Trinket Selection")]
    public GameObject[] TrinketSelectionFirstSelecteds;
    public GameObject[] TrinketSelectionAreas;
    public Text CountDownTrinkets;
    private Coroutine TrinketsCountdownRoutine = null;

    [Header("Map Selection")]
    public GameObject[] MapSelectionFirstSelecteds;
    public Text CountDownMaps;
    private Coroutine MapsCountdownRoutine = null;

    /// <summary>
    /// All players
    /// </summary>
    private Player[] players;
    private EventSystem[] eventSystemInstances = new EventSystem[4];
    private int controllersConnected = 0;

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
        Cursor.visible = false;
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
            TrinketSelectionAreas[i].SetActive(false);
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


        //Clear TrinketPreviewSlots
        foreach (Player player in players)
        {
            removeTrinketPreview(player.playerNumber, 1);
            removeTrinketPreview(player.playerNumber, 2);
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
        {
            Areas[(int)targetPlayer - 1].transform.Find("Ready").gameObject.SetActive(false);

            if (ChampionsCountdownRoutine != null)
                StopCoroutine(ChampionsCountdownRoutine);
            CountDownChampions.gameObject.SetActive(false);
        }
        else
        {
            Areas[(int)targetPlayer - 1].transform.Find("Ready").gameObject.SetActive(true);
            showChampion(targetPlayer, Champion, true);

            if (AllChampionsSelected())
            {
                ChampionsCountdownRoutine = countDownChampions();
                StartCoroutine(ChampionsCountdownRoutine);
            }
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

    /// <summary>
    /// Shows information of passed trinket in passed slot of passed player
    /// </summary>
    public void showTrinket(UserControl.PlayerNumbers targetPlayer, Trinket.Trinkets trinket, Sprite pic, int slot)
    {
        GameObject targetArea = TrinketSelectionAreas[(int)targetPlayer - 1];

        Transform targetSlot = targetArea.transform.Find("Slot" + slot);
        targetSlot.Find("Name").GetComponent<Text>().text = TrinketDatabase.TrinketNames[trinket];

        Image img = targetSlot.Find("BackgroundPortrait").Find("portrait").GetComponent<Image>();
        img.sprite = pic;
        img.enabled = true;

        targetSlot.Find("BackgroundText").Find("Desc").GetComponent<Text>().text = TrinketDatabase.TrinketDescriptions[trinket];

        targetSlot.Find("Frame").gameObject.SetActive(true);
    }

    /// <summary>
    /// Shows information of passed trinket in empty slot of passed player
    /// </summary>
    public void showTrinket(UserControl.PlayerNumbers targetPlayer, Trinket.Trinkets trinket, Sprite pic)
    {
        GameObject targetArea = TrinketSelectionAreas[(int)targetPlayer - 1];
        Player player = players.First(x => x.playerNumber == targetPlayer);

        int slot = 1;
        if (player.trinket2 == Trinket.Trinkets.None && player.trinket1 != Trinket.Trinkets.None)
            slot = 2;

        Transform targetSlot = targetArea.transform.Find("Slot" + slot);
        targetSlot.Find("Name").GetComponent<Text>().text = TrinketDatabase.TrinketNames[trinket];

        Image img = targetSlot.Find("BackgroundPortrait").Find("portrait").GetComponent<Image>();
        img.sprite = pic;
        img.enabled = true;

        targetSlot.Find("BackgroundText").Find("Desc").GetComponent<Text>().text = TrinketDatabase.TrinketDescriptions[trinket];
    }

    /// <summary>
    /// Removes trinket from passed player and preview as well
    /// </summary>
    public void removeTrinket(UserControl.PlayerNumbers targetPlayer, Trinket.Trinkets trinket)
    {
        Player target = players.First(x => x.playerNumber == targetPlayer);

        if (target.trinket1 == trinket)
        {
            target.trinket1 = Trinket.Trinkets.None;
            removeTrinketPreview(targetPlayer, 1, false);

            if (target.trinket2 == Trinket.Trinkets.None)
                removeTrinketPreview(targetPlayer, 2);
        }
        else if (target.trinket2 == trinket)
        {
            target.trinket2 = Trinket.Trinkets.None;
            if (target.trinket1 == Trinket.Trinkets.None)
                removeTrinketPreview(targetPlayer, 2);
            else
                removeTrinketPreview(targetPlayer, 2, false);
        }
        else
            Debug.LogError("Trying to remove unselected trinket " + trinket + " from player " + targetPlayer);

        if (TrinketsCountdownRoutine != null)
        {
            StopCoroutine(TrinketsCountdownRoutine);
            CountDownTrinkets.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Shows information of passed trinket in passed slot of passed player
    /// </summary>
    public void removeTrinketPreview(UserControl.PlayerNumbers targetPlayer, int slot, bool removeInfo = true)
    {
        GameObject targetArea = TrinketSelectionAreas[(int)targetPlayer - 1];

        Transform targetSlot = targetArea.transform.Find("Slot" + slot);

        if (removeInfo)
        {
            targetSlot.Find("Name").GetComponent<Text>().text = "Trinket " + slot;

            Image img = targetSlot.Find("BackgroundPortrait").Find("portrait").GetComponent<Image>();
            img.sprite = null;
            img.enabled = false;

            targetSlot.Find("BackgroundText").Find("Desc").GetComponent<Text>().text = "";
        }

        targetSlot.Find("Frame").gameObject.SetActive(false);
    }

    /// <summary>
    /// Checks which trinket slot from passed player is empty and sets that slots trinket to passed value
    /// </summary>
    public void setTrinket(UserControl.PlayerNumbers targetPlayer, Trinket.Trinkets trinket, Sprite pic)
    {
        Player target = players.First(x => x.playerNumber == targetPlayer);

        if (target.trinket1 == Trinket.Trinkets.None)
        {
            target.trinket1 = trinket;
            showTrinket(targetPlayer, trinket, pic, 1);
        }
        else if (target.trinket2 == Trinket.Trinkets.None)
        {
            target.trinket2 = trinket;
            showTrinket(targetPlayer, trinket, pic, 2);
        }
        else
            Debug.LogError("Trying to add trinket to player" + targetPlayer + " but he has 2 trinkets already");

        if(AllTrinketsSelected())
        {
            TrinketsCountdownRoutine = StartCoroutine(countDownTrinkets());
        }
    }

    /// <summary>
    /// Animated countdown
    /// </summary>
    private IEnumerator countDownChampions()
    {
        CountDownChampions.gameObject.SetActive(true);
        Animator anim = CountDownChampions.gameObject.GetComponent<Animator>();

        int counter = countDownChampionsSeconds;

        while (counter >= 0)
        {
            anim.SetTrigger("animate");
            CountDownChampions.text = counter.ToString();
            counter--;
            yield return new WaitForSeconds(1);
        }

        CountDownChampions.gameObject.SetActive(false);
        GoToTrinketSelection();
    }

    /// <summary>
    /// Animated countdown from trinket selection to map select
    /// </summary>
    private IEnumerator countDownTrinkets()
    {
        CountDownTrinkets.gameObject.SetActive(true);
        Animator anim = CountDownTrinkets.gameObject.GetComponent<Animator>();

        int counter = countDownChampionsSeconds;

        while (counter >= 0)
        {
            anim.SetTrigger("animate");
            CountDownTrinkets.text = counter.ToString();
            counter--;
            yield return new WaitForSeconds(1);
        }

        CountDownTrinkets.gameObject.SetActive(false);
        GoToMapSelection();
    }

    /// <summary>
    /// Checks wheter a trinket can be set for passed player, meaning the player has at least one free trinket slot
    /// </summary>
    public bool canSetForPlayer(UserControl.PlayerNumbers targetPlayer)
    {
        Player target = players.First(x => x.playerNumber == targetPlayer);

        if (target.trinket1 == Trinket.Trinkets.None || target.trinket2 == Trinket.Trinkets.None)
            return true;
        else
            return false;
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
        newSystem.GetComponent<EventSystem>().firstSelectedGameObject = ChampionSelectionFirstSelecteds[ID - 1];

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

    /// <summary>
    /// Sets EventSystems to select GO according to passed array
    /// </summary>
    private void setSelectedGameObjectsForPage(GameObject[] sourceArray)
    {
        for (int i = 0; i < eventSystemInstances.Length; i++)
        {
            if (eventSystemInstances[i] != null)
            {
                if (sourceArray.Length - 1 >= i)
                    eventSystemInstances[i].SetSelectedGameObject(sourceArray[i]);
                else
                    eventSystemInstances[i].SetSelectedGameObject(null);
            }
        }
    }

    public void GoToChampionSelection()
    {
        deselectAll();
        MapSelect.SetActive(false);
        TrinketSelect.SetActive(false);
        ChampionSelect.SetActive(true);

        killCooldowns();

        popUps[0].transform.parent.gameObject.SetActive(true);


        if (AllChampionsSelected())
        {
            //Re select selected champions if existent
            foreach (Player player in players)
            {
                ChampionDatabase.Champions champ = ChampionPrefabs.First(x => x.Value == player.ChampionPrefab).Key; //Get enum value for champi9on prefab

                GameObject toSelect = GameObject.FindObjectsOfType<LocalChampionSelectionButtonChampion>().First(x => //Find button which holds thast enum value and according player number
                x.TargetPlayerNumber == player.playerNumber &&
                x.Champion == champ)
                .gameObject;

                eventSystemInstances[(int)player.playerNumber - 1].SetSelectedGameObject(toSelect); //Set event system to select that button
            }
        }
        else //just set first selecteds
            setSelectedGameObjectsForPage(ChampionSelectionFirstSelecteds);
    }

    /// <summary>
    /// Stops all countdown coroutines and hides according GOs
    /// </summary>
    private void killCooldowns()
    {
        if (TrinketsCountdownRoutine != null)
            StopCoroutine(TrinketsCountdownRoutine);
        if (ChampionsCountdownRoutine != null)
            StopCoroutine(ChampionsCountdownRoutine);
        if (MapsCountdownRoutine != null)
            StopCoroutine(MapsCountdownRoutine);

        CountDownChampions.gameObject.SetActive(false);
        CountDownChampions.gameObject.SetActive(false);
        CountDownMaps.gameObject.SetActive(false);
    }

    public void GoToMapSelection()
    {
        deselectAll();
        killCooldowns();

        LocalMultiplayerManager.Teams = new List<Team>
        {
            new Team(1, players.Where(x => x.TeamNumber == 1).ToList<Player>()),
            new Team(2, players.Where(x => x.TeamNumber == 2).ToList<Player>())
        };

        ChampionSelect.SetActive(false);
        TrinketSelect.SetActive(false);
        MapSelect.SetActive(true);

        setSelectedGameObjectsForPage(MapSelectionFirstSelecteds);
    }

    public void GoToTrinketSelection()
    {
        deselectAll();
        killCooldowns();
        ChampionSelect.SetActive(false);
        TrinketSelect.SetActive(true);
        MapSelect.SetActive(false);
        popUps[0].transform.parent.gameObject.SetActive(false);

        //Re select selected trinkets if existant
        if(AllTrinketsSelected())
        {
            foreach (Player player in players)
            {
                Trinket.Trinkets trinket1 = player.trinket1;
                Trinket.Trinkets trinket2 = player.trinket2;

                //Get buttons for player holding the two trinkets
                LocalChampionSelectionButtonTrinket buttonT1 = GameObject.FindObjectsOfType<LocalChampionSelectionButtonTrinket>().First(x => x.TargetPlayerNumber == player.playerNumber && x.trinket == trinket1);
                LocalChampionSelectionButtonTrinket buttonT2 = GameObject.FindObjectsOfType<LocalChampionSelectionButtonTrinket>().First(x => x.TargetPlayerNumber == player.playerNumber && x.trinket == trinket2);

                buttonT1.Selecting();
                eventSystemInstances[(int)player.playerNumber - 1].SetSelectedGameObject(buttonT2.gameObject);
            }
        }
        else
            setSelectedGameObjectsForPage(TrinketSelectionFirstSelecteds);
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
        AdvancedSceneManager.LoadSceneWithLoadingScreen(mapName, "MultiplayerLoadingScreen");
    }

    private bool AllChampionsSelected()
    {
        //Only allow to start if selections are complete
        bool allSelected = true;
        //Check if every player has selected a champion and 2 different trinkets
        foreach (Player player in players)
            if (player.ChampionPrefab == null)
                allSelected = false;

        return allSelected;
    }

    private bool AllTrinketsSelected()
    {
        //Only allow to start if selections are complete
        bool allSelected = true;
        //Check if every player has selected a champion and 2 different trinkets
        foreach (Player player in players)
            if (player.trinket1 == Trinket.Trinkets.None || player.trinket2 == Trinket.Trinkets.None)
                allSelected = false;

        return allSelected;
    }
}
