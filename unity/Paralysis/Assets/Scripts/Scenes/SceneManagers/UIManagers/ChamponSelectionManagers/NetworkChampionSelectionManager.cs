using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkChampionSelectionManager : ChampionSelectionManager {

    public static NetworkChampionSelectionManager Instance;
     
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
    private PlayerSlot localPlayerInfo;
    private ChampionDatabase.Champions[] allChamps;
    private GameObject OnClickAnimation;
    private GameNetworkChampSelect networkChampSelect;
    private Dictionary <int, PlayerSlot> photonIDToSlot = new Dictionary<int, PlayerSlot>();
    private Dictionary <int, bool> readyDictionary = new Dictionary<int, bool>();
    private bool gameStarting = false;

    protected override void Awake()
    {
        Instance = this;
        base.Awake();
    }

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

        initializeScene();

        ChampionSelectionBar_SelectedGOChangedHandler(null, null);
    }

    public void setTrinket(Trinket.Trinkets trinket, int trinketId)
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
        ShowPrefab(ChampionPrefabs[newChamp], platform.transform, false, true);

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

            PlayerSlot playerInfo = new PlayerSlot();

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

        print("All players ready..");

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

    private void initializeScene()
    {
        intializeReady();

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

    #region callbacks
    private void OnGameStateUpdated() 
    {
        //Someone leaves, switches teams in this scene, just kill the game
        StartCoroutine(GameNetwork.Instance.waitForGameNetworkDestroyed());
    }

    private void OnMasterClientSwitched(PhotonPlayer newMasterClient) 
    {
        StartCoroutine(GameNetwork.Instance.waitForGameNetworkDestroyed());
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

    private class PlayerSlot
    {
        public Text playerNameText;
        public Text championNameText;
        public Image portrait;
        public GameObject slot;
    }
}
