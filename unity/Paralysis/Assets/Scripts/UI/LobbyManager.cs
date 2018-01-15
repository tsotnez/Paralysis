using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour {

    public static LobbyManager Instance;

    public InputField createRoomInputField;
    public InputField playerNameInputField;

    public Button createRoomButton;
    public Button backButton;
    public string roomScene = "NetworkAPITestRoom";
    public string backScene = "NetworkAPITest_Start";

    public GameObject gameNetwork;

    private Text roomStateText;
    private string roomName;

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    private void Start()
    {
        createRoomButton.onClick.AddListener(OnClickCreateRoom);
        backButton.onClick.AddListener(OnBackClicked);

        //Set input fields
        playerNameInputField.text = GameNetwork.Instance.PlayerName;
        createRoomInputField.text = "Room#" + Random.Range(1000, 9999);

        if(GameNetwork.Instance.IsConnected())
        {
            createRoomButton.interactable = true;
        }
    }

    public void OnClickJoinRoom(string roomName)
    {
        if (playerNameInputField.text != "")
        {
            GameNetwork.Instance.setPlayerName(playerNameInputField.text);
        }
        GameNetwork.Instance.joinRoom(roomName);
        SceneManager.LoadScene(roomScene);
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
        if(!GameNetwork.Instance.createRoom(createRoomInputField.text, GameNetwork.MAX_PLAYERS, true, true))
        {
            print("failed to make room");
        }
    }

    public void OnBackClicked()
    {
        //Make sure to destroy the gameNetwork
        Destroy(GameNetwork.Instance.gameObject);
        SceneManager.LoadScene(backScene);
    }

    //TODO - KW phton callback
    private void OnJoinedRoom()
    {
        SceneManager.LoadScene(roomScene);
    }
}
