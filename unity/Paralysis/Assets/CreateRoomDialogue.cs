using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoomDialogue : MonoBehaviour
{

    public class Room
    {
        public string Name;
        public GameplayManager.GameModes GameMode;
        public int MaxPlayers;
        public int CurrentPlayers;
        public bool Locked;

        public Room(string pName, GameplayManager.GameModes pGameMode, int pMaxPlayers, int pCurrentPlayers, bool pLocked)
        {
            Name = pName;
            GameMode = pGameMode;
            MaxPlayers = pMaxPlayers;
            CurrentPlayers = pCurrentPlayers;
            Locked = pLocked;
        }
    }


    public Room RoomToCreate = null;

    public Text GameMode;
    public Text Players;
    public Toggle Locked;
}
