using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class LocalMultiplayerManager : GameplayManager
{
    public GameObject UICameraTop;

    // Hotbar stuff
    public GameObject hotbarPrefab;
    public Transform TopHotbarTransform;
    public Transform BotHotbarTransform;

    protected override void BuildUI()
    {
        // Foreach Team
        GameObject hotbar;
        int totalPlayers = 0;
        for (int i = 0; i < Teams.Count; i++)
        {
            // Foreach Player in Team
            for (int j = 0; j < Teams[i].Length; j++)
            {
                // Add HotBar for Player of Team
                hotbar = AssignPlayerToHotbar(Teams[i][j], ++totalPlayers);

                // Move HotBar to rigth if nessessary (for Player 2 & 4)
                if (totalPlayers % 2 == 0)
                {
                    RectTransform t = hotbar.GetComponent<RectTransform>();
                    t.anchorMax = new Vector2(1, 0.5f);
                    t.anchorMin = new Vector2(1, 0.5f);
                    t.anchoredPosition = new Vector2(-407f, 0);
                }
            }
        }

        // Enable Top Camera
        if (totalPlayers > 2)
        {
            // Set Camera Active
            UICameraTop.SetActive(true);
            // Change Height of Camera Rect, thus there is enough space for top hotbar
            Rect CameraRect = Camera.main.rect;
            CameraRect.height = 0.84f;
            Camera.main.rect = CameraRect;
        }
    }

    private GameObject AssignPlayerToHotbar(Player playerToAssign, int playerCount)
    {
        // Instantiate Hotbar in Scene
        GameObject hotbar;
        if (playerCount < 3)
        {
            // First 2 Players are on Bottom
            hotbar = Instantiate(hotbarPrefab, BotHotbarTransform, false);
        }
        else
        {
            // Next Players are on Top
            hotbar = Instantiate(hotbarPrefab, TopHotbarTransform, false);
        }
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

    protected override void InstantiatePlayers()
    {
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
        ChampionClassController cccPlayer;
        int totalPlayers = 0;
        for (int i = 0; i < Teams.Count; i++)
        {
            int OwnLayer = GameConstants.TEAMLAYERS[i];
            LayerMask whatToHit = SectorLayerMaskManager.CreateLayerMaskWith(GameConstants.TEAMLAYERS.Where(x => x != OwnLayer).ToArray());

            SpawnPoint[] TeamSpawns = Spawns.Where(x => x.teamNumber == i+1).ToArray();
            // Foreach Player in Team
            for (int j = 0; j < Teams[i].Length; j++)
            {
                // Instantiate Player
                player = Teams[i][j];
                SpawnPoint = TeamSpawns[j];
                GoPlayer = Instantiate(player.ChampionPrefab, SpawnPoint.transform.position, Quaternion.identity);
                GoPlayer.GetComponent<UserControl>().playerNumber = player.playerNumber;

                // Handle InputDevice and Layer and WhatToHit
                GoPlayer.GetComponent<UserControl>().inputDevice = player.inputDevice;
                GoPlayer.layer = OwnLayer;
                cccPlayer = GoPlayer.GetComponent<ChampionClassController>();
                cccPlayer.m_whatToHit = whatToHit;

                // Handle Facing Direction
                if (SpawnPoint.facingDir == SpawnPoint.SpawnFacing.left)
                {
                    cccPlayer.Flip();
                }

                // Handle Trinket
                GoPlayer.AddComponent(Trinket.trinketsForNames[player.trinket1]);
                GoPlayer.AddComponent(Trinket.trinketsForNames[player.trinket2]);
                cccPlayer.Trinket1 = GoPlayer.GetComponents<Trinket>()[0];
                cccPlayer.Trinket2 = GoPlayer.GetComponents<Trinket>()[1];
                cccPlayer.Trinket1.trinketNumber = 1;
                cccPlayer.Trinket2.trinketNumber = 2;

                //Set overlay and sort order
                PlayerSpriteRenderer = GoPlayer.transform.Find("graphics").GetComponent<SpriteRenderer>();
                PlayerSpriteRenderer.color = championSpriteOverlayColor;
                PlayerSpriteRenderer.sortingOrder = (++totalPlayers) * (-1); // Increase PlayerCount

                // Save Instantiated Player to Team
                player.InstantiatedPlayer = GoPlayer;

                // Set Target of Camera
                Camera.main.GetComponent<CameraBehaviour>().AddTargetToCamera(GoPlayer.transform);
            }
        }
    }
}
