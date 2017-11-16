﻿using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class LocalMultiplayerManager : MonoBehaviour {

    //GameOver overlay
    public Transform gameOverOverlay;

    //Prefabs for players
    public static GameObject player1 = null;
    public static UserControl.InputDevice inputP1 = UserControl.InputDevice.XboxController;
    public static GameObject player2 = null;
    public static UserControl.InputDevice inputP2 = UserControl.InputDevice.KeyboardMouse;

    //Redundant variables so they can be assigned in the inspector (static ones cant)
    public GameObject defaultPlayer1 = null;
    public GameObject defaultPlayer2 = null;

    //Hotbar prefab
    public GameObject hotbarPrefab;

    //Spawn locations
    public Transform spawnPlayer1;
    public Transform spawnPlayer2;

    //Color to apply to player images to fit the environemts lighting
    public Color championSpriteOverlayColor;

    //Array containign all player game objects
    private List<GameObject> players = new List<GameObject>();

    private void Awake()
    {
        instantiatePlayers();
        buildUI();
    }

    private void buildUI()
    {
        Transform parent = GameObject.Find("MainCanvas").transform.Find("Hotbars");

        GameObject hotbar = Instantiate(hotbarPrefab, parent, false);
        hotbar.transform.position = new Vector3(hotbar.transform.position.x, hotbar.transform.position.y, hotbar.transform.position.z);
        hotbar.GetComponent<HotbarController>().setChampionName(player1.name);
        hotbar.GetComponent<HotbarController>().initAbilityImages(player1.name);

        //Assign hotbar to player
        players[0].GetComponent<CharacterStats>().hotbar = hotbar.GetComponent<HotbarController>();
        players[0].GetComponent<ChampionClassController>().hotbar = hotbar.GetComponent<HotbarController>();

        GameObject hotbar2 = Instantiate(hotbarPrefab, parent, false);
        hotbar2.transform.position = new Vector3(4.4f, hotbar2.transform.position.y, hotbar.transform.position.z);
        hotbar2.GetComponent<HotbarController>().setChampionName(player2.name);
        hotbar2.GetComponent<HotbarController>().initAbilityImages(player2.name);
        players[1].GetComponent<CharacterStats>().hotbar = hotbar2.GetComponent<HotbarController>();
        players[1].GetComponent<ChampionClassController>().hotbar = hotbar2.GetComponent<HotbarController>();
    }

    private void instantiatePlayers()
    {
        if (player1 == null)
            player1 = defaultPlayer1;
        if (player2 == null)
            player2 = defaultPlayer2;

        if (player1 != null && player2 != null)
        {
            //Disable debug buttons
            GameObject.Find("Button_changeCharacter").SetActive(false);
            GameObject.Find("Button_switchMode").SetActive(false);

            //when starting from champion selection, disable debug buttons, instantiate players
            GameObject instPlayer1 = Instantiate(player1, spawnPlayer1.position, Quaternion.identity);
            instPlayer1.GetComponent<UserControl>().inputDevice = inputP1;
            Camera.main.GetComponent<CameraBehaviour>().changeTarget(instPlayer1.transform);

            GameObject instPlayer2 = Instantiate(player2, spawnPlayer2.position, Quaternion.identity);
            instPlayer2.layer = 12;

            //Change player2 prefab to be an enemy to player 1
            LayerMask whatToHit = new LayerMask();
            whatToHit |= (1 << 11);

            instPlayer2.GetComponent<ChampionClassController>().m_whatToHit = whatToHit;
            instPlayer2.GetComponent<UserControl>().inputDevice = inputP2;
            instPlayer2.GetComponent<UserControl>().playerNumber = UserControl.PlayerNumbers.Player2;

            Camera.main.GetComponent<CameraBehaviour>().switchToMultiplayer(instPlayer2.GetComponent<Transform>());

            instPlayer1.transform.Find("graphics").GetComponent<SpriteRenderer>().color = championSpriteOverlayColor;
            instPlayer2.transform.Find("graphics").GetComponent<SpriteRenderer>().color = championSpriteOverlayColor;

            //Add Game Objects to array
            players.Add(instPlayer1);
            players.Add(instPlayer2);
        }
    }

    /// <summary>
    /// Called when a player dies. The duel is over. Show end Screen.
    /// </summary>
    /// <param name="deadPlayer"></param>
    public void gameOver(GameObject deadPlayer)
    {
        //Get the one player who is not dead
        string winner = players.Where(x => x != deadPlayer).ToArray()[0].GetComponent<ChampionClassController>().className;

        gameOverOverlay.Find("Title").GetComponent<Text>().text = winner + " won the game";
        gameOverOverlay.gameObject.SetActive(true);
        deadPlayer.SetActive(false);
        Camera.main.GetComponent<CameraBehaviour>().gameRunning = false;
    }
}
