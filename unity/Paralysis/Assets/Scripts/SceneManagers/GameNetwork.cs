using System.IO;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameNetwork : MonoBehaviour {

    private static GameNetwork instance;
    public static GameNetwork Instance { get { return instance; } }

    private GameObject manager;

    public int sendRate = 70;
    public int sendRateSerialize = 50;
    public byte maxPlayers = 4;
    public string levelToLoad = "Network Test";
    public string gameVersion = "0.0.1-Apla";
    public bool offlineMode = false;

    private PhotonView photonV;

    private int playersFinishedLoadingScene = 0;
    public int PlayersInGame { get { return PhotonNetwork.playerList.Length; } }
    private string playerName;
    public string PlayerName { get { return playerName; } }
    private int playerNetworkNumber = 1;
    public int PlayerNetworkNumber { get { return playerNetworkNumber; } }

    public bool IsMasterClient { get { return PhotonNetwork.isMasterClient; } }
    private Dictionary<int , PhotonPlayer> playerDic; 

    private const string TEST_ROOM_NAME = "TEST_ROOM";

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

    private void Start()
    {
        if(!offlineMode)
        {
            print("Connecting to server..");
            PhotonNetwork.ConnectUsingSettings(gameVersion);
        }
    }

    // Unity Call back
    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        clearNewScene();

        if(offlineMode){
            return;
        }

        manager = GameObject.Find("manager");

        //TODO If is game scene level there must be a manager in it..
        //we should probably switch this to check for scene names and not
        //an object
        if(manager != null)
        {
            if (offlineMode)
            {
                RPC_CreatePlayer();
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

        offlineMode = true;
        PhotonNetwork.offlineMode = true;
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

    public void StartGame()
    {
        if(PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.room.IsOpen = false;
            PhotonNetwork.room.IsVisible = false;
            PhotonNetwork.LoadLevel(levelToLoad);
            print("Start game clicked...");
        }
        else 
        {
            print("Not master client.");
        }
    }

    public RoomInfo[] getCurrenRooms()
    {
        return PhotonNetwork.GetRoomList();
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

        if (PhotonNetwork.CreateRoom(TEST_ROOM_NAME, roomOptions, TypedLobby.Default))
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

    public bool joinRoom(string roomName)
    {
        if (PhotonNetwork.JoinRoom(TEST_ROOM_NAME))
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

    #region Photon callbacks

    //Photon Callback
    private void OnConnectedToMaster()
    {
        print("Connected to master.");
        PhotonNetwork.automaticallySyncScene = false;
        PhotonNetwork.playerName = GameNetwork.Instance.PlayerName;
        PhotonNetwork.JoinLobby(TypedLobby.Default);

        //TODO remove this when we implement our own room creator/joiner
#if UNITY_EDITOR
        createRoom(TEST_ROOM_NAME, 4, true, true);
#else
        joinRoom(TEST_ROOM_NAME);
#endif
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
    private void OnJoinedRoom()
    {
        playerNetworkNumber = PhotonNetwork.playerList.Length;
        print("Joined room: " + playerNetworkNumber);
    }

    //Photon Callback
    private void OnCreatedRoom()
    {

    }

    //Photon Callback
    private void OnMasterClientSwitched(PhotonPlayer newMasterClient)
    {
        quitGame();
    }

    //Photon Callback
    private void OnPhotonPlayerConnected(PhotonPlayer photonPlayer)
    {
        print("player connected: " + photonPlayer.NickName);
    }

    //Photon Callback
    private void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        print("player disconnected: " + otherPlayer.NickName);
        //If we arnt the master client decrease are network number, someone left
        if(!PhotonNetwork.isMasterClient)
        {
            playerNetworkNumber--;
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
        print("Player finished loading sceen.");
        playersFinishedLoadingScene++;
        if (playersFinishedLoadingScene == PhotonNetwork.playerList.Length)
        {
            playersFinishedLoadingScene = 0;
            print("All players are in the game scene.");
            photonV.RPC("RPC_CreatePlayer", PhotonTargets.All);
        }
    }

    [PunRPC]
    public void RPC_CreatePlayer()
    {
        //Tell the manager to spawn the player
        manager.GetComponent<NetworkVersusManager>().spawnPlayer();
    }

    #endregion


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


