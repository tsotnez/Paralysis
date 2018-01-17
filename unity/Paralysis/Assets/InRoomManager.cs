using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InRoomManager : MonoBehaviour {

    //Please notice: The PlayerListEntry objects have a ready image which is supposed to show whether the player has entered the ready state
    //Note 2: The Start-Button in the bottom center may be replaced by a ready button depending on whether the local player owns the room

    public Object currentScene;
    public Image MapPreviewImage;
    public Text MapPreviewText;
    public GameObject PlayerListEntryPrefab;
    public Transform PlayerList;
    public Text PlayerCount;

    int CurrentPlayers = 1;
    int MaxPlayers = 4;

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
    public void AddPlayerToList(string name)
    {
        GameObject newPlayer = Instantiate(PlayerListEntryPrefab, PlayerList);
        newPlayer.transform.Find("Name").GetComponent<Text>().text = name;

        CurrentPlayers++;
        PlayerCount.text = "Players:   " +  CurrentPlayers + " | " + MaxPlayers;
    }
}
