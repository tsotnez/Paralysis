using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InRoomManager : UIManager {

    //Please notice: The PlayerListEntry objects have a ready image which is supposed to show whether the player has entered the ready state
    //Note 2: The Start-Button in the bottom center may be replaced by a ready button depending on whether the local player owns the room

    [HideInInspector]
    public Object currentScene;
    public Image MapPreviewImage;
    public Text MapPreviewText;
    public GameObject PlayerListEntryPrefab;
    public Transform PlayerListTeam1;
    public Transform PlayerListTeam2;
    public Text PlayerCount;
    public Text RoomNameText;
    public Button startGameButton;
    public Button backButton;
    public Button switchTeamButton;

    int CurrentPlayers = 1;
    int MaxPlayers = 4;

    private Dictionary<PhotonPlayer, GameObject> playerDict = new Dictionary<PhotonPlayer, GameObject>();

    protected override void Start()
    {
        base.Start();

        backButton.onClick.AddListener(backPressed);
        startGameButton.onClick.AddListener(goToChampSelect);
        switchTeamButton.onClick.AddListener(switchTeamsPressed);

        RoomNameText.text = GameNetwork.Instance.CurrentRoomInfo.Name;

        if (GameNetwork.Instance.IsMasterClient)
        {
            startGameButton.interactable = true;
        }
        else
        {
            startGameButton.interactable = false;
        }

        OnGameStateUpdated();
        GameNetwork.Instance.OnGameStateUpdate += OnGameStateUpdated;

        print("Started preloading next scene....");
        StartCoroutine(StartLoadingChampSelect());
    }

    public void ChangeCurrentPreviewImage(Image preview)
    {
        MapPreviewImage.sprite = preview.sprite;
    }

    public void ChangeCurrentPreviewMapName(Text text)
    {
        MapPreviewText.text = text.text;
    }

    public void ChangeCurrentMap(Object newMap)
    {
        currentScene = newMap;
    }

    //Call this when a player Joins
    public void AddPlayerToList(PhotonPlayer player, Transform playListTrans)
    {
        GameObject newPlayer;
        newPlayer = Instantiate(PlayerListEntryPrefab, playListTrans);
        newPlayer.transform.Find("Name").GetComponent<Text>().text = player.NickName;
        playerDict.Add(player, newPlayer);
    }

    public void RemovePlayerFromList(PhotonPlayer player)
    {
        CurrentPlayers--;
        setPlayerCountText();
        Destroy(playerDict[player]);
    }

    private void RemoveAll()
    {
        CurrentPlayers = 0;
        foreach(KeyValuePair<PhotonPlayer, GameObject> entry in playerDict)
        {
            Destroy(playerDict[entry.Key]);
        }
        playerDict.Clear();
    }

    private void setPlayerCountText()
    {
        PlayerCount.text = "Players:" +  CurrentPlayers + "/" + MaxPlayers;
    }

    private void goToChampSelect()
    {
        print("Loading next scene.....");
        GameNetwork.Instance.lockCurrentRoom(true);
        StartCoroutine(ActivateChampSelect());
        //PhotonNetwork.LoadLevel(GameConstants.NETWORK_CHAMP_SELECT);
    }

    private void backPressed()
    {
        StartCoroutine(waitForGameNetworkDestroyed());
    }

    private void switchTeamsPressed()
    {
        GameNetwork.Instance.switchPlayerTeam(PhotonNetwork.player.ID);
        switchTeamButton.interactable = false;
        StartCoroutine(setSwitchTeamInteractable());
    }

    private IEnumerator setSwitchTeamInteractable()
    {
        yield return new WaitForSecondsRealtime(2f);
        switchTeamButton.interactable = true;
    }

    private IEnumerator waitForGameNetworkDestroyed ()
    {
        Destroy(GameNetwork.Instance.gameObject);
        yield return new WaitWhile( ()=> GameNetwork.Instance == null);
        SceneManager.LoadScene(GameConstants.MAIN_MENU_SCENE);
    }

    #region PhotonCallbacks

    private void OnMasterClientSwitched(PhotonPlayer newMasterClient)
    {
        backPressed();
    }

    #endregion

    private void OnGameStateUpdated()
    {
        print("Game state updated...");
        RemoveAll();

        List<int> teamOne = GameNetwork.Instance.TeamOneList;
        List<int> teamTwo = GameNetwork.Instance.TeamTwoList;

        PhotonPlayer[] players = GameNetwork.Instance.getPlayerList();

        foreach(int i in teamOne)
        {
            foreach(PhotonPlayer player in players)
            {
                if(player.ID == i)
                {
                    AddPlayerToList(player, PlayerListTeam1);
                }
            }
        }

        foreach(int i in teamTwo)
        {
            foreach(PhotonPlayer player in players)
            {
                if(player.ID == i)
                {
                    AddPlayerToList(player, PlayerListTeam2);
                }
            }
        }

        CurrentPlayers = players.Length;
        setPlayerCountText();
    }

    private void OnDestroy()
    {
        GameNetwork.Instance.OnGameStateUpdate -= OnGameStateUpdated;
    }

    #region asyncload
    private AsyncOperation async;
    private IEnumerator StartLoadingChampSelect()
    {
        async = SceneManager.LoadSceneAsync(GameConstants.NETWORK_CHAMP_SELECT);
        async.allowSceneActivation = false;
        yield return async;
    }

    private IEnumerator ActivateChampSelect()
    {
        if(async == null)
        {
            yield return new WaitForEndOfFrame();
        }
        async.allowSceneActivation = true;
    }
    #endregion

}
