using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoomTableManager : MonoBehaviour {

    public GameObject RoomPrefab;
    public Transform Content;
    public GameObject DialogueWindow;
    private CreateRoomDialogue dialogueScript;

    public Color dark;
    public Color bright;

    private bool lastDark = false;

    private void Start()
    {
        dialogueScript = DialogueWindow.GetComponent<CreateRoomDialogue>();
    }

    //Called, when the user created a room via dialogue. Will transition to the room scene (todo)
    public void createRoom()
    {
        string GM = dialogueScript.GameMode.text;
        string players = dialogueScript.Players.text;
        bool locked = dialogueScript.Locked.isOn;

        DialogueWindow.SetActive(false);
    }

    //Call this Method when refreshing rooms
    public void AddRoomToTable(string pName, GameplayManager.GameModes pGameMode, int pMaxPlayers, int pCurrentPlayers, bool pLocked)
    {
        GameObject NewRoom = Instantiate(RoomPrefab, Content);
        NewRoom.transform.Find("Name").GetComponent<Text>().text = pName;
        NewRoom.transform.Find("gameMode").GetComponent<Text>().text = pGameMode.ToString();
        NewRoom.transform.Find("Players").GetComponent<Text>().text = pCurrentPlayers.ToString() + "/" + pMaxPlayers.ToString();
        NewRoom.transform.Find("locked").GetComponent<Text>().text = pLocked.ToString();

        if (lastDark)
            NewRoom.GetComponent<Image>().color = bright;
        else
            NewRoom.GetComponent<Image>().color = dark;

        lastDark = !lastDark;
    }

    //Shows the create room dialogue
    public void ShowDialogue()
    {
        DialogueWindow.SetActive(true);
    }
    
}
