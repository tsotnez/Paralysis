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

    public string PlayerName { get; set; }
    private PhotonView photonV;
    private PhotonPlayer photonPlayer;
    private int playersInGame = 0;
    public int PlayersInGame { get { return playersInGame; } }

    public bool offlineMode = false;

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

        PlayerName = "Player#" + Random.Range(1000, 9999);

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

    public void setOfflineMode()
    {
        if (offlineMode) return;

        offlineMode = true;
        PhotonNetwork.offlineMode = true;
        PhotonNetwork.CreateRoom("OfflineRoom");
        playersInGame = 1;
    }

    public void setOnlineMode()
    {
        if (!offlineMode) return;

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

    // Unity Call back
    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        clearNewScene();

        if(offlineMode){
            return;
        }

        manager = GameObject.Find("manager");

        //If is game scene level....
        if(manager != null)
        {
            if (offlineMode)
            {
                RPC_CreatePlayer(playersInGame);
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
            playersInGame = 1;
        }
        else
        {
            playersInGame = 0;
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

#if UNITY_EDITOR
        RoomOptions roomOptions = new RoomOptions() {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = maxPlayers };

        if (PhotonNetwork.CreateRoom(TEST_ROOM_NAME, roomOptions, TypedLobby.Default))
        {
            print("create room successfully sent.");
        }
        else
        {
            print("create room failed to send");
        }
#else
        if (PhotonNetwork.JoinRoom(TEST_ROOM_NAME))
        {
            print("Joined room.");
        }
        else
        {
            print("Join room failed!!!.");
        }
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
        print("Joined room.");
    }

    //Photon Callback
    private void OnMasterClientSwitched(PhotonPlayer newMasterClient)
    {
        quitGame();
    }

    //Photon Callback
    private void OnPhotonPlayerConnected(PhotonPlayer photonPlayer)
    {
        print("player connected...");
    }

    //Photon Callback
    private void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        playersInGame--;
    }

    #endregion

    #region RPC

    //RPC stuff
    private void MasterLoadedGame()
    {
        RPC_LoadedGameScene(PhotonNetwork.player);
        photonV.RPC("RPC_LoadGameOthers", PhotonTargets.Others, SceneManager.GetActiveScene().name);
    }

    private void NonMasterLoadedGame()
    {
        photonV.RPC("RPC_LoadedGameScene", PhotonTargets.MasterClient, PhotonNetwork.player);
    }

    [PunRPC]
    private void RPC_LoadGameOthers(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }

    [PunRPC]
    //This method is only called by the server
    private void RPC_LoadedGameScene(PhotonPlayer photonPlayer)
    {
        print("Player joined.");
        playersInGame++;
        if (playersInGame == PhotonNetwork.playerList.Length)
        {
            print("All players are in the game scene.");
            photonV.RPC("RPC_CreatePlayer", PhotonTargets.All, playersInGame);
        }
    }

    [PunRPC]
    public void RPC_CreatePlayer(int playersInGame)
    {
        if (!PhotonNetwork.isMasterClient)
        {
            this.playersInGame = playersInGame;
        }

        //set photon player
        photonPlayer = PhotonNetwork.player;

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


