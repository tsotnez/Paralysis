using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InRoomManager : MonoBehaviour {

    //Please notice: The PlayerListEntry objects have a ready image which is supposed to show whether the player has entered the ready state
    //Note 2: The Start-Button in the bottom center may be replaced by a ready button depending on whether the local player owns the room

    [HideInInspector]
    public Object currentScene;
    public Image MapPreviewImage;
    public Text MapPreviewText;
    public GameObject PlayerListEntryPrefab;
    public Transform PlayerList;
    public Text PlayerCount;
    public Text RoomNameText;
    public Button startGameButton;

    int CurrentPlayers = 1;
    int MaxPlayers = 4;

    private Dictionary<PhotonPlayer, GameObject> playerDict = new Dictionary<PhotonPlayer, GameObject>();

    private void Start()
    {
        startGameButton.onClick.AddListener(startGame);

        RoomNameText.text = GameNetwork.Instance.CurrentRoomInfo.Name;
        GameObject hostEntry = GameObject.FindGameObjectWithTag("PlayerRoomEntry");
        Text hostEntryText = hostEntry.transform.Find("Name").GetComponent<Text>();

        if(GameNetwork.Instance.IsMasterClient)
        {
            hostEntryText.text = GameNetwork.Instance.PlayerName;
        }
        else
        {
            startGameButton.interactable = false;

            PhotonPlayer[] playerList = GameNetwork.Instance.getPlayerList();
            foreach(PhotonPlayer player in playerList)
            {
                if(player.IsMasterClient)
                {
                    hostEntryText.text = GameNetwork.Instance.PlayerName;
                    playerDict.Add(player, hostEntry);
                }
                else
                {
                    AddPlayerToList(player);
                }
            }
        }
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

    private void OnPhotonPlayerConnected(PhotonPlayer player)
    {
        AddPlayerToList(player);
    }

    private void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        RemovePlayerFromList(player);
    }

    //Call this when a player Joins
    public void AddPlayerToList(PhotonPlayer player)
    {
        GameObject newPlayer = Instantiate(PlayerListEntryPrefab, PlayerList);
        newPlayer.transform.Find("Name").GetComponent<Text>().text = player.NickName;

        CurrentPlayers++;
        playerDict.Add(player, newPlayer);
    }

    public void RemovePlayerFromList(PhotonPlayer player)
    {
        CurrentPlayers--;
        Destroy(playerDict[player]);
    }

    private void setPlayerCountText()
    {
        PlayerCount.text = "Players:   " +  CurrentPlayers + " | " + MaxPlayers;
    }

    private void startGame()
    {
        GameNetwork.Instance.StartGame();
    }
}
