using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayerManager : MonoBehaviour {

    public static Player player1;
    public Player rPlayer1;

    public Transform spawnPlayer1;
    public GameObject hotbarPrefab;
    private List<GameObject> players = new List<GameObject>();

    private void Start()
    {
        InstantiatePlayers();
        BuildUi();
    }

    private void BuildUi()
    {
        Transform parent = GameObject.Find("Hotbars").transform;

        GameObject hotbar = Instantiate(hotbarPrefab, parent, false);
        hotbar.GetComponent<HotbarController>().setChampionName(player1.ChampionPrefab.name);
        hotbar.GetComponent<HotbarController>().initAbilityImages(player1.ChampionPrefab.name);
        hotbar.GetComponent<HotbarController>().initTrinketImages(players[0].GetComponents<Trinket>()[0].DisplayName, players[0].GetComponents<Trinket>()[1].DisplayName);

        //Assign hotbar to player
        players[0].GetComponent<CharacterStats>().hotbar = hotbar.GetComponent<HotbarController>();
        players[0].GetComponent<ChampionClassController>().hotbar = hotbar.GetComponent<HotbarController>();
    }

    private void InstantiatePlayers()
    {
        if (player1 == null)
            player1 = rPlayer1;

        //Player1
        GameObject instPlayer1 = Instantiate(player1.ChampionPrefab, spawnPlayer1.position, Quaternion.identity);
        instPlayer1.GetComponent<UserControl>().inputDevice = player1.inputDevice;
        instPlayer1.layer = GameConstants.TEAM_1_LAYER;

        LayerMask whatToHitP1 = new LayerMask();
        whatToHitP1 |= (1 << GameConstants.TEAM_2_LAYER); //Add Team2 as target layer

        instPlayer1.GetComponent<ChampionClassController>().m_whatToHit = whatToHitP1;
        instPlayer1.GetComponent<UserControl>().playerNumber = player1.playerNumber;

        //Trinkets P1
        instPlayer1.AddComponent(Trinket.trinketsForNames[player1.trinket1]);
        instPlayer1.AddComponent(Trinket.trinketsForNames[player1.trinket2]);
        instPlayer1.GetComponent<ChampionClassController>().Trinket1 = instPlayer1.GetComponents<Trinket>()[0];
        instPlayer1.GetComponent<ChampionClassController>().Trinket2 = instPlayer1.GetComponents<Trinket>()[1];
        instPlayer1.GetComponent<ChampionClassController>().Trinket1.trinketNumber = 1;
        instPlayer1.GetComponent<ChampionClassController>().Trinket2.trinketNumber = 2;

        Camera.main.GetComponent<CameraBehaviour>().changeTarget(instPlayer1.transform);

        //Add Game Objects to array
        players.Add(instPlayer1);
    }
}
