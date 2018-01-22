using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class LocalMultiplayerManager : GameplayManager {

    //Hotbar prefab
    public GameObject hotbarPrefab;

    //Spawn locations
    public Transform spawnPlayer1;
    public Transform spawnPlayer2;

    protected override void buildUI()
    {
        Transform parent = GameObject.Find("Hotbars").transform;

        GameObject hotbar = Instantiate(hotbarPrefab, parent, false);
        hotbar.GetComponent<HotbarController>().setChampionName(team1[0].ChampionPrefab.name);
        hotbar.GetComponent<HotbarController>().initAbilityImages(team1[0].ChampionPrefab.name);
        hotbar.GetComponent<HotbarController>().initTrinketImages(players[0].GetComponents<Trinket>()[0].DisplayName, players[0].GetComponents<Trinket>()[1].DisplayName);

        //Assign hotbar to player
        players[0].GetComponent<CharacterStats>().hotbar = hotbar.GetComponent<HotbarController>();
        players[0].GetComponent<ChampionClassController>().hotbar = hotbar.GetComponent<HotbarController>();

        GameObject hotbar2 = Instantiate(hotbarPrefab, parent, false);

        RectTransform t = hotbar2.GetComponent<RectTransform>();

        t.anchorMax = new Vector2(1, 0.5f);
        t.anchorMin = new Vector2(1, 0.5f);
        t.anchoredPosition = new Vector2(-407f, 0);
        hotbar2.GetComponent<HotbarController>().setChampionName(team2[0].ChampionPrefab.name);
        hotbar2.GetComponent<HotbarController>().initAbilityImages(team2[0].ChampionPrefab.name);
        hotbar2.GetComponent<HotbarController>().initTrinketImages(players[1].GetComponents<Trinket>()[0].DisplayName, players[1].GetComponents<Trinket>()[1].DisplayName);
        players[1].GetComponent<CharacterStats>().hotbar = hotbar2.GetComponent<HotbarController>();
        players[1].GetComponent<ChampionClassController>().hotbar = hotbar2.GetComponent<HotbarController>();
    }

    protected override void instantiatePlayers()
    {
        //Defaults for debugging
        if (team1 == null)
            team1 = new Player[] {defaultPlayer1};
        if (team2 == null)
            team2 = new Player[] {defaultPlayer2};

        if (team1 != null && team2 != null)
        {
            //Player1
            GameObject instPlayer1 = Instantiate(team1[0].ChampionPrefab, spawnPlayer1.position, Quaternion.identity);
            instPlayer1.GetComponent<UserControl>().inputDevice = team1[0].inputDevice;
            instPlayer1.layer = GameConstants.TEAM_1_LAYER;

            LayerMask whatToHitP1 = new LayerMask();
            whatToHitP1 |= (1 << GameConstants.TEAM_2_LAYER);

            instPlayer1.GetComponent<ChampionClassController>().m_whatToHit = whatToHitP1;
            instPlayer1.GetComponent<UserControl>().playerNumber = team1[0].playerNumber;

            //Trinkets P1
            instPlayer1.AddComponent(Trinket.trinketsForNames[team1[0].trinket1]);
            instPlayer1.AddComponent(Trinket.trinketsForNames[team1[0].trinket2]);
            instPlayer1.GetComponent<ChampionClassController>().Trinket1 = instPlayer1.GetComponents<Trinket>()[0];
            instPlayer1.GetComponent<ChampionClassController>().Trinket2 = instPlayer1.GetComponents<Trinket>()[1];
            instPlayer1.GetComponent<ChampionClassController>().Trinket1.trinketNumber = 1;
            instPlayer1.GetComponent<ChampionClassController>().Trinket2.trinketNumber = 2;

            Camera.main.GetComponent<CameraBehaviour>().changeTarget(instPlayer1.transform);

            //Player 2
            GameObject instPlayer2 = Instantiate(team2[0].ChampionPrefab, spawnPlayer2.position, Quaternion.identity);
            instPlayer2.layer = GameConstants.TEAM_2_LAYER;

            //Change player2 prefab to be an enemy to player 1
            LayerMask whatToHitP2 = new LayerMask();
            whatToHitP2 |= (1 << GameConstants.TEAM_1_LAYER);

            instPlayer2.GetComponent<ChampionClassController>().m_whatToHit = whatToHitP2;
            instPlayer2.GetComponent<UserControl>().inputDevice = team2[0].inputDevice;
            instPlayer2.GetComponent<UserControl>().playerNumber = team2[0].playerNumber;

            Camera.main.GetComponent<CameraBehaviour>().switchToMultiplayer(instPlayer2.GetComponent<Transform>());

            //Trinkets P2
            instPlayer2.AddComponent(Trinket.trinketsForNames[team2[0].trinket1]);
            instPlayer2.AddComponent(Trinket.trinketsForNames[team2[0].trinket2]);
            instPlayer2.GetComponent<ChampionClassController>().Trinket1 = instPlayer2.GetComponents<Trinket>()[0];
            instPlayer2.GetComponent<ChampionClassController>().Trinket2 = instPlayer2.GetComponents<Trinket>()[1];
            instPlayer2.GetComponent<ChampionClassController>().Trinket1.trinketNumber = 1;
            instPlayer2.GetComponent<ChampionClassController>().Trinket2.trinketNumber = 2;

            //Set overlay
            instPlayer1.transform.Find("graphics").GetComponent<SpriteRenderer>().color = championSpriteOverlayColor;
            instPlayer2.transform.Find("graphics").GetComponent<SpriteRenderer>().color = championSpriteOverlayColor;

            //Add Game Objects to array
            players.Add(instPlayer1);
            players.Add(instPlayer2);
        }
    }
}
