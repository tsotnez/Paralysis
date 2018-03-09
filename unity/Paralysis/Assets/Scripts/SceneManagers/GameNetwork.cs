using System.IO;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameNetwork : MonoBehaviour {

    public const byte MAX_PLAYERS = 6;

    private static GameNetwork instance;
    public static GameNetwork Instance { get { return instance; } }

    private NetworkVersusManager manager;

    public int sendRate = 70;
    public int sendRateSerialize = 50;
    public byte maxPlayers = 4;
    public string gameVersion = "0.0.1-Apla";
    public bool offlineMode = false;

    private PhotonView photonV;

    private bool inGame = false;
    public bool InGame { get { return inGame; } }

    private int playersFinishedLoadingScene = 0;
    public int PlayersInGame { get { return PhotonNetwork.playerList.Length; } }
    public int AICount { get { return 0; } }

    //Set some info about this current player
    private string playerName;
    public string PlayerName { get { return playerName; } }
    private int playerNetworkNumber = 1;
    public int PlayerNetworkNumber { get { return playerNetworkNumber; } }
    private int teamNum = 1;
    public int TeamNum { get { return teamNum; } }

    public RoomInfo CurrentRoomInfo { get { return PhotonNetwork.room; } }
    public bool IsMasterClient { get { return PhotonNetwork.isMasterClient; } }

    //photon player ID, player network number
    public Dictionary <int, int> PlayerDict { get { return new Dictionary<int, int>(playerDic); } }
    private Dictionary<int , int> playerDic;

    //photon player ID, to Team num 1,2,3,4...
    public Dictionary <int, int> PlayerTeamDict { get { return new Dictionary<int, int>(playerTeamDic); } }
    private Dictionary<int , int> playerTeamDic;

    //List of photon IDs for team1
    public List<int> TeamOneList { 
        get { 
            List<int> teamOneList = new List<int>();
            foreach(KeyValuePair<int, int> entry in playerTeamDic)
            {
                if(entry.Value == 1){
                    teamOneList.Add(entry.Key);
                }
            }
            return teamOneList;
        } }

    //List of photon IDs for team2
    public List<int> TeamTwoList { 
        get {
            List<int> teamTwoList = new List<int>();
            foreach(KeyValuePair<int, int> entry in playerTeamDic)
            {
                if(entry.Value == 2){
                    teamTwoList.Add(entry.Key);
                }
            }
            return teamTwoList;
        } }

    //Delegates
    public delegate void gameStateUpdate();
    public event gameStateUpdate OnGameStateUpdate;

    public delegate void allPlayersInScene();
    public event allPlayersInScene OnAllPlayersInScene;

    // Use this for initialization
    private void Awake ()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        photonV = GetComponent<PhotonView>();

        setPlayerName("Player#" + Random.Range(1000, 9999));
        PhotonNetwork.sendRate = sendRate;
        PhotonNetwork.sendRateOnSerialize = sendRateSerialize;
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    public void Connect(string region = "eu")
    {
        if(!offlineMode)
        {
            print("Connecting to server: " + region);
            if(region.ToLower().Equals("us"))
            {
                PhotonNetwork.ConnectToRegion(CloudRegionCode.us, gameVersion);
            }
            else if(region.ToLower().Equals("eu"))
            {
                PhotonNetwork.ConnectToRegion(CloudRegionCode.eu, gameVersion);
            }
            else
            {
                PhotonNetwork.ConnectUsingSettings(gameVersion);
            }
        }
    }

    // Unity Call back
    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if(!IsConnected())return;

        clearNewScene();

        if(offlineMode){
            return;
        }

        if (PhotonNetwork.isMasterClient)
        {
            MasterLoadedGame();
        }
        else
        {
            NonMasterLoadedGame();
        }
    }

    private void clearNewScene()
    {
        manager = null;

        if (offlineMode)
        {
            playersFinishedLoadingScene = 1;
        }
        else
        {
            playersFinishedLoadingScene = 0;
        }
    }

    public void setOfflineMode()
    {
        if (offlineMode) return;
        leaveCurrentRoom();
        PhotonNetwork.Disconnect();
        offlineMode = true;
        PhotonNetwork.offlineMode = offlineMode;
        PhotonNetwork.CreateRoom("OfflineRoom");
        playersFinishedLoadingScene = 1;
        playerNetworkNumber = 1;
    }

    public void setOnlineMode()
    {
        if (!offlineMode) return;
        playerNetworkNumber = 1;
        playersFinishedLoadingScene = 0;

        offlineMode = false;
        if (PhotonNetwork.inRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    public void KickPlayer(string playerName)
    {
        if(PhotonNetwork.isMasterClient)
        {
            foreach (PhotonPlayer player in PhotonNetwork.playerList)
            {
                if(player.NickName == playerName)
                {
                    if (!player.IsMasterClient)
                    {
                        PhotonNetwork.CloseConnection(player);
                    }
                }
            }
        }
    }

    public RoomInfo[] getCurrenRooms()
    {
        return PhotonNetwork.GetRoomList();
    }

    public PhotonPlayer[] getPlayerList()
    {
        return PhotonNetwork.playerList;
    }

    public void setPlayerName(string playerName)
    {
        PhotonNetwork.playerName = playerName;
        this.playerName = playerName;
        print("photon player name set to: " + playerName);
    }

    public bool createRoom(string roomName, byte maxPlayers, bool isVisible, bool isOpen = true)
    {
        RoomOptions roomOptions = new RoomOptions() {
            IsVisible = isVisible,
            IsOpen = isOpen,
            MaxPlayers = maxPlayers };

        if (PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default))
        {
            print("create room successfully sent.");
            return true;
        }
        else
        {
            print("create room failed to send");
            return false;
        }
    }

    public bool createRandomPrivateRoom()
    {
        RoomInfo[] rooms = getCurrenRooms();
        string potentialRoomName = "";
        bool foundRoomName = false;

        while(!foundRoomName)
        {
            potentialRoomName = "Room#" + Random.Range(1000, 9999);
            foreach(RoomInfo roomInfo in rooms)
            {
                if(roomInfo.Name.Equals(potentialRoomName))
                {
                    continue;
                }
            }
            foundRoomName = true;
        }

        return createRoom(potentialRoomName, maxPlayers, false, true);
    }

    public bool joinRoom(string roomName)
    {
        if (PhotonNetwork.JoinRoom(roomName))
        {
            print("Joined room: " + roomName);
            return true;
        }
        else
        {
            print("Join room failed: " + roomName);
            return false;
        }
    }

    public void leaveCurrentRoom()
    {
        if(PhotonNetwork.inRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
    }


    public void StartGame()
    {
        if(PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.room.IsOpen = false;
            PhotonNetwork.room.IsVisible = false;
            PhotonNetwork.LoadLevel(GameConstants.NETWORK_TEST_SCENE);
            print("Start game clicked...");
        }
        else 
        {
            print("Not master client.");
        }
    }

    public void NextScene()
    {
        if(PhotonNetwork.isMasterClient)
        {
            photonV.RPC("RPC_LoadGameOthers", PhotonTargets.Others, SceneManager.GetActiveScene().name);
        }
    }

    public bool isRoomLocked()
    {
        return PhotonNetwork.inRoom && !PhotonNetwork.room.IsOpen && !PhotonNetwork.room.IsVisible;
    }

    public void lockCurrentRoom(bool setLocked)
    {
        if(PhotonNetwork.isMasterClient && PhotonNetwork.inRoom)
        {
            PhotonNetwork.room.IsOpen = !setLocked;
            PhotonNetwork.room.IsVisible = !setLocked;
        }
    }

    public bool IsConnected()
    {
        return PhotonNetwork.connected && !PhotonNetwork.offlineMode;
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }

    public void switchPlayerTeam(int photonP)
    {
        if(!PhotonNetwork.isMasterClient)
        {
            photonV.RPC("RPC_SwitchTeam", PhotonTargets.MasterClient, photonP);
        }
        else
        {
            RPC_SwitchTeam(photonP);
        }
    }
        
    #region setplayerinfo

    private bool switchTeam(int photonP)
    {
        if(!PhotonNetwork.isMasterClient)return false;

        if(playerTeamDic.ContainsKey(photonP))
        {
            int currentTeam = playerTeamDic[photonP];
            if(currentTeam == 1)
            {
                playerTeamDic[photonP] = 2;
            }
            else
            {
                playerTeamDic[photonP] = 1;
            }

            return true;
        }

        return false;
    }

    private void addPlayer(int photonId)
    {
        if(!IsMasterClient)return;

        print("Adding player id: " + photonId + "player network num:" + PlayersInGame);

        playerDic.Add(photonId, PlayersInGame);
        if(balanceTeams() == 1)playerTeamDic[photonId] = 1;
        else playerTeamDic[photonId] = 2;
    }

    private void removePlayer(int photonId)
    {
        if(!IsMasterClient)return;

        print("Removing player id:" + photonId);

        int leavingPlayerNetNum = playerDic[photonId];
        playerDic.Remove(leavingPlayerNetNum);
        foreach(int key in playerDic.Keys)
        {
            //Decrement player network numbers
            if(playerDic[key] > leavingPlayerNetNum)
            {
                playerDic[key] = playerDic[key] - 1;
            }
        }
        removePlayerFromTeamLists(photonId);
    }

    private int balanceTeams()
    {
        int teamOneCount = 0;
        int teamTwoCount = 0;
        foreach(KeyValuePair<int, int> entry in playerTeamDic)
        {
            switch(entry.Value)
            {
                case 1:
                teamOneCount++;
                    break;
                case 2:
                teamTwoCount++;
                    break;
                default:
                    break;
            }
        }

        if(teamOneCount <= teamTwoCount)return 1;
        else return 2;
    }        

    public void removePlayerFromTeamLists(int photonId)
    {
        //if(teamTwoList.Contains(photonId))teamTwoList.Remove(photonId);
        //if(teamOneList.Contains(photonId))teamOneList.Remove(photonId);
        if(playerTeamDic.ContainsKey(photonId))playerTeamDic.Remove(photonId);
    }

    private void setMyTeam()
    {
        int id = PhotonNetwork.player.ID;
        if(playerTeamDic.ContainsKey(id)){
            teamNum = playerTeamDic[id];
        }
        else{
            Debug.LogError("Couldn't find my team number:" + PhotonNetwork.player.ID);
        }
    }

    private void setMyNetworkNumber()
    {
        foreach(KeyValuePair<int, int> entry in playerDic)
        {
            if(entry.Key == PhotonNetwork.player.ID)
            {
                playerNetworkNumber = entry.Value;
                print("Set my network number to: " + playerNetworkNumber);
                return;
            }
        }
        Debug.LogError("Couldn't find my network number:" + PhotonNetwork.player.ID);
    }

    #endregion

    #region Photon callbacks

    //Photon Callback
    private void OnConnectedToMaster()
    {
        print("Connected to master.");
        PhotonNetwork.automaticallySyncScene = false;
        PhotonNetwork.playerName = GameNetwork.Instance.PlayerName;
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    //Photon Callback
    private void OnPhotonCreateRoomFailed(object[] codeAndMessage)
    {
        print("Create room failed: " + codeAndMessage[1]);
    }

    //Photon Callback
    private void OnJoinedLobby()
    {
        print("Joined lobby.");
    }

    //Photon Callback
    // Called for everyone
    private void OnJoinedRoom()
    {
        //playerNetworkNumber = PhotonNetwork.playerList.Length;

        playerDic = new Dictionary<int, int>();
        playerTeamDic = new Dictionary<int, int>();

        if(IsMasterClient)
        {
            addPlayer(PhotonNetwork.player.ID);
        }
    }

    //Photon Callback
    private void OnCreatedRoom()
    {
        //Created room...
    }

    //Photon Callback
    private void OnMasterClientSwitched(PhotonPlayer newMasterClient)
    {
        if(inGame){
            quitGame();
        }
    }

    //Photon Callback
    private void OnPhotonPlayerConnected(PhotonPlayer photonPlayer)
    {
        print("player connected: " + photonPlayer.NickName);
        if(PhotonNetwork.isMasterClient)
        {
            addPlayer(photonPlayer.ID);
            photonV.RPC("RPC_UpdateGameInfo", PhotonTargets.All, playerDic, playerTeamDic);
        }
    }

    //Photon Callback
    private void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        print("player disconnected: " + otherPlayer.NickName);
        if(PhotonNetwork.isMasterClient)
        {
            removePlayer(otherPlayer.ID);
            photonV.RPC("RPC_UpdateGameInfo", PhotonTargets.All, playerDic, playerTeamDic);
        }
    }

    #endregion

    #region RPC

    //RPC stuff
    private void MasterLoadedGame()
    {
        RPC_LoadedGameScene();
        photonV.RPC("RPC_LoadGameOthers", PhotonTargets.Others, SceneManager.GetActiveScene().name);
    }

    private void NonMasterLoadedGame()
    {
        photonV.RPC("RPC_LoadedGameScene", PhotonTargets.MasterClient);
    }

    [PunRPC]
    private void RPC_LoadGameOthers(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }

    [PunRPC]
    //This method is only called by the server
    private void RPC_LoadedGameScene()
    {
        //TODO If is game scene level there must be a manager in it..
        //we should probably switch this to check for scene names and not
        //an object
        GameObject gOmanager = GameObject.Find("manager");
        if(gOmanager != null && gOmanager.GetComponent<NetworkVersusManager>() != null)
        {
            manager = gOmanager.GetComponent<NetworkVersusManager>();
        }

        print("Player finished loading scene.");
        playersFinishedLoadingScene++;
        if (playersFinishedLoadingScene == PhotonNetwork.playerList.Length)
        {
            playersFinishedLoadingScene = 0;
            print("All players are in the game scene.");
            if(OnAllPlayersInScene != null)
                OnAllPlayersInScene();

            if(manager != null)
            {
                photonV.RPC("RPC_SpawnPlayer", PhotonTargets.All);
            }
        }
    }

    [PunRPC]
    public void RPC_SpawnPlayer()
    {
        if(manager == null)
        {
            GameObject gOmanager = GameObject.Find("manager");
            manager = gOmanager.GetComponent<NetworkVersusManager>();
        }

        //Tell the manager to spawn the player
        inGame = true;
        manager.spawnPlayer();
    }

    [PunRPC]
    public void RPC_UpdateGameInfo(Dictionary<int, int> newPlayerDict, Dictionary<int, int> newPlayerTeamDict)
    {
        if(!PhotonNetwork.isMasterClient)
        {
            playerDic = newPlayerDict;
            playerTeamDic = newPlayerTeamDict;
        }

        setMyTeam();
        setMyNetworkNumber();

        if(OnGameStateUpdate != null)
            OnGameStateUpdate();
    }

    [PunRPC]
    public void RPC_SwitchTeam(int photonId)
    {
        if(!PhotonNetwork.isMasterClient) return;

        bool changed = switchTeam(photonId);
        if(changed){
            photonV.RPC("RPC_UpdateGameInfo", PhotonTargets.All, playerDic, playerTeamDic);
        }
            
    }

    #endregion

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
        PhotonNetwork.Disconnect();
    }

    public void quitGame()
    {
        print("Quitting game...");
        PhotonNetwork.Disconnect();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}


