using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class LocalMultiplayerManager : GameplayManager {

    //Hotbar prefab
    public GameObject hotbarPrefab;

    // stuff
    private Transform HotbarTransform;

    protected override void buildUI()
    {
        // Get Transform for Hotbar
        HotbarTransform = GameObject.Find("Hotbars").transform;

        // Foreach Team
        GameObject hotbar;
        int totalPlayers = 0;
        for (int i = 0; i < Teams.Count; i++)
        {
            // Foreach Player in Team
            for (int j = 0; j < Teams[j].Length; j++)
            {
                // Add HotBar for Player of Team
                hotbar = AssignPlayerToHotbar(Teams[i][j]);

                // Move HotBar if nessessary
                if (totalPlayers == 1)
                {
                    RectTransform t = hotbar.GetComponent<RectTransform>();
                    t.anchorMax = new Vector2(1, 0.5f);
                    t.anchorMin = new Vector2(1, 0.5f);
                    t.anchoredPosition = new Vector2(-407f, 0);
                }
                totalPlayers++;
            }
        }
    }

    private GameObject AssignPlayerToHotbar(Player playerToAssign)
    {
        // Instantiate Hotbar in Scene
        GameObject hotbar = Instantiate(hotbarPrefab, HotbarTransform, false);
        HotbarController hotbarController = hotbar.GetComponent<HotbarController>();

        // Apply Champion Name
        hotbarController.setChampionName(playerToAssign.ChampionPrefab.name);
        // Apply Ability Images
        hotbarController.initAbilityImages(playerToAssign.ChampionPrefab.name);
        // Apply Trinkets
        GameObject GoPlayer = playerToAssign.InstantiatedPlayer;
        hotbarController.initTrinketImages(GoPlayer.GetComponents<Trinket>()[0].DisplayName, GoPlayer.GetComponents<Trinket>()[1].DisplayName);

        //Assign hotbar to player
        GoPlayer.GetComponent<CharacterStats>().hotbar = hotbarController;
        GoPlayer.GetComponent<ChampionClassController>().hotbar = hotbarController;

        // Returns Hotbar
        return hotbar;
    }

    protected override void instantiatePlayers()
    {
        // Get all SpawnPoints
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        //Defaults for debugging
        if (Teams == null)
        {
            Teams = new Dictionary<int, Player[]>();
            Teams[0] = new Player[] { defaultPlayer1 };
            Teams[1] = new Player[] { defaultPlayer2 };
        }

        // Foreach Team
        Player player = null;
        GameObject GoPlayer = null;
        SpawnPoint SpawnPoint;
        SpriteRenderer PlayerSpriteRenderer;
        int totalPlayers = 0;
        for (int i = 0; i < Teams.Count; i++)
        {
            LayerMask OwnLayer;
            LayerMask EnemyLayer;
            if (i == 0)
            {
                OwnLayer = GameConstants.TEAM_1_LAYER;
                EnemyLayer = GameConstants.TEAM_2_LAYER;
            }
            else
            {
                OwnLayer = GameConstants.TEAM_2_LAYER;
                EnemyLayer = GameConstants.TEAM_1_LAYER;
            }

            // Foreach Player in Team
            for (int j = 0; j < Teams[j].Length; j++)
            {
                // Instantiate Player
                player = Teams[i][j];
                SpawnPoint = spawnPoints[totalPlayers].GetComponent<SpawnPoint>();
                GoPlayer = Instantiate(player.ChampionPrefab, SpawnPoint.transform.position, Quaternion.identity);
                GoPlayer.GetComponent<UserControl>().playerNumber = player.playerNumber;

                // Handle InputDevice and Layer
                GoPlayer.GetComponent<UserControl>().inputDevice = player.inputDevice;
                GoPlayer.layer = OwnLayer;

                // Handle WhatToHit
                LayerMask whatToHit = new LayerMask();
                whatToHit |= (1 << EnemyLayer);
                GoPlayer.GetComponent<ChampionClassController>().m_whatToHit = whatToHit;

                // Handle Facing Direction
                if (SpawnPoint.facingDir == SpawnPoint.SpawnFacing.left)
                {
                    GoPlayer.GetComponent<ChampionClassController>().Flip();
                }

                // Handle Trinket
                GoPlayer.AddComponent(Trinket.trinketsForNames[player.trinket1]);
                GoPlayer.AddComponent(Trinket.trinketsForNames[player.trinket2]);
                GoPlayer.GetComponent<ChampionClassController>().Trinket1 = GoPlayer.GetComponents<Trinket>()[0];
                GoPlayer.GetComponent<ChampionClassController>().Trinket2 = GoPlayer.GetComponents<Trinket>()[1];
                GoPlayer.GetComponent<ChampionClassController>().Trinket1.trinketNumber = 1;
                GoPlayer.GetComponent<ChampionClassController>().Trinket2.trinketNumber = 2;

                //Set overlay and sort order
                PlayerSpriteRenderer = GoPlayer.transform.Find("graphics").GetComponent<SpriteRenderer>();
                PlayerSpriteRenderer.color = championSpriteOverlayColor;
                PlayerSpriteRenderer.sortingOrder = (++totalPlayers)*(-1); // Increase PlayerCount

                // Save Instantiated Player to Team
                player.InstantiatedPlayer = GoPlayer;

                // Set Target of Camera
                if (totalPlayers == 1)
                {
                    Camera.main.GetComponent<CameraBehaviour>().changeTarget(GoPlayer.transform);
                }
                else
                {
                    Camera.main.GetComponent<CameraBehaviour>().switchToMultiplayer(GoPlayer.transform);
                }
            }
        }
    }
}
