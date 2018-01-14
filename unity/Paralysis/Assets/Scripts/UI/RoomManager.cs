using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour {

    public static RoomManager Instance;

    public Text roomNameText;
    public Text kickPlayerText;

    public Button startMatchButton;
    public Button roomStateButton;
    public Button leaveRoomButton;

    public string lobyScene = "NetworkAPITestLobby";

    private Text roomStateText;
    private string roomName;

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

        bool isMaster = GameNetwork.Instance.IsMasterClient;
        startMatchButton.interactable = isMaster;
        roomStateButton.interactable = isMaster;
        roomStateText.text = "Room State: Open";
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
        GameNetwork.Instance.leaveCurrentRoom();
    }

    private void OnJoinedLobby()
    {
        SceneManager.LoadScene(lobyScene);
    }
}
