using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InRoomManager : MonoBehaviour {

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
