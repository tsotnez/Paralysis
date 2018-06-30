using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkChampionSelectionManager : ChampionSelectionManager {

    //TO KYLE: I commented out much, feel free to keep what you need and delete what you don't. (Open up "Old Stuff" Region) <3


    /// <summary>
    /// Player Object of the guy playing (Should be set externally?) 
    /// </summary>
    public static Player localPlayer = new Player(UserControl.PlayerNumbers.Player1, UserControl.InputDevice.KeyboardMouse, 1);

    /// <summary>
    /// Parent GO of all slots containing the portrtaits of each champion picked for team 1
    /// </summary>
    public Transform slotsTeam1;

    /// <summary>
    /// Parent GO of all slots containing the portrtaits of each champion picked for team 2
    /// </summary>
    public Transform slotsTeam2;
    /// <summary>
    /// Contains all image files of champion portraits shadowed
    /// </summary>
    public Sprite[] championPortraitsShadow;
    /// <summary>
    /// Contains all image files of champion portraits Lit
    /// </summary>
    public Sprite[] championPortraitsLit;
    /// <summary>
    /// Platform on which animated preview will be shown
    /// </summary>
    public GameObject platform;
    public SlideSelection championSelectionBar;
    /// <summary>
    /// Nickname of local Player. i don't know where you get this from...
    /// </summary>
    private GameObject playerSlots;
    private PlayerInfo localPlayerInfo;
    private ChampionDatabase.Champions[] allChamps;
    private GameObject OnClickAnimation;
    private GameNetworkChampSelect networkChampSelect;
    private Dictionary <int, PlayerInfo> photonIDToSlot = new Dictionary<int, PlayerInfo>();
    private Dictionary <int, bool> readyDictionary = new Dictionary<int, bool>();
    private bool gameStarting = false;

    protected override void Start()
    {
        GameNetwork.Instance.OnGameStateUpdate += OnGameStateUpdated;
        GameNetworkChampSelect.Instance.OnPlayerSelectedChamp += OnPlayerSelectedChamp;
        GameNetworkChampSelect.Instance.OnPlayerReady += OnPlayerReady;
        GameNetworkChampSelect.Instance.OnAllReady += OnAllReady;

        OnClickAnimation = Resources.Load<GameObject>("Prefabs/UI/LocalChampionSelection/ClickAnimation");
        networkChampSelect = GetComponent<GameNetworkChampSelect>();
        allChamps = ChampionDatabase.GetAllChampions();
        championSelectionBar.SelectedGOChangedHandler += ChampionSelectionBar_SelectedGOChangedHandler;

        Transform targetTeamParent = GameNetwork.Instance.TeamNum == 1 ? slotsTeam1 : slotsTeam2;

        //TODO setting default trinket trinkets here for
        localPlayer.trinket1 = Trinket.Trinkets.UseTrinket_ReflectDamage;
        localPlayer.trinket2 = Trinket.Trinkets.PassiveTrinket_ChanceToStun;

        intializeReady();
        OnGameStateUpdated();

        ChampionSelectionBar_SelectedGOChangedHandler(null, null);
    }

    internal void setTrinket(Trinket.Trinkets trinket, int trinketId)
    {
        //TODO this method is never called...
        print("Setting trinket: " + trinket.ToString() + " num: " + trinketId);

        if (trinketId == 1)
        {
            localPlayer.trinket1 = trinket;
            GameNetworkChampSelect.Instance.setTrinket(trinket, 1);
        }
        else if (trinketId == 2)
        {
            localPlayer.trinket2 = trinket;
            GameNetworkChampSelect.Instance.setTrinket(trinket, 2);
        }
    }

    private void ChampionSelectionBar_SelectedGOChangedHandler(object sender, EventArgs e)
    {
        if (gameStarting) return;

        ChampionDatabase.Champions newChamp = championSelectionBar.SelectedGO.GetComponent<ChampionVariable>().content;
        Sprite portrait = championPortraitsShadow[Array.IndexOf(allChamps, newChamp)];

        localPlayerInfo.portrait.sprite = portrait;
        localPlayer.ChampionPrefab = ChampionPrefabs[newChamp];

        DestroyExistingPreview(platform.transform);
        ShowPrefab(ChampionPrefabs[newChamp], platform.transform, false);

        GameNetworkChampSelect.Instance.selectedChamipon((int)newChamp);
    }

    /// <summary>
    /// Called when player locked in his champion or time's up
    /// </summary>
    public void ReadyClickHandler()
    {
        if (gameStarting) return;

        StartCoroutine(handleOnclickAnimation(localPlayerInfo.slot.transform));
        ChampionDatabase.Champions newChamp = championSelectionBar.SelectedGO.GetComponent<ChampionVariable>().content;
        Sprite portrait = championPortraitsLit[Array.IndexOf(allChamps, newChamp)];

        localPlayerInfo.portrait.sprite = portrait;
        localPlayer.ChampionPrefab = ChampionPrefabs[newChamp];

        GameNetworkChampSelect.Instance.readyUp((int)newChamp, true);
    }

    /// <summary>
    /// Instantiates Object which plays the on click animation and destroys it after animation has finished
    /// </summary>
    /// <returns></returns>
    private IEnumerator handleOnclickAnimation(Transform parent)
    {
        GameObject anim = Instantiate(OnClickAnimation, parent, false);
        yield return new WaitForSeconds(.333f);
        Destroy(anim);
    }


    private void setSlotInfo(Transform item, int playerNum, List<int> teamList) {

        GameObject slot = item.gameObject;

        if (teamList.Count > playerNum)
        {
            int photonID = teamList[playerNum];

            PlayerInfo playerInfo = new PlayerInfo();

            //Set variables
            playerInfo.playerNameText = item.Find("Labels").Find("PlayerName").GetComponent<Text>();
            playerInfo.championNameText = item.Find("Labels").Find("ChampionName").GetComponent<Text>();
            playerInfo.portrait = item.Find("Portrait").GetComponent<Image>();
            playerInfo.slot = item.gameObject;

            playerInfo.playerNameText.text = GameNetwork.Instance.getPlayerNameForID(photonID);

            photonIDToSlot[photonID] = playerInfo;
            if (photonID == PhotonNetwork.player.ID)
            {
                localPlayerInfo = playerInfo;
            }
        }
        else
        {
            slot.SetActive(false);
        }
    }

    private void checkAllReady()
    {
        foreach (KeyValuePair<int,bool> ready in readyDictionary)
        {
            if (!ready.Value)
            {
                return;
            }
        }

        //We made it through
        GameNetworkChampSelect.Instance.allReadySignal();
    }

    private void intializeReady()
    {
        //Set everyone to not ready
        if (PhotonNetwork.isMasterClient)
        {
            readyDictionary.Clear();
            PhotonPlayer[] players = GameNetwork.Instance.getPlayerList();
            foreach (PhotonPlayer player in players)
            {
                readyDictionary[player.ID] = false;
            }
        }
    }

    #region callbacks
    private void OnGameStateUpdated() 
    {
        if (gameStarting) return;

        List<int> teamOne = GameNetwork.Instance.TeamIdList(1);
        List<int> teamTwo = GameNetwork.Instance.TeamIdList(2);

        photonIDToSlot.Clear();
        foreach (Transform item in slotsTeam1)
        {
            if(item.name.Length > 3 && item.name.StartsWith("Slot"))
            {
                int playerNum = int.Parse(item.name[4].ToString());
                setSlotInfo(item, playerNum - 1, teamOne);
            }
        }

        foreach (Transform item in slotsTeam2)
        {
            if(item.name.Length > 3 && item.name.StartsWith("Slot"))
            {
                int playerNum = int.Parse(item.name[4].ToString());
                setSlotInfo(item, playerNum - 1, teamTwo);
            }
        }
    }


    private void OnPlayerSelectedChamp(int photonID, int champ)
    {
        if(PhotonNetwork.player.ID == photonID)
        {
            return;
        }
        Sprite portrait = championPortraitsShadow[Array.IndexOf(allChamps, ChampionDatabase.getChampionEnumForID(champ))];
        photonIDToSlot[photonID].portrait.sprite = portrait;
    }


    private void OnPlayerReady(int photonID, int champID, bool ready) 
    {
        // If its not my player
        if(PhotonNetwork.player.ID != photonID)
        {
            StartCoroutine(handleOnclickAnimation(photonIDToSlot[photonID].slot.transform));
            ChampionDatabase.Champions newChamp = ChampionDatabase.getChampionEnumForID(champID);
            Sprite portrait = championPortraitsLit[Array.IndexOf(allChamps, newChamp)];
            photonIDToSlot[photonID].portrait.sprite = portrait;
        }

        if (PhotonNetwork.isMasterClient)
        {
            readyDictionary[photonID] = true;
            checkAllReady();
        }
    }

    private void OnAllReady()
    {
        gameStarting = true;
        StartCoroutine(startingGame());
    }


    private void OnDestroy()
    {
        GameNetwork.Instance.OnGameStateUpdate -= OnGameStateUpdated;
        GameNetworkChampSelect.Instance.OnPlayerSelectedChamp -= OnPlayerSelectedChamp;
        GameNetworkChampSelect.Instance.OnPlayerReady -= OnPlayerReady;
        GameNetworkChampSelect.Instance.OnAllReady -= OnAllReady;
    }

    private void OnMasterClientSwitched(PhotonPlayer newMasterClient) 
    {
        StartCoroutine(GameNetwork.Instance.waitForGameNetworkDestroyed());
    }    

    #endregion
    private IEnumerator startingGame()
    {
        //TODO do somekind of count down here..
        //vs_text.text = "Starting game....";

        yield return new WaitForSeconds(3);
        if(GameNetwork.Instance.IsMasterClient)
        {
            GameNetwork.Instance.StartGame();
        }
    }

    private class PlayerInfo 
    {
        public Text playerNameText;
        public Text championNameText;
        public Image portrait;
        public GameObject slot;
    }

    #region Old Stuff
    //public static Player localPlayer = new Player(UserControl.PlayerNumbers.Player1, UserControl.InputDevice.KeyboardMouse, 1); //Object that stores all date about local player, set externally
    //private int playerCount; //How many players are in the room? needed to show/hide additional Platforms. Set externally

    //public GameObject additionalPlatforms3v3;

    //public Transform[] teamOnePlatforms;
    //public Transform[] teamTwoPlatforms;

    //private Transform myPlatform;

    //bool everythingSelected = false;

    //public Button rdyButton;
    //public Button[] championButtons;
    //public Text vs_text;

    //private ChampionHolder champHolder;
    ////PhotonID to transform
    //private Dictionary<int, Transform> transformDict = new Dictionary<int, Transform>();
    //private bool imReady = false;

    //#region overrides
    //protected override void Start()
    //{
    //    base.Start();
    //    champHolder = GameNetwork.Instance.GetComponent<ChampionHolder>();

    //    //ARCHER = 0, KNIGHT = 1, INFANTRY = 2, ALCHEMIST = 3, ASSASSIN = 4
    //    foreach(Button button in championButtons)
    //    {
    //        button.onClick.AddListener(delegate{
    //            champButtonPressed(button);
    //        });
    //    }

    //    GameNetwork.Instance.OnGameStateUpdate += OnGameStateUpdated;
    //    GameNetworkChampSelect.Instance.OnPlayerSelectedChamp += OnPlayerSelectedChamp;
    //    GameNetworkChampSelect.Instance.OnPlayerReady += OnPlayerReady;
    //    GameNetworkChampSelect.Instance.OnAllReady += OnAllReady;


    //    playerCount = GameNetwork.Instance.getPlayerList().Length;
    //    if(playerCount < 6)
    //    {
    //        additionalPlatforms3v3.SetActive(false);
    //    }
    //    if(playerCount < 4)
    //    {
    //        //additionalPlatforms2v2.SetActive(false);
    //    }

    //    OnGameStateUpdated();
    //}

    //protected override void Update()
    //{
    //    base.Update();

    //    //Show ready button when champion and trinkets are selected
    //    everythingSelected = true;

    //    if (localPlayer.ChampionPrefab == null || localPlayer.trinket1 == localPlayer.trinket2)
    //        everythingSelected = false;
    //}

    //public void setTrinket(UserControl.PlayerNumbers targetPlayer, Trinket.Trinkets trinketName, Trinket.Trinkets toOverwrite)
    //{
    //    if (localPlayer.trinket1 == toOverwrite)
    //        localPlayer.trinket1 = trinketName;
    //    else
    //        localPlayer.trinket2 = trinketName;
    //}

    //public void setTrinket1(UserControl.PlayerNumbers targetPlayer, Trinket.Trinkets trinketName)
    //{
    //    localPlayer.trinket1 = trinketName;
    //    GameNetworkChampSelect.Instance.setTrinket(trinketName, 1);
    //}

    //public void setTrinket2(UserControl.PlayerNumbers targetPlayer, Trinket.Trinkets trinketName)
    //{
    //    localPlayer.trinket2 = trinketName;
    //    GameNetworkChampSelect.Instance.setTrinket(trinketName, 2);
    //}

    //public override void setChampion(UserControl.PlayerNumbers targetPlayer, GameObject Champion)
    //{
    //    localPlayer.ChampionPrefab = Champion;
    //    DestroyExistingPreview(myPlatform);
    //    ShowPrefab(Champion, myPlatform, (GameNetwork.Instance.TeamNum == 1) ? false : true );
    //}

    ///// <summary>
    ///// Called when all players are ready
    ///// </summary>
    //public override void startGame()
    //{
    //    if(GameNetwork.Instance.IsMasterClient)
    //    {
    //        GameNetwork.Instance.StartGame();
    //    }
    //}

    //#endregion

    //private void disableAllButtons()
    //{
    //    foreach(Button champB in championButtons)
    //    {
    //        champB.interactable = false;
    //    }
    //    rdyButton.interactable = false;
    //}

    //private void AddPlayerToList(PhotonPlayer player, Transform playerTransform)
    //{
    //    transformDict[player.ID] = playerTransform;

    //    if(player == PhotonNetwork.player)
    //    {
    //        myPlatform = playerTransform;
    //    }
    //    Text playerText = playerTransform.GetComponentInChildren<Text>();
    //    playerText.text = player.NickName;
    //    playerTransform.Find("Ready").GetComponent<Image>().enabled = false;
    //}

    //private void SetTransformToEmpty(Transform playerTransform)
    //{
    //    playerTransform.gameObject.SetActive(false);
    //}

    //private void setReady(Transform trans, bool ready)
    //{
    //    trans.Find("Ready").GetComponent<Image>().enabled = ready;
    //}

    //public void toggleReady()
    //{
    //    if(everythingSelected)
    //    {
    //        //Show/Hide ready sign for local player here. Called by ready button.
    //        imReady = !imReady;
    //        setReady(myPlatform, imReady);
    //        GameNetworkChampSelect.Instance.readyUp(imReady);
    //        checkReadyAll();
    //    }
    //    else
    //    {
    //        StartCoroutine(UIManager.showMessageBox(GameObject.FindObjectOfType<Canvas>(), "Select one champion and two different trinkets."));
    //    }
    //}

    //private void champButtonPressed(Button button)
    //{
    //    //ARCHER = 0, KNIGHT = 1, INFANTRY = 2, ALCHEMIST = 3, ASSASSIN = 4
    //    for(int i = 0; i < championButtons.Length; i++)
    //    {
    //        Button champButton = championButtons[i];
    //        if(champButton == button)
    //        {
    //            GameNetworkChampSelect.Instance.selectedChamipon(i);
    //        }
    //    }
    //}

    //private void checkReadyAll()
    //{
    //    if(!GameNetwork.Instance.IsMasterClient)
    //    {
    //        return;
    //    }

    //    bool allReady = true;
    //    foreach(KeyValuePair<int, Transform> entry in transformDict)
    //    {
    //        Transform trans = entry.Value;
    //        Image img = trans.Find("Ready").GetComponent<Image>();
    //        if(!img.enabled)
    //        {
    //            allReady = false;
    //        }
    //    }

    //    if(allReady)
    //    {
    //        GameNetworkChampSelect.Instance.allReadySignal();
    //    }
    //}

    //private IEnumerator startingGame()
    //{
    //    //TODO do somekind of count down here..
    //    vs_text.text = "Starting game....";

    //    yield return new WaitForSeconds(3);
    //    if(GameNetwork.Instance.IsMasterClient)
    //    {
    //        startGame();
    //    }
    //}

    //#region callbacks
    //private void OnPlayerSelectedChamp(int photonID, int champ)
    //{
    //    if(PhotonNetwork.player.ID == photonID)
    //    {
    //        return;
    //    }

    //    GameObject goChamp = champHolder.getChampionForID(champ);
    //    List<int> teamOne = GameNetwork.Instance.TeamIdList(1);
    //    Transform platform = transformDict[photonID];
    //    DestroyExistingPreview(platform);
    //    ShowPrefab(goChamp, platform, (teamOne.Contains(photonID))? false : true );
    //}

    //private void OnPlayerReady(int photonID, bool ready)
    //{
    //    if(PhotonNetwork.player.ID == photonID)
    //    {
    //        return;
    //    }

    //    setReady(transformDict[photonID], ready);
    //    checkReadyAll();
    //}

    //private void OnAllReady()
    //{
    //    disableAllButtons();
    //    StartCoroutine(startingGame());
    //}

    //private void OnGameStateUpdated()
    //{
    //    List<int> teamOne = GameNetwork.Instance.TeamIdList(1);
    //    List<int> teamTwo = GameNetwork.Instance.TeamIdList(2);

    //    PhotonPlayer[] players = GameNetwork.Instance.getPlayerList();
    //    Dictionary<int, int> playerDict = GameNetwork.Instance.PlayerDict;
    //    transformDict.Clear();

    //    for(int i = 0; i < 3; i++)
    //    {
    //        if(i < teamOne.Count)
    //        {
    //            bool found = false;
    //            int playerId = teamOne[i];
    //            foreach(PhotonPlayer player in players)
    //            {
    //                if(playerId == player.ID)
    //                {
    //                    AddPlayerToList(player, teamOnePlatforms[i]);
    //                    found = true;
    //                }
    //            }
    //            if(!found)SetTransformToEmpty(teamOnePlatforms[i]);
    //        }
    //        else
    //        {
    //            SetTransformToEmpty(teamOnePlatforms[i]);
    //        }

    //        if(i < teamTwo.Count)
    //        {
    //            bool found = false;
    //            int playerId = teamTwo[i];
    //            foreach(PhotonPlayer player in players)
    //            {
    //                if(playerId == player.ID)
    //                {
    //                    AddPlayerToList(player, teamTwoPlatforms[i]);
    //                    found = true;
    //                }
    //            }
    //            if(!found)SetTransformToEmpty(teamTwoPlatforms[i]);
    //        }
    //        else
    //        {
    //            SetTransformToEmpty(teamTwoPlatforms[i]);
    //        }
    //    }       
    //}

    //#endregion

    //private void OnDestroy()
    //{
    //    //GameNetwork.Instance.OnGameStateUpdate -= OnGameStateUpdated;
    //    //GameNetworkChampSelect.Instance.OnPlayerSelectedChamp -= OnPlayerSelectedChamp;
    //    //GameNetworkChampSelect.Instance.OnPlayerReady -= OnPlayerReady;
    //    //GameNetworkChampSelect.Instance.OnAllReady -= OnAllReady;
    //}
    #endregion
}
