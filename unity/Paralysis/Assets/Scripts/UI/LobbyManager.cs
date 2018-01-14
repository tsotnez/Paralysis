using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour {

    public static LobbyManager Instance;

    public InputField createRoomInputField;
    public InputField playerNameInputField;

    public Text roomNameText;
    public Text kickPlayerText;

    public Button startMatchButton;
    public Button roomStateButton;
    public Button leaveRoomButton;
    public Button createRoomButton;
    public Button quitButton;

    private Text roomStateText;
    private string roomName;

    public GameObject lobbyCanvas;
    public GameObject roomCanvas;

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    private void Start()
    {
        roomStateText = roomStateButton.GetComponentInChildren<Text>();

        startMatchButton.onClick.AddListener(OnClickStartGame);
        leaveRoomButton.onClick.AddListener(OnClickLeaveRoom);
        roomStateButton.onClick.AddListener(OnClickRoomState);
        createRoomButton.onClick.AddListener(OnClickCreateRoom);
        quitButton.onClick.AddListener(OnClickQuitGame);

        //Set input fields
        playerNameInputField.text = GameNetwork.Instance.PlayerName;
        createRoomInputField.text = "Room#" + Random.Range(1000, 9999);
        createRoomButton.interactable = false;
    }

    public void OnClickRoomState()
    {
        if(GameNetwork.Instance.isRoomLocked())
        {
            GameNetwork.Instance.lockCurrentRoom(false);
            roomStateText.text = "Room State: Open";
        }
        else
        {
            GameNetwork.Instance.lockCurrentRoom(true);
            roomStateText.text = "Room State: Locked";
        }
    }
        
    public void OnClickStartGame()
    {
        GameNetwork.Instance.StartGame();
    }

    public void OnClickLeaveRoom()
    {
        LobbyManager.Instance.lobbyCanvas.transform.SetAsLastSibling();
        GameNetwork.Instance.leaveCurrentRoom();
    }

    public void OnClickJoinRoom(string roomName)
    {
        if (playerNameInputField.text != "")
        {
            GameNetwork.Instance.setPlayerName(playerNameInputField.text);
        }
        GameNetwork.Instance.joinRoom(roomName);
    }

    public void OnClickCreateRoom()
    {
        if (createRoomInputField.text == "")
        {
            return;
        }
        if (playerNameInputField.text != "")
        {
            GameNetwork.Instance.setPlayerName(playerNameInputField.text);
        }
        GameNetwork.Instance.createRoom(createRoomInputField.text, GameNetwork.MAX_PLAYERS, true, true);
    }

    public void joinedRoom()
    {
        LobbyManager.Instance.roomCanvas.transform.SetAsLastSibling();
        bool isMaster = GameNetwork.Instance.IsMasterClient;
        startMatchButton.interactable = isMaster;
        roomStateButton.interactable = isMaster;
        roomStateText.text = "Room State: Open";
    }

    public void OnClickQuitGame()
    {
        GameNetwork.Instance.quitGame();
    }

    //TODO these photon callbacks should be going through game network
    private void OnJoinedLobby()
    {
        LobbyManager.Instance.lobbyCanvas.transform.SetAsLastSibling();
    }

    private void OnConnectedToMaster()
    {
        createRoomButton.interactable = true;
    }
}
